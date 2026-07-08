using CompanyCrawler.Core.Features.PageAnalysis.Models;

namespace CompanyCrawler.Core.Features;

public static class TagsLoader
{
    public static List<KeywordRule> Load(string path)
    {
        var lines = File.ReadAllLines(path)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var score = 100;

        var result = new List<KeywordRule>();

        foreach (var line in lines)
        {
            result.Add(new KeywordRule
            {
                Keyword = line.Trim(),
                Score = score,
                Reason = $"Matched tag '{line}'"
            });

            score = Math.Max(score - 5, 10);
        }

        return result;
    }
}