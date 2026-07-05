namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class CompanyTag
{
    public string Name { get; set; } = string.Empty;
    
    public int Score { get; set; }
    
    public List<string> Reasons { get; } = [];
}
