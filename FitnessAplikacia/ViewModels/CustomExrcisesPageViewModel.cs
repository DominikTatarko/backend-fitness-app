using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using FitnessAplikacia.Views;

namespace FitnessAplikacia.ViewModels
{
    [QueryProperty(nameof(WorkoutId), "workoutId")] // Fetch workoutId as a query parameter
    public class CustomExercisesPageViewModel : BaseViewModel
    {
        private int _workoutId;
        public int WorkoutId
        {
            get => _workoutId;
            set
            {
                _workoutId = value;
                LoadWorkoutDataAsync(); // Fetch workout details and exercises
            }
        }

        private string _workoutName;
        public string WorkoutName
        {
            get => _workoutName;
            set => SetProperty(ref _workoutName, value);
        }

        private UserWorkoutModel _selectedWorkout;
        public UserWorkoutModel SelectedWorkout
        {
            get => _selectedWorkout;
            set => SetProperty(ref _selectedWorkout, value);
        }


        public ObservableCollection<UserExerciseModel> Exercises { get; set; } = new ObservableCollection<UserExerciseModel>();

        public ICommand AddExerciseCommand { get; }
        public ICommand DeleteExerciseCommand { get; }
        private readonly UserWorkoutService _workoutService;

        public CustomExercisesPageViewModel(UserWorkoutService workoutService)
        {
            _workoutService = workoutService;
            DeleteExerciseCommand = new Command<int>(async (exerciseId) => await DeleteExercise(exerciseId));
            AddExerciseCommand = new Command(async () =>
            {
                if (SelectedWorkout == null)
                {
                    Console.WriteLine("SelectedWorkout is null. Cannot navigate.");
                    await App.Current.MainPage.DisplayAlert("Error", "Please select a workout first.", "OK");
                    return;
                }

                await NavigateToAddExercisePage(SelectedWorkout);
            });

        }



        private async Task LoadWorkoutDataAsync()
        {
            if (WorkoutId == 0) return; // Ensure a valid WorkoutId

            IsBusy = true;
            try
            {
                // Fetch all workouts and find the selected one
                var allWorkouts = await _workoutService.GetAllUserWorkoutsAsync();
                var selectedWorkout = allWorkouts.FirstOrDefault(w => w.UserWorkoutId == WorkoutId);

                if (selectedWorkout == null)
                {
                    Console.WriteLine($"No workout found with ID: {WorkoutId}");
                    return;
                }

                // ✅ SET SelectedWorkout HERE
                SelectedWorkout = selectedWorkout;

                // ✅ Also set the WorkoutName
                WorkoutName = selectedWorkout.WorkoutName;

                // Fetch exercises for this workout
                var exercises = await _workoutService.GetExercisesByWorkoutIdAsync(WorkoutId);
                Exercises.Clear(); // Clear existing exercises
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise); // Add each exercise to the ObservableCollection
                }

                Console.WriteLine($"Workout Loaded: {SelectedWorkout.WorkoutName} (ID: {SelectedWorkout.UserWorkoutId})");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task NavigateToAddExercisePage(UserWorkoutModel workout)
        {
            if (workout == null)
            {
                Console.WriteLine("Workout is null, cannot navigate.");
                return;
            }

            Console.WriteLine($"Navigating to AddExercisePage with Workout ID: {workout.UserWorkoutId}");

            var navigationParams = new Dictionary<string, object>
    {
        { "WorkoutId", workout.UserWorkoutId } // Pass as an integer, not a string
    };

            await Shell.Current.GoToAsync(nameof(AddExercisePage), true, navigationParams);
        }


        private async Task DeleteExercise(int userExerciseId)
        {
            bool isDeleted = await _workoutService.DeleteExerciseAsync(userExerciseId);
            if (isDeleted)
            {
                var exerciseToRemove = Exercises.FirstOrDefault(e => e.UserExerciseId == userExerciseId); // ✅ FIXED
                if (exerciseToRemove != null)
                {
                    Exercises.Remove(exerciseToRemove);
                    Console.WriteLine($"Successfully deleted exercise with ID: {userExerciseId}");
                }
            }
            else
            {
                Console.WriteLine($"Failed to delete exercise with ID: {userExerciseId}");
            }
        }




    }

}



