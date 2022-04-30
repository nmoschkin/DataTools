using Microsoft.Maui.Controls.Compatibility;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DataTools.MAUI.ControlTest
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
                    fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSansSemibold.ttf", "OpenSansSemibold");
                });

            builder.ConfigureEffects(effects =>
            {
                var assemblies = Device.GetAssemblies();
                effects.AddCompatibilityEffects(assemblies);
            });

            return builder.Build();
        }
    }
}