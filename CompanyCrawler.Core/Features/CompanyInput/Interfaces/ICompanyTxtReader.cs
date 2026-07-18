using CompanyCrawler.Core.Features.CompanyInput.Models;

namespace CompanyCrawler.Core.Features.CompanyInput.Interfaces;

public interface ICompanyTxtReader
{
    List<Company> ReadCompanies(string path);
}
