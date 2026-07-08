using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.Config;

public static class ExternalSitesConfig
{
    public static List<ExternalSiteRule> Rules { get; set; } =
    [
        new()
        {
            Type = ExternalSiteType.LinkedIn,

            Domains =
            {
                "linkedin.com",
                "linkedin.in",
                "linkedin.cn",
                "linkedin.co.uk",
                "linkedin.de",
                "linkedin.fr",
                "linkedin.es",
                "linkedin.it",
                "linkedin.nl",
                "linkedin.ca",
                "linkedin.au",
                "linkedin.jp"
            }
        },

        new()
        {
            Type = ExternalSiteType.Github,

            Domains =
            {
                "github.com",
                "github.io",
                "gist.github.com"
            }
        },

        new()
        {
            Type = ExternalSiteType.Upwork,

            Domains =
            {
                "upwork.com"
            }
        },

        new()
        {
            Type = ExternalSiteType.Glassdoor,

            Domains =
            {
                "glassdoor.com",
                "glassdoor.co.uk",
                "glassdoor.de",
                "glassdoor.fr",
                "glassdoor.ca",
                "glassdoor.au"
            }
        },

        new()
        {
            Type = ExternalSiteType.HH,

            Domains =
            {
                "hh.ru",
                "hh.kz",
                "hh.ua",
                "hh.uz",
                "hh.by",
                "hh Azerbaijan"
            }
        },

        new()
        {
            Type = ExternalSiteType.Indeed,

            Domains =
            {
                "indeed.com",
                "indeed.co.uk",
                "indeed.de",
                "indeed.fr",
                "indeed.es",
                "indeed.it",
                "indeed.nl",
                "indeed.ca",
                "indeed.com.au",
                "indeed.co.in",
                "indeed.co.jp",
                "indeed.com.br",
                "indeed.com.mx"
            }
        }
    ];
}

public class ExternalSiteRule
{
    public ExternalSiteType Type { get; set; }
    public List<string> Domains { get; set; } = [];
}