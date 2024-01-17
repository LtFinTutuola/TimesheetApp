using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.View.Navigation;
using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.ViewModel.Page
{
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Settings _settings;

        public SettingsViewModel(DatabaseContext context) : base(context) { }

        public async override Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                Settings = Settings.GetCurrentSettings();
            });
        }

        /// <summary>
        /// push SettingsUpdatePage and pass it parameter to load proper template
        /// </summary>
        /// <param name="category"></param>
        [RelayCommand]
        private async void UpdateSettingsCategory(object category)
        {
            var viewModel = new SettingsUpdateViewModel(_context, (string)category, GetAction((string)category));
            await Shell.Current.Navigation.PushAsync(new SettingsUpdatePage(viewModel));
        }

        /// <summary>
        /// method used to get the string to show on action button, which is also ExecuteActionCommand's parameter
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        private static string GetAction(string category)
        {
            if (category == "Contatti") return "Contatta";
            else if (category == "Ripristina") return "Ripristina";
            else if (category == "Tema") return "Applica";
            else return "Salva";
        }
    }
}
