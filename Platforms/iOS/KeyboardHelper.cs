using UIKit;

namespace TimesheetApp.Platforms.iOS
{
    public static partial class KeyboardHelper
    {
        public static void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}
