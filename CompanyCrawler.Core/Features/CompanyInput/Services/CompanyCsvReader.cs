using System.Globalization;
using CsvHelper;
using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Models;

namespace CompanyCrawler.Core.Features.CompanyInput.Services;

public class CompanyCsvReader : ICompanyCsvReader
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
