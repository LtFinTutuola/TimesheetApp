using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class TimesheetNotesViewModel : BaseViewModel
    {        
        private readonly DailyTimesheet _timeSheet;

        [ObservableProperty]
        private string _notes;

        public TimesheetNotesViewModel(DatabaseContext context, DailyTimesheet timeSheet) : base(context)
        {
            _timeSheet = timeSheet;
            _notes = _timeSheet.Notes;
        }

        [RelayCommand]
        private async void ResetNotes()
        {
            if (await Alerts.ShowConfirm(Captions.TimesheetNotesResetConfirm)) SaveUpdateNotesInternal(string.Empty);
        }

        [RelayCommand]
        private void SaveNotes() => SaveUpdateNotesInternal(_notes);

        private async void SaveUpdateNotesInternal(string value)
        {
            await _timeSheet.SetNotes(value, _context);
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}
