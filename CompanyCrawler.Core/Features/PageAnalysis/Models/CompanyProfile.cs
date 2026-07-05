namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class CompanyProfile
{
    public string CompanyName { get; set; } = string.Empty;
    
    public string Website { get; set; } = string.Empty;
    
    public bool Hiring { get; set; }
    
    public string? CareerPage { get; set; }
    
    public List<CompanyEmail> Emails { get; set; } = [];
    
    public Dictionary<string, CompanyTag> Tags { get; set; } = new();
    
    
    public List<AnalyzerReason> Reasons { get; } = [];

    public void AddTag(string name, int score, string reason)
    {
        if (!Tags.TryGetValue(name, out var tag))
        {
            tag = new CompanyTag { Name = name };
            Tags[name] = tag;
        }

        tag.Score += score;
        tag.Reasons.Add(reason);
    }
}
