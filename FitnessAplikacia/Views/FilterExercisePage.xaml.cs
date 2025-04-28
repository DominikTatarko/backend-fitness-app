using FitnessAplikacia.ViewModels;
using FitnessAplikacia.Services;
using FitnessAplikacia;
using FitnessAplikacia.Models;

namespace FitnessAplikacia.Views;

public partial class FilterExercisePage : ContentPage
{
    private AddExercisePageViewModel _parentViewModel;
    public FilterExercisePage(AddExercisePageViewModel parentViewModel)
	{
		InitializeComponent();
        _parentViewModel = parentViewModel;
        BindingContext = parentViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_parentViewModel != null)
        {
            Console.WriteLine("FilterExercisePage appeared. Loading filter data...");
            await _parentViewModel.LoadFilterData(); // ? Ensure data is loaded when the page appears
        }
        else
        {
            Console.WriteLine("ViewModel is NULL in FilterExercisePage.");
        }
    }


    private async void OnSelectExerciseClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            Console.WriteLine($"Sender is a Button.");

            if (button.CommandParameter is ExerciseDto selectedExercise)
            {
                Console.WriteLine($"Selected Exercise: {selectedExercise.ExerciseName}");

                // Navigate back to the AddExercisePage with the selected exercise name
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
            {
                { "SelectedExerciseName", selectedExercise.ExerciseName }
            });
            }
            else
            {
                Console.WriteLine($"CommandParameter is not of type ExerciseDto or is null.");
                if (button.CommandParameter != null)
                    Console.WriteLine($"CommandParameter type: {button.CommandParameter.GetType().FullName}");
            }
        }
        else
        {
            Console.WriteLine($"Sender is not a Button. Sender type: {sender.GetType().FullName}");
        }
    }








}