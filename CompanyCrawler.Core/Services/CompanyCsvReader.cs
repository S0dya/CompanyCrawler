using System.Globalization;
using CsvHelper;
using CompanyCrawler.Core.Models;

namespace CompanyCrawler.Core.Services;

public class CompanyCsvReader
{
    public List<Company> ReadCompanies(string path)
    {
        using var streamReader = new StreamReader(path);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

        return csvReader
            .GetRecords<Company>()
            .ToList();
    }
}