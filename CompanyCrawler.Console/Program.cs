using CompanyCrawler.Core.Models;
using CompanyCrawler.Core.Services;

var csvReader = new CompanyCsvReader();
var websiteDownloader = new HttpDownloader();
var htmlParser = new HtmlParser();
var linkNormalizer = new LinkNormalizer();
var pageFinder = new PageFinder();

var careerRule = new FinderRule
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

Console.Write("CSV path: ");

var path = Console.ReadLine();

if (string.IsNullOrWhiteSpace(path))
{
    Console.WriteLine("Path cannot be empty.");
    return;
}

var companies = csvReader.ReadCompanies(path);

foreach (var company in companies)
{
    Console.WriteLine($"Downloading {company.Website}...");

    var downloadResult = await websiteDownloader.DownloadWebsiteAsync(company.Website);
    
    Directory.CreateDirectory("DownloadedHtml");

    var host = new Uri(company.Website).Host.Replace(".", "_");

    File.WriteAllText($"DownloadedHtml/{host}.html", downloadResult.Html);

    Console.WriteLine($"Downloaded {downloadResult.Html.Length} characters");

    var links = await htmlParser.GetLinksAsync(downloadResult.Html);

    links = linkNormalizer.Normalize(downloadResult.Website, links);

    
    var candidates = pageFinder.Find(
        links,
        careerRule,
        company.Website);
    
    Console.WriteLine("Normalized links:");

    foreach (var link in links)
    {
        Console.WriteLine(link.Url);
    }

    Console.WriteLine();

    Console.WriteLine("Career candidates:");
    
    foreach (var candidate in candidates)
    {
        Console.WriteLine();
    
        Console.WriteLine(candidate.Link.Url);
    
        Console.WriteLine($"Score: {candidate.Score}");
    
        foreach (var reason in candidate.Reasons)
        {
            Console.WriteLine($"    {reason}");
        }
    }
    
    Console.WriteLine();
}

Console.WriteLine("Finished.");