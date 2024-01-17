using System.Globalization;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Resources.Converters
{
    public class AmountKindDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stampType = (AmountKind)value;
            return stampType switch
            {
                AmountKind.Overtime => "Straordinari",
                AmountKind.Late => "Ritardo",
                AmountKind.HourlyBank => "Banca ore",
                AmountKind.NegativeHourlyBank => "Banca ore",
                AmountKind.NoAmount => "In orario",
                AmountKind.Placeholder => "Orario prev.",
                AmountKind.OvertimeDay => "Giornata str.",
                AmountKind.Permit => "Permesso",
                AmountKind.PendingJustify => "Da giustificare",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
