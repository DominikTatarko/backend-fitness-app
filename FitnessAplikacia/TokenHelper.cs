using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace FitnessAplikacia.Helpers
{
    public static class TokenHelper
    {
        private const string TokenKey = "auth_token";

        // Save the token to SecureStorage
        public static async Task SaveTokenAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                await SecureStorage.SetAsync(TokenKey, token);
            }
        }

        // Retrieve the token from SecureStorage
        public static async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey);
        }

        // Remove the token from SecureStorage
        public static async Task RemoveTokenAsync()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}
