using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.LinkClassification.Services;

public class LinkClassifier(PageClassificationConfig config) : ILinkClassifier
{
    public List<ClassifiedPage> Classify(List<DownloadedPage> pages)
    {
        var result = new List<ClassifiedPage>();

        foreach (var page in pages)
        {
            var classified = new ClassifiedPage
            {
                Page = page
            };

            foreach (var rule in config.Rules)
            {
                var score = CalculateScore(page, rule);

                if (score <= 0) continue;

                classified.Categories.Add(rule.Category);

                classified.Scores[rule.Category] = score;
            }

            if (classified.Categories.Count == 0)
            {
                classified.Categories.Add(LinkCategory.Unknown);
            }

            result.Add(classified);
        }

        return result;
    }

    private static int CalculateScore(DownloadedPage page, LinkCategoryRule rule)
    {
        var score = 0;

        foreach (var keyword in rule.Keywords)
        {
            if (Contains(page.Url, keyword))
                score += 100;

            if (Contains(page.Html, keyword))
                score += 50;

            if (Contains(page.VisibleText, keyword))
                score += 15;
        }

        foreach (var keyword in rule.ExcludeKeywords)
        {
            if (Contains(page.Url, keyword))
                score -= 100;
        }

        return score;
    }

    private static bool Contains(string? text, string keyword)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        return text.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }
}
