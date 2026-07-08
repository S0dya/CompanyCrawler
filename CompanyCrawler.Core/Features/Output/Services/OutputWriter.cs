using System.Globalization;
using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.Output.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace CompanyCrawler.Core.Features.Output.Services;

public class OutputWriter(CrawlPresetConfig preset) : IOutputWriter
{
    public void Write(string path, List<CompanyResult> companies)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, config);

        WriteHeader(csv);
        WriteRows(csv, companies, preset);
    }

    private void WriteHeader(CsvWriter csv)
    {
        csv.WriteField("Company");
        csv.WriteField("Website");

        csv.WriteField("Best Email");

        foreach (var analyzer in preset.KeywordAnalyzersData)
        {
            csv.WriteField($"Best {analyzer.Type} Page");
            csv.WriteField($"Best {analyzer.Type} Keywords");
        }

        csv.WriteField("All Emails");
        
        foreach (var analyzer in preset.KeywordAnalyzersData)
        {
            csv.WriteField($"Other {analyzer.Type} Pages");
            csv.WriteField($"Other {analyzer.Type} Keywords");
        }

        csv.WriteField("LinkedIn");
        csv.WriteField("Github");
        csv.WriteField("Upwork");
        csv.WriteField("HH");
        csv.WriteField("Glassdoor");
        csv.WriteField("Indeed");
        csv.WriteField("Other External Links");
        
        csv.NextRecord();
    }
    
    private static void WriteRows(
        CsvWriter csv,
        IEnumerable<CompanyResult> companies, 
        CrawlPresetConfig preset)
    {
        foreach (var company in companies)
        {
            csv.WriteField(company.CompanyName);
            csv.WriteField(company.Website);

            csv.WriteField(company.BestEmail);

            foreach (var analyzer in preset.KeywordAnalyzersData)
            {
                if (company.Results.TryGetValue(analyzer.Type, out var results) &&
                    results.Count > 0)
                {
                    var best = results[0];

                    csv.WriteField(best.Page);
                    csv.WriteField(string.Join(", ", best.Keywords));
                }
                else
                {
                    csv.WriteField("");
                    csv.WriteField("");
                }
            }
            
            csv.WriteField(company.AllEmails);

            foreach (var analyzer in preset.KeywordAnalyzersData)
            {
                if (company.Results.TryGetValue(analyzer.Type, out var results) &&
                    results.Count > 1)
                {
                    var others = results.Skip(1);

                    csv.WriteField(string.Join("; ", others.Select(x => x.Page)));
                    csv.WriteField(string.Join("; ", others.Select(x => string.Join(", ", x.Keywords))));
                }
                else
                {
                    csv.WriteField("");
                    csv.WriteField("");
                }
            }
            
            csv.WriteField(company.LinkedIn);
            csv.WriteField(company.Github);
            csv.WriteField(company.Upwork);
            csv.WriteField(company.HH);
            csv.WriteField(company.Glassdoor);
            csv.WriteField(company.Indeed);
            csv.WriteField(company.OtherExternalLinks);

            csv.NextRecord();
        }
    }
}