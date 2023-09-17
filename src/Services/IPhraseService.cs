namespace Trachytalk.Services;

public interface IPhraseService
{
    void PhraseSelected(List<string> phrase);
    List<string> GetSuggestions(List<string> phrase);
    List<string> GetSuggestions(string inputText);
}
