namespace TimesheetApp.Controls.View;

public partial class TextView : BaseView
{

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(BaseView),
        string.Empty,
        BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty MaxLenghtProperty = BindableProperty.Create(
        nameof(MaxLenght),
        typeof(int),
        typeof(TextView),
        50);

    public int MaxLenght
    {
        get => (int)GetValue(MaxLenghtProperty);
        set => SetValue(MaxLenghtProperty, value);
    }

    public static readonly BindableProperty IsMultiLineProperty = BindableProperty.Create(
        nameof(IsMultiLine),
        typeof(bool),
        typeof(TextView),
        false);

    public bool IsMultiLine
    {
        get => (bool)GetValue(IsMultiLineProperty);
        set => SetValue(IsMultiLineProperty, value);
    }


    public static readonly BindableProperty ShowLenghtProperty = BindableProperty.Create(
        nameof(ShowLenght),
        typeof(bool),
        typeof(TextView),
        false);

    public bool ShowLenght
    {
        get => (bool)GetValue(ShowLenghtProperty);
        set => SetValue(ShowLenghtProperty, value);
    }

    public TextView()
	{
		InitializeComponent();
    }
}