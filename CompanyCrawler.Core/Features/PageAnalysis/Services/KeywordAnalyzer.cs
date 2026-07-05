using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;

public class KeywordAnalyzer(AnalyzerData data) : IPageAnalyzer
{
    public void Analyze(List<ClassifiedPage> pages, CompanyProfile profile)
    {
        foreach (var classifiedPage in pages)
        {
            if (!data.LinkCategories.Contains(classifiedPage.Categories))
                continue;

            var page = classifiedPage.Page;

            var score = 0;

            foreach (var keyword in data.Keywords)
            {
                if (keyword.SearchInText &&
                    page.VisibleText.Contains(
                        keyword.Keyword,
                        StringComparison.OrdinalIgnoreCase))
                {
                    score += keyword.Score;
                }

                if (keyword.SearchInUrl &&
                    page.Url.Contains(
                        keyword.Keyword,
                        StringComparison.OrdinalIgnoreCase))
                {
                    score += keyword.Score;
                }
            }

            if (score < data.Threshold)
                continue;

            profile.AddTag(
                data.Type.ToString(),
                score,
                page.Url);

            if (data.Type == AnalyzerType.Career)
            {
                profile.Hiring = true;
                profile.CareerPage = page.Url;
            }
        }
    }
}