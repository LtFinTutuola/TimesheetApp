using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Controls.Monthly;

/// <summary>
/// custom control to wrap monthly dailytimesheets list in CalendarPage CarouselView 
/// </summary>
public partial class TimesheetsList : MonthlyControl
{
	public TimesheetsList()
	{
		InitializeComponent();
	}

	protected override void SetDataInternal()
	{
        AmountTimesheets = new(TimeSheets.OrderBy(t => t.Today).Select(t => new AmountTimesheet(true, new(t), t)));
        OnPropertyChanged(nameof(AmountTimesheets));
    }
}