using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.Shared.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.LinkNormalization.Services;

public class LinkNormalizer(IUrlHelper urlHelper) : ILinkNormalizer
{

    public List<WebsiteLink> Normalize(string website, List<WebsiteLink> links)
    {
        if (!TryNormalizeWebsite(website, out var baseUri))
        {
            return [];
        }

        var result = new List<WebsiteLink>();

        var uniqueUrls = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var link in links)
        {
            if (string.IsNullOrWhiteSpace(link.Url))
            {
                continue;
            }

            var url = link.Url.Trim();

            if (url.StartsWith("#"))
            {
                continue;
            }

            if (url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            url = NormalizeUrl(url);

            Uri? absoluteUri = null;

            if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
            {
                absoluteUri = absolute;
            }
            else if (Uri.TryCreate(baseUri, url, out var relative))
            {
                absoluteUri = relative;
            }

            if (absoluteUri == null)
            {
                continue;
            }

            var normalizedUrl = absoluteUri.AbsoluteUri;

            if (!uniqueUrls.Add(normalizedUrl))
            {
                continue;
            }

            result.Add(new WebsiteLink
            {
                Url = normalizedUrl,
                Text = link.Text
            });
        }

        return result;
    }

    private static string NormalizeUrl(string url)
    {
        url = url.Trim();

        if (url.StartsWith("//"))
        {
            return $"https:{url}";
        }

        if (url.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        {
            return $"https://{url}";
        }

        var looksLikeDomain =
            url.Contains('.') &&
            !url.Contains('/') &&
            !url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        if (looksLikeDomain)
        {
            return $"https://{url}";
        }

        return url;
    }

    private bool TryNormalizeWebsite(
        string website,
        out Uri baseUri)
    {
        website = urlHelper.NormalizeWebsite(website);

        return Uri.TryCreate(
            website,
            UriKind.Absolute,
            out baseUri!);
    }
}