using System.Text.Json.Serialization;

namespace FitnessAuthBackend.Models
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public double Weight { get; set; }

        // Use enum here with JsonStringEnumConverter for serialization
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FitnessLevel Level { get; set; }
    }
}
