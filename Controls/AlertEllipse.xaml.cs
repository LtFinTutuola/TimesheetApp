namespace TimesheetApp.Controls;

public partial class AlertEllipse : ContentView
{
    public static readonly BindableProperty FillProperty = BindableProperty.Create(
        nameof(Fill),
        typeof(Brush),
        typeof(AlertEllipse),
        defaultValue: Brush.Red);

    public Brush Fill
    {
        get { return (Brush)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public static readonly BindableProperty AlertProperty = BindableProperty.Create(
        nameof(Alert),
        typeof(object),
        typeof(AlertEllipse),
        defaultValue: null,
        propertyChanged: ShowHide);

    public object Alert
    {
        get { return GetValue(AlertProperty); }
        set { SetValue(AlertProperty, value); }
    }

    public AlertEllipse()
	{
		InitializeComponent();
	}

    public static void ShowHide(BindableObject bindable, object newValue, object oldValue)
    {
        var ellipse = (AlertEllipse)bindable;
        ellipse.ShowHideInternal();
    }

    private void ShowHideInternal()
    {
        Ellipse.Fill = Fill;
        if (Alert is int intAlert && intAlert == 0) Ellipse.IsVisible = true;
        else if (Alert is bool boolAlert && boolAlert) Ellipse.IsVisible = true;
        else Ellipse.IsVisible = false;
    }
}