using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using FitnessAplikacia.ViewModels;
using FitnessAplikacia.Services;

namespace FitnessAplikacia.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly HomePageViewModel _viewModel;
        public HomePage(HomePageViewModel viewModel, AuthService authService)
        {
            InitializeComponent();
            _authService = App.Current.Handler.MauiContext.Services.GetService<AuthService>();
           
            BindingContext = viewModel;

            // Load user info when the page appears
            Loaded += OnPageLoaded;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HomePageViewModel viewModel)
            {
                // Get the selected day from the view model
                var selectedDate = viewModel.SelectedDay?.Date;

                // Check if SelectedDay is not null, then load notes for that day
                if (selectedDate.HasValue)
                {
                    await viewModel.LoadNotesForSelectedDay(selectedDate.Value);
                }
            }
        }

        private async void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            if (BindingContext is HomePageViewModel viewModel)
            {
                await viewModel.LoadNotesForSelectedDay(e.NewDate);
            }
        }

        private async void OnPageLoaded(object sender, EventArgs e)
        {
            if (BindingContext is HomePageViewModel viewModel)
            {
                await viewModel.LoadUserInfoAsync();
            }
        }

        
    }
}
