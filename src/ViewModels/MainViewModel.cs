using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Trachytalk.Models;

namespace Trachytalk.ViewModels;

[INotifyPropertyChanged]
public partial class MainViewModel
{
    [ObservableProperty] private string currentWord;

    public ObservableCollection<Word> WordList { get; set; } = new();

    [RelayCommand]
    public void LetterPressed(string letter)
    {
        CurrentWord = $"{CurrentWord}{letter}";
    }
    
    [RelayCommand]
    private void SpacePressed()
    {
        WordList.Add(new Word(CurrentWord));
        CurrentWord = "";
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
            SpacePressed();

        string phrase = string.Empty;
        
        // speak the current word list using the TextToSpeech API in Essentials
        foreach (var word in WordList)
        {
            phrase += $"{word.Text} ";
        }
        
        await TextToSpeech.SpeakAsync(phrase, CancellationToken.None);
        
        // clear the word list
        WordList.Clear();
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
}