using CompanyCrawler.Core.Features.LinkClassification.Models;
using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.LinkClassification.Interfaces;

public interface ILinkClassifier
{
    List<ClassifiedPage> Classify(List<DownloadedPage> pages);
    List<WebsiteLink> ClassifyLinks(string homepageUrl, List<WebsiteLink> links);
}
