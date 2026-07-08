using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.WebDownload.Models;

public class WebsiteLink
{
    public string Url { get; set; } = "";

    public string Text { get; set; } = "";

    public int Score { get; set; }

    public bool IsExternal { get; set; }

    public ExternalSiteType ExternalType { get; set; }
}