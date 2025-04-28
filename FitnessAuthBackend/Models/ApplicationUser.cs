using Microsoft.AspNetCore.Identity;

namespace FitnessAuthBackend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public double Weight { get; set; }
        public FitnessLevel Level { get; set; }

        // Navigation properties
        
    }

    public enum FitnessLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }
}
