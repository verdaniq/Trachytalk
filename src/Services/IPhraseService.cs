namespace Trachytalk.Services;

public interface IPhraseService
{
    void PhraseSelected(List<string> phrase);
    void UpdatePhraseSuggestions(List<string> phrase);
    void UpdateWordSuggestions(string inputText);

    IDisposable SubscribeToPhraseSuggestions(Action<string> onChange);
    IDisposable SubscribeToWordSuggestions(Action<List<string>> onChange);
}
