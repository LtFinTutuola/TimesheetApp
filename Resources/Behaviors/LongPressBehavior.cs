using System.Windows.Input;

namespace TimesheetApp.Resources.Behaviors
{
    /// <summary>
    /// On click, initialize the timer, which elapses two times, on the second elapse if the button is not
    /// released execute CommandProperty.
    /// If the button is released before the second elapse, reset timer and execute button's command
    /// </summary>
    public class LongPressBehavior : Behavior<Button>
    {
        private readonly object _syncObject = new();
        private Timer _timer;
        private TimeSpan _pressDuration = new(0, 0, 0, 0, 500);

        private bool _commandExecuted = false;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
            typeof(ICommand), typeof(LongPressBehavior), default(ICommand));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LongPressBehavior));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);
            BindingContext = button.BindingContext;
            button.Pressed += OnPressed;
            button.Released += OnReleased;
            button.BindingContextChanged += OnBindingContextChanged;
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            BindingContext = null;
            button.Pressed -= OnPressed;
            button.Released -= OnReleased;
            button.BindingContextChanged -= OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            if (sender is Button button) BindingContext = button.BindingContext;
        }

        private void DeInitializeTimer()
        {
            lock (_syncObject)
            {
                if (_timer == null) return;                
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;

                _pressDuration = new TimeSpan(0, 0, 0, 0, 500);
            }
        }

        private void InitializePressTimer()
        {
            lock (_syncObject)
            {
                _timer = new Timer(Timer_Elapsed, null, 500, 500);
            }
        }

        private void OnPressed(object sender, EventArgs e)
        {
            InitializePressTimer();
            Vibration.Default.Vibrate(1000);

            var button = sender as Button;
            button.Command?.Execute(false);

            if (_commandExecuted == true) _commandExecuted = false;
        }

        private void OnReleased(object sender, EventArgs e)
        {
            Vibration.Default.Cancel();
            var button = sender as Button;
            if (_pressDuration > TimeSpan.Zero && button.Command != null) button.Command.Execute(true); 
            DeInitializeTimer();
        }

        public LongPressBehavior() { }

        private void Timer_Elapsed(object state)
        {
            _pressDuration = _pressDuration.Subtract(new TimeSpan(0, 0, 0, 0, 250));
            if (_pressDuration <= TimeSpan.Zero && Command != null && Command.CanExecute(CommandParameter) && !_commandExecuted)
            {
                Command.Execute(CommandParameter);
                Vibration.Default.Cancel();
                _commandExecuted = true;
            }
        }
    }
}
