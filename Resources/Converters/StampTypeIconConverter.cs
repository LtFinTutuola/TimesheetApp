using System.Globalization;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Resources.Converters
{
    public class StampTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stampType = (StampType)value;
            return stampType switch
            {
                StampType.MorningEnter => "../../Resources/Images/StampType/morningenter.svg",
                StampType.StartLunchPause => "../../Resources/Images/StampType/startlunchpause.svg",
                StampType.EndLunchPause => "../../Resources/Images/StampType/endlunchpause.svg",
                StampType.AfternoonExit => "../../Resources/Images/StampType/afternoonexit.svg",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
