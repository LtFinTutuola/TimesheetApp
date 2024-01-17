using System.Windows.Input;

namespace TimesheetApp.Controls.View
{
    public abstract class BaseView : ContentView
    {
        public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
            nameof(Description), 
            typeof(string), 
            typeof(BaseView), 
            string.Empty);

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly BindableProperty InfoProperty = BindableProperty.Create(
            nameof(Info),
            typeof(string),
            typeof(BaseView),
            string.Empty);

        public string Info
        {
            get => (string)GetValue(InfoProperty);
            set => SetValue(InfoProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(BaseView));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            nameof(ImageSource),
            typeof(string),
            typeof(BaseView),
            string.Empty);

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
    }
}
