using FitnessAplikacia.Services;
using FitnessAplikacia.Views;
using FitnessAplikacia.ViewModels;

namespace FitnessAplikacia
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        private readonly AuthService _authService;

        public App(AuthService authService)
        {
            // Configure services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            _authService = authService;
            InitializeComponent();

            // Temporary loading screen
            MainPage = new ContentPage();
            CheckAuthentication(); // Check token and navigate to the appropriate page
        }

        public void NavigateToPage(Page page)
        {
            MainPage = page;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<HttpClient>();
            services.AddSingleton<AuthService>();
            services.AddSingleton<MealService>();
            services.AddTransient<MealsViewModel>();
            services.AddTransient<MealsPage>();
            // Add other services or pages as needed
        }

        private async void CheckAuthentication()
        {
            // Check if the user is already authenticated
            if (await _authService.IsAuthenticatedAsync())
            {
                // If authenticated, navigate to the main app (AppShell)
                MainPage = new AppShell();
            }
            else
            {
                // If not authenticated, navigate to the login page
                MainPage = new NavigationPage(new LoginPage(_authService));
            }
        }
    }
}
