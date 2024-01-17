namespace TimesheetApp.Controls.View;

public partial class SelectView : BaseView
{
    public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
            nameof(Selected),
            typeof(bool),
            typeof(SelectView),
            false);

    public bool Selected
    {
        get => (bool)GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }

    public SelectView()
	{
		InitializeComponent();
	}

    protected void SelectViewTapped(object sender, EventArgs e)
    {
        Selected = !Selected;
        OnPropertyChanged(nameof(Selected));
        Command.Execute(Description);
    }
}