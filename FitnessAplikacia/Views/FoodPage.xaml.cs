using FitnessAplikacia.Services;
using Microsoft.Maui.Controls;
using FitnessAplikacia.ViewModels;


namespace FitnessAplikacia.Views;

public partial class FoodPage : ContentPage
{
	public FoodPage()
	{
		InitializeComponent();
	}

    private async void OnOpenCalorieCalculatorTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CalorieCalculator());
    }


    private async void OnOpenMealsPageTapped(object sender, EventArgs e)
    {
        var mealService = ServiceHelper.ServiceProvider.GetRequiredService<MealService>();
        var mealsPage = ServiceHelper.ServiceProvider.GetRequiredService<MealsPage>();

        await Navigation.PushAsync(mealsPage);
    }




}