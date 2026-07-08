namespace CompanyCrawler.Core.Features.Scoring.Models;

public class LinkScoreRule
{
    public int Score { get; set; }

    public List<string> Keywords { get; set; } = [];
}