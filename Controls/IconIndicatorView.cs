
namespace TimesheetApp.Controls
{
    /// <summary> custom control used to emulate indicator view and show images instead of simple shapes </summary>
    public class IconIndicatorView : CollectionView
    {
        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            nameof(Position),
            typeof(int),
            typeof(IconIndicatorView),
            defaultValue:-1,
            propertyChanged: SetData);

        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public IconIndicatorView()
        {
            ItemsLayout = new GridItemsLayout(ItemsLayoutOrientation.Horizontal);
            SelectionMode = SelectionMode.Single;
            InputTransparent = true;
            Margin = new Thickness(0, 8, 0, -8);
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            Loaded += IconIndicatorView_Loaded;
        }

        private static void SetData(BindableObject bindable, object oldValue, object newValue)
        {
            var indicator = (IconIndicatorView)bindable;
            indicator.SetDataInternal();
        }

        private void SetDataInternal()
        {
            if(ItemsSource is string[] sources) SelectedItem = sources[Position];
        }
        private void IconIndicatorView_Loaded(object sender, EventArgs e)
        {
            if (sender is IconIndicatorView iconIndicatorView && iconIndicatorView.ItemsSource is string[] sources)
                SelectedItem = sources.FirstOrDefault();
        }
    }
}
