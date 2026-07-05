using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.WebDownload.Interfaces;

public interface IWebDownloader
{
    Task<DownloadedPage> DownloadAsync(string url);
}
