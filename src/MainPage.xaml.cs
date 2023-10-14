using System;
using Trachytalk.ViewModels;

namespace Trachytalk;

public partial class MainPage : ContentPage
{
	private MainViewModel _viewModel;
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

	private void Button_OnClicked(object sender, EventArgs e)
	{
		var btn = (Button)sender;
		_viewModel.LetterPressed(btn.Text);
	}

	private void WordAdded(object sender, EventArgs e)
	{
		var lastItemIndex = _viewModel.WordList.Count - 1;
		WordListCollection.ScrollTo(lastItemIndex, position: ScrollToPosition.MakeVisible);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.WordListChanged += WordAdded;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		_viewModel.WordListChanged -= WordAdded;
    }
}
