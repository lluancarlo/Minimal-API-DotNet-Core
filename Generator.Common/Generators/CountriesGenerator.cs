using Generator.Common.Records;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Minimal.DAL.Entities;
using static Generator.Common.Records.CountryRec;
using static Minimal.Domain.Enums;

namespace BigGenerator.Generators;

public class CountriesGenerator : BaseGenerator
{
    public override async Task<GeneratorResponse> Run()
    {
        if (Context.Countries.AsNoTracking().Any())
            return GeneratorResponse.Ok("Country table is not empty, skipping generator ...");
        
        try
        {
            var countryList = new List<Country>();
            var baseUrl = "https://restcountries.com/v3.1/all?fields=name,cca2,cca3,ccn3continents";
            var fields = new List<string> { "name", "cca2", "cca3", "ccn3", "continents" };

            using (HttpClient client = new HttpClient())
            {
                var requestUrl = GetUrl(baseUrl, fields);
                var response = await client.GetAsync(requestUrl);
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<IEnumerable<ResultCountry>>(jsonString);

                    if (result != null)
                        foreach (var c in result)
                            countryList.Add(new Country
                            {
                                Name = c.Name.Common ?? string.Empty,
                                Iso31661Alpha2 = c.Cca2,
                                Iso31661Alpha3 = c.Cca3,
                                Iso31661Numeric = c.Ccn3,
                                Continent = GetEnumByName<Continents>(c.Continents[0].Replace(" ", string.Empty))
                            });
                }

                if (countryList.Count > 0)
                {
                    countryList = countryList.OrderBy(o => o.Name).ToList();
                    await Context.Countries.AddRangeAsync(countryList);
                    await Context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            return GeneratorResponse.Error($" EXCEPTION! : {ex.Message}");
        }

        return GeneratorResponse.Ok();
    }

    private string GetUrl(string baseUrl, IEnumerable<string>? fields)
    {
        var url = baseUrl;

        if (fields != null)
        {
            url += url.EndsWith('?') ? string.Empty : ' ';

            // Add fields to url
            url += "fields=";
            foreach (var field in fields)
                url += field + ',';

            if (url.EndsWith(','))
                url = url.Remove(url.Length - 1);
        }

        return url;
    }
}