using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;
using TimesheetApp.Model.Entities;

[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]
namespace TimesheetApp;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        var theme = Settings.GetCurrentSettings().Theme;
        if (!theme) AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
