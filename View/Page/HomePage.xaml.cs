using TimesheetApp.ViewModel.Page;

namespace TimesheetApp.View.Page;

public partial class HomePage : BaseContentPage
{
    public HomePage(HomePageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}