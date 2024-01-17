using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;

namespace TimesheetApp.ViewModel
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected readonly DatabaseContext _context;

        public BaseViewModel(DatabaseContext context)
        {
            _context = context;
        }

        public virtual Task LoadDataAsync() { return Task.CompletedTask; }

        protected static async Task ExecuteAsync(Func<Task> operation)
        {
            try { await operation?.Invoke(); }
            catch { }
        }

        /// <summary> Static command to wrap PopAsync method </summary>
        [RelayCommand]
        public static async void GoBack() => await Shell.Current.Navigation.PopAsync();
    }
}
