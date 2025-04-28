using FitnessAplikacia.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FitnessAplikacia.Views
{
    public partial class StatisticPage : ContentPage
    {
        private readonly FitnessService _fitnessService;
        private DateTime _lastUpdatedDate;

        // Inject the FitnessService using constructor injection
        public StatisticPage(FitnessService fitnessService)
        {
            InitializeComponent();
            _fitnessService = fitnessService;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await LoadStatsAsync();
        }

        public string StatusMessage { get; set; }

        private async Task LoadStatsAsync()
        {
            try
            {
                // Show loading indicator
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;

                // Fetch stats
                var stats = await _fitnessService.GetStatsAsync();

                // Hide loading indicator once stats are fetched
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;

                if (stats != null)
                {
                    BindingContext = stats;
                }
                else
                {
                    await DisplayAlert("Error", "Failed to load statistics.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Hide the loading indicator in case of an error
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnSubmitTestDay(object sender, EventArgs e)
        {
            if (SubmitTestButton == null) return; // 🔍 Prevent null reference crash

            // 🏆 Add animation for better UX
            await SubmitTestButton.ScaleTo(0.9, 100);
            await SubmitTestButton.ScaleTo(1, 100);

            if (_lastUpdatedDate == DateTime.Today)
            {
                await DisplayAlert("Error", "You can only submit a test once per day.", "OK");
                return;
            }

            if (!int.TryParse(pushUpTestEntry.Text, out int pushUpCount) ||
                !int.TryParse(pullUpTestEntry.Text, out int pullUpCount))
            {
                await DisplayAlert("Invalid Input", "Please enter valid numbers.", "OK");
                return;
            }

            var performance = new PerformanceDto
            {
                Date = DateTime.Today,
                PushUpCount = pushUpCount,
                PullUpCount = pullUpCount
            };

            bool success = await _fitnessService.SetPerformanceAsync(performance);

            if (success)
            {
                

                // 🔄 Reload UI
                await LoadStatsAsync();

                _lastUpdatedDate = DateTime.Today;
            }
            else
            {
                await DisplayAlert("Error", "Failed to record test day.", "OK");
            }
        }


        private async void OnSetGoalsClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Scale down the button for animation effect
                await button.ScaleTo(0.9, 100); // Scale down in 100ms
                await button.ScaleTo(1, 100);   // Scale back to original size in 100ms
            }
            // Resolve SetGoalPage with DI to ensure it gets the FitnessService
            var goalPage = new SetGoalPage(_fitnessService);
            await Navigation.PushAsync(goalPage);
        }

        


    }

}
