using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Models;

namespace CompanyCrawler.Core.Features.Output.Models;

public class CompanyResult
{
    public string CompanyName { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public bool IsHiring { get; set; }

    public string Tags { get; set; } = string.Empty;

    public string CareerPage { get; set; } = string.Empty;

    public string BestEmail { get; set; } = string.Empty;

    public string AllEmails { get; set; } = string.Empty;
    
    public Dictionary<AnalyzerType, List<AnalyzerResult>> Results { get; set; } = [];

    public string Language { get; set; } = string.Empty;
    
    public string LinkedIn { get; set; } = "";
    public string Github { get; set; } = "";
    public string Upwork { get; set; } = "";
    public string HH { get; set; } = "";
    public string Glassdoor { get; set; } = "";
    public string Indeed { get; set; } = "";

    public string OtherExternalLinks { get; set; } = "";
}
