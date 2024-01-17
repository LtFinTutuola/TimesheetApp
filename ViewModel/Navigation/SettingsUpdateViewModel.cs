using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Entities;
using TimesheetApp.Model.Utilities;
using TimesheetApp.ViewModel.Page;

namespace TimesheetApp.ViewModel.Navigation
{
    public partial class SettingsUpdateViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Settings _settings;

        public string Title { get; set; }
        public string Action { get; set; }
        public bool CanReset { get; set; }

        public SettingsUpdateViewModel(DatabaseContext context, string title, string action) : base(context)
        {
            Title = title;
            Action = action;
            CanReset = action == "Salva";
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteAsync(async () => Settings = Settings.GetCurrentSettings());
        }

        /// <summary> command to execute proper action, based on action button parameter </summary>
        [RelayCommand]
        private async void ExecuteAction()
        {
            switch (Action)
            {
                case "Salva":
                    if (!await Alerts.ShowConfirm(GetConfirmCaption())) return;
                    if (await Settings.SaveSettings(_settings) && await UpdateTimesheets()) await Shell.Current.Navigation.PopToRootAsync();
                    else await Alerts.ShowUnknownError();
                    break;

                case "Applica":
                    await Settings.SaveSettings(_settings);
                    App.ApplyCurrentTheme();
                    await Shell.Current.Navigation.PopToRootAsync();
                    break;

                case "Ripristina":
                    if (!await Alerts.ShowConfirm(Captions.SettingsDBResetConfirm)
                     || !await Alerts.ShowConfirm(Captions.SettingsDBResetSecondConfirm)) return;
                    
                    File.Delete(DatabaseContext.DbPath);
                    File.Delete(Settings.GetSettingsPath());

                    Application.Current.Quit();
                    break;

                case "Contatta":
                    try
                    {
                        if (Email.Default.IsComposeSupported)
                        {
                            var message = new EmailMessage
                            {
                                Subject = GetEmailSubject(Settings.EmailParameters.EmailType),
                                Body = Settings.EmailParameters.Body,
                                BodyFormat = EmailBodyFormat.PlainText,
                                To = new List<string>() { "timesheetapp.helpdesk@gmail.com" }
                            };

                            await Email.Default.ComposeAsync(message);
                        }
                    }
                    catch { }
                    break;
            }
        }

        /// <summary> reset a specific settings category </summary>
        [RelayCommand]
        private async void ResetPartiallySettings()
        {
            var defSettings = Settings.GetDefaultSettings();
            switch (Title)
            {
                case "Straordinari":
                    _settings.OvertimeOnEarlyMorningEnter = defSettings.OvertimeOnEarlyMorningEnter;
                    _settings.OvertimeOnLateAfternoonExit = defSettings.OvertimeOnLateAfternoonExit;
                    _settings.MinOvertimeCounted = defSettings.MinOvertimeCounted;
                    break;
                case "Banca ore": 
                    _settings.HourlyBankOnEarlyMorningEnter = defSettings.HourlyBankOnEarlyMorningEnter;
                    _settings.HourlyBankOnLateExit = defSettings.HourlyBankOnLateExit;
                    _settings.MaxHourlyBankConsumption = defSettings.MaxHourlyBankConsumption;
                    break;
                case "Ritardo":
                    _settings.LateOnEarlyLunchStamp = defSettings.LateOnEarlyLunchStamp;
                    _settings.MinLateCounted = defSettings.MinLateCounted;
                    break;

                case "Giorni lavorativi":
                    _settings.WorkingDays = defSettings.WorkingDays;
                    break;
                //case "Tema": break; // da implementare
            }

            if (!await Alerts.ShowConfirm(Captions.SettingsResetConfirm, Title) 
             || !await Alerts.ShowConfirm(GetConfirmCaption())) return;

            if (await Settings.SaveSettings(_settings) 
             && await UpdateTimesheets()) await Shell.Current.Navigation.PopAsync();
            else await Alerts.ShowError(Captions.SettingsUpdateFail);
        }

        /// <summary> 
        /// update timesheet after a settings update, eg. re-evaulate overtime amounts if min overtime raises from 30min to 45min 
        /// </summary>
        private async Task<bool> UpdateTimesheets()
        {
            try
            {
                foreach(var tsheet in await _context.GetAllAsync<DailyTimesheet>())
                {
                    if (Title == "Giorni lavorativi") await tsheet.SetOvertimeDay(!_settings.WorkingDays.Contains(tsheet.Today.DayOfWeek), _context);
                    else await tsheet.SetAmounts(_context);
                }

                return true;
            }
            catch { return false; }
        }

        /// <summary> get caption to show in update confirm alert, based on showed settings category </summary>
        private string GetConfirmCaption()
        {
            return Title switch
            {
                "Straordinari" => Captions.SettingsUpdateOvertimeConfirm,
                "Banca ore" => Captions.SettingsUpdateHourlyBankConfirm,
                "Ritardo" => Captions.SettingsUpdateLateConfirm,
                "Giorni lavorativi" => Captions.SettingsUpdateOvertimeDayConfirm,
                _ => ""
            };
        }

        /// <summary> get help desk mail subject, based on user choice </summary>
        private static string GetEmailSubject(EmailType emailType)
        {
            return emailType switch
            {
                EmailType.HelpOrQuestion => "Domanda o aiuto",
                EmailType.Suggestion => "Future implementazioni",
                EmailType.Bug => "Bug o malfunzionamenti",
                EmailType.Feedback => "Feedback",
                EmailType.Other => "Altro",
                _ => ""
            };
        }
    }
}
