using System.Collections.ObjectModel;
using FitnessAplikacia.Services;
using FitnessAplikacia.Models;
using System.Windows.Input;


namespace FitnessAplikacia.Views;

public partial class CustomWorkoutsPage : ContentPage
{
    private readonly CustomWorkoutsPageViewModel _viewModel;

    public CustomWorkoutsPage(CustomWorkoutsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh workouts whenever the page appears
        await _viewModel. RefreshWorkouts();
    }

}