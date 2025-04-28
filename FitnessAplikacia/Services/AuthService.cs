using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;
using FitnessAplikacia.Models;
using FitnessAplikacia.Helpers;


namespace FitnessAplikacia.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _lastError;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Login Method
        public async Task<bool> LoginAsync(LoginModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonResponse}");
                    var token = JsonConvert.DeserializeObject<AuthResponse>(jsonResponse)?.Token;

                    if (!string.IsNullOrEmpty(token))
                    {
                        await TokenHelper.SaveTokenAsync(token);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        return true;
                    }
                }
            }
            catch (Exception ex) { 
                Console.WriteLine($"Login failed: {ex.Message}");
            }
            

            return false;
        }

        // Register Method
        public async Task<bool> RegisterAsync(UserModel user)
        {

            _lastError = null;
            var response = await _httpClient.PostAsJsonAsync("Auth/register", user);


            if (!response.IsSuccessStatusCode)
            {
                // Capture the error response
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorDetails = JsonConvert.DeserializeObject<List<ErrorDetail>>(errorResponse);

                _lastError = errorDetails?.FirstOrDefault()?.Description ?? "Unknown error.";
                return false;
            }

            return true;
        }

        public string GetLastErrorAsync()
        {
            return _lastError;
        }

        // Logout Method
        public async Task LogoutAsync()
        {
            // Clear the token from SecureStorage
            await TokenHelper.RemoveTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // Get Current User (Optional)
        public async Task<string> GetCurrentUserAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            try
            {
                var response = await _httpClient.GetAsync("Auth/user-info");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user info: {ex.Message}");
            }

            return null;
        }

        public async Task<string> GetUserEmailAsync()
        {
            var userInfo = await GetCurrentUserAsync(); // Fetch user info (JSON string or similar)

            if (!string.IsNullOrEmpty(userInfo))
            {
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModel>(userInfo);
                return user?.Email; // Assuming UserModel has an Email property
            }

            return null; // Return null if userInfo is not available
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await TokenHelper.GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }






    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }

    

    public class ErrorDetail
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
