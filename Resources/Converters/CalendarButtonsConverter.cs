using System.Globalization;

namespace TimesheetApp.Resources.Converters
{
    public class CalendarButtonsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime month) return month.AddMonths((bool)parameter ? 1 : -1).ToString("MMMM yyyy");            
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
