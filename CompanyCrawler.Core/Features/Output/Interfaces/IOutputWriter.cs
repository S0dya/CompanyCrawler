using CompanyCrawler.Core.Features.Output.Models;

namespace CompanyCrawler.Core.Features.Output.Interfaces;

public interface IOutputWriter
{
    void Write(string path, List<CompanyResult> companies);
}
