using System.Collections;
using TimesheetApp.Model.Context;

namespace TimesheetApp.Model.Entities
{
    public enum AmountKind
    {
        NoAmount,
        Overtime,
        Late,
        HourlyBank,
        NegativeHourlyBank,
        Placeholder,
        OvertimeDay,
        Permit,
        PendingJustify
    }

    public class DailyTimesheet : BaseEntity, IEnumerable<AmountKind>
    {
        public DateTime Today { get; set; }
        public TimeSpan HourlyBankAmount { get; set; }
        public TimeSpan OvertimeAmount { get; set; }
        public TimeSpan LateAmount { get; set; }
        public int WorkshiftID { get; set; }
        public bool IsOvertimeDay { get; set; }
        public string Notes { get; set; }

        public Workshift Workshift;

        public DailyTimesheet()
        {
            Today = DateTime.Today;
            IsOvertimeDay = !Settings.GetCurrentSettings().WorkingDays.Contains(Today.DayOfWeek);
        }

        public DailyTimesheet(DateTime today)
        {
            Today = today.Date;
            IsOvertimeDay = !Settings.GetCurrentSettings().WorkingDays.Contains(Today.DayOfWeek);
        }

        public DailyTimesheet(DateTime today, Workshift workshift)
        {
            Today = today.Date;
            Workshift = workshift;
            WorkshiftID = workshift.ID;
            IsOvertimeDay = !Settings.GetCurrentSettings().WorkingDays.Contains(Today.DayOfWeek);
        }

        public async Task SetWorkshift()
        {
            var context = new DatabaseContext();
            if (WorkshiftID != 0) Workshift = (await context.GetFileteredAsync<Workshift>(w => w.ID == WorkshiftID)).FirstOrDefault();
            else
            {
                var @default = await context.GetFileteredAsync<Workshift>(w => w.IsDefault);
                if (@default != null && @default.Any())
                {
                    Workshift = @default.FirstOrDefault();
                    WorkshiftID = Workshift.ID;
                    if (ID != 0) await context.UpdateItemAsync(this);
                }
            }
        }

        public async Task SetNotes(string value, DatabaseContext context)
        {
            Notes = value;
            if (ID == 0) await context.AddItemAsync(this);
            else if (!await context.UpdateItemAsync(this)) await Alerts.ShowError(Captions.TimesheetNotesFail);
        }

        public async Task SetOvertimeDay(bool isOvertime, DatabaseContext context)
        {
            IsOvertimeDay = isOvertime;

            if (ID == 0) await context.AddItemAsync(this);
            else if (!await context.UpdateItemAsync(this)) await Alerts.ShowError(Captions.TimesheetOvertimeFail);
            
            if (!await SetAmounts(new DatabaseContext())) await Alerts.ShowError(Captions.TimesheetAmounsOvertimeFail);
        }

        public async Task<bool> ChangeWorkshift(Workshift workshift, DatabaseContext context)
        {
            WorkshiftID = workshift.ID;
            Workshift = workshift;

            return ID == 0 || await SetAmounts(context);
        }

