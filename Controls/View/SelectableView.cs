namespace TimesheetApp.Controls.View
{
    public abstract class SelectableView : BaseView
    {
        public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
            nameof(Selected),
            typeof(bool),
            typeof(SelectView),
            false);

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        protected abstract void SelectViewTapped(object sender, EventArgs e);
    }
}
