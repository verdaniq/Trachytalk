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
}

