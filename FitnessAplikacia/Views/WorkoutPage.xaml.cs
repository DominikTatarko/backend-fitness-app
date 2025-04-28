using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FitnessAplikacia.Services;

namespace FitnessAplikacia.Views
{
    [QueryProperty(nameof(ProgramId), "programId")]
    [QueryProperty(nameof(Day), "day")]
    public partial class WorkoutPage : ContentPage
    {
        private readonly ProgramService _programService;

        public ObservableCollection<FitnessExercise> Exercises { get; set; } = new();

        // Bindable properties for query parameters
        public int ProgramId { get; set; }
        public int Day { get; set; }

        // Fixed the command type to match FitnessExercise
        public Command<FitnessExercise> NavigateToVideoCommand { get; }

        public WorkoutPage(ProgramService programService)
        {
            InitializeComponent();
            _programService = programService;
            BindingContext = this;

            // Initialize command with correct parameter type
            NavigateToVideoCommand = new Command<FitnessExercise>(OnNavigateToVideo);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            Console.WriteLine($"WorkoutPage: Received ProgramId={ProgramId}, Day={Day}");
            LoadWorkout(ProgramId, Day);
        }

        private async Task LoadWorkout(int programId, int day)
        {
            try
            {
                Console.WriteLine($"Loading workout for ProgramId: {programId}, Day: {day}");
                var exercises = await _programService.GetWorkoutsAsync(programId, day);

                Exercises.Clear();
                foreach (var exercise in exercises)
                {
                    Console.WriteLine($"Processing exercise: {exercise.ExerciseName}, Sets: {exercise.Sets}, Reps: {exercise.Reps}");

                    if (exercise.Reps?.ToLower() == "rest day" || exercise.Reps?.ToLower() == "rest")
                    {
                        Exercises.Add(new FitnessExercise
                        {
                            ExerciseName = "REST DAY",
                            Description = "Take a break today and recover!",
                            Reps = "",
                            ExerciseOrder = 0
                        });
                    }
                    else
                    {
                        Exercises.Add(new FitnessExercise
                        {
                            ExerciseName = exercise.ExerciseName,
                            Description = $"Serie: {exercise.Sets ?? 0}, Odych: {exercise.Rest ?? "No Rest Info"}",
                            Reps = $"Opakovania: {exercise.Reps}",
                            ExerciseOrder = exercise.ExerciseOrder,
                            YouTubeLink = exercise.YouTubeLink // Ensure YouTubeLink exists
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading workout: {ex.Message}");
                await DisplayAlert("Error", "Unable to load the workout.", "OK");
            }
        }

        private async void OnNavigateToVideo(FitnessExercise selectedExercise)
        {
            if (selectedExercise == null)
            {
                Console.WriteLine("No exercise selected.");
                return;
            }

            Console.WriteLine($"Navigating to video for: {selectedExercise.ExerciseName}");

            if (!string.IsNullOrEmpty(selectedExercise.YouTubeLink))
            {
                // Convert YouTube URL to Embed Format
                string embedUrl = ConvertToEmbedUrl(selectedExercise.YouTubeLink);
                Console.WriteLine($"Navigating to VideoPage with URL: {embedUrl}");

                // Use Uri.EscapeDataString to prevent URL encoding issues
                await Shell.Current.GoToAsync($"VideoPage?videoUrl={Uri.EscapeDataString(embedUrl)}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No video available for this exercise.", "OK");
            }
        }



        private string ConvertToEmbedUrl(string youtubeUrl)
        {
            if (string.IsNullOrWhiteSpace(youtubeUrl))
                return string.Empty;

            string videoId = "";
            var match = Regex.Match(youtubeUrl, @"(?:youtu\.be/|youtube\.com/(?:.*v=|.*/([^/?]*)[^/]*))([^?&]*)");

            if (match.Success)
            {
                videoId = match.Groups[2].Value;
            }

            return !string.IsNullOrEmpty(videoId) ? $"https://www.youtube.com/embed/{videoId}" : string.Empty;
        }
    }
}
