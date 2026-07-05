using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.PageCrawling.Interfaces;

public interface IPageCrawler
{
    Task<List<DownloadedPage>> CrawlAsync(DownloadedPage homepage, List<PageCandidate> candidates);
}
