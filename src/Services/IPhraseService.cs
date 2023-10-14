namespace Trachytalk.Services;

public interface IPhraseService
{
    void PhraseSelected(List<string> phrase);
    string GetPhraseSuggestions(List<string> phrase);
    List<string> GetWordSuggestions(string inputText);
}
