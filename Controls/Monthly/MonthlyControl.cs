using System.Collections.ObjectModel;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;
using TimesheetApp.View;
using TimesheetApp.ViewModel.Page;

namespace TimesheetApp.Controls.Monthly
{
    public class MonthlyControl : ContentView
    {
        public static readonly BindableProperty TimeSheetsProperty = BindableProperty.Create(
            nameof(TimeSheets),
            typeof(IEnumerable<DailyTimesheet>),
            typeof(MonthlyControl),
            defaultValue: null,
            propertyChanged: SetData);

        public IEnumerable<DailyTimesheet> TimeSheets
        {
            get { return (IEnumerable<DailyTimesheet>)GetValue(TimeSheetsProperty); }
            set { SetValue(TimeSheetsProperty, value); }
        }

        private static void SetData(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MonthlyControl)bindable;
            if (newValue != null && newValue != oldValue) control.SetDataInternal();
        }

        protected virtual void SetDataInternal() { }

        public ObservableCollection<AmountTimesheet> AmountTimesheets { get; set; }

        protected void DaySelectedEventHandler(object sender, EventArgs e)
        {
            var ts = (sender as BindableObject).BindingContext as AmountTimesheet;
            App.Services.GetService<HomePageViewModel>().ChangeDailyTimesheet(ts.Timesheet.Today);

            ((AppShell)App.Current.MainPage).SwitchToTab(0);
        }
    }
}
