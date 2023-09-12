namespace Trachytalk.Services;

internal interface IPhraseService
{
    Task PhraseSelected(List<string> phrase);
    Task WordAdded(string inputText);
}
