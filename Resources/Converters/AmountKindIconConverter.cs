using System.Globalization;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Resources.Converters
{
    public class AmountKindIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var stampType = (AmountKind)value;
                return stampType switch
                {
                    AmountKind.Overtime => "../../Resources/Images/Amounts/overtime.png",
                    AmountKind.Late => "../../Resources/Images/Amounts/late.png",
                    AmountKind.HourlyBank => "../../Resources/Images/Amounts/hourlybank.png",
                    AmountKind.NegativeHourlyBank => "../../Resources/Images/Amounts/negativehourlybank.png",
                    AmountKind.NoAmount => "../../Resources/Images/Amounts/noamount.png",
                    AmountKind.Placeholder => "../../Resources/Images/clock.png",
                    AmountKind.OvertimeDay => "../../Resources/Images/Amounts/overtimeday.png",
                    AmountKind.Permit => "../../Resources/Images/Amounts/permit.png",
                    AmountKind.PendingJustify => "../../Resources/Images/Amounts/pendingjustify.png",
                    _ => ""
                };
            }
            catch { return ""; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
