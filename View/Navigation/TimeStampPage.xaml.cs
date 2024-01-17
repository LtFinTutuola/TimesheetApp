using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class TimeStampPage : BaseContentPage
{
	public TimeStampPage(TimeStampViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}