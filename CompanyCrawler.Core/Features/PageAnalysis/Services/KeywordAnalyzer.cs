using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using Microsoft.Extensions.Logging;

namespace CompanyCrawler.Core.Features.PageAnalysis.Services;

public class KeywordAnalyzer(AnalyzerData data, ILogger<KeywordAnalyzer> logger) : IPageAnalyzer
{
    public void Analyze(List<ClassifiedPage> pages, CompanyProfile profile)
    {
        foreach (var classifiedPage in pages)
        {
            // if (!data.LinkCategories.Intersect(classifiedPage.Categories).Any()) continue;
            
            if ((data.Scope == AnalyzerScope.Internal && classifiedPage.IsExternal) || 
                (data.Scope == AnalyzerScope.External && !classifiedPage.IsExternal)) continue;

            var page = classifiedPage.Page;

            var score = 0;
            var keywords = new List<string>(); 
            
            foreach (var k in data.Keywords)
            {
                logger.LogInformation(
                    "'{Keyword}' Text={Text} Url={Url}",
                    k.Keyword,
                    k.SearchInText,
                    k.SearchInUrl);
            }
            
            foreach (var keyword in data.Keywords)
            {
                if ((keyword.SearchInText &&
                    page.VisibleText.Contains(keyword.Keyword, StringComparison.OrdinalIgnoreCase)) || 
                    (keyword.SearchInUrl &&
                    page.Url.Contains(keyword.Keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    score += keyword.Score;
                    
                    keywords.Add(keyword.Keyword);
                    
                    profile.Reasons.Add(new AnalyzerReason
                    {
                        Analyzer = data.Type,
                        Source = keyword.Keyword,
                        Reason = keyword.Reason,
                        Score = keyword.Score,
                        Page = page.Url
                    });
                }
            }
            
            if (score < data.Threshold) continue;

            if (!profile.Results.ContainsKey(data.Type))
            {
                profile.Results[data.Type] = new List<AnalyzerResult>();
            }
            
            var result = new AnalyzerResult()
            {
                Score = score,
                Page = page.Url,
                Keywords = keywords.ToArray(),
            };

            profile.Results[data.Type].Add(result);
        }
    }
}