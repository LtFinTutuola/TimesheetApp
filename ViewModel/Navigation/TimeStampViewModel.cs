using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.Popups;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class TimeStampViewModel : BaseViewModel
    {
        [ObservableProperty]
        private TimeStamp _timeStamp;

        [ObservableProperty]
        private StampType[] _availableStamps;

        private readonly DailyTimesheet _timesheet;

        public bool CanDelete { get; set; }
        public bool ShowLunchPauseOptions { get; set; }

        public TimeStampViewModel(DatabaseContext context, DailyTimesheet timeSheet, TimeStamp stamp) : base(context)
        {
            TimeStamp = stamp;

            _timesheet = timeSheet;
            ShowLunchPauseOptions = timeSheet.Workshift.HasLunchPause;
            AvailableStamps = timeSheet.Workshift.GetAvailableStampTypes();
        }

        /// <summary>
        /// validate current stamp and select operation to perform based on validation result
        /// </summary>
        [RelayCommand]
        private async void SaveUpdateTimestamp()
        {
            var validation = await _timeStamp.Validate(await _context.GetFileteredAsync<TimeStamp>
                (t => t.TimesheetID == _timeStamp.TimesheetID && t.ID != _timeStamp.ID));

            switch (validation)
            {
                // validation is not succedeed, is not possible to add current timestamp
                case false: break;

                // is alredy registered a timestamp with timestamp's stamp type and user wants to overwrite it
                case null: 
                    await OverwriteTimestampInternal();
                    break;

                // validation succedeed, save timestamp if not exist or update it if is alredy registered
                case true:
                    if (_timeStamp.ID == 0) await SaveTimestampInternal();
                    else await UpdateTimestampInternal();
                    break;
            }
        }

        /// <summary> delete current timestamp and its timesheet's amounts </summary>
        [RelayCommand]
        private async void DeleteTimeStamp()
        {
            bool confirm = await Alerts.ShowConfirm(Captions.TimestampDelConfirm);
            if (confirm && await _context.DeleteItemAsync(_timeStamp))
            {
                await UpdateTimesheetAmounts();
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }

        /// <summary> open timepicker popup and set timestamp time </summary>
        [RelayCommand]
        private async void UpdateStampTime()
        {
            var popup = new TimePickerPopup(_timeStamp.Stamp.TimeOfDay, "Modifica timbratura");
            if (!true.Equals(await Shell.Current.CurrentPage.ShowPopupAsync(popup))) return;

            TimeStamp.Stamp = new(TimeStamp.Stamp.Date.Ticks + popup.Time.Ticks);
            OnPropertyChanged(nameof(TimeStamp));
        }

        /// <summary> update current timestamp and its timesheet's amounts </summary>
        private async Task SaveTimestampInternal()
        {
            if (_timesheet.ID == 0)
            {
                await _context.AddItemAsync(_timesheet);
                _timeStamp.TimesheetID = _timesheet.ID;
            }
            if (await _context.AddItemAsync(_timeStamp))
            {
                await UpdateTimesheetAmounts();
                await Shell.Current.Navigation.PopAsync();
            }
            else await Alerts.ShowError(Captions.TimestampFail);
        }

        /// <summary> update current timestamp and its timesheet's amounts </summary>
        private async Task UpdateTimestampInternal()
        {
            if (await _context.UpdateItemAsync(_timeStamp))
            {
                await UpdateTimesheetAmounts();
                await Shell.Current.Navigation.PopAsync();
            }
            else await Alerts.ShowError(Captions.TimestampUpdateFail);
        }

        /// <summary> 
        /// overwrite alredy registered timestamp, detecting and updating it; then update timesheet amounts
        /// </summary>
        private async Task OverwriteTimestampInternal()
        {
            var currentStamp = (await _context.GetFileteredAsync<TimeStamp>
                (t => t.TimesheetID == _timeStamp.TimesheetID && t.StampType == _timeStamp.StampType)).FirstOrDefault();
            if(currentStamp == null) await Alerts.ShowUnknownError();
            else
            {
                currentStamp.Stamp = _timeStamp.Stamp;
                if ((bool)await currentStamp.Validate(await _context.GetFileteredAsync<TimeStamp>
                    (t => t.TimesheetID == currentStamp.TimesheetID && t.ID != currentStamp.ID))
                    && await _context.UpdateItemAsync(currentStamp))
                {
                    await UpdateTimesheetAmounts();
                    await Shell.Current.Navigation.PopAsync();
                }
            }
        }

        /// <summary> method to wrap _timesheet.SetAmounts() </summary>
        private async Task UpdateTimesheetAmounts()
        {
            if (!await _timesheet.SetAmounts(_context)) await Alerts.ShowError(Captions.TimesheetAmounsFail);
        }
    }
}
