using TimesheetApp.ViewModel.Navigation;

namespace TimesheetApp.View.Navigation;

public partial class TimesheetNotesPage : BaseContentPage
{
	public TimesheetNotesPage(TimesheetNotesViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}