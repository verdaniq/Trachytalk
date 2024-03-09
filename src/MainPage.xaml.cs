using System;
using Sentry;
using Trachytalk.Services;
using Trachytalk.ViewModels;

namespace Trachytalk;

public partial class MainPage : ContentPage
{
	private MainViewModel _viewModel;
	private ILoggingService _loggingService;
	
	public MainPage(MainViewModel viewModel, ILoggingService loggingService)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_loggingService = loggingService;
	}
	private async void Button_OnClicked(object sender, EventArgs e)
	{
		try
		{
			var btn = (Button)sender;
			_viewModel.LetterPressed(btn.Text);
		}
		catch (Exception exception)
		{
			_loggingService.LogError(exception);
		}
	}

	private async void WordAdded(object sender, EventArgs e)
	{
		try
		{
			var lastItemIndex = _viewModel.WordList.Count - 1;
			WordListCollection.ScrollTo(lastItemIndex, position: ScrollToPosition.MakeVisible);
		}
		catch (Exception exception)
		{
			_loggingService.LogError(exception);
		}
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
