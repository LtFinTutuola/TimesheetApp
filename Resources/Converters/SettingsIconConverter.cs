using System.Globalization;

namespace TimesheetApp.Resources.Converters
{
    public class SettingsIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Straordinari" => "../../Resources/Images/Amounts/overtime.png",
                "Ritardo" => "../../Resources/Images/Amounts/late.png",
                "Banca ore" => "../../Resources/Images/Amounts/hourlybank.png",
                "Giorni lavorativi" => "../../Resources/Images/calendarday.png",
                "Tema" => "../../Resources/Images/theme.png",
                "Contatti" => "../../Resources/Images/info.png",
                "Ripristina" => "../../Resources/Images/reset.png",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
