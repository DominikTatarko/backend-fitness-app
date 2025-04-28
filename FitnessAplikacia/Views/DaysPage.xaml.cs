using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FitnessAplikacia.Services;
using System.Windows.Input;

namespace FitnessAplikacia.Views
{
    [QueryProperty(nameof(ProgramId), "programId")]
    public partial class DaysPage : ContentPage, IQueryAttributable
    {
        private readonly ProgramService _programService;
        public ObservableCollection<int> Days { get; set; }
        public int ProgramId { get; set; }
        public ICommand NavigateToWorkoutCommand { get; }
        public DaysPage(ProgramService programService)
        {
            InitializeComponent();
            _programService = programService;
            Days = new ObservableCollection<int>();
            BindingContext = this;
            NavigateToWorkoutCommand = new Command<int>(OnNavigateToWorkout);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("programId") && query["programId"] is string programIdStr)
            {
                ProgramId = int.Parse(programIdStr);
                Console.WriteLine($"Received programId: {ProgramId}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (ProgramId != 0)
            {
                await LoadDaysAsync(ProgramId);
            }
        }

        private async Task LoadDaysAsync(int programId)
        {
            try
            {
                var days = await _programService.GetDaysAsync(programId);

                Days.Clear();
                foreach (var day in days)
                {
                    Days.Add(day);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load days: {ex.Message}", "OK");
            }
        }

        private async void OnDaySelected(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is int day)
            {
                int programId = Convert.ToInt32(await SecureStorage.GetAsync("programId"));
                Console.WriteLine($"Navigating to WorkoutPage with ProgramId: {programId} and Day: {day}");
                await Shell.Current.GoToAsync($"/WorkoutPage?programId={programId}&day={day}");
              
            }
        }

        private async void OnNavigateToWorkout(int day)
        {
            if (day > 0)
            {
                Console.WriteLine($"Navigating to WorkoutPage with ProgramId: {ProgramId} and Day: {day}");
                await Shell.Current.GoToAsync($"/WorkoutPage?programId={ProgramId}&day={day}");
            }
            else
            {
                Console.WriteLine("Invalid day selected!");
            }
        }
    }
}
