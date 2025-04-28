using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using FitnessAplikacia.Models;
using Newtonsoft.Json;

namespace FitnessAplikacia.Services
{
    public class UserWorkoutService
    {
        private readonly HttpClient _httpClient;

        public UserWorkoutService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        // Get all workouts for the authenticated user
        public async Task<List<UserWorkoutModel>> GetAllUserWorkoutsAsync()
        {
            try
            {
                // Request to the API endpoint to get all user workouts
                var response = await _httpClient.GetAsync("UserWorkout");

                // Ensure successful response
                response.EnsureSuccessStatusCode();

                // Read and parse the response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);

                // Deserialize and return list of workouts
                return JsonConvert.DeserializeObject<List<UserWorkoutModel>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching workouts: {ex.Message}");
                return new List<UserWorkoutModel>(); // Return empty list if failed
            }
        }

        // Add a new workout
        public async Task<UserWorkoutModel> CreateWorkoutAsync(CreateWorkoutDto createWorkoutDto)
        {
            try
            {
                // Serialize the new workout object into JSON
                var json = JsonConvert.SerializeObject(createWorkoutDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request to create a new workout
                var response = await _httpClient.PostAsync("UserWorkout/create", content);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Deserialize and return the created workout
                return await response.Content.ReadFromJsonAsync<UserWorkoutModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating workout: {ex.Message}");
                return null; // Return null if failed
            }
        }

        // Add an exercise to a workout
        public async Task<UserExerciseModel> AddExerciseToWorkoutAsync(UserExerciseDto userExerciseDto)
        {
            try
            {
                // Debug: Print the data being sent
                Console.WriteLine($"Sending Data: {JsonConvert.SerializeObject(userExerciseDto, Formatting.Indented)}");

                // Convert DTO to JSON
                var json = JsonConvert.SerializeObject(userExerciseDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await _httpClient.PostAsync("UserWorkout/addExercise", content);

                // Log the response status
                Console.WriteLine($"Response Status: {response.StatusCode}");

                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                // If response is not successful, log the error and return null
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error adding exercise: {response.StatusCode} - {responseContent}");
                    return null;
                }

                // Deserialize and return the added exercise
                return JsonConvert.DeserializeObject<UserExerciseModel>(responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddExerciseToWorkoutAsync: {ex.Message}");
                return null;
            }
        }


        // Get workout details by ID
        public async Task<UserWorkoutDetailsModel> GetWorkoutDetailsAsync(int id)
        {
            try
            {
                // Get the details of the workout from the API
                var response = await _httpClient.GetAsync($"UserWorkout/{id}");

                // Ensure successful response
                response.EnsureSuccessStatusCode();

                // Deserialize to UserWorkoutModel
                var userWorkoutModel = await response.Content.ReadFromJsonAsync<UserWorkoutModel>();

                if (userWorkoutModel == null)
                {
                    return null; // Return null if no workout is found
                }

                // Manually map UserWorkoutModel to UserWorkoutDetailsModel
                var workoutDetails = new UserWorkoutDetailsModel
                {
                    UserWorkoutId = userWorkoutModel.UserWorkoutId,
                    WorkoutName = userWorkoutModel.WorkoutName,
                    CreatedAt = userWorkoutModel.CreatedAt,
                    // Add other properties as needed, for example:
                    // Exercises = userWorkoutModel.UserExercises
                };

                // Return the mapped details
                return workoutDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching workout details: {ex.Message}");
                return null; // Return null if failed
            }
        }

        // Delete a workout
        public async Task<bool> DeleteWorkoutAsync(int id)
        {
            try
            {
                // Send DELETE request to remove the workout
                var response = await _httpClient.DeleteAsync($"UserWorkout/deleteWorkout/{id}");

                // Return true if the deletion was successful
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting workout: {ex.Message}");
                return false; // Return false if failed
            }
        }

        // Delete an exercise from a workout
        public async Task<bool> DeleteExerciseAsync(int exerciseId)
        {
            try
            {
                // Send DELETE request to remove the exercise from the workout
                var response = await _httpClient.DeleteAsync($"UserWorkout/deleteExercise/{exerciseId}");

                // Return true if the deletion was successful
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting exercise: {ex.Message}");
                return false; // Return false if failed
            }
        }

        // Get exercises by workout ID
        public async Task<List<UserExerciseModel>> GetExercisesByWorkoutIdAsync(int workoutId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"UserWorkout/{workoutId}");

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                response.EnsureSuccessStatusCode();

                // Deserialize into the full workout model
                var workoutData = JsonConvert.DeserializeObject<WorkoutWithExercisesModel>(responseContent);

                // Return only the exercises list
                return workoutData?.Exercises ?? new List<UserExerciseModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercises for workout {workoutId}: {ex.Message}");
                return new List<UserExerciseModel>(); // Return an empty list if failed
            }
        }


        public async Task<List<ExerciseDto>> GetExercisesByFilterAsync(int? muscleGroupId, int? difficultyId, string searchTerm)
        {
            try
            {
                var queryParams = new List<string>();
                if (muscleGroupId.HasValue)
                    queryParams.Add($"muscleGroupId={muscleGroupId.Value}");
                if (difficultyId.HasValue)
                    queryParams.Add($"difficultyId={difficultyId.Value}");
                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                string queryString = string.Join("&", queryParams);
                string url = $"UserWorkout/filter?{queryString}";
                Console.WriteLine($"Requesting URL: {_httpClient.BaseAddress}{url}");

                var result = await _httpClient.GetFromJsonAsync<List<ExerciseDto>>(url);
                if (result == null || !result.Any())
                    Console.WriteLine("No exercises were returned by the API.");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetExercisesByFilterAsync: {ex.Message}");
                throw;
            }
        }




        public async Task<FilterDataDto> GetFilterDataAsync()
        {
            try
            {
                var url = "UserWorkout/filter-data";
                Console.WriteLine($"Requesting URL: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FilterDataDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching filter data: {ex.Message}");
                return null;
            }
        }






    }
}
