using TimesheetApp.Model.Entities;

namespace TimesheetApp;

public partial class App : Application
{
    public static IServiceProvider Services;

    public App(IServiceProvider services)
	{
        Services = services;
        InitializeComponent();
        MainPage = new AppShell();
        ApplyCurrentTheme();
    }

    public static void ApplyCurrentTheme()
    {
        var theme = Settings.GetCurrentSettings().Theme;
        if (theme) Current.UserAppTheme = AppTheme.Light;
        else Current.UserAppTheme = AppTheme.Dark;
    }    
}
