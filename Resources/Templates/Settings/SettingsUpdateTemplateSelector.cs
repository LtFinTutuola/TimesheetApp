using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.Resources.Templates.Settings
{
    public class SettingsUpdateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OvertimeTemplate { get; set; }
        public DataTemplate HourlyBankTemplate { get; set; }
        public DataTemplate LateTemplate { get; set; }
        public DataTemplate WorkingDaysTemplate { get; set; }
        public DataTemplate ThemeTemplate { get; set; }
        public DataTemplate ContactsTemplate { get; set; }
        public DataTemplate ResetTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var vm = (SettingsUpdateViewModel)container.BindingContext;

            if (vm.Title == "Straordinari") return OvertimeTemplate;
            else if (vm.Title == "Banca ore") return HourlyBankTemplate;
            else if (vm.Title == "Ritardo") return LateTemplate;
            else if (vm.Title == "Giorni lavorativi") return WorkingDaysTemplate;
            else if (vm.Title == "Tema") return ThemeTemplate;
            else if (vm.Title == "Contatti") return ContactsTemplate;
            else /*if (vm.Title == "Ripristina")*/ return ResetTemplate;
        }
    }
}
