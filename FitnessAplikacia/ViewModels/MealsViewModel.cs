
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FitnessAplikacia.Services;

using FitnessAplikacia.Views;
using FitnessAplikacia.Models;

namespace FitnessAplikacia.ViewModels
{
    public class MealsViewModel : BaseViewModel
    {
        public MealsViewModel() : this(new MealService(new HttpClient())) { }
        private readonly MealService _mealService;
        public ObservableCollection<MealSection> MealSections { get; } = new ObservableCollection<MealSection>();

        public MealsViewModel(MealService mealService)
        {
            _mealService = mealService ?? throw new ArgumentNullException(nameof(mealService));
            // Optionally call LoadMeals with default parameters if needed.
            LoadMealsAsync();
        }

        private async void LoadMealsAsync()
        {
            try
            {
                var mealSections = await _mealService.GetMealsGroupedByTypeAsync();
                MealSections.Clear();

                foreach (var section in mealSections)
                {
                    foreach (var meal in section.Meals)
                    {
                        Console.WriteLine($"Meal: {meal.NazovJedla}, Image: {meal.NazovObrJedlo}");
                    }

                    MainThread.BeginInvokeOnMainThread(() => MealSections.Add(section));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading meals: {ex.Message}");
            }
        }


        public async Task NavigateToMealDetailAsync(int jedloId)
        {
            try
            {
                var mealDetail = await _mealService.GetMealDetailsAsync(jedloId);

                if (mealDetail != null)
                {
                    // Pass the correct MealDetailDto to the MealDetailPage constructor
                    await Application.Current.MainPage.Navigation.PushAsync(new MealDetailPage(mealDetail));
                }
                else
                {
                    Console.WriteLine("Meal detail not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to meal detail: {ex.Message}");
            }
        }
    }
}
