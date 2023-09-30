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
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Montserrat-VariableFont_wght.ttf", "Montserrat");
                fonts.AddFont("Montserrat-ExtraBold.ttf", "MontserratExtraBold");
				fonts.AddFont("FASolid.ocf", "FontAwesomeSolid");
            });

		builder.Services.AddSingleton<Database>();
        builder.Services.AddSingleton<IPhraseService, PhraseService>();

        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
