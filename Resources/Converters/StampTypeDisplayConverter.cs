using System.Globalization;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Resources.Converters
{
    public class StampTypeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stampType = (StampType)value;
            return stampType switch
            {
                StampType.MorningEnter => "Entrata",
                StampType.StartLunchPause => "Inizio pausa pranzo",
                StampType.EndLunchPause => "Fine pausa pranzo",
                StampType.AfternoonExit => "Uscita",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
