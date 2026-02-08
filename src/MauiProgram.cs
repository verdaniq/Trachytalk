using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Trachytalk.Data;
using Trachytalk.Services;
using Trachytalk.ViewModels;

namespace Trachytalk;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			// Uncomment the following code to enable Sentry in release builds for testing
// #if DEBUG
// #else
// 			.UseSentry(options => {
// 				// The DSN is the only required setting.
// 				options.Dsn = "https://35e7b2917b27c508ffd3f7f4ff9db9c4@o4506416056500224.ingest.sentry.io/4506417486954496";
//
// 				// Use debug mode if you want to see what the SDK is doing.
// 				// Debug messages are written to stdout with Console.Writeline,
// 				// and are viewable in your IDE's debug console or with 'adb logcat', etc.
// 				// This option is not recommended when deploying your application.
// 				options.Debug = true;
// 				
// 				options.DiagnosticLevel = SentryLevel.Debug;
//
// 				// Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
// 				// We recommend adjusting this value in production.
// 				options.TracesSampleRate = 1.0;
//
// 				// Other Sentry options can be set here.
// 				options.EnableTracing = true;
//
// 				options.IsGlobalModeEnabled = true;
// 			})
// #endif
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Montserrat-VariableFont_wght.ttf", "Montserrat");
                fonts.AddFont("Montserrat-ExtraBold.ttf", "MontserratExtraBold");
				fonts.AddFont("FASolid.otf", "FontAwesomeSolid");
				fonts.AddFont("FluentSystemIcons-Filled.ttf", "FluentIcons");
            });

		builder.Services.AddSingleton<Database>();
        builder.Services.AddSingleton<IPhraseService, PhraseService>();

        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddSingleton<ILoggingService, DebugLoggingService>();
#else
		builder.Services.AddSingleton<ILoggingService, DebugLoggingService>();
		// use the following for Sentry builds
		//builder.Services.AddSingleton<ILoggingService, LoggingService>();
#endif

		return builder.Build();
	}
}
