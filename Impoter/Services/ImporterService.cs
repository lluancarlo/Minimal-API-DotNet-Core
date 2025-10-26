using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Impoter.Dtos;
using System.Text.RegularExpressions;

namespace Impoter.Services;

public class ImporterService : IImporterService
{
    private const string BaseUrl = "https://www.coniugazione.it/verbo/{0}.php";
    private readonly HttpClient _httpClient;

    public ImporterService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<VerbConjugation?> GetVerbConjugation(string verb)
    {
        var url = string.Format(BaseUrl, verb.Trim().ToLowerInvariant());

        var (isSuccess, response) = await FetchHtml(url);
        if (isSuccess)
            return ParseConjugation(verb, response, url);

        return null;
    }

    private async Task<(bool, string)> FetchHtml(string url)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; VocabuApiScraper/1.0)");

        try
        {
            var res = await _httpClient.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var response = await res.Content.ReadAsStringAsync();
            return (true, response);
        }
        catch (Exception ex)
        {
            return (false, $"Error on {nameof(ImporterService)}: {ex}");
        }
    }

    private static VerbConjugation ParseConjugation(string verb, string html, string pageUrl)
    {
        var parser = new HtmlParser();
        var doc = parser.ParseDocument(html);

        var container = doc.QuerySelector("#contenu");
        if (container == null)
            throw new InvalidOperationException("Conteúdo '#contenu' não encontrado na página.");

        // --- extract title, description and related links ---
        var title = doc.QuerySelector("h1.titre_fiche span")?.TextContent?.Trim() ?? string.Empty;

        IElement? blocWithAux = null;
        foreach (var b in container.QuerySelectorAll("div.bloc"))
        {
            var bold = b.QuerySelector("b")?.TextContent?.Trim();
            if (!string.IsNullOrEmpty(bold) && bold.StartsWith("Ausiliare", StringComparison.OrdinalIgnoreCase))
            {
                blocWithAux = b;
                break;
            }
        }
        // fallback: first .bloc if specific one not found
        if (blocWithAux == null)
            blocWithAux = container.QuerySelector("div.bloc");

        var description = blocWithAux?.TextContent?.Trim() ?? string.Empty;

        var relatedLinks = new List<LinkInfo>();
        if (blocWithAux != null)
        {
            foreach (var a in blocWithAux.QuerySelectorAll("a"))
            {
                var text = a.TextContent?.Trim() ?? string.Empty;
                var href = a.GetAttribute("href") ?? string.Empty;
                string fullHref;
                try
                {
                    if (Uri.TryCreate(href, UriKind.Absolute, out var abs))
                        fullHref = abs.ToString();
                    else
                        fullHref = new Uri(new Uri(pageUrl), href).ToString();
                }
                catch
                {
                    fullHref = href;
                }

                relatedLinks.Add(new LinkInfo(text, fullHref));
            }
        }

        // --- extract translations (Traduzione) ---
        var translations = new List<TranslationEntry>();
        // Try to find the specific translation block: look for div with class 'blochome' or similar
        var translationBlock = container.QuerySelector("div.blochome") ?? container.QuerySelector("div.blochome.b.t22");
        if (translationBlock != null)
        {
            translations = ParseTranslationBlock(translationBlock, pageUrl);
        }
        else
        {
            // alternative: find H2.mode whose span starts with "Traduzione" and then locate following div
            var tradH2 = container.QuerySelectorAll("h2.mode").FirstOrDefault(h =>
                h.QuerySelector("span")?.TextContent?.Trim().StartsWith("Traduzione", StringComparison.OrdinalIgnoreCase) == true);
            if (tradH2 != null)
            {
                // find next sibling div with class 'blochome' or any div following the H2
                var next = tradH2.NextElementSibling;
                while (next != null && !(next.ClassList?.Contains("blochome") == true))
                    next = next.NextElementSibling;
                if (next != null)
                {
                    translations = ParseTranslationBlock(next, pageUrl);
                }
            }
        }

        var info = new VerbInfo(title, NormalizeWhitespace(description), relatedLinks, translations);

        // --- existing parsing of modes / tenses ---
        var modes = new List<Mode>();
        Mode? currentMode = null;

        foreach (var child in container.Children)
        {
            if (child.ClassList != null && child.ClassList.Contains("mode") && string.Equals(child.NodeName, "H2", StringComparison.OrdinalIgnoreCase))
            {
                var modeName = child.QuerySelector("span")?.TextContent?.Trim() ?? child.TextContent.Trim();
                currentMode = new Mode(modeName, new List<Tense>());
                modes.Add(currentMode);
                continue;
            }

            if (child.ClassList != null && child.ClassList.Contains("tempstab"))
            {
                var tenseName = child.QuerySelector("h3.tempsheader")?.TextContent?.Trim() ?? "Unknown";
                var tempsCorps = child.QuerySelector("div.tempscorps");
                var rows = ParseTempsCorps(tempsCorps);
                var tense = new Tense(tenseName, rows);

                // If there's no preceding mode (unexpected), create a default
                if (currentMode == null)
                {
                    currentMode = new Mode("Unknown", new List<Tense>());
                    modes.Add(currentMode);
                }

                currentMode.Tenses.Add(tense);
            }
        }

        return new VerbConjugation(verb, modes, info);
    }

    private static List<TranslationEntry> ParseTranslationBlock(IElement block, string pageUrl)
    {
        var result = new List<TranslationEntry>();
        if (block == null) return result;

        // Split block content by BR elements into lines (preserve nodes per line)
        var linesNodes = SplitByBr(block);

        foreach (var lineNodes in linesNodes)
        {
            var lineText = BuildLineText(lineNodes);
            if (string.IsNullOrWhiteSpace(lineText)) continue;

            // Attempt to extract translation after arrow '➔'
            var arrowIndex = lineText.IndexOf('➔');
            string translationText = string.Empty;
            string leftText = lineText;
            if (arrowIndex >= 0)
            {
                leftText = lineText.Substring(0, arrowIndex).Trim();
                translationText = lineText.Substring(arrowIndex + 1).Trim();
            }
            else
            {
                // fallback: try '->'
                var arrow2 = lineText.IndexOf("->", StringComparison.Ordinal);
                if (arrow2 >= 0)
                {
                    leftText = lineText.Substring(0, arrow2).Trim();
                    translationText = lineText.Substring(arrow2 + 2).Trim();
                }
            }

            // Determine language code: prefer the second IMG in the line nodes
            string langCode = string.Empty;
            var imgs = lineNodes.OfType<IElement>().Where(n => string.Equals(n.NodeName, "IMG", StringComparison.OrdinalIgnoreCase)).ToList();
            if (imgs.Count >= 2)
            {
                var src = imgs[1].GetAttribute("src") ?? string.Empty;
                langCode = ExtractLanguageCodeFromImgSrc(src, pageUrl);
            }
            else if (imgs.Count == 1)
            {
                // If only one img, it may be the target flag (rare). Try to use it.
                var src = imgs[0].GetAttribute("src") ?? string.Empty;
                langCode = ExtractLanguageCodeFromImgSrc(src, pageUrl);
            }
            else
            {
                // No images: try to infer language from leftText (e.g., "Italiano essere ➔  Ingles: to be")
                // look for known language names present in leftText or right side
                langCode = InferLanguageCodeFromText(leftText, translationText);
            }

            var languageName = MapLanguageCodeToPortugueseName(langCode);

            // Clean translation text (remove dangling colons or language names like "Ingles:" etc.)
            translationText = Regex.Replace(translationText, @"^\s*[:\-–—]\s*", "");
            // If translationText contains a language label like "Ingles: to be", remove the label
            translationText = Regex.Replace(translationText, @"^[A-Za-zÀ-ÖØ-öø-ÿ\s]+:\s*", "");

            if (string.IsNullOrWhiteSpace(translationText))
            {
                // as fallback, try to get text nodes after the second img in the line nodes
                translationText = ExtractTextAfterSecondImage(lineNodes);
            }

            result.Add(new TranslationEntry(languageName, langCode ?? string.Empty, NormalizeWhitespace(translationText)));
        }

        return result;
    }

    private static string ExtractTextAfterSecondImage(List<INode> nodes)
    {
        int imgCount = 0;
        var parts = new List<string>();
        bool afterSecondImg = false;
        foreach (var n in nodes)
        {
            if (n is IElement el && string.Equals(el.NodeName, "IMG", StringComparison.OrdinalIgnoreCase))
            {
                imgCount++;
                if (imgCount >= 2) afterSecondImg = true;
                continue;
            }

            if (afterSecondImg)
            {
                var t = n.TextContent ?? string.Empty;
                parts.Add(t);
            }
        }
        var text = string.Join("", parts).Replace("\u00A0", " ").Trim();
        text = Regex.Replace(text, @"\s+", " ");
        return text;
    }

    private static string ExtractLanguageCodeFromImgSrc(string src, string pageUrl)
    {
        if (string.IsNullOrWhiteSpace(src)) return string.Empty;
        try
        {
            if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
            {
                var baseUri = new Uri(pageUrl);
                var abs = new Uri(baseUri, src);
                src = abs.ToString();
            }
            // filename expected like ".../img/l/de.png"
            var file = src.Split('/').LastOrDefault() ?? src;
            var dot = file.IndexOf('.');
            if (dot > 0)
            {
                var name = file.Substring(0, dot);
                // if file is like "de.png" or "it.png", return name
                // sometimes the file name could be "en.png" or "it.png"
                return name.ToLowerInvariant();
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string InferLanguageCodeFromText(string left, string right)
    {
        // quick heuristics mapping common Portuguese or English names to codes
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "italiano", "it" }, { "italia", "it" },
            { "alemao", "de" }, { "alemão", "de" }, { "german", "de" },
            { "ingles", "en" }, { "inglês", "en" }, { "english", "en" },
            { "frances", "fr" }, { "francês", "fr" }, { "francais", "fr" },
            { "espanhol", "es" }, { "español", "es" }, { "espanol", "es" },
            { "portugues", "pt" }, { "português", "pt" }
        };

        foreach (var kv in map)
        {
            if (left.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0 ||
                right.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                return kv.Value;
        }

        return string.Empty;
    }

    private static string MapLanguageCodeToPortugueseName(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return string.Empty;
        switch (code.ToLowerInvariant())
        {
            case "it": return "Italiano";
            case "de": return "Alemão";
            case "en": return "Inglês";
            case "fr": return "Francês";
            case "es": return "Espanhol";
            case "pt": return "Português";
            default: return code; // fallback to code if unknown
        }
    }

    private static List<List<INode>> SplitByBr(IElement element)
    {
        var lines = new List<List<INode>>();
        var buffer = new List<INode>();
        foreach (var node in element.ChildNodes)
        {
            if (string.Equals(node.NodeName, "BR", StringComparison.OrdinalIgnoreCase))
            {
                lines.Add(new List<INode>(buffer));
                buffer.Clear();
            }
            else
            {
                // sometimes there are nested elements like DIVs or text nodes; flatten them
                buffer.Add(node);
            }
        }

        if (buffer.Count > 0)
            lines.Add(buffer);

        return lines;
    }

    private static List<ConjugationRow> ParseTempsCorps(IElement? corps)
    {
        var result = new List<ConjugationRow>();
        if (corps == null) return result;

        // Build lines by splitting on BR elements (AngleSharp keeps BR nodes)
        var buffer = new List<INode>();
        foreach (var node in corps.ChildNodes)
        {
            if (string.Equals(node.NodeName, "BR", StringComparison.OrdinalIgnoreCase))
            {
                var lineText = BuildLineText(buffer);
                AddIfValid(lineText, result);
                buffer.Clear();
            }
            else
            {
                buffer.Add(node);
            }
        }

        // last line (if any)
        if (buffer.Count > 0)
        {
            var lineText = BuildLineText(buffer);
            AddIfValid(lineText, result);
        }

        return result;
    }

    private static string BuildLineText(IEnumerable<INode> nodes)
    {
        // Concatenate text content of the nodes, preserving spacing
        var parts = new List<string>();
        foreach (var n in nodes)
        {
            var t = n.TextContent ?? string.Empty;
            parts.Add(t);
        }

        var line = string.Join("", parts).Replace("\u00A0", " ").Trim();
        // Normalize multiple spaces
        line = Regex.Replace(line, @"\s+", " ");
        return line;
    }

    private static string NormalizeWhitespace(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var s = input.Replace("\u00A0", " ");
        s = Regex.Replace(s, @"\s+", " ");
        return s.Trim();
    }

    private static readonly Regex PronounRegex = new(
        @"^(che\s+(io|tu|lui|lei|noi|voi|loro)|io|tu|lui|lei|noi|voi|loro)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static void AddIfValid(string lineText, List<ConjugationRow> output)
    {
        if (string.IsNullOrWhiteSpace(lineText))
            return;

        // Some lines show "-" (no pronoun) or just the form
        if (lineText.Trim() == "-")
        {
            output.Add(new ConjugationRow(string.Empty, "-"));
            return;
        }

        var m = PronounRegex.Match(lineText);
        if (m.Success)
        {
            var person = m.Value.Trim();
            var form = lineText.Substring(m.Length).Trim();
            output.Add(new ConjugationRow(person, string.IsNullOrEmpty(form) ? string.Empty : form));
            return;
        }

        // Otherwise, fallback: split first token as person, rest as form
        var firstSpace = lineText.IndexOf(' ');
        if (firstSpace <= 0)
        {
            // only one token -> treat as form without person
            output.Add(new ConjugationRow(string.Empty, lineText));
        }
        else
        {
            var person = lineText.Substring(0, firstSpace).Trim();
            var form = lineText.Substring(firstSpace + 1).Trim();
            output.Add(new ConjugationRow(person, form));
        }
    }
}
