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
}
