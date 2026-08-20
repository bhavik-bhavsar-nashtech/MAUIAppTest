using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace MAUIAppTest
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // 1. Core Platform Service Registration
            builder.Services.AddSingleton<Services.DatabaseService>();

            // 2. MVVM Processing Engine Registration
            builder.Services.AddTransient<ViewModels.EmployeeViewModel>();

            // 3. UI Presentation Layer Registration
            builder.Services.AddTransient<EmployeePage>();
            builder.Services.AddTransient<LoginPage>();

            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnResume(activity =>
                    {
                        System.Diagnostics.Debug.WriteLine("Android Activity Resumed");
                    });
                });
#endif
            });

            return builder.Build();
        }
    }
}
