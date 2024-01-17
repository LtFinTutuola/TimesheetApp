using CommunityToolkit.Maui.Views;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Popups;

public enum Options
{
	ChangeWorkshift,
	SetAsOvertime,
	Delete,
	Notes,
	None
}

public partial class OptionsPopup : Popup
{
	public Options Options { get; set; } = Options.None;

	public OptionsPopup()
	{
		InitializeComponent();
	}

	public void SetAppearingOptions(DailyTimesheet tsheet)
	{
		workshiftAlert.Alert = tsheet.WorkshiftID == 0;
		deleteLayout.IsEnabled = tsheet.ID != 0;
		deleteLayout.Opacity = deleteLayout.IsEnabled ? 1 : 0.4;
		SetOvertimeTab(tsheet.IsOvertimeDay);
	}

	private void WorkshiftLayoutTap(object sender, EventArgs e) => CloseWithParameters(Options.ChangeWorkshift);

	private void OvertimeLayoutTap(object sender, EventArgs e) => CloseWithParameters(Options.SetAsOvertime);
    
	private void NotesLayoutTap(object sender, EventArgs e) => CloseWithParameters(Options.Notes);

    private void DeleteLayoutTap(object sender, EventArgs e) => CloseWithParameters(Options.Delete);

	private void CloseWithParameters(Options selected)
	{
		Options = selected;
		Close();
	}

	private void SetOvertimeTab(bool isOvertime)
	{
		if(!isOvertime)
		{
			OvertimeLabel.Text = "Straordinari";
			OvertimeInfoLabel.Text = Captions.TimesheetSetAsOvertimeInfo;
		}
		else
		{
			OvertimeLabel.Text = "Giornata lavorativa";
			OvertimeInfoLabel.Text = Captions.TimesheetRemoveOvertimeInfo;
		}
	}
}