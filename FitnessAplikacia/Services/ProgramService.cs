using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Maui.Devices;

namespace FitnessAplikacia.Services
{
    public class ProgramService
    {
        private readonly HttpClient _httpClient;

        public ProgramService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Fetch all programs
        public async Task<List<FitnessProgram>> GetProgramsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<FitnessProgram>>("Programs");
                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;

            }
            catch (Exception ex)
            {
                // Log raw response
                var response = await _httpClient.GetAsync("Programs");
                Console.WriteLine($"Raw response: {await response.Content.ReadAsStringAsync()}");
                Console.WriteLine($"Error in GetProgramsAsync: {ex.Message}");
                throw;
            }

        }


        // Fetch days for a specific program
        public async Task<List<int>> GetDaysAsync(int programId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<int>>($"Programs/{programId}/days");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching days for program {programId}: {ex.Message}");
                throw;
            }
        }
        // Fetch workouts for a specific day in a program
        public async Task<List<WorkoutDay>> GetWorkoutsAsync(int programId, int day)
        {
            var response = await _httpClient.GetAsync($"Programs/{programId}/days/{day}/workouts");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Fetched JSON: {json}");  // Debugging line
                return JsonConvert.DeserializeObject<List<WorkoutDay>>(json);
            }
            Console.WriteLine($"Failed to fetch workouts: {response.StatusCode}");
            return new List<WorkoutDay>();
        }
    }

    // Models for API responses
    public class FitnessProgram
    {
        public int programId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string nazovObrProgram { get; set; }
        public List<WorkoutDay> Days { get; set; }
    }

    public class FitnessExercise
    {
        
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public int? Sets { get; set; }
        public string Reps { get; set; }
        
        public int? ExerciseOrder { get; set; }

        public string YouTubeLink { get; set; }
    }
    public class WorkoutDay
    {
        public int ProgramWorkoutId { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }
        public string DayName { get; set; } // e.g., "Monday"
        public string Activities { get; set; } // e.g., "Chest and Triceps"
        public int? Sets { get; set; } // Ensure this exists
        public string Reps { get; set; }
        public string Rest { get; set; } = "N/A";
        public string ExerciseName { get; set; }
        public int? ExerciseOrder { get; set; }
        public string YouTubeLink {  get; set; }
    }


}
