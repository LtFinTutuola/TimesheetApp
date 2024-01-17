using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class TimeStampDetailPage : BaseContentPage
{
	public TimeStampDetailPage(TimeStampDetailViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}