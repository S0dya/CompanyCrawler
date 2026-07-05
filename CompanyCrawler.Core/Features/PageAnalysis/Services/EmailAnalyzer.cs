using System.Text.RegularExpressions;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Services;

public class EmailAnalyzer(AnalyzerData data) : IPageAnalyzer
{
    private static readonly Regex EmailRegex = new(
        @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}",
        RegexOptions.Compiled);

    public void Analyze(List<DownloadedPage> pages, CompanyProfile profile)
    {
        foreach (var page in pages)
        {
            var matches = EmailRegex.Matches(page.VisibleText);
            
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
                    if (!address.Contains(keyword.Keyword, StringComparison.OrdinalIgnoreCase)) continue;

                    email.Score += keyword.Score;
                    
                    profile.Reasons.Add(new AnalyzerReason()
                    {
                        Analyzer = data.Type,
                        Score = keyword.Score
                    });
                    
                    
                }
            }
        }

        profile.Emails.Sort((a, b) => b.Score.CompareTo(a.Score));
    }
}
