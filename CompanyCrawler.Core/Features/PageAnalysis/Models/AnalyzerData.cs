using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

using CompanyCrawler.Core.Features.LinkClassification.Models;

public class AnalyzerData
{
    public AnalyzerType Type { get; init; }

    public int Threshold { get; init; }

    public List<KeywordRule> Keywords { get; init; } = [];
    
    public LinkCategory[] LinkCategories { get; init; } = [];
}