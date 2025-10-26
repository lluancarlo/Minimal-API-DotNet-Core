namespace Minimal.Application.Common.DTOs;

/// <summary>
/// Public DTO representing verb conjugation data.
/// Used by the Application layer.
/// </summary>
public class VerbConjugationData
{
    public string Verb { get; private set; }
    public IReadOnlyList<ModeData> Modes { get; private set; }
    public VerbInfoData Info { get; private set; }

    private VerbConjugationData(
        string verb,
        IList<ModeData> modes,
        VerbInfoData info)
    {
        Verb = verb;
        Modes = modes.ToList().AsReadOnly();
        Info = info;
    }

    public static VerbConjugationData Create(
        string verb,
        IList<ModeData> modes,
        VerbInfoData info)
    {
        if (string.IsNullOrWhiteSpace(verb))
            throw new ArgumentException("Verb cannot be empty", nameof(verb));

        var lowerVerb = verb.ToLowerInvariant();
        if (!lowerVerb.EndsWith("are") && !lowerVerb.EndsWith("ere") && !lowerVerb.EndsWith("ire"))
            throw new ArgumentException("Italian verb must end in -are, -ere, or -ire", nameof(verb));

        if (modes == null || modes.Count == 0)
            throw new ArgumentException("Modes cannot be empty", nameof(modes));

        if (info == null)
            throw new ArgumentNullException(nameof(info));

        return new VerbConjugationData(lowerVerb, modes, info);
    }

    public bool IsRegular()
    {
        return Verb.EndsWith("are") || Verb.EndsWith("ere") || Verb.EndsWith("ire");
    }

    public ModeData? GetMode(string modeName)
    {
        return Modes.FirstOrDefault(m =>
            m.Name.Equals(modeName, StringComparison.OrdinalIgnoreCase));
    }
}

public class ModeData
{
    public string Name { get; private set; }
    public IReadOnlyList<TenseData> Tenses { get; private set; }

    private ModeData(string name, IList<TenseData> tenses)
    {
        Name = name;
        Tenses = tenses.ToList().AsReadOnly();
    }

    public static ModeData Create(string name, IList<TenseData> tenses)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mode name cannot be empty", nameof(name));

        if (tenses == null || tenses.Count == 0)
            throw new ArgumentException("Tenses cannot be empty", nameof(tenses));

        return new ModeData(name, tenses);
    }

    public TenseData? GetTense(string tenseName)
    {
        return Tenses.FirstOrDefault(t =>
            t.Name.Equals(tenseName, StringComparison.OrdinalIgnoreCase));
    }
}

public class TenseData
{
    public string Name { get; private set; }
    public IReadOnlyList<ConjugationRowData> Rows { get; private set; }

    private TenseData(string name, IList<ConjugationRowData> rows)
    {
        Name = name;
        Rows = rows.ToList().AsReadOnly();
    }

    public static TenseData Create(string name, IList<ConjugationRowData> rows)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tense name cannot be empty", nameof(name));

        if (rows == null || rows.Count == 0)
            throw new ArgumentException("Conjugation rows cannot be empty", nameof(rows));

        return new TenseData(name, rows);
    }

    public string? GetFormForPerson(string person)
    {
        return Rows.FirstOrDefault(r =>
            r.Person.Equals(person, StringComparison.OrdinalIgnoreCase))?.Form;
    }
}

public class ConjugationRowData
{
    public string Person { get; private set; }
    public string Form { get; private set; }

    private ConjugationRowData(string person, string form)
    {
        Person = person;
        Form = form;
    }

    public static ConjugationRowData Create(string person, string form)
    {
        if (string.IsNullOrWhiteSpace(person))
            throw new ArgumentException("Person cannot be empty", nameof(person));

        if (string.IsNullOrWhiteSpace(form))
            throw new ArgumentException("Form cannot be empty", nameof(form));

        return new ConjugationRowData(person.Trim(), form.Trim());
    }

    public bool IsFirstPersonSingular()
    {
        return Person.Equals("io", StringComparison.OrdinalIgnoreCase);
    }
}

public class VerbInfoData
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IReadOnlyList<LinkInfoData> RelatedLinks { get; private set; }
    public IReadOnlyList<TranslationEntryData> Translations { get; private set; }

    private VerbInfoData(
        string title,
        string description,
        IList<LinkInfoData> relatedLinks,
        IList<TranslationEntryData> translations)
    {
        Title = title;
        Description = description;
        RelatedLinks = relatedLinks.ToList().AsReadOnly();
        Translations = translations.ToList().AsReadOnly();
    }

    public static VerbInfoData Create(
        string title,
        string description,
        IList<LinkInfoData>? relatedLinks = null,
        IList<TranslationEntryData>? translations = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        return new VerbInfoData(
            title,
            description ?? string.Empty,
            relatedLinks ?? new List<LinkInfoData>(),
            translations ?? new List<TranslationEntryData>());
    }

    public string? GetTranslation(string languageCode)
    {
        return Translations.FirstOrDefault(t =>
            t.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase))?.Translation;
    }
}

public class LinkInfoData
{
    public string Text { get; private set; }
    public string Href { get; private set; }

    private LinkInfoData(string text, string href)
    {
        Text = text;
        Href = href;
    }

    public static LinkInfoData Create(string text, string href)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Link text cannot be empty", nameof(text));

        if (string.IsNullOrWhiteSpace(href))
            throw new ArgumentException("Link href cannot be empty", nameof(href));

        // Validação de URL
        if (!Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out _))
            throw new ArgumentException("Invalid href format", nameof(href));

        return new LinkInfoData(text, href);
    }

    public bool IsAbsoluteUrl()
    {
        return Uri.TryCreate(Href, UriKind.Absolute, out _);
    }
}

public class TranslationEntryData
{
    public string Language { get; private set; }
    public string LanguageCode { get; private set; }
    public string Translation { get; private set; }

    private TranslationEntryData(string language, string languageCode, string translation)
    {
        Language = language;
        LanguageCode = languageCode;
        Translation = translation;
    }

    public static TranslationEntryData Create(
        string language,
        string languageCode,
        string translation)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be empty", nameof(language));

        if (string.IsNullOrWhiteSpace(languageCode))
            throw new ArgumentException("Language code cannot be empty", nameof(languageCode));

        if (string.IsNullOrWhiteSpace(translation))
            throw new ArgumentException("Translation cannot be empty", nameof(translation));

        // Validação de código ISO 639-1 (2 letras) ou ISO 639-2 (3 letras)
        var code = languageCode.ToLowerInvariant();
        if (code.Length < 2 || code.Length > 3 || !code.All(char.IsLetter))
            throw new ArgumentException(
                "Language code must be 2-3 letters (ISO 639)",
                nameof(languageCode));

        return new TranslationEntryData(language, code, translation);
    }
}
