using CommunityToolkit.Maui;
using Cs2DemoViewer.Maui.Services;
using Cs2DemoViewer.Maui.ViewModels;
using Cs2DemoViewer.Maui.Views;
using Microsoft.Extensions.Logging;

namespace Cs2DemoViewer.Maui;

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
            });

        // Регистрация сервисов
        builder.Services.AddSingleton<IDemoService, DemoService>();
        builder.Services.AddSingleton<IMapService, MapService>();

        // ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<DemoViewModel>();
        builder.Services.AddTransient<RoundViewModel>();

        // Views (Pages)
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DemoPage>();
        builder.Services.AddTransient<RoundPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
