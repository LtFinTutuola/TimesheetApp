namespace TimesheetApp.Controls.View;

public partial class ThemeView : SelectableView
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

    protected override void SelectViewTapped(object sender, EventArgs e) => Command.Execute(Theme);
}