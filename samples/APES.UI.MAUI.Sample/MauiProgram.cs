using APES.UI.XF;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace APES.UI.MAUI.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			})
			.ConfigureContextMenuContainer();

		return builder.Build();
	}
}
