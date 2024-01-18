using System.Collections.ObjectModel;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Controls.Timesheet;

/// <summary>
/// custom control to wrap dailytimesheet amounts list in HomePage CarouselView
/// </summary>
public partial class ExtendedAmountsControl : TimesheetControl
{
    public ObservableCollection<AmountsDictionary> Amounts { get; set; } = new();

    public ExtendedAmountsControl()
	{
		InitializeComponent();
	}

    protected override void SetDataInternal(object value)
    {
        try
        {
            var tSheet = (DailyTimesheet)value;
            var rawAmounts = new List<AmountsDictionary>
            {
                new(AmountKind.Overtime, tSheet.OvertimeAmount.ToString("hh\\:mm")),
                new(AmountKind.Late, tSheet.LateAmount.ToString("hh\\:mm"))
            };

            if (Settings.GetCurrentSettings().HourlyBankOnLateExit)
                rawAmounts.Add(new(tSheet.HourlyBankAmount >= TimeSpan.Zero
                    ? AmountKind.HourlyBank : AmountKind.NegativeHourlyBank, tSheet.HourlyBankAmount.ToString("hh\\:mm")));

            Amounts = new(rawAmounts);
            OnPropertyChanged(nameof(Amounts));
        }
        catch { }
    }
}