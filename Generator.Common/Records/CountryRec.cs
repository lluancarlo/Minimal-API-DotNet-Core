namespace Generator.Common.Records;

public class CountryRec
{
    public record ResultCountryNativeNameEng
    {
        public string? Official { get; set; }
        public string? Common { get; set; }
    }

    public record ResultCountryNativeName
    {
        public ResultCountryNativeNameEng? NativeName { get; set; }
    }

    public record ResultCountryName
    {
        public string? Common { get; set; }
        public string? official { get; set; }
        public ResultCountryNativeName? NativeName { get; set; }
    }

    public record ResultCountry
    {
        public required ResultCountryName Name { get; set; }
        public required string Cca2 { get; set; }
        public required string Cca3 { get; set; }
        public required string Ccn3 { get; set; }
        public required string[] Continents { get; set; }
    }
}
