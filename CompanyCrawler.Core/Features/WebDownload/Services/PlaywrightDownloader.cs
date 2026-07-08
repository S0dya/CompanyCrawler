using CompanyCrawler.Core.Features.Config;
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
                Headless = SystemConfig.Browser.Headless,
                Args = SystemConfig.Browser.Args
            });

            context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = SystemConfig.Browser.UserAgent,
                IgnoreHTTPSErrors = SystemConfig.Browser.IgnoreHTTPSErrors
            });

            page = await context.NewPageAsync();

            var waitUntil = SystemConfig.Browser.WaitUntil.ToLower() switch
            {
                "load" => WaitUntilState.Load,
                "domcontentloaded" => WaitUntilState.DOMContentLoaded,
                "networkidle" => WaitUntilState.NetworkIdle,
                _ => WaitUntilState.DOMContentLoaded
            };

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = waitUntil,
                Timeout = SystemConfig.Browser.TimeoutMs
            });

            foreach (var selector in SystemConfig.Browser.MenuButtonSelectors)
            {
                var locator = page.Locator(selector);

                if (await locator.CountAsync() > 0)
                {
                    try
                    {
                        await locator.First.ClickAsync();
                        await page.WaitForTimeoutAsync(SystemConfig.Browser.MenuClickDelayMs);
                        break;
                    }
                    catch
                    {
                    }
                }
            }

            await Task.Delay(SystemConfig.Browser.PageLoadDelayMs);

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
