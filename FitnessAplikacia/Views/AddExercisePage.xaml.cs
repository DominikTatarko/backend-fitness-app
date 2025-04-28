using FitnessAplikacia.ViewModels;
using FitnessAplikacia.Services;
using FitnessAplikacia;
using System.Web;
using FitnessAplikacia.Models;
using Newtonsoft.Json;

namespace FitnessAplikacia.Views;

[QueryProperty(nameof(Workout), "Workout")]
public partial class AddExercisePage : ContentPage
{
    private UserWorkoutModel _workout;
    public UserWorkoutModel Workout
    {
        get => _workout;
        set
        {
            _workout = value;

            // Do something with the workout, such as updating the UI
            OnPropertyChanged(nameof(Workout));
        }
    }

    public AddExercisePage(AddExercisePageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Use the workout object as needed
        if (Workout != null)
        {
            Console.WriteLine($"Navigated with Workout: {Workout.WorkoutName}");
        }
    }




}