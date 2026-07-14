using CompanyCrawler.Core.Features.Shared.Interfaces;

namespace CompanyCrawler.Core.Features.Shared.Services;

public class UrlHelper : IUrlHelper
{
    public string NormalizeWebsite(string website)
    {
        website = website.Trim();

        if (website.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        {
            website = $"https://{website}";
        }

        var looksLikeDomain =
            website.Contains('.') &&
            !website.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !website.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        if (looksLikeDomain)
        {
            website = $"https://{website}";
        }

        return website;
    }

    public bool TryGetHost(string website, out string host)
    {
        host = "";

        website = NormalizeWebsite(website);

        if (!Uri.TryCreate(website, UriKind.Absolute, out var uri))
        {
            return false;
        }

        host = uri.Host.Replace(".", "_");

        return true;
    }
}
