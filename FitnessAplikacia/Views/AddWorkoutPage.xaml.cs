using FitnessAplikacia.Models;

namespace FitnessAplikacia.Views
{
    public partial class AddWorkoutPage : ContentPage
    {
        public AddWorkoutPage(AddWorkoutPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}
