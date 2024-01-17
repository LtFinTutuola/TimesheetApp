using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Controls.Timesheet;

public partial class ListAmountsControl : TimesheetControl
{
    public static readonly BindableProperty TimeStampProperty = BindableProperty.Create(
        nameof(TimeStamp),
        typeof(TimeStamp),
        typeof(ListAmountsControl),
        defaultValue: null,
        propertyChanged: SetAmounts);

    public TimeStamp TimeStamp
    {
        get { return (TimeStamp)GetValue(TimeStampProperty); }
        set { SetValue(TimeStampProperty, value); }
    }

    public static readonly BindableProperty IsCompactProperty = BindableProperty.Create(
       nameof(IsCompact),
       typeof(bool),
       typeof(ListAmountsControl),
       defaultValue: false);

    public bool IsCompact
    {
        get { return (bool)GetValue(IsCompactProperty); }
        set { SetValue(IsCompactProperty, value); }
    }

    public ListAmountsControl()
	{
        InitializeComponent();
    }

    protected override void SetDataInternal(object value)
    {
        if (TimeStamp != null) return;
        AmountsList.ItemsSource = (System.Collections.IEnumerable)((DailyTimesheet)value).GetEnumerator();
    }

    private static void SetAmounts(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue != oldValue && newValue != null)
        {
            var timeStampAmountsControl = (ListAmountsControl)bindable;
            timeStampAmountsControl.SetAmountsInternal(newValue);
        }
    }

    private async void SetAmountsInternal(object value)
    {
        await Task.Run(async () =>
        {
            var rawAmounts = new List<AmountKind>();
            var settings = Settings.GetCurrentSettings();
            var stamp = (TimeStamp)value;
            var tSheet = TimeSheet ?? await GetCurrentTimesheet(stamp.TimesheetID);

            if (tSheet.IsOvertimeDay) rawAmounts.Add(AmountKind.OvertimeDay);
            else
            {
                switch (stamp.StampType)
                {
                    case StampType.MorningEnter:
                        rawAmounts = (await DailyTimesheet.GetMorningEnterAmounts(settings, stamp, tSheet.Workshift)).Keys.ToList();
                        break;
                    case StampType.StartLunchPause:
                        if (DailyTimesheet.GetLunchPauseStartingLate(settings, stamp, tSheet.Workshift) != TimeSpan.Zero) rawAmounts.Add(AmountKind.Late);
                        break;
                    case StampType.EndLunchPause:
                        if (DailyTimesheet.GetLunchPauseEndingLate(settings, stamp, tSheet.Workshift) != TimeSpan.Zero) rawAmounts.Add(AmountKind.Late);
                        break;
                    case StampType.AfternoonExit:
                        rawAmounts = (await DailyTimesheet.GetAfternoonExitAmounts(settings, stamp, tSheet.Workshift)).Keys.ToList();
                        break;
                }
            }
            AmountsList.ItemsSource = rawAmounts.Where(a => a != AmountKind.NoAmount);
        });        
    }

    private static async Task<DailyTimesheet> GetCurrentTimesheet(int ID)
    {
        var context = new DatabaseContext();
        var tsheet = (await context.GetFileteredAsync<DailyTimesheet>(t => t.ID == ID)).FirstOrDefault();
        await tsheet.SetWorkshift();

        return tsheet;
    }
}