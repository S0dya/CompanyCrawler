namespace CompanyCrawler.Core.Features.LinkClassification.Models;

public class ClassifiedCandidates
{
    public Dictionary<LinkCategory, List<PageCandidate>> CategoryLinks { get; set; } = new();
}
