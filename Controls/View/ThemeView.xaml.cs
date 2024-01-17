namespace TimesheetApp.Controls.View;

public partial class ThemeView : SelectView
{
    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
            nameof(Theme),
            typeof(bool),
            typeof(ThemeView),
            false);

    public bool Theme
    {
        get => (bool)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public ThemeView()
	{
		InitializeComponent();
	}

    private new void SelectViewTapped(object sender, EventArgs e) => Command.Execute(Theme);
}