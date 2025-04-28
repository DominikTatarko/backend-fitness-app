

namespace FitnessAplikacia.Views;

public partial class CalorieCalculator : ContentPage
{
	public CalorieCalculator()
	{
		InitializeComponent();
	}

    private void OnCalculateCaloriesClicked(object sender, EventArgs e)
    {
        if (double.TryParse(WeightEntry.Text, out double weight) &&
            double.TryParse(HeightEntry.Text, out double height) &&
            double.TryParse(AgeEntry.Text, out double age) &&
            GenderPicker.SelectedItem != null)
        {
            string gender = GenderPicker.SelectedItem.ToString();
            double bmr = 0;

            // Calculate BMR using Harris-Benedict Equation
            if (gender == "Muž")
            {
                bmr = 88.362 + (13.397 * weight) + (4.799 * height) - (5.677 * age);
            }
            else if (gender == "Žena")
            {
                bmr = 447.593 + (9.247 * weight) + (3.098 * height) - (4.330 * age);
            }

            ResultLabel.Text = $"Tvoje denný príjem kaloríí bude: {Math.Round(bmr)} kcal.";
        }
        else
        {
            ResultLabel.Text = "Please enter valid details.";
        }
    }

}