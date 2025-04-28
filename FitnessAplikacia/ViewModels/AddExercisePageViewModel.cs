using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FitnessAplikacia.Views;
using System.ComponentModel;

namespace FitnessAplikacia.ViewModels
{
    [QueryProperty(nameof(WorkoutId), "WorkoutId")]
    
    [QueryProperty(nameof(SelectedExerciseName), "SelectedExerciseName")]
    public class AddExercisePageViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly UserWorkoutService _exerciseService;

        public ObservableCollection<ExerciseDto> AllExercises { get; set; } // Contains all exercises
        public ObservableCollection<MuscleGroupDto> MuscleGroups { get; set; } = new ObservableCollection<MuscleGroupDto>();
        public ObservableCollection<DifficultyDto> Difficulties { get; set; } = new ObservableCollection<DifficultyDto>();
        public ObservableCollection<ExerciseDto> FilteredExercises { get; set; } = new ObservableCollection<ExerciseDto>();

        private int _workoutId;
        public int WorkoutId
        {
            get => _workoutId;
            set
            {
                _workoutId = value;
                LoadSelectedWorkout(); // Fetch workout details and exercises
            }
        }

        private MuscleGroupDto _selectedMuscleGroup;
        public MuscleGroupDto SelectedMuscleGroup
        {
            get => _selectedMuscleGroup;
            set => SetProperty(ref _selectedMuscleGroup, value);
        }

