using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FitnessAplikacia.Services;

namespace FitnessAplikacia.Services
{
    public class FitnessService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        public FitnessService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authService = authService;
        }

        // Fetch user statistics
        public async Task<StatsDto> GetStatsAsync()
        {
            try
            {
                var url = new Uri(_httpClient.BaseAddress, "Fitness/get-stats"); // Use injected BaseAddress
                Console.WriteLine($"Requesting URL: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Body: {responseBody}");

                return JsonConvert.DeserializeObject<StatsDto>(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stats: {ex.Message}");
                return null;
            }
        }

        // Set user goals
        public async Task<bool> SetGoalAsync(GoalDto goal)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Fitness/set-goal", goal);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting goal: {ex.Message}");
                return false;
            }
        }

        // Submit performance data
        public async Task<bool> SetPerformanceAsync(PerformanceDto performance)
        {
            try
            {
                // Get the logged-in user's email (UserId)
                var userEmail = await _authService.GetUserEmailAsync();

                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("User is not logged in, cannot set performance.");
                    return false;
                }

                // Set UserId (email) in the performance data
                performance.UserId = userEmail;

                // Send performance data to the server
                var response = await _httpClient.PostAsJsonAsync("Fitness/set-performance", performance);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting performance: {ex.Message}");
                return false;
            }
        }




    }
}


// DTOs (Data Transfer Objects)
public class StatsDto
    {
        public int PushUpGoal { get; set; }
        public int PullUpGoal { get; set; }
        public int HighestPushUp { get; set; }
        public int HighestPullUp { get; set; }
        public int HighestPushUpLastWeek { get; set; }
        public int HighestPullUpLastWeek { get; set; }
        public int LastPushUpValue { get; set; }
        public int LastPullUpValue { get; set; }
    }

    public class GoalDto
    {
        public int PushUpGoal { get; set; }
        public int PullUpGoal { get; set; }
        public DateTime TargetDate { get; set; }
    }

public class PerformanceDto
{
    public DateTime Date { get; set; }
    public int PushUpCount { get; set; }
    public int PullUpCount { get; set; }
    public string UserId { get; set; } // Add UserId property
}

