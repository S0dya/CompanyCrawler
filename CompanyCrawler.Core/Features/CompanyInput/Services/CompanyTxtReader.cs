using CompanyCrawler.Core.Features.CompanyInput.Interfaces;
using CompanyCrawler.Core.Features.CompanyInput.Models;

namespace CompanyCrawler.Core.Features.CompanyInput.Services;

public class CompanyTxtReader : ICompanyTxtReader
{
    public List<Company> ReadCompanies(string path)
    {
        var lines = File.ReadAllLines(path);
        var companies = new List<Company>();

        foreach (var line in lines)
        {
            var url = line.Trim();
            if (!string.IsNullOrWhiteSpace(url))
            {
                companies.Add(new Company { Website = url });
            }
        }

        return companies;
    }
}
