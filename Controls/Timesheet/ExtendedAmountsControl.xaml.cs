using System.Collections.ObjectModel;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Controls.Timesheet;

public partial class ExtendedAmountsControl : TimesheetControl
{
    //public static readonly BindableProperty AmountsProperty = BindableProperty.Create(
    //       nameof(Amounts),
    //       typeof(ObservableCollection<AmountsDictionary>),
    //       typeof(ExtendedAmountsControl));


    //public ObservableCollection<AmountsDictionary> Amounts
    //{
    //    get { return (ObservableCollection<AmountsDictionary>)GetValue(AmountsProperty); }
    //    set { SetValue(AmountsProperty, value); }
    //}

    //public static readonly BindableProperty HorizontalProperty = BindableProperty.Create(
    //       nameof(Horizontal),
    //       typeof(bool),
    //       typeof(ExtendedAmountsControl),
    //       defaultValue:false);

    //public bool Horizontal
    //{
    //    get { return (bool)GetValue(HorizontalProperty); }
    //    set { SetValue(HorizontalProperty, value); }
    //}
    public ObservableCollection<AmountsDictionary> Amounts { get; set; }

    public ExtendedAmountsControl()
	{
		InitializeComponent();
        BindingContext = this;
	}

    protected override void SetDataInternal(object value)
    {
        try
        {
            var tSheet = (DailyTimesheet)value;
            var rawAmounts = new List<AmountsDictionary>
            {
                new(AmountKind.Overtime, tSheet.OvertimeAmount.ToString("hh\\:mm")),
                new(AmountKind.Late, tSheet.LateAmount.ToString("hh\\:mm"))
            };

            if (Settings.GetCurrentSettings().HourlyBankOnLateExit)
                rawAmounts.Add(new(tSheet.HourlyBankAmount >= TimeSpan.Zero
                    ? AmountKind.HourlyBank : AmountKind.NegativeHourlyBank, tSheet.HourlyBankAmount.ToString("hh\\:mm")));

            //amountsList.ItemsSource = rawAmounts;
            Amounts = new(rawAmounts);
            OnPropertyChanged(nameof(Amounts));
        }
        catch { }
    }

    //public static void SetAmounts(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var amountsControl = (ExtendedAmountsControl)bindable;
    //    if (newValue != null && newValue != oldValue) amountsControl.SetAmountsInternal(newValue);
    //}

    //private void SetAmountsInternal(object value)
    //{
    //    try
    //    {
    //        //amountsList.ItemsSource = (IEnumerable<AmountsDictionary>)value;
    //        Amounts = new((IEnumerable<AmountsDictionary>)value);
    //        OnPropertyChanged(nameof(Amounts));
    //    }
    //    catch { }
    //}
}