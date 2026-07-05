namespace CompanyCrawler.Core.Features.WebDownload.Models;

public class DownloadedPage
{
    public string Url { get; set; } = string.Empty;
    
    public string Html { get; set; } = string.Empty;
    
    public List<WebsiteLink> Links { get; set; } = [];
    
    public string VisibleText { get; set; } = string.Empty;
    
    public bool UsedBrowser { get; set; }
    
    public string Language { get; set; } = "Unknown";
}
