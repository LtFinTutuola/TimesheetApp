namespace TimesheetApp.Model.Utilities
{
    public enum EmailType
    {
        HelpOrQuestion,
        Suggestion,
        Bug,
        Feedback,
        Other
    }
    public class EmailParameters
    {
        public EmailType EmailType { get; set; }
        public string Body { get; set; }
    }
}
