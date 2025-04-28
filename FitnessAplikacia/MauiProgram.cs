using Microsoft.Extensions.Logging;
using FitnessAplikacia.Views;
using FitnessAplikacia.Services;
using FitnessAplikacia.ViewModels;

namespace FitnessAplikacia
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("RobotoCondensed-Italic-VariableFont_wght.ttf", "RobotoCondensedItalic");
                    fonts.AddFont("RobotoCondensed-VariableFont_wght.ttf", "RobotoCondensedRegular");
                });


            builder.Logging.AddDebug();
            builder.Services.AddTransient<FoodPage>();
            
            builder.Services.AddTransient<CalorieCalculator>();
            builder.Services.AddSingleton<WorkoutPage>();
            builder.Services.AddSingleton<DaysPage>();
            builder.Services.AddSingleton<ProgramsPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<MealsPage>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<MealsViewModel>();
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddSingleton<UserWorkoutService>();
            builder.Services.AddTransient<CustomWorkoutsPage>();
            builder.Services.AddSingleton<StatisticPage>();
            builder.Services.AddTransient<CustomWorkoutsPageViewModel>();
            builder.Services.AddTransient<AddWorkoutPageViewModel>();
            builder.Services.AddTransient<AddWorkoutPage>();
            builder.Services.AddTransient<CustomExercisesPageViewModel>();
            builder.Services.AddSingleton<UserWorkoutService>();
            builder.Services.AddTransient<AddExercisePageViewModel>();
            builder.Services.AddTransient<AddExercisePage>();
            builder.Services.AddTransient<FilterExercisePage>();
            builder.Services.AddTransient<SetGoalPage>();


            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://192.168.0.114:5036/api/")
                };
                return client;
            });
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ProgramService>();
            builder.Services.AddSingleton<NotesService>();
            builder.Services.AddTransient<MealService>();
            builder.Services.AddSingleton<FitnessService>();

            var app = builder.Build();

            // Set the ServiceProvider in ServiceHelper
            ServiceHelper.ServiceProvider = app.Services;

            return app;
        }
    }
}
