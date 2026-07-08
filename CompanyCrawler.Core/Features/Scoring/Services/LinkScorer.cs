using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.Scoring.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.Scoring.Services;

public class LinkScorer() : ILinkScorer
{
    public List<WebsiteLink> Sort(string homepageUrl, List<WebsiteLink> links)
    {
        var homeHost = new Uri(homepageUrl).Host;

        foreach (var link in links)
        {
            link.Score = Score(homeHost, link.Url);
        }

        return links
            .OrderByDescending(x => x.Score)
            .ToList();
    }

    private int Score(string homeHost, string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return int.MinValue;
        
        var score = 0;
        var path = uri.AbsolutePath;
        
        foreach (var rule in SystemConfig.Scoring.Rules)
        {
            foreach (var keyword in rule.Keywords)
                if (ContainsToken(path, keyword)) score += rule.Score;
        }
        foreach (var rule in SystemConfig.Scoring.PenaltyRules)
        {
            foreach (var keyword in rule.Keywords)
                if (ContainsToken(path, keyword)) score += rule.Score;
        }
        
        foreach (var ext in SystemConfig.Scoring.IgnoreExtensions)
        {
            if (uri.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                score -= 1000;
        }
        
        score -= uri.AbsolutePath.Length / SystemConfig.Scoring.PathLengthPenalty;

        return score;
    }
    
    private static bool ContainsToken(string text, string keyword)
    {
        var tokens = text
            .Split(['/', '-', '_', '.'], StringSplitOptions.RemoveEmptyEntries);

        return tokens.Any(x =>
            x.Equals(keyword, StringComparison.OrdinalIgnoreCase));
    }
}