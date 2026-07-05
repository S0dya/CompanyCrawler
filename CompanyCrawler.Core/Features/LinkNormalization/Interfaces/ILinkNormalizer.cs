using CompanyCrawler.Core.Features.WebDownload.Models;

namespace CompanyCrawler.Core.Features.LinkNormalization.Interfaces;

public interface ILinkNormalizer
{
    List<WebsiteLink> Normalize(string website, List<WebsiteLink> links);
}
