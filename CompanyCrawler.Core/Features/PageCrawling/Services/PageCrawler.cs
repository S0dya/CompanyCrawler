using System.Diagnostics;
using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.Scoring.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;
using Microsoft.Extensions.Logging;

namespace CompanyCrawler.Core.Features.PageCrawling.Services;

public class PageCrawler(
    IWebDownloader downloader,
    ILinkNormalizer linkNormalizer,
    ILogger<PageCrawler> logger,
    CrawlPresetConfig preset,
    ILinkScorer linkScorer,
    ILinkClassifier linkClassifier)
    : IPageCrawler
{
    private readonly int _maxDepth = preset.CrawlSettings.MaxDepth;
    private readonly int _maxPages = preset.CrawlSettings.MaxPages;

    public async Task<List<DownloadedPage>> CrawlAsync(
        DownloadedPage homepage,
        List<WebsiteLink> links, 
        CompanyProfile profile)
    {
        var pages = new List<DownloadedPage>
        {
            homepage
        };
        
        var homeHost = new Uri(homepage.Url).Host;

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            homepage.Url
        };

        var queue = new PriorityQueue<(WebsiteLink Link, int Depth), int>();
        
        links = linkScorer.Sort(homepage.Url, links);
        
        foreach (var link in links)
        {
            if (link.IsExternal) continue;
            
            queue.Enqueue((link, 1), -link.Score);
        }

        logger.LogInformation("Starting crawl with {QueueCount} links in queue", queue.Count);

        var processedCount = 0;

        var stopwatch = Stopwatch.StartNew();
        
        while (queue.Count > 0)
        {
            var (link, depth) = queue.Dequeue();
            if (pages.Count >= _maxPages) break;
            
            if (stopwatch.Elapsed > TimeSpan.FromMinutes(SystemConfig.MaxPageCrawlingInMinutes))
            {
                logger.LogInformation("Reached crawl timeout");
                break;
            }

            if (depth > _maxDepth)
            {
                continue;
            }

            if (!visited.Add(link.Url))
            {
                continue;
            }

            processedCount++;
            
            if (processedCount % 10 == 0 || queue.Count <= 5)
            {
                logger.LogInformation(
                    "Crawling progress: {ProcessedCount}/{TotalCount} pages, {QueueCount} links remaining in queue",
                    processedCount,
                    _maxPages,
                    queue.Count);
            }

            try
            {
                await Task.Delay(Random.Shared.Next((int)SystemConfig.CrawlDelayMs.X, (int)SystemConfig.CrawlDelayMs.Y));
                
                logger.LogDebug("Downloading page {PageNumber}: {Url}", processedCount, link.Url);
                var page = await downloader.DownloadAsync(link.Url);
                
                if (page.Html.Length > SystemConfig.MaxPageLength)
                {
                    logger.LogDebug("Skipping huge page ({Length} chars): {Url}", page.Html.Length, page.Url); 
                    continue;
                }
                
                pages.Add(page);

                logger.LogDebug(
                    "Downloaded {Url} - {CharacterCount} chars, {LinkCount} links found",
                    link.Url,
                    page.Html?.Length ?? 0,
                    page.Links.Count);

                if (depth == _maxDepth)
                {
                    continue;
                }

                var discoveredLinks = linkNormalizer
                    .Normalize(page.Url, page.Links)
                    .DistinctBy(x => x.Url)
                    .Take(SystemConfig.MaxLinksPerPage)
                    .ToList();

                discoveredLinks = linkClassifier.ClassifyLinks(homepage.Url, discoveredLinks);

                foreach (var externalLink in discoveredLinks.Where(x => x.IsExternal))
                {
                    profile.ExternalLinks.Add(new ExternalLink
                    {
                        Url = externalLink.Url,
                        Type = externalLink.ExternalType
                    });
                }

                var internalLinks = discoveredLinks
                    .Where(x => !x.IsExternal)
                    .Where(x =>
                    {
                        try
                        {
                            return new Uri(x.Url).Host.Equals(
                                homeHost,
                                StringComparison.OrdinalIgnoreCase);
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .ToList();

                internalLinks = linkScorer.Sort(page.Url, internalLinks);

                foreach (var discoveredLink in internalLinks)
                {
                    if (visited.Contains(discoveredLink.Url))
                        continue;

                    queue.Enqueue(
                        (discoveredLink, depth + 1),
                        -discoveredLink.Score);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Couldn't crawl {Url}",
                    link.Url);
            }

            if (pages.Count >= _maxPages)
            {
                logger.LogInformation("Reached max pages limit: {MaxPages}", _maxPages);
                break;
            }
        }

        logger.LogInformation("Crawl completed: {PageCount} pages downloaded", pages.Count);
        return pages;
    }
}