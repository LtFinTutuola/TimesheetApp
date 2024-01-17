using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using TimesheetApp.Model.Context;
using TimesheetApp.Model.Utilities;

namespace TimesheetApp.Model.Entities
{
    public partial class Settings : ObservableObject
    {
        public bool HourlyBankOnLateExit { get; set; }
        public bool HourlyBankOnEarlyMorningEnter { get; set; }
        public TimeSpan MaxHourlyBankConsumption { get; set; }
        public bool OvertimeOnLateAfternoonExit { get; set; }
        public bool OvertimeOnEarlyMorningEnter { get; set; }
        public TimeSpan MinOvertimeCounted { get; set; }
        public bool LateOnEarlyLunchStamp { get; set; }
        public TimeSpan MinLateCounted { get; set; }
        public List<DayOfWeek> WorkingDays { get; set; }
        public bool Theme { get; set; }

        [JsonIgnore]
        public EmailParameters EmailParameters { get; set; } = new();

        public static Settings GetDefaultSettings()
        {
            return new Settings
            {
                HourlyBankOnLateExit = true,
                HourlyBankOnEarlyMorningEnter = true,
                MaxHourlyBankConsumption = new TimeSpan(-2, 30, 0),
                MinOvertimeCounted = new TimeSpan(0, 30, 0),
                MinLateCounted = new TimeSpan(0, 15, 0),
                LateOnEarlyLunchStamp = false,
                OvertimeOnLateAfternoonExit = true,
                OvertimeOnEarlyMorningEnter = false,
                WorkingDays = new() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                Theme = true
            };
        }

        public static Settings GetCurrentSettings()
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
            if (File.Exists(path)) return JsonSerializer.Deserialize<Settings>(File.ReadAllText(path));            
            else
            {
                string json = JsonSerializer.Serialize(GetDefaultSettings());
                File.WriteAllText(path, json);
                return GetDefaultSettings();
            }
        }
        public static async Task<bool> SaveSettings(Settings settings)
        {
            try
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                await Alerts.ShowAlert(Captions.SettingsUpdateFail);
                return false;
            }
        }

        public static string GetSettingsPath() => Path.Combine(FileSystem.AppDataDirectory, "settings.json");

        [RelayCommand]
        private void SelectWorkingDay(object selView)
        {
            DayOfWeek day = (string)selView switch
            {
                "Lunedì" => DayOfWeek.Monday,
                "Martedì" => DayOfWeek.Tuesday,
                "Mercoledì" => DayOfWeek.Wednesday,
                "Giovedì" => DayOfWeek.Thursday,
                "Venerdì" => DayOfWeek.Friday,
                "Sabato" => DayOfWeek.Saturday,
                "Domenica" => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };

            if (WorkingDays.Contains(day)) WorkingDays.Remove(day);
            else WorkingDays.Add(day);
        }

        [RelayCommand]
        private void SetTheme(bool theme)
        {
            Theme = theme;
            OnPropertyChanged(nameof(Theme));
        }
    }
}
