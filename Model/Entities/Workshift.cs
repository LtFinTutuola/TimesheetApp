using TimesheetApp.Model.Context;

namespace TimesheetApp.Model.Entities
{
    public class Workshift : BaseEntity
    {
        public string Description { get; set; }
        public TimeSpan MorningEnter { get; set; }
        public TimeSpan AfternoonExit { get; set; }
        public TimeSpan DailyWorkingHours => AfternoonExit - MorningEnter;
        public bool HasLunchPause { get; set; }
        public TimeSpan LunchPauseStarting { get; set; }
        public TimeSpan LunchPauseEnding { get; set; }
        public TimeSpan PauseDuration => LunchPauseEnding - LunchPauseStarting;
        public TimeSpan HourlyFlex { get; set; }
        public bool IsDefault { get; set; }

        public Workshift() { }

        /// <summary>
        /// deny validation in following scenrios
        /// - description is null or empty
        /// - another workshift has same description
        /// - another workshift has same timetables
        /// - timetables aren't in correct hourly order
        /// </summary>
        public override async Task<bool?> Validate(object entities)
        {
            var workshifts = (IEnumerable<Workshift>)entities;
            if (string.IsNullOrEmpty(Description))
            {
                await Alerts.ShowError(Captions.WorkshiftDescMissing);
                return false;
            }
            if (workshifts.Any(w => w.Description == Description))
            {
                await Alerts.ShowError(Captions.WorkshiftDescAlredyExist);
                return false;
            }
            if (workshifts.Any(w => w.MorningEnter == MorningEnter && w.AfternoonExit == AfternoonExit))
            {
                await Alerts.ShowError(Captions.WorkshiftHrsAlredyExist);
                return false;
            }
            if (HasLunchPause && (LunchPauseStarting <= MorningEnter 
                || LunchPauseEnding <= LunchPauseStarting 
                || AfternoonExit <= LunchPauseEnding))
            {
                await Alerts.ShowError(Captions.WorkshiftOrderFail);
                return false;
            }
            if (AfternoonExit <= MorningEnter)
            {
                await Alerts.ShowError(Captions.WorkshiftOrderFail);
                return false;
            }
            return true;
        }

        /// <summary> get available stamp types to show in TimeStampPage </summary>
        public StampType[] GetAvailableStampTypes()
        {
            if (HasLunchPause) return (StampType[])Enum.GetValues(typeof(StampType));
            else return new StampType[] { StampType.MorningEnter, StampType.AfternoonExit };
        }
    }
}
