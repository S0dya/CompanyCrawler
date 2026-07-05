using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.PageAnalysis.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.PageAnalysis.Interfaces;

public interface IPageAnalyzer
{
    AnalyzerType Type { get; }

    void Analyze(List<ClassifiedPage> pages, CompanyProfile profile);
}
