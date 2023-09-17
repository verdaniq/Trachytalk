using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Trachytalk.Models;
using Trachytalk.Services;

namespace Trachytalk.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string currentWord;

    public ObservableCollection<Word> WordList { get; set; } = new();

    public ObservableCollection<string> Suggestions { get; set; } = new();

    public IPhraseService _phraseService { get; }
    
    
    public MainViewModel(IPhraseService phraseService)
    {
        _phraseService = phraseService;
    }

    [RelayCommand]
    public void LetterPressed(string letter)
    {
        CurrentWord = $"{CurrentWord}{letter}";

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

        Observable.FromAsync(() => Task.Run(() => _phraseService.PhraseSelected(WordList.Select(x => x.Text).ToList())))
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
        }
    }

    private void UpdateWordSuggestions()
    {
        Observable.FromAsync(() => Task.Run(() => _phraseService.GetSuggestions(CurrentWord)))
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestions =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Suggestions.Clear();

                    foreach (var suggestion in suggestions)
                    {
                        Suggestions.Add(suggestion);
                    }
                });
            });
    }

    private void UpdatePhraseSuggestions()
    {
        var phrase = WordList.Select(w => w.Text).ToList();

        Observable.FromAsync(() => Task.Run(() => _phraseService.GetSuggestions(phrase)))
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestions =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Suggestions.Clear();

                    foreach (var suggestion in suggestions)
                    {
                        Suggestions.Add(suggestion);
                    }
                });
            });
    }
}