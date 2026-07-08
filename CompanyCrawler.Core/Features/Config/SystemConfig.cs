using System.Numerics;
using CompanyCrawler.Core.Features.Scoring.Models;

namespace CompanyCrawler.Core.Features.Config;

public static class SystemConfig
{
    public static int MaxLinksPerPage = 100;
    public static int MaxPageCrawlingInMinutes = 2;
    public static Vector2 CrawlDelayMs = new Vector2(300, 800);
    public static int PlaywrightDownloadAnchor = 150;
    public static int MaxPageLength = 550_000;
    
    public static class Browser
    {
        public static bool Headless = true;
        public static string[] Args = 
        [
            "--no-sandbox",
            "--disable-setuid-sandbox",
            "--disable-dev-shm-usage"
        ];
        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
        public static bool IgnoreHTTPSErrors = true;
        public static string WaitUntil = "DOMContentLoaded";
        public static int TimeoutMs = 30000;
        public static string[] MenuButtonSelectors = 
        [
            "button[aria-label*=menu i]",
            "button[class*=menu i]",
            "button[class*=hamburger i]",
            ".hamburger",
            ".menu-toggle",
            ".navbar-toggler"
        ];
        public static int MenuClickDelayMs = 500;
        public static int PageLoadDelayMs = 500;
    }

    public static class Paths
    {
        public static string DataFolder = "Data";
        public static string PresetsFolder = "Presets";
        public static string TagsFolder = "Tags";
        public static string InputFolder = "Input";
        public static string OutputFolder = "Output";
        public static string LogFolder = "Logs";
        public static string DownloadedHtmlFolder = "DownloadedHtml";
        public static string TagsFileName = "Tags.txt";
        public static string OutputFileName = "Output.csv";
    }

    public static class Scoring
    {
        public static int SameDomainScore = 500;
        public static int ExternalDomainPenalty = -500;
        public static int PathLengthPenalty = 4;
        public static int MinimumScore = -200;

        public static readonly LinkScoreRule[] Rules = 
        [
            new LinkScoreRule()
            {
                Score = 150,
                Keywords = new()
                {
                    "career",
                    "careers"
                }, 
            },
            new LinkScoreRule()
            {
                Score = 120,
                Keywords = new()
                {
                    "job",
                    "jobs",
                    "vacancy",
                    "vacancies"
                },
            },
            new LinkScoreRule()
            {
                Score = 100,
                Keywords = new()
                {
                    "team",
                    "people"
                },
            },
            new LinkScoreRule()
            {
                Score = 70,
                Keywords = new()
                {
                    "company",
                    "about"
                },
            },
            new LinkScoreRule()
            {
                Score = 50,
                Keywords = new()
                {
                    "contact"
                },
            },
        ];

        public static readonly LinkScoreRule[] PenaltyRules =
        [
            new LinkScoreRule()
            {
                Score = -80,
                Keywords = new()
                {
                    "blog",
                    "article",
                    "articles"
                },
            },
            new LinkScoreRule()
            {
                Score = -150,
                Keywords = new()
                {
                    "privacy",
                    "cookie",
                    "cookies",
                    "terms"
                },
            },
            new LinkScoreRule()
            {
                Score = -550,
                Keywords = new()
                {
                    "portfolio",
                    "project",
                    "projects",
                    "case-study",
                    "privacy",
                    "cookie",
                    "terms",
                    "press",
                    "news",
                    "article",
                    "blog",
                    "author",
                    "tag",
                    "category"
                },
            },
        ];
        
        public static string[] IgnoreExtensions = 
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".svg",
            ".gif",
            ".pdf",
            ".zip"
        ];
    }
}
