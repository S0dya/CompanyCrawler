using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class AnalyzerCategoriesConfig
{
    public Dictionary<AnalyzerType, LinkCategory[]> Categories { get; } = new()
    {
        [AnalyzerType.Career] =
        [
            LinkCategory.Career,
            LinkCategory.Company,
            LinkCategory.Contact,
            LinkCategory.Blog
        ],

        [AnalyzerType.Email] =
        [
            LinkCategory.Contact,
            LinkCategory.Career,
            LinkCategory.Company,
            LinkCategory.Blog,
            LinkCategory.Portfolio
        ],

        [AnalyzerType.Tags] =
        [
            LinkCategory.Company,
            LinkCategory.Services,
            LinkCategory.Portfolio,
            LinkCategory.Blog
        ],
    };
}