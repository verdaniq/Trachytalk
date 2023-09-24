using Trachytalk.Data;

namespace Trachytalk.Services;

public class PhraseService : IPhraseService
{
    private readonly Database _database;

    public PhraseService(Database database)
    {
        _database = database;
    }
    
    public string GetPhraseSuggestions(List<string> phrase)
    {
        var searchText = string.Join(" ", phrase);

        var suggestion = _database.GetTopPhrase(searchText);

        if (suggestion is not null)
        {
            return suggestion.Text;
        }

        return string.Empty;
    }

    public List<string> GetWordSuggestions(string inputText)
    {
        var suggestions = _database.GetMatchingWords(inputText);

        var results = new List<string>();

        foreach (var suggestion in suggestions)
        {
            if (!string.IsNullOrEmpty(suggestion?.Text))
            {
                results.Add(suggestion.Text);
            }
        }

        return results;
    }

    public void PhraseSelected(List<string> phrase)
    {
        var inputText = string.Join(" ", phrase);

        foreach (var word in phrase)
        {
            _database.AddOrUpdateEntry(word);
        }

        _database.AddOrUpdateEntry(inputText);
    }
}
