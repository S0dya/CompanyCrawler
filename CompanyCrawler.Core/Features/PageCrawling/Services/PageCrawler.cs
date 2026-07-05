using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;
using Microsoft.Extensions.Logging;

namespace CompanyCrawler.Core.Features.PageCrawling.Services;

public class PageCrawler(IWebDownloader downloader, ILogger<PageCrawler> logger) : IPageCrawler
{
    public async Task<List<DownloadedPage>> CrawlAsync(DownloadedPage homepage, List<PageCandidate> candidates)
    {
        var pages = new List<DownloadedPage>
        {
            homepage
        };

        var urls = candidates
            .Select(x => x.Link.Url)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var url in urls)
        {
            try
            {
                logger.LogInformation("Crawling {Url}", url);

                var page = await downloader.DownloadAsync(url);

                pages.Add(page);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to crawl {Url}", url);
            }
        }

        return pages;
    }
}
