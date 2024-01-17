using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;
using TimesheetApp.View.Navigation;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class TimeStampDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        private TimeStamp _timeStamp;

        [ObservableProperty]
        private ObservableCollection<TimeStamp> _timeStamps;

        [ObservableProperty]
        private ObservableCollection<AmountsDictionary> _timestampInfo = new();

        private readonly bool _canDelete;
        private readonly DailyTimesheet _timesheet;

        public TimeStampDetailViewModel(DatabaseContext context, bool canDelete, DailyTimesheet timesheet) : base(context)
        { 
            _canDelete = canDelete;
            _timesheet = timesheet;
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                TimeStamp = (await _context.GetFileteredAsync<TimeStamp>(t => t.ID == _timeStamp.ID)).FirstOrDefault();
                await Task.WhenAll(SetTimestampInfo(), GetTimestamps());
            });
        }

        /// <summary> push TimeStampViewModel to update or delete current timestamp </summary>
        [RelayCommand]
        private async void UpdateTimeStamp()
        {            
            var viewModel = new TimeStampViewModel(_context, _timesheet, (TimeStamp)_timeStamp.Clone())
            {
                CanDelete = _canDelete
            };
            await Shell.Current.Navigation.PushAsync(new TimeStampPage(viewModel));
        }

        /// <summary> push another instance of TimeStampDetailPage to show details of another timestamp </summary>
        [RelayCommand]
        private async void ChangeTimestampDetail(TimeStamp stamp)
        {
            var tsheet = (await _context.GetFileteredAsync<DailyTimesheet>(t => t.ID == stamp.TimesheetID)).FirstOrDefault();
            await tsheet.SetWorkshift();
            var viewModel = new TimeStampDetailViewModel(_context, _canDelete, tsheet)
            {
                TimeStamp = stamp
            };
            await Shell.Current.Navigation.PushAsync(new TimeStampDetailPage(viewModel));
        }

        /// <summary> set current timestamp's main info, such as expected time and amounts </summary>
        private async Task SetTimestampInfo()
        {
            var rawInfo = new List<AmountsDictionary>();
            var settings = Settings.GetCurrentSettings();

            switch (_timeStamp.StampType)
            {
                case StampType.MorningEnter:
                    var enter = _timesheet.Workshift.MorningEnter.ToString("hh\\:mm");
                    if (_timesheet.Workshift.HourlyFlex != TimeSpan.Zero) enter += $" - {_timesheet.Workshift.MorningEnter + _timesheet.Workshift.HourlyFlex:hh\\:mm}";

                    rawInfo.Add(new(AmountKind.Placeholder, enter));
                    rawInfo.AddRange(await GetTimestampAmounts(DailyTimesheet.GetMorningEnterAmounts, settings, _timeStamp, _timesheet));
                    break;
                case StampType.StartLunchPause:
                    rawInfo.Add(new(AmountKind.Placeholder, _timesheet.Workshift.LunchPauseStarting.ToString("hh\\:mm")));
                    rawInfo.Add(GetPauseTimeDifference(DailyTimesheet.GetLunchPauseStartingLate, settings));
                    break;
                case StampType.EndLunchPause:
                    rawInfo.Add(new(AmountKind.Placeholder, _timesheet.Workshift.LunchPauseEnding.ToString("hh\\:mm")));
                    rawInfo.Add(GetPauseTimeDifference(DailyTimesheet.GetLunchPauseEndingLate, settings));
                    break;
                case StampType.AfternoonExit:
                    // Workaround to solve lambda expression bug, to check in release mode
                    var ID = _timesheet.ID;
                    var morning = (await _context.GetFileteredAsync<TimeStamp>(t => t.TimesheetID == ID && t.StampType == StampType.MorningEnter)).FirstOrDefault();

                    rawInfo.Add(new(AmountKind.Placeholder, DailyTimesheet.GetEndOfWorkingDay(morning, _timesheet.Workshift).ToString("hh\\:mm")));
                    rawInfo.AddRange(await GetTimestampAmounts(DailyTimesheet.GetAfternoonExitAmounts, settings, _timeStamp, _timesheet));
                    break;
            }
            TimestampInfo = new(rawInfo);
        }

        /// <summary> method to wrap result of timestamp amount evaulation into a list of structures to show it </summary>
        private async Task<List<AmountsDictionary>> GetTimestampAmounts(Func<Settings, TimeStamp, Workshift, DatabaseContext, Task<Dictionary<AmountKind, TimeSpan>>> getAmounts, 
            Settings settings, TimeStamp stamp, DailyTimesheet tsheet)
        {
            var result = new List<AmountsDictionary>();
            var amounts = tsheet.IsOvertimeDay 
                ? new Dictionary<AmountKind, TimeSpan>() { { AmountKind.OvertimeDay, TimeSpan.Zero } }
                : await getAmounts(settings, stamp, tsheet.Workshift, _context);

            foreach (var amount in amounts) result.Add(new(amount.Key, amount.Value.ToString("hh\\:mm")));
            return result;
        }

        /// <summary> method to wrap result of pause timestamp amount evaulation into a structure to show it </summary>
        private AmountsDictionary GetPauseTimeDifference(Func<Settings, TimeStamp, Workshift, TimeSpan> getLate, 
            Settings settings, TimeStamp stamp = null)
        {
            var span = _timesheet.IsOvertimeDay 
                ? TimeSpan.Zero 
                : getLate(settings, stamp ?? _timeStamp, _timesheet.Workshift);
            var kind = _timesheet.IsOvertimeDay
                ? AmountKind.OvertimeDay 
                : (span > TimeSpan.Zero ? AmountKind.Late : AmountKind.NoAmount);
            return new(kind, span.ToString("hh\\:mm"));
        }

        /// <summary> get another timestamps of the same type as selected timestamp </summary>
        private async Task GetTimestamps()
        {
            var settings = Settings.GetCurrentSettings();
            var stamps = new List<TimeStamp>((await _context.GetFileteredAsync<TimeStamp>(t => t.StampType == _timeStamp.StampType))
                .Where(t => t.ID != _timeStamp.ID
                        && t.Stamp.Year == _timeStamp.Stamp.Year
                        && t.Stamp.Month == _timeStamp.Stamp.Month).OrderByDescending(t => t.Stamp));

            TimeStamps = new(stamps);
        }
    }
}
