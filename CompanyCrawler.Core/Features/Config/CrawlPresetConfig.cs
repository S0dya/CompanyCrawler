using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Models;

namespace CompanyCrawler.Core.Features.Config;

public class CrawlPresetConfig
{
    public string Name { get; set; } = "";
    public List<LinkCategoryRule> LinkCategories { get; set; } = [];
    public List<AnalyzerData> KeywordAnalyzersData { get; set; } = [];
    public AnalyzerData EmailAnalyzerData { get; set; } = new();
    public CrawlConfig CrawlSettings { get; set; } = new();
    public OutputConfig Output { get; set; } = new();
}

public class CrawlConfig
{
    public int MaxDepth { get; set; } = 2;
    public int MaxPages { get; set; } = 10;
    public string SitemapPath { get; set; } = "/sitemap.xml";
}

public class OutputConfig
{
    public Dictionary<AnalyzerType, PrintType> Analyzers { get; set; } = [];

    public bool PrintBestEmail { get; set; }
    public bool PrintAllEmails { get; set; }

    public bool PrintLinkedIn { get; set; }
    public bool PrintGithub { get; set; }
    public bool PrintUpwork { get; set; }
    public bool PrintHH { get; set; }
    public bool PrintGlassdoor { get; set; }
    public bool PrintIndeed { get; set; }

    public bool PrintOtherExternalLinks { get; set; }
}

public enum PrintType
{
    None,
    Keywords,
    Links,
    KeywordsAndLinks,
    KeywordsAndLinksAndOther,
    BestLink,
}