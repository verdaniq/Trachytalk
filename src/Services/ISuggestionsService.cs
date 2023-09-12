namespace Trachytalk.Services;

internal interface ISuggestionsService
{
    Task<string> GetSuggestions(List<string> phrase, string currentWord);
    
}
