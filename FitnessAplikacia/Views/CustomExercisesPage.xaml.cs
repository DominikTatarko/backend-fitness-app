using FitnessAplikacia.ViewModels;


namespace FitnessAplikacia.Views;

public partial class CustomExercisesPage : ContentPage
{

    public CustomExercisesPage()
    {
        InitializeComponent();

        // Use dependency injection to resolve the ViewModel
        BindingContext = ServiceHelper.GetService<CustomExercisesPageViewModel>();
    }
    public CustomExercisesPage(CustomExercisesPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}