namespace CompanyCrawler.Core.Models;

public class FinderRule
{
    public List<string> IncludeKeywords { get; init; } = [];

    public List<string> ExcludeKeywords { get; init; } = [];

    public bool PreferSameDomain { get; init; } = true;

    public int MaxResults { get; init; } = 5;
}