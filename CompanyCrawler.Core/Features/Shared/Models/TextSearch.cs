namespace CompanyCrawler.Core.Features.Shared.Models;

public class TextSearch : ITextSearch
{
    public bool ContainsKeyword(
        string[] textTokens,
        string[] keywordTokens)
    {
        if (keywordTokens.Length == 0)
            return false;

        for (var i = 0; i <= textTokens.Length - keywordTokens.Length; i++)
        {
            var matched = true;

            for (var j = 0; j < keywordTokens.Length; j++)
            {
                if (!textTokens[i + j].Equals(keywordTokens[j]))
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
                return true;
        }

        return false;
    }
    
    public string[] Tokenize(string text)
    {
        return text
            .ToLowerInvariant()
            .Split(
                [
                    ' ',
                    '\n',
                    '\r',
                    '\t',
                    '.',
                    ',',
                    ':',
                    ';',
                    '/',
                    '\\',
                    '-',
                    '_',
                    '(',
                    ')',
                    '[',
                    ']',
                    '{',
                    '}',
                    '"',
                    '\'',
                    '!',
                    '?'
                ],
                StringSplitOptions.RemoveEmptyEntries);
    }
}
