namespace TimesheetApp.Controls.View;

public partial class SwitchView : BaseView
{
    public static readonly BindableProperty SwitchProperty = BindableProperty.Create(
        nameof(Switch),
        typeof(bool),
        typeof(SwitchView),
        default,
        BindingMode.TwoWay);

    public bool Switch
    {
        get => (bool)GetValue(SwitchProperty);
        set => SetValue(SwitchProperty, value);
    }

    public SwitchView()
	{
		InitializeComponent();
	}
}