        public async Task<bool> SetAmounts(DatabaseContext context)
        {
            var settings = Settings.GetCurrentSettings();
            var stamps = await context.GetFileteredAsync<TimeStamp>(t => t.TimesheetID == ID);

            if (WorkshiftID != 0 && Workshift == null) await SetWorkshift();
            if (Workshift == null) return false;

            LateAmount = new();
            OvertimeAmount = new();
            HourlyBankAmount = new();

            if(!IsOvertimeDay)
            {
                foreach (var stamp in stamps)
                {
                    switch (stamp.StampType)
                    {
                        case StampType.MorningEnter:
                            var rawMorningAmounts = await GetMorningEnterAmounts(settings, stamp, Workshift);

                            if (rawMorningAmounts.ContainsKey(AmountKind.Overtime)) OvertimeAmount = rawMorningAmounts[AmountKind.Overtime];
                            if (rawMorningAmounts.ContainsKey(AmountKind.HourlyBank)) HourlyBankAmount = rawMorningAmounts[AmountKind.HourlyBank];
                            if (rawMorningAmounts.ContainsKey(AmountKind.Late)) LateAmount = rawMorningAmounts[AmountKind.Late];
                            break;

                        case StampType.StartLunchPause:
                            LateAmount += GetLunchPauseStartingLate(settings, stamp, Workshift);
                            break;

                        case StampType.EndLunchPause:
                            LateAmount += GetLunchPauseEndingLate(settings, stamp, Workshift);
                            break;

                        case StampType.AfternoonExit:
                            var rawAfternoonAmounts = await GetAfternoonExitAmounts(settings, stamp, Workshift);

                            if (rawAfternoonAmounts.ContainsKey(AmountKind.Overtime)) OvertimeAmount += rawAfternoonAmounts[AmountKind.Overtime];
                            if (rawAfternoonAmounts.ContainsKey(AmountKind.HourlyBank)) HourlyBankAmount += rawAfternoonAmounts[AmountKind.HourlyBank];
                            if (rawAfternoonAmounts.ContainsKey(AmountKind.NegativeHourlyBank)) HourlyBankAmount += rawAfternoonAmounts[AmountKind.NegativeHourlyBank];
                            break;
                    }
                }
                return await context.UpdateItemAsync(this);
            }
            else if (IsOvertimeDay && stamps.Any(s => s.StampType == StampType.AfternoonExit))
            {
                var enter = stamps.FirstOrDefault(s => s.StampType == StampType.MorningEnter);
                var exit = stamps.FirstOrDefault(s => s.StampType == StampType.AfternoonExit);

                var workingHours = exit.Stamp - enter.Stamp - Workshift.PauseDuration;

                OvertimeAmount = GetTimeDifferenceAmount(enter.Stamp.TimeOfDay + workingHours, enter.Stamp.TimeOfDay, false, settings.MinOvertimeCounted);
                
                if(settings.HourlyBankOnEarlyMorningEnter || settings.HourlyBankOnLateExit)
                    HourlyBankAmount = TimeSpan.FromMinutes(workingHours.TotalMinutes % settings.MinOvertimeCounted.TotalMinutes);

                return await context.UpdateItemAsync(this);
            }
            else return true;
        }      

        public static TimeSpan GetLunchPauseStartingLate(Settings settings, TimeStamp stamp, Workshift workshift)
        {
            if (settings.LateOnEarlyLunchStamp && stamp.Stamp.TimeOfDay < workshift.LunchPauseStarting)
                return GetTimeDifferenceAmount(stamp.Stamp.TimeOfDay, workshift.LunchPauseStarting, true, settings.MinLateCounted);

            else return TimeSpan.Zero;
        }

        public static TimeSpan GetLunchPauseEndingLate(Settings settings, TimeStamp stamp, Workshift workshift)
        {
            if (stamp.Stamp.TimeOfDay > workshift.LunchPauseEnding)
                return GetTimeDifferenceAmount(stamp.Stamp.TimeOfDay, workshift.LunchPauseEnding, true, settings.MinLateCounted);

            else return TimeSpan.Zero;
        }

        public static async Task<Dictionary<AmountKind, TimeSpan>> GetMorningEnterAmounts(Settings settings, TimeStamp stamp, Workshift workshift, DatabaseContext context = null)
        {
            var result = new Dictionary<AmountKind, TimeSpan>();

            if (stamp.HasPermit) result.Add(AmountKind.Permit, TimeSpan.Zero);            
            else if (stamp.Stamp.TimeOfDay < workshift.MorningEnter)
            {
                var rawDiff = workshift.MorningEnter - stamp.Stamp.TimeOfDay;
                if (settings.HourlyBankOnEarlyMorningEnter)
                    result.Add(AmountKind.HourlyBank, TimeSpan.FromMinutes(rawDiff.TotalMinutes % settings.MinOvertimeCounted.TotalMinutes));

                if (settings.OvertimeOnEarlyMorningEnter && rawDiff >= settings.MinOvertimeCounted)
                    result.Add(AmountKind.Overtime, GetTimeDifferenceAmount(stamp.Stamp.TimeOfDay, workshift.MorningEnter,
                        false, settings.MinOvertimeCounted));
            }
            else if (stamp.Stamp.TimeOfDay > workshift.MorningEnter + workshift.HourlyFlex)
                result.Add(AmountKind.Late, GetTimeDifferenceAmount(stamp.Stamp.TimeOfDay, workshift.MorningEnter + workshift.HourlyFlex,
                    true, settings.MinLateCounted));

            else result.Add(AmountKind.NoAmount, TimeSpan.Zero);
            return result;
        }

