using CommunityToolkit.Maui.Views;

namespace TimesheetApp.Popups;

public partial class TimePickerPopup : Popup
{
	public TimeSpan Time;

	public TimePickerPopup(TimeSpan time, string message)
	{
		InitializeComponent();
		SetPicker(time, message);
	}

	private void ConfirmEvent(object sender, EventArgs e)
	{
		Time = new TimeSpan(int.Parse(HourEntry.Text), int.Parse(MinuteEntry.Text), 0);

#if ANDROID
        Platforms.Android.KeyboardHelper.HideKeyboard();
#elif IOS
        Platforms.iOS.KeyboardHelper.HideKeyboard();
#endif
        Close(true);
	}

	private void SetPicker(TimeSpan time, string message)
	{
		HourEntry.Text = time.Hours.ToString().PadLeft(2, '0');
		MinuteEntry.Text = time.Minutes.ToString().PadLeft(2, '0');
		MessageLabel.Text = message;
	}

    private void HourEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = SetText(e.NewTextValue, 24, "01");
    }

    private void MinuteEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = SetText(e.NewTextValue, 60, "00");
    }

    /// <summary>
    /// parse text to remove dots and ensure that value doesn't exceed maxValue
    /// </summary>
    /// <param name="text">text to parse</param>
    /// <param name="maxValue">maximum value allowed, 23 for hours and 59 for minutes</param>
    /// <param name="default">default text to return if text is null or empty</param>
    /// <returns></returns>
    private static string SetText(string text, int maxValue, string @default)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
            var rawText = text.Length > 2 ? text.Remove(0, 1) : text;
            rawText = rawText.Replace(".", "");
            rawText = int.Parse(rawText) > maxValue ? rawText.Remove(0, 1) : rawText;

            return rawText.Length == 1 ? rawText.PadLeft(2, '0') : rawText;
        }
        else return @default;
    }
}