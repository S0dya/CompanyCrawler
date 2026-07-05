using CompanyCrawler.Core.Features.CompanyInput.Models;

namespace CompanyCrawler.Core.Features.CompanyInput.Interfaces;

public interface ICompanyCsvReader
{
    List<Company> ReadCompanies(string path);
}
