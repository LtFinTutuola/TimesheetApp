using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class SettingsUpdatePage : BaseContentPage
{
    public SettingsUpdatePage(SettingsUpdateViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}