        private DifficultyDto _selectedDifficulty;
        public DifficultyDto SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
        }

        private string _selectedExerciseName;
        public string SelectedExerciseName
        {
            get => _selectedExerciseName;
            set => SetProperty(ref _selectedExerciseName, value);
        }

        private UserWorkoutModel _selectedWorkout;
        public UserWorkoutModel SelectedWorkout
        {
            get => _selectedWorkout;
            set => SetProperty(ref _selectedWorkout, value);  // If using INotifyPropertyChanged
        }



        public string SearchTerm { get; set; }
        private int _sets;
        public int Sets
        {
            get => _sets;
            set => SetProperty(ref _sets, value);
        }

        private string _reps;
        public string Reps
        {
            get => _reps;
            set => SetProperty(ref _reps, value);
        }

        private string _rest;
        public string Rest
        {
            get => _rest;
            set => SetProperty(ref _rest, value);
        }


        public ICommand FilterExercisesCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand NavigateToFilterPageCommand { get; }
        public ICommand CreateExerciseCommand { get; }

        public AddExercisePageViewModel(UserWorkoutService exerciseService)
        {
            _exerciseService = exerciseService;

            // Initialize collections
            AllExercises = new ObservableCollection<ExerciseDto>();

            FilterExercisesCommand = new Command(async () => await FilterExercises());
            SelectExerciseCommand = new Command<ExerciseDto>(async (exercise) => await SelectExercise(exercise));
            NavigateToFilterPageCommand = new Command(async () => await NavigateToFilterPage());
            CreateExerciseCommand = new Command(OnAddExerciseClicked);

            // ✅ Load Exercises on Startup
            Task.Run(async () => await LoadAllExercises());
        }


        private async void OnAddExerciseClicked()
        {
            // Ensure an exercise is selected
            if (string.IsNullOrWhiteSpace(SelectedExerciseName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please select an exercise.", "OK");
                return;
            }

            // Ensure a workout is selected
            if (SelectedWorkout == null || WorkoutId == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Workout ID is missing.", "OK");
                return;
            }

            // Validate input fields
            if (Sets <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid number of sets.", "OK");
                return;
            }

            if (!int.TryParse(Reps, out int reps) || reps <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid number of reps.", "OK");
                return;
            }

            if (!int.TryParse(Rest, out int rest) || rest < 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid rest time.", "OK");
                return;
            }

            // Find the selected exercise
            var selectedExercise = AllExercises.FirstOrDefault(ex =>
                string.Equals(ex.ExerciseName, SelectedExerciseName, StringComparison.OrdinalIgnoreCase));

            if (selectedExercise == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Selected exercise not found.", "OK");
                return;
            }
            
            // Create exercise DTO
            var userExerciseDto = new UserExerciseDto
            {
                UserWorkoutId = WorkoutId,
                ExerciseId = selectedExercise.ExerciseId,
                Sets = Sets,
                Reps = reps.ToString(),
                Rest = rest.ToString()
            };

            // Save to database
            var addedExercise = await _exerciseService.AddExerciseToWorkoutAsync(userExerciseDto);
            if (addedExercise != null)
            {
                await App.Current.MainPage.DisplayAlert("Success", "Exercise added successfully!", "OK");

                // ✅ Navigate back to CustomExercisePage after successful addition
                await Shell.Current.GoToAsync("../../..");




            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Failed to add exercise.", "OK");
            }
        }







        private async Task NavigateToFilterPage()
        {
            var navigationParams = new Dictionary<string, object>
        {
            { "WorkoutId", WorkoutId } // Ensure WorkoutId is passed
        };

            Console.WriteLine($"🚀 Navigating to FilterExercisePage with parameters: {JsonConvert.SerializeObject(navigationParams)}");

            await Shell.Current.GoToAsync(nameof(FilterExercisePage), true, navigationParams);
        }


        public async Task LoadFilterData()
        {
            IsBusy = true;
            try
            {
                Console.WriteLine("Fetching filter data in ViewModel...");
                var filterData = await _exerciseService.GetFilterDataAsync();

                if (filterData != null)
                {
                    Console.WriteLine($"Filter data: {JsonConvert.SerializeObject(filterData)}");

                    // Populate Muscle Groups
                    MuscleGroups.Clear();
                    foreach (var muscleGroup in filterData.MuscleGroups)
                    {
                        MuscleGroups.Add(muscleGroup);
                    }

                    // Populate Difficulties
                    Difficulties.Clear();
                    foreach (var difficulty in filterData.Difficulties)
                    {
                        Difficulties.Add(difficulty);
                    }

                    Console.WriteLine("Data populated successfully in ViewModel.");
                }
                else
                {
                    Console.WriteLine("Filter data is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading filter data in ViewModel: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        



        private async Task FilterExercises()
        {
            IsBusy = true;
            try
            {
                // Validate and extract filter IDs
                int? muscleGroupId = SelectedMuscleGroup != null && SelectedMuscleGroup.MuscleGroupId != 0
                    ? SelectedMuscleGroup.MuscleGroupId
                    : (int?)null;

                int? difficultyId = SelectedDifficulty != null && SelectedDifficulty.DifficultyId != 0
                    ? SelectedDifficulty.DifficultyId
                    : (int?)null;

                // Ensure searchTerm is a valid string
                string searchTerm = string.IsNullOrEmpty(SearchTerm) ? "" : SearchTerm;

                // Debug the query parameters
                System.Diagnostics.Debug.WriteLine($"Filter Query: muscleGroupId={muscleGroupId}, difficultyId={difficultyId}, searchTerm=\"{searchTerm}\"");

                // Fetch filtered exercises
                var exercises = await _exerciseService.GetExercisesByFilterAsync(muscleGroupId, difficultyId, searchTerm);

                // Randomize and limit to 20 exercises if more are returned
                var limitedExercises = exercises
                    .OrderBy(_ => Guid.NewGuid()) // Randomize order
                    .Take(20) // Limit to 20 items
                    .ToList();

                // Update filtered exercises list
                FilteredExercises.Clear();
                foreach (var exercise in limitedExercises)
                {
                    FilteredExercises.Add(exercise);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to filter exercises: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }




        private async Task SelectExercise(ExerciseDto exercise)
        {
            try
            {
                // Log the value of WorkoutId to ensure it's not 0
                Console.WriteLine($"SelectExercise triggered. WorkoutId: {WorkoutId}, ExerciseName: {exercise.ExerciseName}");

                // Check if WorkoutId is valid (not 0) before proceeding
                if (WorkoutId == 0)
                {
                     Console.WriteLine("Error: WorkoutId is 0.");
                    await App.Current.MainPage.DisplayAlert("Error", "WorkoutId is not set correctly.", "OK");
                    return; // Exit if WorkoutId is not valid
                }

                // Log the parameters being passed
                Console.WriteLine($"Navigating to AddExercisePage with WorkoutId: {WorkoutId} and ExerciseName: {exercise.ExerciseName}");

                // Encode the exercise name for the query string
                string exerciseName = Uri.EscapeDataString(exercise.ExerciseName);

                // Navigate back to AddExercisePage with the exercise name and WorkoutId
                var navigationParams = new Dictionary<string, object>
                {
                    { "WorkoutId", WorkoutId },
                    { "SelectedExerciseName", exercise.ExerciseName }
                };

                // Debug before navigation
                Console.WriteLine($"Navigating with parameters: {JsonConvert.SerializeObject(navigationParams)}");

                await Shell.Current.GoToAsync(nameof(AddExercisePage), true, navigationParams);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to select exercise: {ex.Message}", "OK");
            }
        }




        public override void OnNavigatedTo(IDictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);

            // Log the navigation parameters
            Console.WriteLine($"OnNavigatedTo triggered with parameters: {JsonConvert.SerializeObject(parameters)}");

            if (parameters.ContainsKey("Workout"))
            {
                string workoutJson = parameters["Workout"] as string;
                if (!string.IsNullOrEmpty(workoutJson))
                {
                    var workout = JsonConvert.DeserializeObject<UserWorkoutModel>(Uri.UnescapeDataString(workoutJson));
                    // Log the workout data
                    Console.WriteLine($"Navigated to AddExercisePage with workout: {workout?.WorkoutName}");
                }
            }
        }



        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Console.WriteLine($"ApplyQueryAttributes triggered. Query Parameters: {JsonConvert.SerializeObject(query)}");

            if (query.TryGetValue("WorkoutId", out var workoutIdObj) && int.TryParse(workoutIdObj.ToString(), out int workoutId))
            {
                WorkoutId = workoutId;
                Console.WriteLine($"Assigned WorkoutId: {WorkoutId}");
            }
            else
            {
                Console.WriteLine("WorkoutId parameter not found or invalid.");
            }

            if (query.TryGetValue("SelectedExerciseName", out var exerciseNameObj))
            {
                SelectedExerciseName = exerciseNameObj.ToString();
                Console.WriteLine($"Assigned Exercise Name: {SelectedExerciseName}");
            }
        }




        private async Task LoadSelectedWorkout()
        {
            if (WorkoutId == 0)
            {
                Console.WriteLine("LoadSelectedWorkout() skipped: WorkoutId is 0.");
                return;
            }

            Console.WriteLine($"Fetching workout for WorkoutId: {WorkoutId}");
            var allWorkouts = await _exerciseService.GetAllUserWorkoutsAsync();
            var selectedWorkout = allWorkouts.FirstOrDefault(w => w.UserWorkoutId == WorkoutId);

            if (selectedWorkout != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SelectedWorkout = selectedWorkout;
                    Console.WriteLine($"Workout Loaded: {SelectedWorkout.WorkoutName} (ID: {SelectedWorkout.UserWorkoutId})");
                });
            }
            else
            {
                Console.WriteLine("Workout not found.");
            }
        }


        private async Task LoadAllExercises()
        {
            try
            {
                Console.WriteLine("Fetching all exercises (no filters applied)...");

                // Call the existing method with null filters to get all exercises
                var exercises = await _exerciseService.GetExercisesByFilterAsync(null, null, "");

                if (exercises != null && exercises.Any())
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AllExercises.Clear();
                        foreach (var exercise in exercises)
                        {
                            AllExercises.Add(exercise);
                        }
                    });

                    Console.WriteLine($"Loaded {AllExercises.Count} exercises.");
                }
                else
                {
                    Console.WriteLine("No exercises found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercises: {ex.Message}");
            }
        }





    }

}

