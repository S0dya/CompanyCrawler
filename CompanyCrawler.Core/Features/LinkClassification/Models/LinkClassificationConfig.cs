namespace CompanyCrawler.Core.Features.LinkClassification.Models;

public class PageClassificationConfig
{
    public List<LinkCategoryRule> Rules { get; } =
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
                "join",
                "join-us",
                "work",
                "hiring"
            }
        },

        new()
        {
            Category = LinkCategory.Contact,

            Keywords =
            {
                "contact",
                "contacts",
                "reach-us",
                "support"
            }
        },

        new()
        {
            Category = LinkCategory.Blog,

            Keywords =
            {
                "blog",
                "article",
                "articles"
            }
        },

        new()
        {
            Category = LinkCategory.News,

            Keywords =
            {
                "news"
            }
        },

        new()
        {
            Category = LinkCategory.Portfolio,

            Keywords =
            {
                "portfolio",
                "projects",
                "case-study"
            }
        },

        new()
        {
            Category = LinkCategory.Services,

            Keywords =
            {
                "services",
                "solutions"
            }
        },

        new()
        {
            Category = LinkCategory.Company,

            Keywords =
            {
                "company",
                "about"
            }
        }
    ];
}