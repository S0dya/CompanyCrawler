using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.LinkClassification.Models;

public class ClassifiedPage
{
    public required DownloadedPage Page { get; init; }
    public Dictionary<LinkCategory, int> Scores { get; } = [];
    public bool IsExternal { get; set; }
    public ExternalSiteType ExternalType { get; set; }
    public HashSet<LinkCategory> Categories { get; } = [];
}