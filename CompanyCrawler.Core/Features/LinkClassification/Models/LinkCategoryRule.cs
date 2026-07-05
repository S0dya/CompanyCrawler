namespace CompanyCrawler.Core.Features.LinkClassification.Models;

public class LinkCategoryRule
{
    public LinkCategory Category { get; init; }

    public List<string> Keywords { get; } = [];

    public List<string> ExcludeKeywords { get; } = [];
}