using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Models;
using Microsoft.Playwright;

namespace CompanyCrawler.Core.Features.WebDownload.Services;

public class PlaywrightDownloader : IWebDownloader, IAsyncDisposable
{
    private readonly IPlaywright _playwright;

    private PlaywrightDownloader(IPlaywright playwright)
    {
        _playwright = playwright;
    }

    public static async Task<PlaywrightDownloader> CreateAsync()
    {
        var playwright = await Playwright.CreateAsync();
        return new PlaywrightDownloader(playwright);
    }

    public async Task<DownloadedPage> DownloadAsync(string url)
    {
        IBrowser? browser = null;
        IBrowserContext? context = null;
        IPage? page = null;
        string html = string.Empty;
        string finalUrl = url;

        try
        {
            browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage"
                }
            });

            context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                IgnoreHTTPSErrors = true
            });

            page = await context.NewPageAsync();

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });
            
            var menuButtons = new[]
            {
                "button[aria-label*=menu i]",
                "button[class*=menu i]",
                "button[class*=hamburger i]",
                ".hamburger",
                ".menu-toggle",
                ".navbar-toggler"
            };

            foreach (var selector in menuButtons)
            {
                var locator = page.Locator(selector);

                if (await locator.CountAsync() > 0)
                {
                    try
                    {
                        await locator.First.ClickAsync();
                        await page.WaitForTimeoutAsync(500);
                        break;
                    }
                    catch
                    {
                    }
                }
            }

            await Task.Delay(500);

            html = await page.ContentAsync();
            
            var links = new List<WebsiteLink>();

            var anchors = await page.Locator("a").AllAsync();

            foreach (var anchor in anchors)
            {
                var href = await anchor.GetAttributeAsync("href");

                if (string.IsNullOrWhiteSpace(href))
                {
                    continue;
                }

                var text = (await anchor.InnerTextAsync()).Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    text = await anchor.GetAttributeAsync("title") ?? "";
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    text = await anchor.GetAttributeAsync("aria-label") ?? "";
                }

                links.Add(new WebsiteLink
                {
                    Url = href,
                    Text = text
                });
            }
            
            finalUrl = page.Url;
            
            var visibleText = await page
                .Locator("body")
                .InnerTextAsync();
            
            var language = await page.EvaluateAsync<string>(
                "() => document.documentElement.lang || ''");

            return new DownloadedPage
            {
                Url = finalUrl,
                Html = html,
                Links = links,
                VisibleText = visibleText,
                UsedBrowser = true,
                Language = string.IsNullOrEmpty(language) ? "Unknown" : language,
            };
        }
        catch
        {
            return new DownloadedPage
            {
                Url = url,
                Html = string.Empty,
                UsedBrowser = true
            };
        }
        finally
        {
            if (page != null)
            {
                try
                {
                    await page.CloseAsync();
                }
                catch
                {
                    // Ignore
                }
            }

            if (context != null)
            {
                try
                {
                    await context.DisposeAsync();
                }
                catch
                {
                    // Ignore
                }
            }

            if (browser != null)
            {
                try
                {
                    await browser.DisposeAsync();
                }
                catch
                {
                    // Ignore
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _playwright.Dispose();
    }
}
