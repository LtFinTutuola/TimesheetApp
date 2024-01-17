using TimesheetApp.Model.Context;
using CommunityToolkit.Maui;
using TimesheetApp.ViewModel.Navigation;
using TimesheetApp.View.Navigation;
using TimesheetApp.ViewModel.Page;
using TimesheetApp.View.Page;

namespace TimesheetApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("AlarmClock.ttf", "AlarmClock");
            });

        #region Pages
        builder.Services.AddSingleton<DatabaseContext>();

        builder.Services.AddSingleton<HomePageViewModel>();
        builder.Services.AddSingleton<HomePage>();

        builder.Services.AddSingleton<CalendarViewModel>();
        builder.Services.AddSingleton<CalendarPage>();

        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();
        #endregion

        #region Navigation
        builder.Services.AddSingleton<TimeStampDetailViewModel>();
        builder.Services.AddSingleton<TimeStampDetailPage>();

        builder.Services.AddSingleton<TimeStampViewModel>();
        builder.Services.AddSingleton<TimeStampPage>();

        builder.Services.AddSingleton<WorkshiftRegistryViewModel>();
        builder.Services.AddSingleton<WorkshiftRegistryPage>();

        builder.Services.AddSingleton<WorkshiftViewModel>();
        builder.Services.AddSingleton<WorkshiftPage>();

        builder.Services.AddSingleton<SettingsUpdatePage>();
        builder.Services.AddSingleton<SettingsUpdateViewModel>();

        builder.Services.AddSingleton<TimesheetNotesViewModel>();
        builder.Services.AddSingleton<TimesheetNotesPage>();
        #endregion

        return builder.Build();
    }
}
