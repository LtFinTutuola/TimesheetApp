using System.Globalization;
using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Resources.Converters
{
    public class EmailTypeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var emailType = (EmailType)value;
            return emailType switch
            {
                EmailType.HelpOrQuestion => "Domanda o aiuto",
                EmailType.Suggestion => "Future implementazioni",
                EmailType.Bug => "Bug o malfunzionamenti",
                EmailType.Feedback => "Feedback",
                EmailType.Other => "Altro",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
