using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Services;
using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Services;
using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.LinkNormalization.Services;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.Output.Services;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Services;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.PageCrawling.Services;
using CompanyCrawler.Core.Features.Sitemap.Interfaces;
using CompanyCrawler.Core.Features.Sitemap.Services;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompanyCrawler;

public static class DependencyInjection
{
    public static async Task<IServiceProvider> ConfigureAsync()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(configure => configure.AddConsole());

        services.AddSingleton<ICompanyCsvReader, CompanyCsvReader>();
        services.AddSingleton<ILinkNormalizer, LinkNormalizer>();
        services.AddSingleton<ISitemapDownloader, SitemapDownloader>();
        services.AddSingleton<ILinkClassifier, LinkClassifier>();
        services.AddSingleton<IOutputWriter, OutputWriter>();

        services.AddSingleton<IPageAnalyzer, CareerAnalyzer>();
        services.AddSingleton<IPageAnalyzer, EmailAnalyzer>();
        services.AddSingleton<IPageAnalyzer, TagsAnalyzer>();

        var playwrightDownloader = await PlaywrightDownloader.CreateAsync();
        services.AddSingleton<IWebDownloader>(playwrightDownloader);

        services.AddScoped<IPageCrawler, PageCrawler>();
        
        services.AddSingleton(new AnalyzerCategoriesConfig());

        foreach (var data in AnalyzerConfig.Data)
        {
            services.AddSingleton<IPageAnalyzer>(new KeywordAnalyzer(data));
        }

        services.AddSingleton<IPageAnalyzer, EmailAnalyzer>(AnalyzerConfig.Data.First(x => x.Type == AnalyzerType.Email));

        return services.BuildServiceProvider();
    }
}
