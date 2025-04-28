using FitnessAplikacia.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace FitnessAplikacia.Views
{
    public partial class SetGoalPage : ContentPage
    {
        private readonly FitnessService _fitnessService;

        public SetGoalPage(FitnessService fitnessService)
        {
            InitializeComponent();
            _fitnessService = fitnessService;
        }

        private async void OnSaveGoalsClicked(object sender, EventArgs e)
        {
            if (int.TryParse(pushUpGoalEntry.Text, out int pushUpGoal) &&
                int.TryParse(pullUpGoalEntry.Text, out int pullUpGoal))
            {
                var goal = new GoalDto
                {
                    PushUpGoal = pushUpGoal,
                    PullUpGoal = pullUpGoal,
                    TargetDate = DateTime.UtcNow.AddDays(30) // Example: 30 days target
                };
                var button = sender as Button;
                if (button != null)
                {
                    // Scale down the button for animation effect
                    await button.ScaleTo(0.9, 100);
                    // Scale it back to normal
                    await button.ScaleTo(1, 100);
                }

                bool success = await _fitnessService.SetGoalAsync(goal);
                if (success)
                {
                    
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update goals.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter valid numbers for goals.", "OK");
            }
        }
    }
}
