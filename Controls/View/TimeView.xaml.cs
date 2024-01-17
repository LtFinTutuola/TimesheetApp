using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;

namespace TimesheetApp.Controls.View;

public partial class TimeView : BaseView
{
    public static readonly BindableProperty TimeProperty = BindableProperty.Create(
        nameof(Time),
        typeof(TimeSpan),
        typeof(TimeView),
        TimeSpan.Zero,
        BindingMode.TwoWay);

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public static readonly BindableProperty PickerTextProperty = BindableProperty.Create(
        nameof(PickerText),
        typeof(string),
        typeof(TimeView),
        default);

    public string PickerText
    {
        get => (string)GetValue(PickerTextProperty);
        set => SetValue(PickerTextProperty, value);
    }

    public TimeView()
    {
		InitializeComponent();
	}

    [RelayCommand]
    private async void SetTime()
    {
        var popup = new Popups.TimePickerPopup(new(Time.Ticks), PickerText ?? "Modifica orario");
        if (true.Equals(await Shell.Current.CurrentPage.ShowPopupAsync(popup))) Time = new(popup.Time.Ticks);
    }
}