        public static async Task<Dictionary<AmountKind, TimeSpan>> GetAfternoonExitAmounts(Settings settings, TimeStamp stamp, Workshift workshift, DatabaseContext context = null)
        {
            var result = new Dictionary<AmountKind, TimeSpan>();
            if(stamp.HasPermit) result.Add(AmountKind.Permit, TimeSpan.Zero);
            else
            {
                var morningEnter = (await (context ?? new DatabaseContext()).GetFileteredAsync<TimeStamp>(
                    t => t.TimesheetID == stamp.TimesheetID && t.StampType == StampType.MorningEnter)).FirstOrDefault();

                var endOfDay = GetEndOfWorkingDay(morningEnter, workshift);
                var rawDiff = stamp.Stamp.TimeOfDay - endOfDay;

                // get hourly bank amount
                if (settings.HourlyBankOnLateExit && rawDiff != TimeSpan.Zero)
                {
                    if (rawDiff < TimeSpan.Zero)
                    {
                        result.Add(AmountKind.NegativeHourlyBank, Math.Abs(rawDiff.TotalMinutes) > Math.Abs(settings.MaxHourlyBankConsumption.TotalMinutes)
                            ? settings.MaxHourlyBankConsumption : rawDiff);

                        if(Math.Abs(rawDiff.TotalMinutes) > Math.Abs(settings.MaxHourlyBankConsumption.TotalMinutes))
                            result.Add(AmountKind.PendingJustify, GetTimeDifferenceAmount(
                                TimeSpan.FromMinutes(endOfDay.TotalMinutes - Math.Abs(settings.MaxHourlyBankConsumption.TotalMinutes)), 
                                stamp.Stamp.TimeOfDay, true, settings.MinLateCounted));
                    }
                    else result.Add(AmountKind.HourlyBank, TimeSpan.FromMinutes(rawDiff.TotalMinutes % settings.MinOvertimeCounted.TotalMinutes));
                }
                // get late afternoon exit overtime
                if (settings.OvertimeOnLateAfternoonExit && rawDiff >= settings.MinOvertimeCounted)
                    result.Add(AmountKind.Overtime, GetTimeDifferenceAmount(stamp.Stamp.TimeOfDay, endOfDay, false, settings.MinOvertimeCounted));

                if (!result.Any()) result.Add(AmountKind.NoAmount, TimeSpan.Zero);
            }            
            return result;
        }

        private static TimeSpan GetTimeDifferenceAmount(TimeSpan firstStamp, TimeSpan secondStamp, bool isLate, TimeSpan diffRatio)
        {
            if(isLate) return diffRatio 
                    * Math.Ceiling((double)(Math.Abs((firstStamp - secondStamp).TotalMinutes)
                    / diffRatio.TotalMinutes));
            else return diffRatio 
                    * Math.Floor((double)(Math.Abs((firstStamp - secondStamp).TotalMinutes)
                    / diffRatio.TotalMinutes));
        }

        public static TimeSpan GetEndOfWorkingDay(TimeStamp morningEnter, Workshift workshift)
        {
            if (morningEnter.Stamp.TimeOfDay >= workshift.MorningEnter &&
                morningEnter.Stamp.TimeOfDay <= workshift.MorningEnter + workshift.HourlyFlex)
                return morningEnter.Stamp.TimeOfDay + workshift.DailyWorkingHours;

            else if (morningEnter.Stamp.TimeOfDay < workshift.MorningEnter) return workshift.AfternoonExit;
            else return workshift.AfternoonExit + workshift.HourlyFlex;
        }
        #region Ienumerable
        private IEnumerable<AmountKind> GetAmounts()
        {
            if (IsOvertimeDay && OvertimeAmount > TimeSpan.Zero) yield return AmountKind.OvertimeDay;
            else
            {
                if (OvertimeAmount > TimeSpan.Zero) yield return AmountKind.Overtime;
                if (LateAmount > TimeSpan.Zero) yield return AmountKind.Late;
                if (HourlyBankAmount > TimeSpan.Zero) yield return AmountKind.HourlyBank;
                if (HourlyBankAmount < TimeSpan.Zero) yield return AmountKind.NegativeHourlyBank;
            }
        }

        public IEnumerator<AmountKind> GetEnumerator() => GetAmounts().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
