using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Services;

public class ExternalLinksAnalyzer : IPageAnalyzer
{
    public void Analyze(List<ClassifiedPage> pages, CompanyProfile profile)
    {
        foreach (var page in pages)
        {
            if (!page.IsExternal)
                continue;

            if (profile.ExternalLinks.Any(x => x.Url == page.Page.Url))
                continue;

            profile.ExternalLinks.Add(new ExternalLink
            {
                Url = page.Page.Url,
                Type = page.ExternalType
            });
        }
    }
}