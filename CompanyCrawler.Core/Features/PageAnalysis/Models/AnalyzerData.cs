using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class AnalyzerData
{
    public AnalyzerType Type { get; init; }
    public int Threshold { get; init; }
    public List<KeywordRule> Keywords { get; set; } = [];
    public LinkCategory[] LinkCategories { get; init; } = [];
    public AnalyzerScope Scope { get; set; }
}