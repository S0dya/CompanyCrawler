using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.LinkClassification.Config;

public class PageClassificationConfig
{
    public List<LinkCategoryRule> Rules { get; set; } =
    [
        new()
        {
            Category = LinkCategory.Career,

            Keywords =
            {
                "career",
                "careers",
                "job",
                "jobs",
                "vacancy",
                "vacancies",
                "opening",
                "openings",
                "position",
                "positions",
                "role",
                "roles",
                "join",
                "join-us",
                "join-us-team",
                "join-our-team",
                "work",
                "work-with-us",
                "hire",
                "hiring",
                "employment",
                "opportunity",
                "opportunities",
                "recruitment",
                "recruiting",
                "talent",
                "apply",
                "apply-now",
                "apply-online",
                "grow",
                "our-team",
                "team"
            }
        },

        new()
        {
            Category = LinkCategory.Contact,

            Keywords =
            {
                "contact",
                "contacts",
                "contact-us",
                "reach-us",
                "reach",
                "support",
                "help",
                "office",
                "offices",
                "location",
                "locations",
                "email",
                "get-in-touch",
                "connect",
                "connect-with-us"
            }
        },

        new()
        {
            Category = LinkCategory.Company,

            Keywords =
            {
                "about",
                "about-us",
                "company",
                "studio",
                "our-story",
                "story",
                "history",
                "mission",
                "vision",
                "values",
                "culture",
                "people",
                "leadership",
                "management",
                "executive",
                "founder",
                "founders",
                "who-we-are"
            }
        },

        new()
        {
            Category = LinkCategory.Portfolio,

            Keywords =
            {
                "portfolio",
                "projects",
                "project",
                "our-work",
                "work",
                "case-study",
                "case-studies",
                "cases",
                "showcase",
                "products",
                "product",
                "games",
                "game"
            }
        },

        new()
        {
            Category = LinkCategory.Services,

            Keywords =
            {
                "services",
                "service",
                "solutions",
                "solution",
                "expertise",
                "what-we-do",
                "development",
                "outsourcing"
            }
        },

        new()
        {
            Category = LinkCategory.Blog,

            Keywords =
            {
                "blog",
                "blogs",
                "article",
                "articles",
                "post",
                "posts",
                "insights",
                "resources"
            }
        },

        new()
        {
            Category = LinkCategory.News,

            Keywords =
            {
                "news",
                "press",
                "press-release",
                "press-releases",
                "media",
                "announcement",
                "announcements",
                "updates"
            }
        }
    ];
}