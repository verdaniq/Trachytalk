using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Trachytalk.Models;
using Trachytalk.Services;

namespace Trachytalk.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string currentWord;

    public ObservableCollection<Word> WordList { get; set; } = new();

    public ObservableCollection<string> Suggestions { get; set; } = new();

    public IPhraseService _phraseService { get; }

    private string _suggestedPhrase = string.Empty;
    private List<string> _suggestedWords = new();
    
    
    public MainViewModel(IPhraseService phraseService)
    {
        _phraseService = phraseService;
    }

    [RelayCommand]
    public void LetterPressed(string letter)
    {
        CurrentWord = $"{CurrentWord}{letter}";

        UpdatePhraseSuggestions();
        UpdateWordSuggestions();
    }
    
    [RelayCommand]
    private void SpacePressed()
    {
        WordList.Add(new Word(CurrentWord));
        CurrentWord = "";
        Suggestions.Clear();

        UpdatePhraseSuggestions();
    }
    
    [RelayCommand]
    private void BackspacePressed()
    {
        if (CurrentWord.Length > 0)
        {
            CurrentWord = CurrentWord.Substring(0, CurrentWord.Length - 1);
        }

        UpdateWordSuggestions();

        if (CurrentWord.Length == 0)
        {
            UpdatePhraseSuggestions();
        }
    }

    [RelayCommand]
    private async Task SpeakPressed()
    {
        if (CurrentWord.Length > 0)
        {
            WordList.Add(new Word(CurrentWord));
        }

        string phrase = string.Empty;
        
        foreach (var word in WordList)
        {
            phrase += $"{word.Text} ";
        }
        
        await TextToSpeech.SpeakAsync(phrase, CancellationToken.None);

        Observable.Start(() => _phraseService.PhraseSelected(WordList.Select(x => x.Text).ToList()))
            .SubscribeOn(TaskPoolScheduler.Default)
            .Subscribe();

        WordList.Clear();
        Suggestions.Clear();
    }

    [RelayCommand]
    private void RemoveWord(string id)
    {
        if (WordList.Any((w => w.Id == id)))
        {
            var word = WordList.FirstOrDefault(w => w.Id == id);
            WordList.Remove(word);
            UpdatePhraseSuggestions();
        }
    }

    [RelayCommand]
    private void SuggestionTapped(string suggestion)
    {
        if (string.IsNullOrWhiteSpace(suggestion))
        {
            return;
        }

        if (suggestion.Contains(' '))
        {
            var words = suggestion.Split(' ');
            
            WordList.Clear();
            
            foreach (var word in words)
            {
                WordList.Add(new Word(word));
            }
        }
        else
        {
            CurrentWord = suggestion;
        }

        _suggestedPhrase = string.Empty;
        _suggestedWords.Clear();
        Suggestions.Clear();
    }

    private void UpdateWordSuggestions()
    {
        Observable.Start(() => _phraseService.GetWordSuggestions(CurrentWord))
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestions =>
            {
                _suggestedWords.Clear();
                _suggestedWords.AddRange(suggestions);
                UpdateSuggestionsList();
            });
    }

    private void UpdatePhraseSuggestions()
    {
        var phrase = WordList.Select(w => w.Text).ToList();

        if (!string.IsNullOrEmpty(CurrentWord))
        {
            phrase.Add(CurrentWord); 
        }

        Observable.Start(() => _phraseService.GetPhraseSuggestions(phrase))
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestion =>
            {
                _suggestedPhrase = suggestion;
                UpdateSuggestionsList();
            });
    }

    private void UpdateSuggestionsList()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Suggestions.Clear();

            if (!string.IsNullOrWhiteSpace(_suggestedPhrase))
            {
                Suggestions.Add(_suggestedPhrase);
            }
            else
            {
                _suggestedPhrase = string.Empty;
            }

            foreach (var suggestion in _suggestedWords)
            {
                Suggestions.Add(suggestion);
            }
        });    
    }
}