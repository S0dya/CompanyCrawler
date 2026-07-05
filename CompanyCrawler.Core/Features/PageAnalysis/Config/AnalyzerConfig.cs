using CompanyCrawler.Core.Features.LinkClassification.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class AnalyzerConfig
{
    public static readonly List<AnalyzerData> Data =
    [
        new()
        {
            Type = AnalyzerType.Career,
            Threshold = 120,

            Keywords =
            [
                new(){ Keyword="career", Score=80 },
                new(){ Keyword="careers", Score=80 },
                new(){ Keyword="vacancy", Score=80 },
                new(){ Keyword="vacancies", Score=80 },
                new(){ Keyword="job", Score=60 },
                new(){ Keyword="jobs", Score=60 },
                new(){ Keyword="join us", Score=80 },
                new(){ Keyword="we are hiring", Score=100 },
                new(){ Keyword="apply now", Score=80 }
            ],
            
            LinkCategories = 
            [
                LinkCategory.Career,
                LinkCategory.Company,
                LinkCategory.Contact,
                LinkCategory.Blog
            ],
        },

        new()
        {
            Type = AnalyzerType.Email,

            Keywords =
            [
                new() { Keyword = "jobs", Score = 100 },
                new() { Keyword = "career", Score = 100 },
                new() { Keyword = "hr", Score = 90 },
                new() { Keyword = "recruit", Score = 90 },
                new() { Keyword = "talent", Score = 80 },
                new() { Keyword = "hello", Score = 60 },
                new() { Keyword = "contact", Score = 50 },
                new() { Keyword = "info", Score = 40 },
                new() { Keyword = "support", Score = 10 },
            ]
        },
        
        new()
        {
            Type = AnalyzerType.Tags,
            Threshold = 100,

            Keywords =
            [
                new(){ Keyword="unity", Score=100 },
                new(){ Keyword="unity3d", Score=100 },
                new(){ Keyword="game developer", Score=80 },
                new(){ Keyword="c#", Score=40 }
            ],
            
            LinkCategories = 
            [
                LinkCategory.Company,
                LinkCategory.Services,
                LinkCategory.Portfolio,
                LinkCategory.Blog
            ],
        },

        new()
        {
            Type = AnalyzerType.WorkFormat,
            Threshold = 100,

            Keywords =
            [
                new(){ Keyword="remote", Score=100 },
                new(){ Keyword="work from home", Score=100 },
                new(){ Keyword="hybrid", Score=60 },
                new(){ Keyword="Office", Score=40 },
                new(){ Keyword="On-site", Score=40 },
            ],

            LinkCategories = 
            [
                LinkCategory.Company,
                LinkCategory.Services,
                LinkCategory.Portfolio,
                LinkCategory.Blog
            ],
        }
    ];
}