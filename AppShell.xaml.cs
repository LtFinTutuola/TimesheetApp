namespace TimesheetApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	public void SwitchToTab(int index)
	{
        MainThread.BeginInvokeOnMainThread(() =>
        {
            switch (index)
            {
                case 0: mainShell.CurrentItem = HomeTab; break;
                case 1: mainShell.CurrentItem = CalendarTab; break;
                case 2: mainShell.CurrentItem = SettingsTab; break;
            };
        });
    }
}
