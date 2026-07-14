using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Services;
using CompanyCrawler.Core.Features.Config;
using CompanyCrawler.Core.Features.LinkClassification.Config;
using CompanyCrawler.Core.Features.LinkClassification.Interfaces;
using CompanyCrawler.Core.Features.LinkClassification.Services;
using CompanyCrawler.Core.Features.LinkNormalization.Interfaces;
using CompanyCrawler.Core.Features.LinkNormalization.Services;
using CompanyCrawler.Core.Features.Output.Interfaces;
using CompanyCrawler.Core.Features.Output.Services;
using CompanyCrawler.Core.Features.PageAnalysis.Interfaces;
using CompanyCrawler.Core.Features.PageAnalysis.Services;
using CompanyCrawler.Core.Features.PageCrawling.Interfaces;
using CompanyCrawler.Core.Features.PageCrawling.Services;
using CompanyCrawler.Core.Features.Scoring.Interfaces;
using CompanyCrawler.Core.Features.Scoring.Services;
using CompanyCrawler.Core.Features.Shared.Interfaces;
using CompanyCrawler.Core.Features.Shared.Models;
using CompanyCrawler.Core.Features.Shared.Services;
using CompanyCrawler.Core.Features.Sitemap.Interfaces;
using CompanyCrawler.Core.Features.Sitemap.Services;
using CompanyCrawler.Core.Features.WebDownload.Interfaces;
using CompanyCrawler.Core.Features.WebDownload.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompanyCrawler;

public static class DependencyInjection
{
    public static async Task<IServiceProvider> ConfigureAsync(CrawlPresetConfig preset)    
    {
        var services = new ServiceCollection();

        services.AddSingleton(preset);
        
        services.AddLogging(configure => 
        {
            configure.AddConsole();
            // configure.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddSingleton<ICompanyCsvReader, CompanyCsvReader>();
        services.AddSingleton<ILinkNormalizer, LinkNormalizer>();
        services.AddSingleton<ISitemapDownloader, SitemapDownloader>();
        services.AddSingleton<ILinkClassifier, LinkClassifier>();
        services.AddSingleton<IOutputWriter, OutputWriter>();
        services.AddSingleton<ILinkScorer, LinkScorer>();   
        services.AddSingleton<ITextSearch, TextSearch>();
        services.AddSingleton<IUrlHelper, UrlHelper>();

        var playwrightDownloader = await PlaywrightDownloader.CreateAsync();
        services.AddSingleton<IWebDownloader>(playwrightDownloader);

        services.AddScoped<IPageCrawler, PageCrawler>();
        
        services.AddSingleton(new PageClassificationConfig
        {
            Rules = preset.LinkCategories
        });

        foreach (var analyzer in preset.KeywordAnalyzersData)
        {
            services.AddSingleton<IPageAnalyzer>(sp => 
                new KeywordAnalyzer(analyzer, sp.GetRequiredService<ILogger<KeywordAnalyzer>>(), sp.GetRequiredService<ITextSearch>()));
        }

        services.AddSingleton<IPageAnalyzer>(new EmailAnalyzer(preset.EmailAnalyzerData));

        return services.BuildServiceProvider();
    }
}
