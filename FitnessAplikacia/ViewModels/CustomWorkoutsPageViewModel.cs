using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessAplikacia.Views;
using FitnessAplikacia.ViewModels;
using FitnessAplikacia;

public class CustomWorkoutsPageViewModel : BaseViewModel
{
    private readonly UserWorkoutService _userWorkoutService;

    public ObservableCollection<UserWorkoutModel> Workouts { get; set; } = new ObservableCollection<UserWorkoutModel>();

    public ICommand AddWorkoutCommand { get; }
    public ICommand DeleteWorkoutCommand { get; }
    public ICommand SeeWorkoutCommand { get; }
    public CustomWorkoutsPageViewModel(UserWorkoutService userWorkoutService)
    {
        _userWorkoutService = userWorkoutService;

        // Command to navigate to AddWorkoutPage
        AddWorkoutCommand = new Command(async () => await NavigateToAddWorkoutPage());
        DeleteWorkoutCommand = new Command<UserWorkoutModel>(async (workout) => await DeleteWorkout(workout));
        SeeWorkoutCommand = new Command<UserWorkoutModel>(async (workout) => await NavigateToCustomExercisesPage(workout));
        // Fetch existing workouts on load
        LoadWorkoutsAsync();
    }

    private async Task LoadWorkoutsAsync()
    {
        IsBusy = true;
        try
        {
            var workouts = await _userWorkoutService.GetAllUserWorkoutsAsync();
            Workouts.Clear();
            foreach (var workout in workouts)
            {
                Workouts.Add(workout);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NavigateToAddWorkoutPage()
    {
        // Navigate to AddWorkoutPage
        await Shell.Current.GoToAsync(nameof(AddWorkoutPage));
    }

    public async Task RefreshWorkouts()
    {
        await LoadWorkoutsAsync();
    }

    private async Task DeleteWorkout(UserWorkoutModel workout)
    {
        if (workout == null)
            return;

        // Confirmation before deletion
        bool isConfirmed = await App.Current.MainPage.DisplayAlert("Confirm Delete", $"Do you want to delete {workout.WorkoutName}?", "Yes", "No");
        if (!isConfirmed)
            return;

        try
        {
            // Call service to delete the workout
            await _userWorkoutService.DeleteWorkoutAsync(workout.UserWorkoutId);

            // Remove from ObservableCollection
            Workouts.Remove(workout);
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", $"Failed to delete workout: {ex.Message}", "OK");
        }
    }

    private async Task NavigateToCustomExercisesPage(UserWorkoutModel workout)
    {
        if (workout == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Workout not found.", "OK");
            return;
        }

        // Navigate to CustomExercisesPage with WorkoutId as a query parameter
        await Shell.Current.GoToAsync($"{nameof(CustomExercisesPage)}?workoutId={workout.UserWorkoutId}");
    }


}
