namespace TimesheetApp.Controls.View;

public partial class SelectView : SelectableView
{
    public SelectView()
	{
		InitializeComponent();
	}

    protected override void SelectViewTapped(object sender, EventArgs e)
    {
        Selected = !Selected;
        OnPropertyChanged(nameof(Selected));
        Command.Execute(Description);
    }
}