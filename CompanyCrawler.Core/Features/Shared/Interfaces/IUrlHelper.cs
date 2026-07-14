namespace CompanyCrawler.Core.Features.Shared.Interfaces;

public interface IUrlHelper
{
    string NormalizeWebsite(string website);
    bool TryGetHost(string website, out string host);
}
