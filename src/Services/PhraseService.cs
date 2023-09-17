using Trachytalk.Data;

namespace Trachytalk.Services;

public class PhraseService : IPhraseService
{
    private readonly Database _database;

    public PhraseService(Database database)
    {
        _database = database;
    }
    
    public List<string> GetSuggestions(List<string> phrase)
    {
        var searchText = string.Join(" ", phrase);

        var suggestions = _database.GetMatchingPhrases(searchText);

        var results = new List<string>();

        foreach (var suggestion in suggestions)
        {
            results.Add(suggestion.Text);
        }

        return results;
    }

    public List<string> GetSuggestions(string inputText)
    {
        var suggestions = _database.GetMatchingWords(inputText);

        var results = new List<string>();

        foreach (var suggestion in suggestions)
        {
            results.Add(suggestion.Text);
        }

        return results;
    }

    public void PhraseSelected(List<string> phrase)
    {
        var inputText = string.Join(" ", phrase);

        _database.AddOrUpdateEntry(inputText);
    }
}
