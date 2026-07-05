using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.Sitemap.Interfaces;

public interface ISitemapDownloader
{
    Task<List<WebsiteLink>> DownloadSitemapAsync(string website);
}
