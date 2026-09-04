using Microsoft.Extensions.Logging;

namespace MauiApp4_elect4
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<Services.LocationService>();
            builder.Services.AddTransient<ViewModels.TrackOrderViewModel>();
            builder.Services.AddTransient<Views.TrackOrderPage>();

            return builder.Build();
        }
    }
}
