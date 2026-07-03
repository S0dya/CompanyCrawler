using CompanyCrawler.Core.Models;

namespace CompanyCrawler.Core.Services;

public class LinkNormalizer
{
    public List<WebsiteLink> Normalize(string website, List<WebsiteLink> links)
    {
        var baseUri = new Uri(website);
        var result = new List<WebsiteLink>();
        var uniqueUrls = new HashSet<string>();

        foreach (var link in links)
        {
            if (string.IsNullOrWhiteSpace(link.Url)) continue;

            var url = link.Url.Trim();

            if (url.StartsWith("#")) continue;

            if (url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) continue;

            if (url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)) continue;

            if (url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)) continue;

            var absoluteUri = Uri.TryCreate(url, UriKind.Absolute, out var absolute)
                ? absolute
                : new Uri(baseUri, url);

            if (!uniqueUrls.Add(absoluteUri.AbsoluteUri)) continue;

            result.Add(new WebsiteLink
            {
                Url = absoluteUri.AbsoluteUri,
                Text = link.Text
            });
        }

        return result;
    }
}