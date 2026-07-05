using CompanyCrawler;
using CompanyCrawler.Core;
using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Models;
using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.Output.Models;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.Sitemap.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceProvider = await DependencyInjection.ConfigureAsync();

var csvReader = serviceProvider.GetRequiredService<ICompanyCsvReader>();
var websiteDownloader = serviceProvider.GetRequiredService<IWebDownloader>();
var linkNormalizer = serviceProvider.GetRequiredService<ILinkNormalizer>();
var sitemapDownloader = serviceProvider.GetRequiredService<ISitemapDownloader>();
var linkClassifier = serviceProvider.GetRequiredService<ILinkClassifier>();
var outputWriter = serviceProvider.GetRequiredService<IOutputWriter>();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var analyzers = serviceProvider.GetServices<IPageAnalyzer>().ToList();

Console.Write("CSV path: ");

var path = Console.ReadLine();

if (string.IsNullOrWhiteSpace(path))
{
    logger.LogError("CSV path cannot be empty");
    return;
}

var results = new List<CompanyResult>();
var companies = csvReader.ReadCompanies(path);

logger.LogInformation("Loaded {CompanyCount} companies from CSV", companies.Count);

foreach (var company in companies)
{
    logger.LogInformation("Downloading {Website}", company.Website);

    var homepage = await websiteDownloader.DownloadAsync(company.Website);
    
    var sitemapLinks = await sitemapDownloader.DownloadSitemapAsync(company.Website);

    logger.LogInformation("Found {SitemapCount} sitemap links", sitemapLinks.Count);
    foreach (var sitemapLink in sitemapLinks)
    {
        logger.LogDebug("Sitemap link: {Url}", sitemapLink.Url);
    }
    
    homepage.Links.AddRange(sitemapLinks);
    
    Directory.CreateDirectory("DownloadedHtml");

    var host = new Uri(company.Website).Host.Replace(".", "_");

    File.WriteAllText($"DownloadedHtml/{host}.html", homepage.Html);

    logger.LogInformation("Downloaded {CharacterCount} characters", homepage.Html.Length);

    var links = linkNormalizer.Normalize(
        homepage.Url,
        homepage.Links);
    
    logger.LogInformation("Normalized to {LinkCount} unique links", links.Count);
    
    var classificationRule = new FinderRule
    {
        IncludeKeywords =
        {
            "career",
            "careers",
            "job",
            "jobs",
            "vacancy",
            "vacancies",
            "join",
            "join-us",
            "hiring"
        },
        ExcludeKeywords =
        {
            "blog",
            "news",
            "article",
            "upwork"
        },
        MaxResults = 5
    };
    
    var candidates = linkClassifier.Classify(
        links,
        classificationRule,
        company.Website);
    
    // Create scoped crawler for each company
    using var scope = serviceProvider.CreateScope();
    var crawler = scope.ServiceProvider.GetRequiredService<IPageCrawler>();
    
    var pages = await crawler.CrawlAsync(
        homepage,
        candidates.CategoryLinks
            .Where(x => x.Value.Count > 0)
            .SelectMany(x => x.Value)
            .DistinctBy(x => x.Link.Url)
            .ToList());
    
    logger.LogInformation("Crawled {PageCount} pages", pages.Count);
    
    var profile = new CompanyProfile();
    foreach (var analyzer in analyzers)
    {
        analyzer.Analyze(pages, profile);
    }
    
    logger.LogInformation("Analysis - Hiring: {IsHiring}, CareerPage: {CareerPage}", profile.Hiring, profile.CareerPage);

    logger.LogInformation("Found {EmailCount} emails", profile.Emails.Count);
    foreach (var email in profile.Emails)
    {
        logger.LogInformation("Email: {Address}, Score: {Score}", email.Address, email.Score);
        foreach (var reason in email.Reasons)
        {
            logger.LogDebug("  Reason: {Reason}", reason);
        }
    }
    
    foreach (var reason in profile.Reasons)
    {
        logger.LogInformation("[{Analyzer}] +{Score} '{Source}' ({Reason}) on {Page}", 
            reason.Analyzer, reason.Score, reason.Source, reason.Reason, reason.Page);
    }
    
    var result = new CompanyResult
    {
        CompanyName = profile.CompanyName,

        Website = profile.Website,

        IsHiring = profile.Hiring,

        Tags = string.Join(
            "; ",
            profile.Tags.Values
                .OrderByDescending(x => x.Score)
                .Select(x => x.Name)),

        CareerPage = profile.CareerPage ?? "",

        BestEmail = profile.Emails
            .OrderByDescending(x => x.Score)
            .FirstOrDefault()?.Address ?? "",

        AllEmails = string.Join(
            "; ",
            profile.Emails
                .OrderByDescending(x => x.Score)
                .Select(x => x.Address)),
        
        Language = homepage.Language,
    };

    results.Add(result);
}

var outputPath = Path.GetFullPath("Output.csv");

outputWriter.Write(outputPath, results);

logger.LogInformation("Output saved to {OutputPath}", outputPath);
logger.LogInformation("Finished processing {ResultCount} companies", results.Count);