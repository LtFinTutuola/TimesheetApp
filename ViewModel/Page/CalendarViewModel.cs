using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;

namespace TimesheetApp.ViewModel.Page
{
    public partial class CalendarViewModel : BaseViewModel
    {
        [ObservableProperty]
        private DateTime _currentMonth = DateTime.Now.Date;

        [ObservableProperty]
        private ObservableCollection<DailyTimesheet> _timeSheets;

        [ObservableProperty]
        private ObservableCollection<AmountsDictionary> _amounts;

        public CalendarViewModel(DatabaseContext context) : base(context) { }

        public async override Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var monthDays = GetDays(_currentMonth.Year, _currentMonth.Month, false);
                var startRange = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
                var endRange = new DateTime(_currentMonth.Year, _currentMonth.Month, DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month));
                
                var stamps = (await _context.GetFileteredAsync<TimeStamp>(t => t.Stamp >= startRange && t.Stamp <= endRange)).Select(s => s.TimesheetID);
                TimeSheets = new(await _context.GetFileteredAsync<DailyTimesheet>(t => monthDays.Contains(t.Today) 
                                                                                    && t.Today <= DateTime.Today
                                                                                    && stamps.Contains(t.ID)));
                await GetMonthAmount();
            });
        }

        /// <summary> reload whole page based on new month to show </summary>
        [RelayCommand]
        public async void ChangeMonth(bool prevNext)
        {
            CurrentMonth = _currentMonth.AddMonths(prevNext ? 1 : -1);
            await LoadDataAsync();
        }

        /// <summary>
        /// get overtime amount for current month and current hourly bank amount, considering all previous month of current year
        /// </summary>
        private async Task GetMonthAmount()
        {
            var totOvertime = new TimeSpan(_timeSheets.Select(t => t.OvertimeAmount.Ticks).Sum());
            var totLate = new TimeSpan(_timeSheets.Select(t => t.LateAmount.Ticks).Sum());
            var rawAmounts = new List<AmountsDictionary>
            {
                new(AmountKind.Overtime, FormatAmount(new TimeSpan(_timeSheets.Select(t => t.OvertimeAmount.Ticks).Sum()))),
                new(AmountKind.Late, FormatAmount(new TimeSpan(_timeSheets.Select(t => t.LateAmount.Ticks).Sum())))
            };

            if (Settings.GetCurrentSettings().HourlyBankOnLateExit)
            {
                var startYear = new DateTime(CurrentMonth.Year, 1, 1);
                var currentMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month));
                var tsheets = await _context.GetFileteredAsync<DailyTimesheet>(t => t.Today <= currentMonth && t.Today >= startYear);
                var hrlBank = new TimeSpan(tsheets.Select(t => t.HourlyBankAmount.Ticks).Sum());
                rawAmounts.Add(new(hrlBank >= TimeSpan.Zero
                    ? AmountKind.HourlyBank : AmountKind.NegativeHourlyBank, FormatAmount(hrlBank)));
            }
            Amounts = new(rawAmounts);
        }

        /// <summary> Get days to show in calendar </summary>
        /// <param name="year">Year to consider</param>
        /// <param name="month">Month to consider</param>
        /// <param name="monthExtended">if true, returns 42 days to fill calendar view, otherwise returns only requested month timesheets</param>
        public static List<DateTime> GetDays(int year, int month, bool monthExtended)
        {
            var monthDays = Enumerable.Range(1, DateTime.DaysInMonth(year, month)).Select(day => new DateTime(year, month, day)).ToList();

            if(monthExtended)
            {
                while (monthDays.FirstOrDefault().DayOfWeek != DayOfWeek.Monday) monthDays.Insert(0, monthDays.FirstOrDefault().AddDays(-1));
                var remainingDays = 42 - monthDays.Count;

                for (int i = 1; i <= remainingDays; i++) monthDays.Add(monthDays.LastOrDefault().AddDays(1));
            }
            return monthDays;
        }

        private static string FormatAmount(TimeSpan amount)
        {
            return string.Format("{0}:{1}",
                     Convert.ToInt32(Math.Abs(amount.TotalHours)).ToString().PadLeft(2, '0'),
                     Convert.ToInt32(Math.Abs(amount.Minutes)).ToString().PadLeft(2, '0'));
        }
    }
}
