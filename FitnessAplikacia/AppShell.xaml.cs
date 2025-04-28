
using FitnessAplikacia.Views;

namespace FitnessAplikacia
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Console.WriteLine("Registering DaysPage route...");
            Routing.RegisterRoute(nameof(DaysPage), typeof(DaysPage));
            Routing.RegisterRoute("WorkoutPage", typeof(WorkoutPage));
            Routing.RegisterRoute(nameof(MealsPage), typeof(MealsPage));
            Routing.RegisterRoute(nameof(MealDetailPage), typeof(MealDetailPage));
            Routing.RegisterRoute(nameof(CustomWorkoutsPage), typeof(CustomWorkoutsPage));
            Routing.RegisterRoute(nameof(AddWorkoutPage), typeof(AddWorkoutPage));
            Routing.RegisterRoute(nameof(CustomExercisesPage), typeof(CustomExercisesPage));
            Routing.RegisterRoute(nameof(AddExercisePage), typeof(AddExercisePage));
            Routing.RegisterRoute(nameof(FilterExercisePage), typeof(FilterExercisePage));
            Routing.RegisterRoute(nameof(SetGoalPage), typeof(SetGoalPage));
            Routing.RegisterRoute(nameof(VideoPage), typeof(VideoPage));







        }


    }
}
