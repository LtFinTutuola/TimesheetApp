using System.Globalization;

namespace TimesheetApp.Resources.Converters
{
    public class WorkingDaySelectedCommand : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is List<DayOfWeek> weekdays)
            {
                return (string)values[1] switch
                {
                    "Lunedì" => weekdays.Contains(DayOfWeek.Monday),
                    "Martedì" => weekdays.Contains(DayOfWeek.Tuesday),
                    "Mercoledì" => weekdays.Contains(DayOfWeek.Wednesday),
                    "Giovedì" => weekdays.Contains(DayOfWeek.Thursday),
                    "Venerdì" => weekdays.Contains(DayOfWeek.Friday),
                    "Sabato" => weekdays.Contains(DayOfWeek.Saturday),
                    "Domenica" => weekdays.Contains(DayOfWeek.Sunday),
                    _ => false
                };
            }
            else return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
