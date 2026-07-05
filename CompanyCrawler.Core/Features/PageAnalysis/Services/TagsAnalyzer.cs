using AngleSharp.Common;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Services;

public class TagsAnalyzer() : IPageAnalyzer
{
    public IReadOnlyCollection<LinkCategory> Categories =>
    [
        LinkCategory.Career,
        LinkCategory.Company,
        LinkCategory.Blog
    ];

    public void Analyze(
        List<ClassifiedPage> pages,
        CompanyProfile profile)
    {
        foreach (var classifiedPage in classifiedPages)
        {
            var text = classifiedPage.Page.VisibleText;

            foreach (var keyword in Keywords)
            {
                if (!text.Contains(
                        keyword,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                profile.AddTag(
                    "Unity",
                    100,
                    $"Found '{keyword}'");

                break;
            }
        }
    }
}
