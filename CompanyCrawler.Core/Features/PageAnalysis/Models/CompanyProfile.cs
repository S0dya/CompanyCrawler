using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class CompanyProfile
{
    public string CompanyName { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public List<CompanyEmail> Emails { get; set; } = [];
    public Dictionary<AnalyzerType, List<AnalyzerResult>> Results { get; set; } = new();
    public List<ExternalLink> ExternalLinks { get; } = [];
    
    public List<AnalyzerReason> Reasons { get; } = [];
}
