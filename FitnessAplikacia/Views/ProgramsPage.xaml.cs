using FitnessAplikacia.Services;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;
using FitnessAplikacia.ViewModels;


namespace FitnessAplikacia.Views
{
    public partial class ProgramsPage : ContentPage
    {
        private readonly ProgramService _programService;
        public ObservableCollection<FitnessProgram> Programs { get; set; }
        public ICommand NavigateToDaysCommand { get; }
        public ProgramsPage(ProgramService programService)
        {
            InitializeComponent();
            _programService = programService;
            Programs = new ObservableCollection<FitnessProgram>();
            BindingContext = this;
            NavigateToDaysCommand = new Command<FitnessProgram>(OnNavigateToDays);
            
            LoadPrograms();
        }

        private async void OnCustomWorkoutsTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                await frame.ScaleTo(0.95, 100); // Shrink button
                await frame.ScaleTo(1, 100);   // Return to original size
            }
            // Log navigation for debugging
            Console.WriteLine("Navigating to CustomWorkoutsPage...");

            // Perform navigation
            await Shell.Current.GoToAsync("CustomWorkoutsPage");
        }

        private async void LoadPrograms()
        {
            try
            {
                Console.WriteLine("Loading programs...");
                var programs = await _programService.GetProgramsAsync();
                Console.WriteLine($"Loaded {programs.Count} programs.");

                Programs.Clear();
                foreach (var program in programs)
                {
                    Programs.Add(program);
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Network error: {httpEx.Message}");
                await DisplayAlert("Error", $"Failed to connect to API: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            }
        }

        private void OnProgramSelected(object sender, SelectionChangedEventArgs e)
        {
            
            var selectedProgram = e.CurrentSelection.FirstOrDefault() as FitnessProgram;
            if (selectedProgram != null)
            {
                OnNavigateToDays(selectedProgram);
            }
        }

        private async void OnProgramTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            if (frame != null)
            {
                // Animate scale down and up
                await frame.ScaleTo(0.9, 100); // Shrink frame
                await frame.ScaleTo(1, 100);   // Return to original size
            }
        }
        private async void OnNavigateToDays(FitnessProgram selectedProgram)
        {
            if (selectedProgram != null)
            {
                Console.WriteLine($"Tapped on program: {selectedProgram.name}");
                await Shell.Current.GoToAsync($"/DaysPage?programId={selectedProgram.programId}");
            }
            else
            {
                Console.WriteLine("Selected program is null!");
            }
        }

        
    }
}
