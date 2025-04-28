
using FitnessAplikacia.Models;

namespace FitnessAplikacia.Views
{
    public partial class MealDetailPage : ContentPage
    {
        // Constructor now accepts a MealDetailDto
        public MealDetailPage(MealDetailDto mealDetail)
        {
            InitializeComponent();
            
            // Set the BindingContext to the passed MealDetailDto
            BindingContext = mealDetail;
        }
    }
}
