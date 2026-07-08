using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.LinkClassification.Config;
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
            
            var uri = new Uri(page.Url);

            foreach (var rule in ExternalSitesConfig.Rules)
            {
                if (rule.Domains.Any(x =>
                        uri.Host.Equals(x, StringComparison.OrdinalIgnoreCase) ||
                        uri.Host.EndsWith("." + x, StringComparison.OrdinalIgnoreCase)))
                {
                    classified.IsExternal = true;
                    classified.ExternalType = rule.Type;
                    break;
                }
            }

            if (classified.IsExternal == false)
            {
                classified.ExternalType = ExternalSiteType.Unknown;
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
    
    public List<WebsiteLink> ClassifyLinks(
        string homepageUrl,
        List<WebsiteLink> links)
    {
        var homeHost = new Uri(homepageUrl).Host;

        foreach (var link in links)
        {
            if (!Uri.TryCreate(link.Url, UriKind.Absolute, out var uri))
                continue;

            var sameHost =
                uri.Host.Equals(homeHost, StringComparison.OrdinalIgnoreCase);

            if (sameHost)
            {
                link.IsExternal = false;
                link.ExternalType = ExternalSiteType.Unknown;
                continue;
            }

            link.IsExternal = true;
            link.ExternalType = ExternalSiteType.Unknown;

            foreach (var rule in ExternalSitesConfig.Rules)
            {
                if (!rule.Domains.Any(x =>
                        uri.Host.Equals(x, StringComparison.OrdinalIgnoreCase) ||
                        uri.Host.EndsWith("." + x, StringComparison.OrdinalIgnoreCase)))
                    continue;

                link.ExternalType = rule.Type;
                break;
            }
        }

        return links;
    }
    
}
