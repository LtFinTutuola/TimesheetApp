using System.Diagnostics;
using TimesheetApp.Model.Entities;

namespace TimesheetApp.Controls.Timesheet;

public partial class RadialProgressBarControl : TimesheetControl
{
    public static readonly BindableProperty TimeStampsProperty = BindableProperty.Create(
        nameof(TimeStamps),
        typeof(IEnumerable<TimeStamp>),
        typeof(ExtendedAmountsControl),
        defaultValue: null);

    public IEnumerable<TimeStamp> TimeStamps
    {
        get { return (IEnumerable<TimeStamp>)GetValue(TimeStampsProperty); }
        set { SetValue(TimeStampsProperty, value); }
    }

    public double Progress { get; set; } = 0d;
    public Color ProgressColor { get; set; } = new(81, 43, 212);
    public Color BackColor { get; set; } = SetBackColor();

    private IDispatcherTimer _timer;
    private readonly RadialProgressbarDrawable _RadialProgressbarDrawable;

    public RadialProgressBarControl()
    {
        InitializeComponent();
        _RadialProgressbarDrawable = new RadialProgressbarDrawable(this);
        Initialize();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        try
        {
            base.OnSizeAllocated(width, height);

            var ratio = Math.Min(Width, Height);
            ProgressView.HeightRequest = ratio;
            ProgressView.WidthRequest = ratio;
            ProgressLabel.FontSize = ratio * 0.15;
            ProgressView.Invalidate();
        }
        catch { }
    }

    public void Initialize()
    {
        ProgressView.Drawable = _RadialProgressbarDrawable;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                SetTimerToShow();
            };
        });
    }

    protected override void SetDataInternal(object value)
    {        
        if (TimeSheet.Today == DateTime.Today) _timer.Start();
        else
        {
            _timer.Stop();
            ProgressLabel.Text = "--:--:--";
            NextStampLabel.Text = "--:--";
            DesciptionLabel.Text = "...";
            Progress = 0d;
            ProgressView.Invalidate();
        }
    }

    /// <summary>
    /// evaluating completion percentage of current working day and show it graphically
    /// showing remaining time to end of working day, to the end of lunch pause or next ovetime ratio completed
    /// </summary>
    private async void SetTimerToShow()
    {
        try
        {
            // if there aren't timestamps, show default values
            if (TimeStamps == null || !TimeStamps.Any())
            {
                ProgressLabel.Text = "--:--:--";
                NextStampLabel.Text = "--:--";
                DesciptionLabel.Text = "...";
                Progress = 0d;
            }
            else
            {
                var lastStamp = TimeStamps.LastOrDefault();
                if (TimeSheet.Workshift == null) await TimeSheet.SetWorkshift();

                switch (lastStamp.StampType)
                {
                    case StampType.StartLunchPause:
                        // if lunch pause is alredy expired, show a red alert and full percentage completion
                        if (TimeSheet.Workshift.LunchPauseEnding < DateTime.Now.TimeOfDay)
                        {
                            ProgressLabel.Text = "00:00:00";
                            NextStampLabel.Text = TimeSheet.Workshift.LunchPauseEnding.ToString("hh\\:mm");
                            DesciptionLabel.Text = "Fine pausa pranzo";
                            Progress = 100d;
                            ProgressColor = new Color(255, 0, 0);
                        }
                        // show remaining time to lunch pause ending
                        else
                        {
                            ProgressLabel.Text = (TimeSheet.Workshift.LunchPauseEnding - DateTime.Now.TimeOfDay).ToString("hh\\:mm\\:ss");
                            NextStampLabel.Text = TimeSheet.Workshift.LunchPauseEnding.ToString("hh\\:mm");
                            DesciptionLabel.Text = "Fine pausa pranzo";
                            Progress = GetProgressPercentage(TimeSheet.Workshift.LunchPauseStarting, TimeSheet.Workshift.LunchPauseEnding);
                            ProgressColor = new Color(81, 43, 212);
                        }
                        break;

                    case StampType.MorningEnter:
                    case StampType.EndLunchPause:
                        var morningEnter = lastStamp.StampType == StampType.MorningEnter
                            ? lastStamp : TimeStamps.FirstOrDefault(t => t.StampType == StampType.MorningEnter);

                        var endOfWorkingDay = DailyTimesheet.GetEndOfWorkingDay(morningEnter, TimeSheet.Workshift);
                        
                        // if expected end of working day is not occurred yet, show remaining time percentage
                        if (endOfWorkingDay > DateTime.Now.TimeOfDay)
                        {
                            ProgressLabel.Text = (endOfWorkingDay - DateTime.Now.TimeOfDay).ToString("hh\\:mm\\:ss");
                            NextStampLabel.Text = endOfWorkingDay.ToString("hh\\:mm");
                            DesciptionLabel.Text = "Fine giornata";
                            Progress = GetProgressPercentage(morningEnter.Stamp.TimeOfDay, endOfWorkingDay);
                            ProgressColor = new Color(81, 43, 212);
                        }
                        // calculate next overtime ratio and remaining time to it
                        else
                        {
                            var settings = Settings.GetCurrentSettings();
                            do
                            {
                                endOfWorkingDay += settings.MinOvertimeCounted;
                                if (endOfWorkingDay > DateTime.Now.TimeOfDay)
                                {
                                    ProgressLabel.Text = (endOfWorkingDay - DateTime.Now.TimeOfDay).ToString("hh\\:mm\\:ss");
                                    NextStampLabel.Text = endOfWorkingDay.ToString("hh\\:mm");
                                    DesciptionLabel.Text = "Prossimo straordinario";
                                    Progress = GetProgressPercentage(endOfWorkingDay - settings.MinOvertimeCounted, endOfWorkingDay);
                                    ProgressColor = new Color(66, 212, 43);
                                }
                            }
                            while (endOfWorkingDay < DateTime.Now.TimeOfDay);
                        }
                        break;

                    // if working day is completed, show a placeholder message
                    case StampType.AfternoonExit:
                        ProgressLabel.Text = "00:00:00";
                        NextStampLabel.Text = "--:--";
                        DesciptionLabel.Text = "Giornata finita!";
                        Progress = 100d;
                        ProgressColor = new Color(164, 150, 212);
                        break;
                }
            }
            BackColor = SetBackColor();
            ProgressView.Invalidate();
        }
        catch { ProgressLabel.Text = "--:--:--"; }
    }

    private static Color SetBackColor() => Settings.GetCurrentSettings().Theme 
        ? new Color(243, 242, 238) 
        : new Color(33, 33, 33);

    private static double GetProgressPercentage(TimeSpan startingTime, TimeSpan endingTime)
    {
        try
        {
            var currentTicks = DateTime.Now.TimeOfDay.Ticks - startingTime.Ticks;
            var totalTicks = endingTime.Ticks - startingTime.Ticks;
            return currentTicks * 100 / totalTicks;
        }
        catch { return 0; }
    }
}