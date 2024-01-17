using TimesheetApp.ViewModel;

namespace TimesheetApp.View
{
    public class BaseContentPage : ContentPage
    {
        protected readonly BaseViewModel _viewModel;
        public BaseContentPage(BaseViewModel viewModel)
        {
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        /// <summary> workaround to avoid lag or poor performance issues during pop / push transition execution </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(500).ContinueWith(async delegate { await _viewModel.LoadDataAsync(); });
        }
    }
}
