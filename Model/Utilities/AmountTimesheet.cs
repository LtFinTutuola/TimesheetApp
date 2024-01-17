using TimesheetApp.Model.Entities;

namespace TimesheetApp.Model.Utilities
{
    public class AmountTimesheet
    {
        public bool IsCurrentMonth { get; set; }
        public List<AmountKind> Amounts { get; set; }
        public DailyTimesheet Timesheet { get; set; }

        public AmountTimesheet(bool isCurrentMonth, List<AmountKind> amounts, DailyTimesheet timesheet)
        {
            IsCurrentMonth = isCurrentMonth;
            Amounts = amounts;
            Timesheet = timesheet;
        }
    }
}
