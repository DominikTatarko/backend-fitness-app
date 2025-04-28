namespace FitnessAplikacia.Views;
using Microsoft.Maui.Controls;
using System;
using FitnessAplikacia.ViewModels;
using FitnessAplikacia.Models;
using FitnessAplikacia.Services;
using System.Text.Json;

public partial class MealsPage : ContentPage
{
    private readonly MealsViewModel _viewModel;
    private readonly MealService _mealService;
    public MealsPage(MealsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;  // The viewModel is provided by DI
        BindingContext = viewModel;
    }

    private async void OnMealTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MealDto selectedMeal)
        {
            var viewModel = BindingContext as MealsViewModel;
            if (viewModel != null)
            {
                await viewModel.NavigateToMealDetailAsync(selectedMeal.JedloId);
            }
        }
        else
        {
            Console.WriteLine("Selected meal is null or of incorrect type.");
        }
    }






}