using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class ExternalLink
{
    public ExternalSiteType Type { get; set; }
    public string Url { get; set; } = "";
}