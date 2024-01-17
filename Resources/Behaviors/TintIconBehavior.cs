using CommunityToolkit.Maui.Behaviors;

namespace TimesheetApp.Resources.Behaviors
{
    class TintIconBehavior : Behavior<Image>
    {
        public static readonly BindableProperty TintColorProperty =
            BindableProperty.CreateAttached("TintColor", 
            typeof(Color), 
            typeof(TintIconBehavior),
            null);

        public static Color GetTintColor(BindableObject view) => (Color)view.GetValue(TintColorProperty);
        public static void SetTintColor(BindableObject view, Color color)
        {
            view.SetValue(TintColorProperty, color);
        }

        public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached("AttachBehavior",
            typeof(bool),
            typeof(TintIconBehavior),
            false,
            propertyChanged: OnAttachBehaviorChanged);

        public static bool GetAttachBehavior(BindableObject view) => (bool)view.GetValue(AttachBehaviorProperty);
        public static void SetAttachBehavior(BindableObject view, bool value)
        {
            view.SetValue(AttachBehaviorProperty, value);
        }
        static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
        {
            if(view is Microsoft.Maui.Controls.View image)
            {
                bool attachBehavior = (bool)newValue;
                if (attachBehavior) image.Behaviors.Add(new IconTintColorBehavior() { TintColor = GetTintColor(image)});
                else image.Behaviors.Remove(image.Behaviors.FirstOrDefault(b => b is IconTintColorBehavior));
            }
        }
    }
}
