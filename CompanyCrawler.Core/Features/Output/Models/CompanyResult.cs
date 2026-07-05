namespace CompanyCrawler.Core.Features.Output.Models;

public class CompanyResult
{
    public string CompanyName { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public bool IsHiring { get; set; }

    public string Tags { get; set; } = string.Empty;

    public string CareerPage { get; set; } = string.Empty;

    public string BestEmail { get; set; } = string.Empty;

    public string AllEmails { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;
}
