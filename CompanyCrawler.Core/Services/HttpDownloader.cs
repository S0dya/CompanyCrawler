using System.Net;
using System.Net.Http.Headers;
using CompanyCrawler.Core.Models;

namespace CompanyCrawler.Core.Services;

public class HttpDownloader
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

    public async Task<DownloadedPage> DownloadWebsiteAsync(string website)
    {
        var response = await _httpClient.GetAsync(website);

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        return new DownloadedPage
        {
            Website = response.RequestMessage!.RequestUri!.ToString(),
            Html = html
        };
    }
}