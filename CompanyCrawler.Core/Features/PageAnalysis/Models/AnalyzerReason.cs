namespace CompanyCrawler.Core.Features.PageAnalysis.Models;

public class AnalyzerReason
{
    public AnalyzerType Analyzer;

    public string Source = "";

    public string Reason = "";

    public int Score;

    public string Page = "";
}