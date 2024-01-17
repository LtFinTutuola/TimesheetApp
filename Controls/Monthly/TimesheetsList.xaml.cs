using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Controls.Monthly;

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