namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class KeywordRule
{
    public string Keyword { get; init; } = "";

    public int Score { get; init; }

    public bool SearchInText { get; init; } = true;

    public bool SearchInUrl { get; init; } = true;
    
    public string Reason { get; init; } = "";
}