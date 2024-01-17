using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.View.Navigation;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class WorkshiftRegistryViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<Workshift> _workshifts;

        [ObservableProperty]
        private Workshift _currentWorkshift;

        private readonly DailyTimesheet _currentTimesheet;

        public WorkshiftRegistryViewModel(DatabaseContext context, DailyTimesheet currentTimesheet) : base(context)
        {
            _currentWorkshift = currentTimesheet.Workshift ?? new Workshift();
            _currentTimesheet = currentTimesheet;
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var workshift = await _context.GetFileteredAsync<Workshift>(w => w.ID == _currentWorkshift.ID);
                if (workshift != null && workshift.Any()) CurrentWorkshift = workshift.FirstOrDefault();
                Workshifts = new(await _context.GetFileteredAsync<Workshift>(w => w.ID != _currentWorkshift.ID));
            });
        }

        /// <summary> push WorkshiftPage to create a new workshift </summary>
        [RelayCommand]
        private async void CreateNewWorkshift()
        {
            var viewModel = new WorkshiftViewModel(_context, new Workshift());
            await Shell.Current.Navigation.PushAsync(new WorkshiftPage(viewModel));
        }

        /// <summary> push WorkshiftPage and specify if is possible to delete selected workshift </summary>
        [RelayCommand]
        private async void UpdateDeleteWorkshift(Workshift workshift)
        {
            var viewModel = new WorkshiftViewModel(_context, (Workshift)workshift.Clone())
            {
                IsReferenced = (await _context.GetFileteredAsync<DailyTimesheet>(t => t.WorkshiftID == workshift.ID)).Any()
            };
            await Shell.Current.Navigation.PushAsync(new WorkshiftPage(viewModel));
        }

        /// <summary> assign selected workshift to current timesheet </summary>
        [RelayCommand]
        private async void AssignWorkshift(Workshift workshift)
        {
            if (await _currentTimesheet.ChangeWorkshift(workshift, _context)) await Shell.Current.Navigation.PopAsync(true);
            else await Alerts.ShowError(Captions.WorkshiftSetFail);
        }
    }
}
