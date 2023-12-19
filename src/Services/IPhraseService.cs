namespace Trachytalk.Services;

public interface IPhraseService
{
    void PhraseSelected(List<string> phrase);
    void UpdatePhraseSuggestions(List<string> phrase);
    void UpdateWordSuggestions(string inputText);
    
    IObservable<string> PhraseSuggestionObservable { get; }
    IObservable<List<string>> WordSuggestionObservable { get; }
}
