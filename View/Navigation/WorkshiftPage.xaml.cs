using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class WorkshiftPage : BaseContentPage
{
	public WorkshiftPage(WorkshiftViewModel viewModel) : base(viewModel) 
	{
		InitializeComponent();
	}
}