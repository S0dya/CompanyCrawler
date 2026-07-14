using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CompanyCrawler;
using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Models;
using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.Output.Models;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.Shared.Interfaces;
using CompanyCrawler.Core.Features.Sitemap.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var root = Path.Combine(AppContext.BaseDirectory, SystemConfig.Paths.DataFolder);
var presetFolder = Path.Combine(root, SystemConfig.Paths.PresetsFolder);

var presets = Directory.GetFiles(presetFolder, "*.json");
for (var i = 0; i < presets.Length; i++)
{
    Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(presets[i])}");
}
Console.Write("> ");
var index = int.Parse(Console.ReadLine()!);
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
};
var preset = JsonSerializer.Deserialize<CrawlPresetConfig>(File.ReadAllText(presets[index - 1]), options)!;

var tagsFolder = Path.Combine(root, SystemConfig.Paths.TagsFolder);
var inputFolder = Path.Combine(root, SystemConfig.Paths.InputFolder);
var outputFolder = Path.Combine(root, SystemConfig.Paths.OutputFolder);

var tagsPath = Path.Combine(tagsFolder, SystemConfig.Paths.TagsFileName);
var csvFiles = Directory.GetFiles(inputFolder, "*.csv");
var outputPath = Path.Combine(outputFolder, SystemConfig.Paths.OutputFileName);

Directory.CreateDirectory(SystemConfig.Paths.DownloadedHtmlFolder);

Console.WriteLine($"Analyzers: {preset.KeywordAnalyzersData.Count}");
foreach (var analyzer in preset.KeywordAnalyzersData)
{
    Console.WriteLine(analyzer.Type);
}

var tagsAnalyzer = preset.KeywordAnalyzersData.First(x => x.Type == AnalyzerType.Tags);
tagsAnalyzer.Keywords = TagsLoader.Load(tagsPath);

var serviceProvider = await DependencyInjection.ConfigureAsync(preset);
    
var csvReader = serviceProvider.GetRequiredService<ICompanyCsvReader>();
var websiteDownloader = serviceProvider.GetRequiredService<IWebDownloader>();
var linkNormalizer = serviceProvider.GetRequiredService<ILinkNormalizer>();
var sitemapDownloader = serviceProvider.GetRequiredService<ISitemapDownloader>();
var linkClassifier = serviceProvider.GetRequiredService<ILinkClassifier>();
var outputWriter = serviceProvider.GetRequiredService<IOutputWriter>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var urlHelper = serviceProvider.GetRequiredService<IUrlHelper>();

var analyzers = serviceProvider.GetServices<IPageAnalyzer>().ToList();

Console.WriteLine("Available CSV files:");
for (var i = 0; i < csvFiles.Length; i++)
{
    Console.WriteLine($"{i + 1}. {Path.GetFileName(csvFiles[i])}");
}
Console.Write("> ");
var csvIndex = int.Parse(Console.ReadLine()!);

var companies = csvReader.ReadCompanies(csvFiles[csvIndex - 1]);

logger.LogInformation("Loaded {CompanyCount} companies from CSV", companies.Count);

var parallelOptions = new ParallelOptions{ MaxDegreeOfParallelism = 8 };
var results = new ConcurrentBag<CompanyResult>();
await Parallel.ForEachAsync(companies, parallelOptions, async (company, ct) =>
{
    try
    {
        var result = await ProcessCompanyAsync(company);

        results.Add(result);
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Fatal error while processing {Website}",
            company.Website);
    }
});

logger.LogInformation("All companies processed. Writing results to CSV");

var inputFileName = Path.GetFileNameWithoutExtension(csvFiles[csvIndex - 1]);
var outputFile = Path.Combine(outputFolder, inputFileName + "_" + SystemConfig.Paths.OutputFileName);

outputWriter.Write(outputFile, results.ToList());

Process.Start(new ProcessStartInfo
{
    FileName = outputFile,
    UseShellExecute = true
});

