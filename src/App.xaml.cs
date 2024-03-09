using Trachytalk.Services;

namespace Trachytalk;

public partial class App : Application
{
	private ILoggingService _loggingService;
	
	public App(MainPage page, ILoggingService loggingService)
	{
		_loggingService = loggingService;
		InitializeComponent();
		
		AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
		{
			var exception = (Exception)args.ExceptionObject;
			_loggingService.LogError(exception);
		};
		
		AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
		{
			_loggingService.LogError(args.Exception);
		};
		
		MainPage = page;
	}
}
