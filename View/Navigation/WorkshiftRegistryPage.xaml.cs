using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class WorkshiftRegistryPage : BaseContentPage
{
	public WorkshiftRegistryPage(WorkshiftRegistryViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}