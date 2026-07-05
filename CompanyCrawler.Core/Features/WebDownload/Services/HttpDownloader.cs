using System.Net;
using System.Net.Http.Headers;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.WebDownload.Services;

public class HttpDownloader : IWebDownloader
{
    private readonly HttpClient _httpClient;

    public HttpDownloader()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression =
                DecompressionMethods.GZip |
                DecompressionMethods.Deflate |
                DecompressionMethods.Brotli
        };

        _httpClient = new HttpClient(handler);

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(
            "en-US,en;q=0.9");

        _httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd(
            "gzip, deflate, br");
    }

    public async Task<DownloadedPage> DownloadAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        return new DownloadedPage
        {
            Url = response.RequestMessage!.RequestUri!.ToString(),
            Html = html,
            UsedBrowser = false
        };
    }
}
