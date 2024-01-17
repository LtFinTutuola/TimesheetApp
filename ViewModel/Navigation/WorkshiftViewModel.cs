using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class WorkshiftViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Workshift _workshift;

        public bool IsReferenced;

        public WorkshiftViewModel(DatabaseContext context, Workshift workshift) : base(context)
        {
            _workshift = workshift;
        }

        public override async Task LoadDataAsync() => await ExecuteAsync(async () => { });

        /// <summary> 
        /// command to wrap all available actions on main button click, depending on current workshift
        /// </summary>
        [RelayCommand]
        private async void SaveUpdateWorkshift()
        {
            if (_workshift.IsDefault) await UpdateDefaultWorkshift();
            if (_workshift.ID == 0) await SaveWorkshiftInternal();
            else await UpdateWorkshiftInternal();
        }

        /// <summary> delete current workshift if no timesheets uses it </summary>
        [RelayCommand]
        private async void DeleteWorkshift()
        {
            var timesheets = await _context.GetFileteredAsync<DailyTimesheet>(t => t.WorkshiftID == _workshift.ID);
            if(timesheets != null && timesheets.Any())
            {
                await Alerts.ShowError(Captions.WorkshiftInUse);
                return;
            }
            if (!await Alerts.ShowConfirm(Captions.WorkshiftDelete)) return;
            if (await _context.DeleteItemAsync(Workshift)) await Shell.Current.Navigation.PopToRootAsync();
            else await Alerts.ShowError(Captions.WorkshiftDeleteFail);
        }

        /// <summary> save current workshift and pop page </summary>
        private async Task SaveWorkshiftInternal()
        {
            if ((bool)!await _workshift.Validate(await _context.GetAllAsync<Workshift>())) return;
            if (await _context.AddItemAsync(_workshift)) await Shell.Current.Navigation.PopAsync();
            else await Alerts.ShowUnknownError();
        }

        /// <summary> 
        /// update current workshift and call UpdateCascadeTimesheets to update all timesheets are using it
        /// </summary>
        private async Task UpdateWorkshiftInternal()
        {
            if ((bool)!await _workshift.Validate(await _context.GetFileteredAsync<Workshift>(w => w.ID != _workshift.ID))) return;
            if (!await UpdateCascadeTimesheets())
            {
                await Alerts.ShowError(Captions.WorkshiftCascUpdateFail);
                return;
            }
            if (await _context.UpdateItemAsync(_workshift)) await Shell.Current.Navigation.PopAsync();
            else await Alerts.ShowUnknownError();
        }

        /// <summary> Set current workshift as default </summary>
        private async Task UpdateDefaultWorkshift()
        {
            var workshifts = await _context.GetFileteredAsync<Workshift>(w => w.ID != _workshift.ID && w.IsDefault);
            foreach(var workshift in workshifts)
            {
                workshift.IsDefault = false;
                await _context.UpdateItemAsync(workshift);
            }
        }

        /// <summary>
        /// Update all timesheets who has selected workshift to re-evaluate late, overtime and hourly bank
        /// </summary>
        private async Task<bool> UpdateCascadeTimesheets()
        {
            var timesheets = await _context.GetFileteredAsync<DailyTimesheet>(t => t.WorkshiftID == _workshift.ID);
            if (timesheets.Any() && !await Alerts.ShowConfirm(Captions.WorkshiftCascUpdate)) return false;
            foreach(var ts in timesheets) if (!await ts.ChangeWorkshift(_workshift, _context)) return false;
            return true;
        }
    }
}
