using TimesheetApp.Model.Entities;

namespace TimesheetApp.Model.Utilities
{
    public struct AmountsDictionary
    {
        public AmountKind AmountKind { get; set; }
        public string Amount { get; set; }
        public AmountsDictionary(AmountKind amountKind, string amount)
        {
            AmountKind = amountKind;
            Amount = amount == "00:00"
                ? amount
                : (AmountKind == AmountKind.NegativeHourlyBank || AmountKind == AmountKind.Late
                    ? $"-{amount}"
                    : (AmountKind == AmountKind.NoAmount || AmountKind == AmountKind.Placeholder 
                    || AmountKind == AmountKind.PendingJustify
                        ? amount
                        : $"+{amount}"));
        }
    }
}
