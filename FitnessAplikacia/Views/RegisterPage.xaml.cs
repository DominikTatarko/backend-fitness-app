using FitnessAplikacia.Services;
using FitnessAplikacia.Models;
using System;

namespace FitnessAplikacia.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService; // Initialize AuthService
        }
        private async void NavigateToLogin(object sender, EventArgs e)
        {
            Application.Current.MainPage = new LoginPage(_authService);
            
        }
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(UsernameEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) ||
                string.IsNullOrEmpty(PasswordEntry.Text) || string.IsNullOrEmpty(ConfirmPasswordEntry.Text) ||
                string.IsNullOrEmpty(WeightEntry.Text) || LevelPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (!int.TryParse(WeightEntry.Text, out int weight))
            {
                await DisplayAlert("Error", "Invalid weight. Please enter a valid number.", "OK");
                return;
            }

            // Map Level input to numeric values
            int level;
            switch (LevelPicker.SelectedItem?.ToString().ToLower())
            {
                case "beginner":
                    level = 0;
                    break;
                case "intermediate":
                    level = 1;
                    break;
                case "advanced":
                    level = 2;
                    break;
                default:
                    await DisplayAlert("Error", "Please select a level.", "OK");
                    return;
            }

            // Create a UserModel instance
            var user = new UserModel
            {
                UserName = UsernameEntry.Text,
                Email = EmailEntry.Text,
                Password = PasswordEntry.Text,
                Weight = weight,
                Level = level.ToString() // Convert level to string to match backend expectations
            };

            try
            {
                // Call AuthService to register
                var success = await _authService.RegisterAsync(user);

                if (success)
                {
                    Application.Current.MainPage = new LoginPage(_authService);
                    
                }
                else
                {
                    var errorDetails = _authService.GetLastErrorAsync(); // Assuming a helper method
                    await DisplayAlert("Error", errorDetails ?? "Registration failed. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                 await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