async Task<CompanyResult> ProcessCompanyAsync(Company company)
{
    logger.LogInformation("=== Processing company: {Website} ===", company.Website);

    logger.LogInformation("Downloading homepage for {Website}", company.Website);
    var homepage = await websiteDownloader.DownloadAsync(company.Website);
    logger.LogInformation("Homepage downloaded: {CharacterCount} characters, {LinkCount} links", homepage.Html.Length, homepage.Links.Count);
    
    logger.LogInformation("Downloading sitemap for {Website}", company.Website);
    var sitemapLinks = await sitemapDownloader.DownloadSitemapAsync(company.Website);

    logger.LogInformation("Found {SitemapCount} sitemap links", sitemapLinks.Count);
    
    homepage.Links.AddRange(sitemapLinks);
    logger.LogInformation("Total links after sitemap: {Count}", homepage.Links.Count);

    if (!urlHelper.TryGetHost(company.Website, out var host))
    {
        logger.LogWarning(
            "Skipping company because URL is invalid: {Website}",
            company.Website);

        return new CompanyResult
        {
            CompanyName = company.Website,
            Website = company.Website
        };
    }

    File.WriteAllText($"{SystemConfig.Paths.DownloadedHtmlFolder}/{host}.html", homepage.Html);

    logger.LogInformation("Saved homepage HTML to {Folder}/{File}", SystemConfig.Paths.DownloadedHtmlFolder, host + ".html");

    logger.LogInformation("Normalizing links from homepage");
    var links = linkNormalizer.Normalize(homepage.Url, homepage.Links);

    logger.LogInformation("Normalized {Count} unique links", links.Count);
    
    links = linkClassifier.ClassifyLinks(homepage.Url, links);

    var externalLinks = links
        .Where(x => x.IsExternal)
        .ToList();

    var profile = new CompanyProfile();
    foreach (var externalLink in externalLinks)
    {
        profile.ExternalLinks.Add(new ExternalLink
        {
            Url = externalLink.Url,
            Type = externalLink.ExternalType
        });
    }

    links = links
        .Where(x => !x.IsExternal)
        .ToList();

    logger.LogInformation(
        "Found {InternalCount} internal links and {ExternalCount} external links",
        links.Count,
        externalLinks.Count);
    
    using var scope = serviceProvider.CreateScope();
    var crawler = scope.ServiceProvider.GetRequiredService<IPageCrawler>();

    logger.LogInformation("Starting web crawling");
    var pages = await crawler.CrawlAsync(homepage, links, profile);

    logger.LogInformation("Crawling completed: {Count} pages downloaded", pages.Count);

    logger.LogInformation("Classifying pages");
    var classifiedPages = linkClassifier.Classify(pages);

    logger.LogInformation("Classification completed: {Count} pages classified", classifiedPages.Count);

    logger.LogInformation("Running {AnalyzerCount} analyzers", analyzers.Count);
    foreach (var analyzer in analyzers)
    {
        logger.LogDebug("Running analyzer: {AnalyzerType}", analyzer.GetType().Name);
        analyzer.Analyze(classifiedPages, profile);
    }
    logger.LogInformation("All analyzers completed");
    
    logger.LogInformation("Found {EmailCount} emails", profile.Emails.Count);
    logger.LogInformation("Found {ReasonCount} analysis reasons", profile.Reasons.Count);
    
    var result = new CompanyResult
    {
        CompanyName = GetCompanyName(company.Website),
        Website = company.Website,

        BestEmail = profile.Emails
            .OrderByDescending(x => x.Score)
            .FirstOrDefault()?.Address ?? "",

        AllEmails = string.Join(
            "; ",
            profile.Emails
                .OrderByDescending(x => x.Score)
                .Select(x => x.Address)),

        Language = homepage.Language,

        Results = profile.Results
            .ToDictionary(k => k.Key, v => v.Value
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToList()),
        
        LinkedIn = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.LinkedIn)?.Url ?? "",
        Github = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.Github)?.Url ?? "",
        Upwork = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.Upwork)?.Url ?? "",
        HH = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.HH)?.Url ?? "",
        Glassdoor = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.Glassdoor)?.Url ?? "",
        Indeed = profile.ExternalLinks.FirstOrDefault(x => x.Type == ExternalSiteType.Indeed)?.Url ?? "",
        
        OtherExternalLinks = string.Join(
            "; ",
            profile.ExternalLinks
                .Where(x => x.Type == ExternalSiteType.Unknown)
                .DistinctBy(x => x.Url)
                .Take(5)
                .Select(x => x.Url))
    };

    logger.LogInformation("Completed processing company: {Website}", company.Website);
    
    return result;
}

string GetCompanyName(string website)
{
    var normalizedWebsite = urlHelper.NormalizeWebsite(website);
    var host = new Uri(normalizedWebsite).Host.Replace("www.", "");
    var name = host.Split('.')[0];

    return CultureInfo.InvariantCulture.TextInfo
        .ToTitleCase(name);
}