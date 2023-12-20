using System.Reactive.Linq;
using System.Reactive.Subjects;
using Trachytalk.Data;

namespace Trachytalk.Services;

public class PhraseService : IPhraseService
{
    private readonly Database _database;
    private ILoggingService _loggingService;
    
    
    private readonly Subject<string> _phraseSuggestionSubject = new();
    public IObservable<string> PhraseSuggestionObservable => _phraseSuggestionSubject.AsObservable();
    
    
    private readonly Subject<List<string>> _wordSuggestionSubject = new();
    public IObservable<List<string>> WordSuggestionObservable => _wordSuggestionSubject.AsObservable();

    
    public PhraseService(Database database, ILoggingService loggingService)
    {
        _database = database;
        _loggingService = loggingService;
    }
    
    public void UpdatePhraseSuggestions(List<string> phrase)
    {
        try
        {
            var searchText = string.Join(" ", phrase);

            var suggestion = _database.GetTopPhrase(searchText);

            if (suggestion is null)
            {
                return;
            }
            
            _phraseSuggestionSubject.OnNext(suggestion.Text);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
            throw;
        }
    }

    public void UpdateWordSuggestions(string inputText)
    {
        try
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

            _wordSuggestionSubject.OnNext(results);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
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
                _database.AddOrUpdateEntry(word);
            }

            _database.AddOrUpdateEntry(inputText);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
            throw;
        }
    }
}
