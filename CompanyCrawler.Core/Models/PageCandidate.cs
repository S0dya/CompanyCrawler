namespace CompanyCrawler.Core.Models;

public class PageCandidate
{
    public WebsiteLink Link { get; init; } = null!;

    public int Score { get; set; }

    public List<string> Reasons { get; } = [];
}