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

    private IPhraseService _phraseService { get; }
    private ILoggingService _loggingService;

    private string _suggestedPhrase = string.Empty;
    private List<string> _suggestedWords = new();

    public event EventHandler? WordListChanged;

    public MainViewModel(IPhraseService phraseService, ILoggingService loggingService)
    {
        _phraseService = phraseService;
        _loggingService = loggingService;
        StartSubscriptions();
    }

    private void StartSubscriptions()
    {
        // word subscriptions
        _loggingService.LogMessage("Subscribing to word suggestions...");
        _phraseService.WordSuggestionObservable
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestions =>
            {
                _loggingService.LogMessage("Received word suggestions. Updating suggestions list...");
                try
                {
                    _loggingService.LogMessage("Clearing existing suggestions...");
                    _suggestedWords.Clear();
                    _loggingService.LogMessage("Cleared.");
                    _loggingService.LogMessage("Adding new suggestions...");
                    _suggestedWords.AddRange(suggestions);
                    _loggingService.LogMessage("Added.");
                        
                    _loggingService.LogMessage("Updating suggestions list...");
                    UpdateSuggestionsList();
                }
                catch (Exception e)
                {
                    _loggingService.LogError(e);
                }
            }, error =>
            {
                _loggingService.LogMessage("ERROR: Failed to get word suggestions from subscription.");
                _loggingService.LogError(error);
            }, () =>
            {
                _loggingService.LogMessage("Completed.");
            });
        
        // phrase subscriptions
        
        _loggingService.LogMessage("Subscribing to phrase suggestions...");
        _phraseService.PhraseSuggestionObservable
            .SubscribeOn(TaskPoolScheduler.Default)
            .ObserveOn(Scheduler.Default)
            .Subscribe(suggestion =>
                {
                    _loggingService.LogMessage("Received phrase suggestion. Updating suggestions list...");
                    try
                    {
                        _loggingService.LogMessage("Setting suggested phrase...");
                        _suggestedPhrase = suggestion;
                        _loggingService.LogMessage("Set.");
                        
                        _loggingService.LogMessage("Updating suggestions list...");
                        UpdateSuggestionsList();
                    }
                    catch (Exception e)
                    {
                        _loggingService.LogError(e);
                    }
                },
                error =>
                {
                    _loggingService.LogMessage("ERROR: Failed to get phrase suggestion from subscription.");
                    _loggingService.LogError(error);
                },
                () =>
                {
                    _loggingService.LogMessage("Completed.");
                });
    }

    [RelayCommand]
    public async void LetterPressed(string letter)
    {
        try
        {
            CurrentWord = $"{CurrentWord}{letter}";

            if (WordList.Any(w => w.IsCurrentWord))
            {
                var word = WordList.FirstOrDefault(w => w.IsCurrentWord);
                WordList.Remove(word);
            }

            WordList.Add(new Word(CurrentWord, true));

            UpdatePhraseSuggestions();

            UpdateWordSuggestions();

            WordListChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
            throw;
        }
    }

    [RelayCommand]
    private void SpacePressed()
    {
        if (!string.IsNullOrWhiteSpace(CurrentWord))
        {
            if (WordList.Any(w => w.IsCurrentWord))
            {
                var word = WordList.FirstOrDefault(w => w.IsCurrentWord);
                WordList.Remove(word);
            }

            WordList.Add(new Word(CurrentWord));
            CurrentWord = "";
            Suggestions.Clear();

            UpdatePhraseSuggestions();
        }
    }

    [RelayCommand]
    private void BackspacePressed()
    {
        if (CurrentWord.Length > 0)
        {
            CurrentWord = CurrentWord.Substring(0, CurrentWord.Length - 1);
        }

        if (WordList.Any(w => w.IsCurrentWord))
        {
            var word = WordList.FirstOrDefault(w => w.IsCurrentWord);
            WordList.Remove(word);
        }

        WordList.Add(new Word(CurrentWord, true));

        UpdateWordSuggestions();

        if (CurrentWord.Length == 0)
        {
            UpdatePhraseSuggestions();
        }
    }

    [RelayCommand]
    private async Task SpeakPressed()
    {
        string phrase = string.Empty;

        foreach (var word in WordList)
        {
            phrase += $"{word.Text} ";
        }

        await TextToSpeech.SpeakAsync(phrase, CancellationToken.None);

        Task.Run(() =>
        {
            try
            {
                _phraseService.PhraseSelected(WordList.Select(x => x.Text).ToList());
            }
            catch (Exception e)
            {
                _loggingService.LogError(e);
            }
        });

        
        CurrentWord = string.Empty;
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
            CurrentWord = string.Empty;
        }
        else
        {
            CurrentWord = suggestion;
            SpacePressed();
        }

        _suggestedPhrase = string.Empty;
        _suggestedWords.Clear();
        Suggestions.Clear();
    }

    private async void UpdateWordSuggestions()
    {
        try
        {
            _phraseService.UpdateWordSuggestions(CurrentWord);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
        }
    }

    private void UpdatePhraseSuggestions()
    {
        try
        {
            _loggingService.LogMessage("Creating phrase from word list...");
            var phrase = WordList.Select(w => w.Text).ToList();
            _loggingService.LogMessage("Created.");

            if (!string.IsNullOrEmpty(CurrentWord))
            {
                _loggingService.LogMessage("Adding current word to phrase...");
                phrase.Add(CurrentWord);
                _loggingService.LogMessage("Added.");
            }
            
            _phraseService.UpdatePhraseSuggestions(phrase);
        }
        catch (Exception e)
        {
            _loggingService.LogError(e);
        }
    }

    private void UpdateSuggestionsList()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                _loggingService.LogMessage("Clearing suggestions list...");
                Suggestions.Clear();
                _loggingService.LogMessage("Cleared.");
            }
            catch (Exception e)
            {
                _loggingService.LogError(e);
            }

            if (!string.IsNullOrWhiteSpace(_suggestedPhrase))
            {
                _loggingService.LogMessage("Suggested phrase is not empty. Adding to suggestions list...");
                try
                {
                    Suggestions.Add(_suggestedPhrase);
                    _loggingService.LogMessage("Added.");
                }
                catch (Exception e)
                {
                    _loggingService.LogError(e);
                }
            }
            else
            {
                try
                {
                    _suggestedPhrase = string.Empty;
                }
                catch (Exception e)
                {
                    _loggingService.LogError(e);
                }
            }

            try
            {
                _loggingService.LogMessage("Creating word suggestions from suggestions list...");
                var wordsCopy = _suggestedWords.ToList();
                _loggingService.LogMessage("Created.");

                foreach (var suggestion in wordsCopy)
                {
                    _loggingService.LogMessage("Adding word suggestion to suggestions list...");
                    Suggestions.Add(suggestion);
                    _loggingService.LogMessage("Added.");
                }
            }
            catch (Exception e)
            {
                _loggingService.LogError(e);
            }
        });
    }
}
