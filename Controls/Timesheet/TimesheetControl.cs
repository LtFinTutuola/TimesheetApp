using TimesheetApp.Model.Entities;

namespace TimesheetApp.Controls.Timesheet
{
    public class TimesheetControl : ContentView
    {
        public static readonly BindableProperty TimeSheetProperty = BindableProperty.Create(
            nameof(TimeSheet),
            typeof(DailyTimesheet),
            typeof(TimesheetControl),
            defaultValue: null,
            propertyChanged: SetData);

        public DailyTimesheet TimeSheet
        {
            get { return (DailyTimesheet)GetValue(TimeSheetProperty); }
            set { SetValue(TimeSheetProperty, value); }
        }

        private static void SetData(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (TimesheetControl)bindable;
            if (newValue != null && newValue != oldValue) control.SetDataInternal(newValue);
        }

        protected virtual void SetDataInternal(object value) { }
    }
}
