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
}

public class CrawlConfig
{
    public int MaxDepth { get; set; } = 2;
    public int MaxPages { get; set; } = 10;
    public string SitemapPath { get; set; } = "/sitemap.xml";
}