using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessAplikacia.ViewModels;
using FitnessAplikacia;

public class AddWorkoutPageViewModel : BaseViewModel
{
    private readonly UserWorkoutService _userWorkoutService;

    public string WorkoutName { get; set; }

    public ICommand SaveWorkoutCommand { get; }

    public AddWorkoutPageViewModel(UserWorkoutService userWorkoutService)
    {
        _userWorkoutService = userWorkoutService;

        // Command to save the workout
        SaveWorkoutCommand = new Command(async () => await SaveWorkout());
    }

    private async Task SaveWorkout()
    {
        if (string.IsNullOrWhiteSpace(WorkoutName))
        {
            await App.Current.MainPage.DisplayAlert("Error", "Workout name cannot be empty.", "OK");
            return;
        }

        try
        {
            var newWorkout = new CreateWorkoutDto { WorkoutName = WorkoutName };
            await _userWorkoutService.CreateWorkoutAsync(newWorkout);

            // Navigate back to CustomWorkoutsPage
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", $"Failed to save workout: {ex.Message}", "OK");
        }
    }
}
