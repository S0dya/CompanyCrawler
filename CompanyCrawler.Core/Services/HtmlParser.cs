using AngleSharp;
using CompanyCrawler.Core.Models;

namespace CompanyCrawler.Core.Services;

public class HtmlParser
{
    public async Task<List<WebsiteLink>> GetLinksAsync(string html)
    {
        var context = BrowsingContext.New(Configuration.Default);

        var document = await context.OpenAsync(req => req.Content(html));

        var result = new List<WebsiteLink>();

        foreach (var link in document.QuerySelectorAll("a"))
        {
            var href = link.GetAttribute("href");

            if (string.IsNullOrWhiteSpace(href))
            {
                continue;
            }

            var text = link.TextContent.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                text = link.GetAttribute("title") ?? "";
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = link.GetAttribute("aria-label") ?? "";
            }

            result.Add(new WebsiteLink
            {
                Url = href,
                Text = text
            });
        }

        return result;
    }
}