using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimesheetApp.View.Navigation;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.ViewModel.Navigation;
using Microsoft.Maui.Handlers;
using TimesheetApp.Popups;
using CommunityToolkit.Maui.Views;

namespace TimesheetApp.ViewModel.Page
{
    public partial class HomePageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private DailyTimesheet _timeSheet;

        [ObservableProperty]
        private ObservableCollection<TimeStamp> _timeStamps;

        public HomePageViewModel(DatabaseContext context) : base(context) { }

        public HomePageViewModel(DatabaseContext context, DailyTimesheet timeSheet) : base(context)
        {
            TimeSheet = timeSheet;
        }

        #region Data
        public async override Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                await GetDataInternal(_timeSheet == null ? DateTime.Today : _timeSheet.Today);
            });
        }

        /// <summary>
        /// Load data, such as current timesheet and its timestamps, based on specified date
        /// </summary>
        private async Task GetDataInternal(DateTime date)
        {
            var timeSheet = await _context.GetFileteredAsync<DailyTimesheet>(t => t.Today == date);
            if (timeSheet != null && timeSheet.Any())
            {
                TimeSheet = timeSheet.FirstOrDefault();
                await TimeSheet.SetWorkshift();
            }
            else
            {
                // get default workshift, if exists assign it to timesheet
                var @default = (await _context.GetFileteredAsync<Workshift>(w => w.IsDefault)).FirstOrDefault();
                TimeSheet = @default == null ? new(date) : new(date, @default);
            }
            // Workaround to solve lambda expression bug on nested properties
            var ID = _timeSheet.ID;
            TimeStamps = new(await _context.GetFileteredAsync<TimeStamp>(t => t.TimesheetID == ID));
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async void ShowOptionsPopup()
        {
            var popup = new OptionsPopup();
            popup.SetAppearingOptions(TimeSheet);
            await Shell.Current.ShowPopupAsync(popup);

            switch (popup.Options)
            {
                case Options.ChangeWorkshift: ChangeWorkshift(); break;
                case Options.SetAsOvertime: SetOvertimeDay(); break;
                case Options.Delete: DeleteTimesheet(); break;
                case Options.Notes: ShowNotes(); break;
                case Options.None: return;
            }
        }

        /// <summary>
        /// push TimeStampViewModel and fill it with current time of day and expected stamp type
        /// </summary>
        [RelayCommand]
        public async Task AddNewTimeStamp(bool? execute)
        {
            if (!true.Equals(execute)) return;
            if (_timeSheet.Workshift == null)
            {
                await Alerts.ShowAlert(Captions.TimestampWsMissing);
                return;
            }
            var viewModel = new TimeStampViewModel(_context, _timeSheet, new TimeStamp(_timeSheet, GetStampType()));
            await Shell.Current.Navigation.PushAsync(new TimeStampPage(viewModel));
        }

        /// <summary>
        /// Command invoked by long pressing action button, it adds a new timestamp getting automatically stamp type
        /// </summary>
        [RelayCommand]
        public async Task AddDirectlyNewTimeStamp()
        {
            if (_timeSheet.Workshift == null) return;
            if (_timeStamps.Any() && _timeStamps.LastOrDefault().StampType == StampType.AfternoonExit) return;
            await ExecuteAsync(async () =>
            {
                if (_timeSheet.ID == 0) await _context.AddItemAsync(_timeSheet);
                var stamp = new TimeStamp(_timeSheet, GetStampType());

                if ((bool)await stamp.Validate(_timeStamps) && await _context.AddItemAsync(stamp)) TimeStamps.Add(stamp);
                else await Alerts.ShowError(Captions.TimestampFail);

                if (!await _timeSheet.SetAmounts(_context)) await Alerts.ShowError(Captions.TimesheetAmounsFail);
            });
        }

        /// <summary>
        /// show details of selected timestamp, such as its amounts and similar timestamps in current month
        /// </summary>
        [RelayCommand]
        public async Task ShowTimeStampDetails(TimeStamp stamp)
        {
            var viewModel = new TimeStampDetailViewModel(_context, _timeStamps.LastOrDefault().ID == stamp.ID, _timeSheet)
            {
                TimeStamp = stamp
            };
            await Shell.Current.Navigation.PushAsync(new TimeStampDetailPage(viewModel));
        }

        /// <summary>
        /// command used to wrap GetDataInternal and reload whole page 
        /// </summary>
        [RelayCommand]
        public async void ChangeDailyTimesheet(DateTime date) => await GetDataInternal(date);

        /// <summary>
        /// command used to open parameter datepicker from another button
        /// </summary>
        /// <param name="picker">DatePicker to open</param>
        [RelayCommand]
        public static void ForceDatepickerExecution(DatePicker picker)
        {
#if ANDROID
            var handler = picker.Handler as DatePickerHandler;
            handler.PlatformView.PerformClick();
#endif
        }
        #endregion

        /// <summary> push TimesheetNotesViewModel </summary>
        #region Options
        private async void ShowNotes()
        {
            var viewModel = new TimesheetNotesViewModel(_context, _timeSheet);
            await Shell.Current.Navigation.PushAsync(new TimesheetNotesPage(viewModel));
        }

        /// <summary> wrap _timeSheet.SetOvertimeDay and reload page </summary>
        private async void SetOvertimeDay()
        {
            await _timeSheet.SetOvertimeDay(!_timeSheet.IsOvertimeDay, _context);
            await GetDataInternal(_timeSheet.Today);
        }

        /// <summary> push WorkshiftRegistryViewModel </summary>
        [RelayCommand]
        private async void ChangeWorkshift()
        {
            var viewModel = new WorkshiftRegistryViewModel(_context, _timeSheet);
            await Shell.Current.Navigation.PushAsync(new WorkshiftRegistryPage(viewModel));
        }

        /// <summary> delete current timesheet and its timestamps, then reload page </summary>
        private async void DeleteTimesheet()
        {
            if (!await Alerts.ShowConfirm(Captions.TimesheetDeleteConfirm)) return;
            if (!await _context.DeleteItemAsync(_timeSheet))
            {
                await Alerts.ShowUnknownError();
                return;
            }
            foreach (var stamp in _timeStamps) await _context.DeleteItemAsync(stamp);
            await GetDataInternal(_timeSheet.Today);
        }
        #endregion

        /// <summary> get expected stamp type for next timestamp, based on alredy registered timestamps </summary>
        private StampType GetStampType()
        {
            var lastStamp = _timeStamps.LastOrDefault();
            if (lastStamp == null) return StampType.MorningEnter;

            if (lastStamp.StampType == StampType.MorningEnter && !_timeSheet.Workshift.HasLunchPause) return StampType.AfternoonExit;

            if (lastStamp.StampType == StampType.MorningEnter
                && DateTime.Now.TimeOfDay > _timeSheet.Workshift.LunchPauseStarting.Add(TimeSpan.FromMinutes(-20))
                && DateTime.Now.TimeOfDay < _timeSheet.Workshift.LunchPauseStarting.Add(TimeSpan.FromMinutes(20)))
                return StampType.StartLunchPause;

            if (lastStamp.StampType == StampType.StartLunchPause) return StampType.EndLunchPause;

            return StampType.AfternoonExit;
        }
    }
}
