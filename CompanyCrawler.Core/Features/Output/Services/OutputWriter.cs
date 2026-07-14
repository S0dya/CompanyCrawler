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
        WriteRows(csv, companies);
    }

    private void WriteHeader(CsvWriter csv)
    {
        csv.WriteField("Company");
        csv.WriteField("Website");

        if (preset.Output.PrintBestEmail)
            csv.WriteField("Best Email");

        foreach (var analyzer in preset.KeywordAnalyzersData)
        {
            var printType = preset.Output.Analyzers[analyzer.Type];

            switch (printType)
            {
                case PrintType.None:
                    break;

                case PrintType.Keywords:
                    csv.WriteField(analyzer.Type);
                    break;

                case PrintType.Links:
                    csv.WriteField(analyzer.Type);
                    break;

                case PrintType.BestLink:
                    csv.WriteField(analyzer.Type);
                    break;

                case PrintType.KeywordsAndLinks:
                    csv.WriteField($"{analyzer.Type} Page");
                    csv.WriteField($"{analyzer.Type} Keywords");
                    break;

                case PrintType.KeywordsAndLinksAndOther:
                    csv.WriteField($"{analyzer.Type} Page");
                    csv.WriteField($"{analyzer.Type} Keywords");

                    csv.WriteField($"Other {analyzer.Type} Pages");
                    csv.WriteField($"Other {analyzer.Type} Keywords");
                    break;
            }
        }

        if (preset.Output.PrintAllEmails)
            csv.WriteField("All Emails");
        
        if (preset.Output.PrintLinkedIn)
            csv.WriteField("LinkedIn");

        if (preset.Output.PrintGithub)
            csv.WriteField("Github");

        if (preset.Output.PrintUpwork)
            csv.WriteField("Upwork");

        if (preset.Output.PrintHH)
            csv.WriteField("HH");

        if (preset.Output.PrintGlassdoor)
            csv.WriteField("Glassdoor");

        if (preset.Output.PrintIndeed)
            csv.WriteField("Indeed");

        if (preset.Output.PrintOtherExternalLinks)
            csv.WriteField("Other External Links");

        csv.NextRecord();
    }

    private void WriteRows(
        CsvWriter csv,
        IEnumerable<CompanyResult> companies)
    {
        foreach (var company in companies)
        {
            csv.WriteField(company.CompanyName);
            csv.WriteField(company.Website);

            if (preset.Output.PrintBestEmail)
                csv.WriteField(company.BestEmail);

            foreach (var analyzer in preset.KeywordAnalyzersData)
            {
                var printType = preset.Output.Analyzers[analyzer.Type];

                company.Results.TryGetValue(
                    analyzer.Type,
                    out var results);

                results ??= [];

                switch (printType)
                {
                    case PrintType.None:
                        break;

                    case PrintType.Keywords:
                    {
                        csv.WriteField(
                            GetTopKeywords(results));

                        break;
                    }

                    case PrintType.Links:
                    {
                        csv.WriteField(
                            string.Join(
                                "; ",
                                results.Select(x => x.Page)));

                        break;
                    }

                    case PrintType.BestLink:
                    {
                        csv.WriteField(
                            results.FirstOrDefault()?.Page ?? "");

                        break;
                    }

                    case PrintType.KeywordsAndLinks:
                    {
                        var best = results.FirstOrDefault();

                        csv.WriteField(best?.Page ?? "");

                        csv.WriteField(
                            best == null
                                ? ""
                                : string.Join(", ", best.Keywords));

                        break;
                    }

                    case PrintType.KeywordsAndLinksAndOther:
                    {
                        var best = results.FirstOrDefault();

                        csv.WriteField(best?.Page ?? "");

                        csv.WriteField(
                            best == null
                                ? ""
                                : string.Join(", ", best.Keywords));

                        var others = results.Skip(1).ToList();

                        csv.WriteField(
                            string.Join(
                                "; ",
                                others.Select(x => x.Page)));

                        csv.WriteField(
                            string.Join(
                                "; ",
                                others.Select(
                                    x => string.Join(", ", x.Keywords))));

                        break;
                    }
                }
            }
            
            if (preset.Output.PrintAllEmails)
                csv.WriteField(company.AllEmails);

            if (preset.Output.PrintLinkedIn)
                csv.WriteField(company.LinkedIn);

            if (preset.Output.PrintGithub)
                csv.WriteField(company.Github);

            if (preset.Output.PrintUpwork)
                csv.WriteField(company.Upwork);

            if (preset.Output.PrintHH)
                csv.WriteField(company.HH);

            if (preset.Output.PrintGlassdoor)
                csv.WriteField(company.Glassdoor);

            if (preset.Output.PrintIndeed)
                csv.WriteField(company.Indeed);

            if (preset.Output.PrintOtherExternalLinks)
                csv.WriteField(company.OtherExternalLinks);

            csv.NextRecord();
        }
    }

    private static string GetTopKeywords(
        List<AnalyzerResult> results)
    {
        return string.Join(
            ", ",
            results
                .SelectMany(x => x.Keywords)
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(x => x.Count())
                .ThenBy(x => x.Key)
                .Select(x => x.Key));
    }
}