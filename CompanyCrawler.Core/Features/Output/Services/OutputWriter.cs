using System.Globalization;
using CsvHelper;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.Output.Models;

namespace CompanyCrawler.Core.Features.Output.Services;

public class OutputWriter : IOutputWriter
{
    public void Write(string path, List<CompanyResult> companies)
    {
        using var writer = new StreamWriter(path);

        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(companies);
    }
}
