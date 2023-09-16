using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Trachytalk.Models;
using Trachytalk.Services;

namespace Trachytalk.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string currentWord;

    public ObservableCollection<Word> WordList { get; set; } = new();

    public ObservableCollection<string> SuggestedWords { get; set; } = new();
    public ObservableCollection<string> SuggestedPhrases { get; set; } = new();

    public IPhraseService _phraseService { get; }

    private CancellationTokenSource _cts;

    public MainViewModel(IPhraseService phraseService)
    {
        _phraseService = phraseService;
    }

    [RelayCommand]
    public void LetterPressed(string letter)
    {
        CurrentWord = $"{CurrentWord}{letter}";
        
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        _cts = new CancellationTokenSource();

        Task.Delay(500, _cts.Token)
            .ContinueWith(async _ =>
            {
                await UpdateWordSuggestions();
            });
    }
    
    [RelayCommand]
    private async Task SpacePressed()
    {
        WordList.Add(new Word(CurrentWord));
        CurrentWord = "";
        SuggestedWords.Clear();

        await UpdatePhraseSuggestions();
    }
    
    [RelayCommand]
    private void BackspacePressed()
    {
        if (CurrentWord.Length > 0)
        {
            CurrentWord = CurrentWord.Substring(0, CurrentWord.Length - 1);
        }
    }

    [RelayCommand]
    private async Task SpeakPressed()
    {
        //Add the current word
        if (CurrentWord.Length > 0)
        {
            WordList.Add(new Word(CurrentWord));
        }

        string phrase = string.Empty;
        
        // speak the current word list using the TextToSpeech API in Essentials
        foreach (var word in WordList)
        {
            phrase += $"{word.Text} ";
        }
        
        await TextToSpeech.SpeakAsync(phrase, CancellationToken.None);

        await _phraseService.PhraseSelected(WordList.Select(x => x.Text).ToList());

        // clear the word list
        WordList.Clear();
        SuggestedWords.Clear();
        SuggestedPhrases.Clear();
    }

    [RelayCommand]
    private void RemoveWord(string id)
    {
        if (WordList.Any((w => w.Id == id)))
        {
            var word = WordList.FirstOrDefault(w => w.Id == id);
            WordList.Remove(word);
        }
    }

    private async Task UpdateWordSuggestions()
    {
        var suggestions = await _phraseService.GetSuggestions(CurrentWord);

        SuggestedWords.Clear();

        foreach (var suggestion in suggestions)
        {
            SuggestedWords.Add(suggestion);
        }
    }

    private async Task UpdatePhraseSuggestions()
    {
        var phrase = WordList.Select(w => w.Text).ToList();
        var suggestions = await _phraseService.GetSuggestions(phrase);

        SuggestedPhrases.Clear();

        foreach (var suggestion in suggestions)
        {
            SuggestedPhrases.Add(suggestion);
        }
    }
}