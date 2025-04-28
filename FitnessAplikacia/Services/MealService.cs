using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using FitnessAplikacia.Models;
using System.Threading.Tasks;

namespace FitnessAplikacia.Services
{
    public class MealService
    {
        private readonly HttpClient _httpClient;

        public MealService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<MealSection>> GetMealsGroupedByTypeAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<MealDto>>("Meals");

            if (response == null)
            {
                throw new HttpRequestException("No meals were found.");
            }

            var groupedMeals = response
                .GroupBy(meal => meal.TypJedla)
                .Select(group => new MealSection
                {
                    TypeName = group.Key,
                    Meals = group.ToList()
                })
                .ToList();

            return groupedMeals;
        }

        public async Task<MealDetailDto> GetMealDetailsAsync(int jedloId)
        {
            try
            {
                // Fetch the meal data grouped by type
                var meals = await GetMealsGroupedByTypeAsync();

                // Find the meal with the specified 'jedloId'
                var meal = meals
                    .SelectMany(m => m.Meals) // Flatten the grouped meals list
                    .FirstOrDefault(m => m.JedloId == jedloId); // Find by JedloId

                if (meal != null)
                {
                    // If the meal is found, return the meal details including the steps (kroky)
                    var mealDetail = new MealDetailDto
                    {
                        JedloId = meal.JedloId,
                        NazovJedla = meal.NazovJedla,
                        Kalorie = meal.Kalorie,
                        Sacharidy = meal.Sacharidy,
                        Tuky = meal.Tuky,
                        TypJedla = meal.TypJedla,
                        Kroky = meal.Kroky // Assuming meal has a 'Kroky' field (steps)
                    };

                    return mealDetail;
                }
                else
                {
                    Console.WriteLine($"Meal with ID {jedloId} not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching meal details: {ex.Message}");
                return null;
            }
        }



    }

    public class MealSection
    {
        public string TypeName { get; set; }
        public List<MealDto> Meals { get; set; }
    }

    

    

    public class KrokDto
    {
        public int CisloKroku { get; set; }
        public string PopisKroku { get; set; }
    }
}
