using TimesheetApp.Model.Context;
using TimesheetApp.Resources.Converters;

namespace TimesheetApp.Model.Entities
{
    public enum StampType
    {
        MorningEnter,
        StartLunchPause,
        EndLunchPause,
        AfternoonExit
    }

    public class TimeStamp : BaseEntity
    {
        public DateTime Stamp { get; set; }
        public StampType StampType { get; set; }
        public int TimesheetID { get; set; }
        public bool HasPermit { get; set; }

        public TimeStamp(DailyTimesheet timesheet, StampType stampType)
        {
            Stamp = timesheet.Today + DateTime.Now.TimeOfDay;
            TimesheetID = timesheet.ID;
            StampType = stampType;
        }
        public TimeStamp() { }

        /// <summary>
        /// deny validation in following scenarios
        /// - timestamp hour is in future
        /// - another timestamp with the same stamp type is alredy registered in referred timesheet
        /// - violation of timesheet timetable order
        /// - if timestamp is EndLunchPause and StartLunchPause is missing
        /// </summary>
        public override async Task<bool?> Validate(object entities)
        {
            try
            {
                if(Stamp > DateTime.Now.AddMinutes(1))
                {
                    await Alerts.ShowError(Captions.TimestampFutureFail);
                    return false;
                }

                var timestamps = (IEnumerable<TimeStamp>)entities;
                if (timestamps.Any(t => t.StampType == StampType))
                {
                    if (await Alerts.ShowConfirm(Captions.TimestampAlrReg, (string)StampTypeDisplayConverter.Convert(StampType))) return null;
                    else return false;
                }
                if (!timestamps.Any() && StampType != StampType.MorningEnter)
                {
                    await Alerts.ShowError(Captions.TimestampMorningMissing);
                    return false;
                }
                switch (StampType)
                {
                    case StampType.MorningEnter:
                        if (ID == 0 && timestamps.Any())
                        {
                            await Alerts.ShowError(Captions.TimestampFail);
                            return false;
                        }
                        else return await CheckTimestampsChronoOrder(null, timestamps.FirstOrDefault(t => t.StampType == StampType.StartLunchPause) 
                            ?? timestamps.FirstOrDefault(t => t.StampType == StampType.AfternoonExit));

                    case StampType.StartLunchPause:
                        if (ID == 0 && timestamps.LastOrDefault().StampType != StampType.MorningEnter)
                        {
                            await Alerts.ShowError(timestamps.Any(t => t.StampType == StampType.MorningEnter) ?
                                Captions.TimestampOrderFail : Captions.TimestampMorningMissing);
                            return false;
                        }
                        else return await CheckTimestampsChronoOrder(timestamps.FirstOrDefault(t => t.StampType == StampType.MorningEnter),
                            timestamps.FirstOrDefault(t => t.StampType == StampType.EndLunchPause));

                    case StampType.EndLunchPause:
                        if (ID == 0 && timestamps.LastOrDefault().StampType != StampType.StartLunchPause)
                        {
                            await Alerts.ShowError(timestamps.Any(t => t.StampType == StampType.StartLunchPause) ?
                                Captions.TimestampOrderFail : Captions.TimestampLunchStartMissing);
                            return false;
                        }
                        else return await CheckTimestampsChronoOrder(timestamps.FirstOrDefault(t => t.StampType == StampType.StartLunchPause),
                            timestamps.FirstOrDefault(t => t.StampType == StampType.AfternoonExit));

                    case StampType.AfternoonExit:
                        if (ID == 0 && timestamps.LastOrDefault().StampType == StampType.StartLunchPause)
                        {
                            await Alerts.ShowError(Captions.TimestampLunchEndMissing);
                            return false;
                        }
                        else return await CheckTimestampsChronoOrder(timestamps.FirstOrDefault(t => t.StampType == StampType.EndLunchPause)
                            ?? timestamps.FirstOrDefault(t => t.StampType == StampType.MorningEnter), null);
                }
                return true;
            }
            catch 
            {
                await Alerts.ShowUnknownError();
                return false; 
            }
        }

        private async Task<bool> CheckTimestampsChronoOrder(TimeStamp previous, TimeStamp next)
        {
            if(previous != null && Stamp <= previous.Stamp)
            {
                await Alerts.ShowError(Captions.TimestampOrderFailPrev);
                return false;
            }
            if(next != null && Stamp >= next.Stamp)
            {
                await Alerts.ShowError(Captions.TimestampOrderFailNext);
                return false;
            }
            return true;
        }
    }
}
