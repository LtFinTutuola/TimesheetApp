using CommunityToolkit.Maui.Alerts;

namespace TimesheetApp.Model.Context
{
    public static class Alerts
    {
        public static async Task ShowError(string caption, string header = null)
        {
            await Shell.Current.DisplayAlert(header ?? "Operazione annullata", caption, "OK");
        }

        public static async Task ShowUnknownError()
        {
            await Shell.Current.DisplayAlert("Operazione annullata", "Errore sconosciuto.", "OK");
        }

        public static async Task<bool> ShowConfirm(string caption, string header = null)
        {
            return await Shell.Current.DisplayAlert(header ?? "Conferma operazione", caption, "Conferma", "Annulla");
        }

        public static async Task ShowAlert(string caption, string header = null)
        {
            await Shell.Current.DisplayAlert(header ?? "Attenzione", caption, "OK");
        }
    }
}
