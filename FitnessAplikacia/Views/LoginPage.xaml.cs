namespace FitnessAplikacia.Views;
using FitnessAplikacia.Services;
using FitnessAplikacia.Models;
public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    public LoginPage(AuthService authService)
	{
		InitializeComponent();
        _authService = authService;
    }
    private void OnBackClicked(object sender, EventArgs e)
    {
        if (Application.Current is App app)
        {
            app.NavigateToPage(new RegisterPage(_authService));
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var model = new LoginModel
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        // Call AuthService to log in
        var success = await _authService.LoginAsync(model);
        if (success)
        {
            // SecureStorage already handled in LoginAsync
            
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid email or password.", "OK");
        }
    }


}