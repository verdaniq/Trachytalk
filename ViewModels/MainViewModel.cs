using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Trachytalk.ViewModels;

[INotifyPropertyChanged]
public partial class MainViewModel
{
    [ObservableProperty] private string currentWord;

    public ObservableCollection<string> WordList { get; set; } = new();
    
    [RelayCommand]
    private void SpacePressed()
    {
        WordList.Add(CurrentWord);
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
    private void SpeakPressed()
    {
        // speak the current word list using the TextToSpeech API in Essentials
        
        // clear the word list
        WordList.Clear();
    }
}