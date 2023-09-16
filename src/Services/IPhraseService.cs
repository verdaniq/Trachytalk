namespace Trachytalk.Services;

public interface IPhraseService
{
    Task PhraseSelected(List<string> phrase);
    Task<List<string>> GetSuggestions(List<string> phrase);
    Task<List<string>> GetSuggestions(string inputText);
}
