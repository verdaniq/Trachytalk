using System;
using Sentry;
using Trachytalk.Services;

namespace Trachytalk;

public partial class App : Application
{
	private ILoggingService _loggingService;
	
	public App(MainPage page, ILoggingService loggingService)
	{
		InitializeComponent();
		
		AppDomain.CurrentDomain.UnhandledException += async (sender, args) =>
		{
			var exception = (Exception)args.ExceptionObject;
			_loggingService.LogError(exception);
			await page.DisplayAlert("Error", exception.Message, "OK");
			await page.DisplayAlert("Error", exception.StackTrace, "OK");
		};
		
		AppDomain.CurrentDomain.FirstChanceException += async (sender, args) =>
		{
			_loggingService.LogError(args.Exception);
			await page.DisplayAlert("Error", args.Exception.Message, "OK");
			await page.DisplayAlert("Error", args.Exception.StackTrace, "OK");
		};
		
		MainPage = page;
	}
}
