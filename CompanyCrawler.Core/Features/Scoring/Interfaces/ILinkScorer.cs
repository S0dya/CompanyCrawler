using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.Scoring.Interfaces;

public interface ILinkScorer
{
    List<WebsiteLink> Sort(string homepageUrl, List<WebsiteLink> link);
}