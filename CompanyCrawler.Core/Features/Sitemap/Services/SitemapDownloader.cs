using System.Xml.Linq;
using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.Sitemap.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.Sitemap.Services;

public class SitemapDownloader(CrawlPresetConfig preset) : ISitemapDownloader
{
    private readonly HttpClient _httpClient = new();
    
    public async Task<List<WebsiteLink>> DownloadSitemapAsync(string website)
    {
        try
        {
            var url = website.TrimEnd('/') + preset.CrawlSettings.SitemapPath;

            var xml = await _httpClient.GetStringAsync(url);

            var doc = XDocument.Parse(xml);

            return doc
                .Descendants()
                .Where(x => x.Name.LocalName == "loc")
                .Select(x => new WebsiteLink()
                {
                    Url = x.Value,
                    Text = "",
                })
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}
