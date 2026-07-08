using System.Text.RegularExpressions;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Services;

public class EmailAnalyzer(AnalyzerData data) : IPageAnalyzer
{
    private static readonly Regex EmailRegex = new(
        @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}",
        RegexOptions.Compiled);

    public void Analyze(List<ClassifiedPage> pages, CompanyProfile profile)
    {
        foreach (var classifiedPage in pages)
        {
            var page = classifiedPage.Page;
            
            var matches = EmailRegex.Matches(page.VisibleText);
            
            if ((data.Scope == AnalyzerScope.Internal && classifiedPage.IsExternal) || 
                (data.Scope == AnalyzerScope.External && !classifiedPage.IsExternal)) continue;
            
            foreach (Match match in matches)
            {
                var address = match.Value.ToLowerInvariant();

                if (profile.Emails.Any(x => x.Address == address))
                {
                    continue;
                }

                var email = new CompanyEmail
                {
                    Address = address
                };

                foreach (var keyword in data.Keywords)
                {
                    var local = address.Split('@')[0];
                    
                    if (!local.Contains(keyword.Keyword, StringComparison.OrdinalIgnoreCase)) continue;

                    email.Score += keyword.Score;
                    email.Reasons.Add(keyword.Reason);
                    
                    profile.Reasons.Add(new AnalyzerReason
                    {
                        Analyzer = data.Type,
                        Source = keyword.Keyword,
                        Reason = keyword.Reason,
                        Score = keyword.Score,
                        Page = page.Url
                    });
                }
                
                profile.Emails.Add(email);
            }
        }
    }
}
