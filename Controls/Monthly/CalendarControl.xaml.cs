using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;
using TimesheetApp.ViewModel.Page;

namespace TimesheetApp.Controls.Monthly;

public partial class CalendarControl : MonthlyControl
{
    public static readonly BindableProperty MonthProperty = BindableProperty.Create(
        nameof(Month),
        typeof(DateTime),
        typeof(CalendarControl),
        defaultValue: DateTime.Now.Date);

    public DateTime Month
    {
        get { return (DateTime)GetValue(MonthProperty); }
        set { SetValue(MonthProperty, value); }
    }

    bool isBusy = false;
    public bool IsBusy
    {
        get { return isBusy; }
        set { isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    public CalendarControl()
	{
		InitializeComponent();
	}

	protected override void SetDataInternal()
	{
        Dispatcher.DispatchAsync(async () =>
        {
            IsBusy = true;
            await Task.Run(() => PopulateCalendar());
            IsBusy = false;
        });
    }

    private Task PopulateCalendar()
    {
        var days = CalendarViewModel.GetDays(Month.Year, Month.Month, true);
        var tSheets = TimeSheets;
        foreach (var day in days) if (!tSheets.Any(t => t.Today == day)) tSheets = tSheets.Append(new DailyTimesheet(day));

        AmountTimesheets = new(tSheets.OrderBy(t => t.Today).Select(t => new AmountTimesheet(t.Today.Month == Month.Month, new(t), t)));
        OnPropertyChanged(nameof(AmountTimesheets));

        return Task.CompletedTask;
    }
}