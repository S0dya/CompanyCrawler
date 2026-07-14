namespace CompanyCrawler.Core.Features.Shared.Models;

public interface ITextSearch
{
    bool ContainsKeyword(string[] textTokens, string[] keywordTokens);
    string[] Tokenize(string text);
}
