using Trachytalk.Data;

namespace Trachytalk.Services;

public class PhraseService(Database database, ILoggingService loggingService) : IPhraseService
{
    private readonly State<string> _phraseSuggestionState = new(string.Empty);
    private readonly State<List<string>> _wordSuggestionState = new([]);
    
    public IDisposable SubscribeToPhraseSuggestions(Action<string> onChange)
        => _phraseSuggestionState.Subscribe(onChange);

    public IDisposable SubscribeToWordSuggestions(Action<List<string>> onChange)
        => _wordSuggestionState.Subscribe(onChange);
    
    public void UpdatePhraseSuggestions(List<string> phrase)
    {
        try
        {
            var searchText = string.Join(" ", phrase);

            var suggestion = database.GetTopPhrase(searchText);

            if (suggestion is null)
            {
                _phraseSuggestionState.SetValue(string.Empty);
                return;
            }
            
            _phraseSuggestionState.SetValue(suggestion.Text);
        }
        catch (Exception e)
        {
            loggingService.LogError(e);
            throw;
        }
    }

    public void UpdateWordSuggestions(string inputText)
    {
        try
        {
            var suggestions = database.GetMatchingWords(inputText);

            var results = (from suggestion in suggestions where !string.IsNullOrEmpty(suggestion?.Text) select suggestion.Text).ToList();

            _wordSuggestionState.SetValue(results);
        }
        catch (Exception e)
        {
            loggingService.LogError(e);
            throw;
        }
    }
    

    public void PhraseSelected(List<string> phrase)
    {
        var inputText = string.Join(" ", phrase);
        
        try
        {
            foreach (var word in phrase)
            {
                database.AddOrUpdateEntry(word);
            }

            database.AddOrUpdateEntry(inputText);
        }
        catch (Exception e)
        {
            loggingService.LogError(e);
            throw;
        }
    }
}
