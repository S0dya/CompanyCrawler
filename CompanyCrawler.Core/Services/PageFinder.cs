using CompanyCrawler.Core.Models;

namespace CompanyCrawler.Core.Services;

public class PageFinder
{
    public List<PageCandidate> Find(List<WebsiteLink> links, FinderRule rule, string website)
    {
        var result = new List<PageCandidate>();
        var websiteHost = new Uri(website).Host;

        foreach (var link in links)
        {
            var candidate = new PageCandidate
            {
                Link = link
            };

            foreach (var keyword in rule.IncludeKeywords)
            {
                if (ContainsSegment(link.Url, keyword))
                {
                    candidate.Score += 100;
                    candidate.Reasons.Add($"URL contains '{keyword}'");
                }

                if (ContainsSegment(link.Text, keyword))
                {
                    candidate.Score += 60;
                    candidate.Reasons.Add($"Text contains '{keyword}'");
                }
            }

            foreach (var keyword in rule.ExcludeKeywords)
            {
                if (ContainsSegment(link.Url, keyword))
                {
                    candidate.Score -= 200;
                    candidate.Reasons.Add($"Excluded by '{keyword}'");
                }
            }

            if (rule.PreferSameDomain)
            {
                if (candidate.Score != 0 && new Uri(link.Url).Host == websiteHost)
                {
                    candidate.Score += 20;
                    candidate.Reasons.Add("Same domain");
                }
            }

            if (candidate.Score > 0)
            {
                result.Add(candidate);
            }
        }

        return result
            .OrderByDescending(x => x.Score)
            .Take(rule.MaxResults)
            .ToList();
    }

    private static bool ContainsSegment(string text, string keyword)
    {
        var separators = new[]
        {
            '/',
            // '-',
            // '_',
            '.',
            '?',
            '=',
            '&'
        };

        var tokens = text
            .Split(separators, StringSplitOptions.RemoveEmptyEntries);

        return tokens.Any(x => 
            x.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }
}