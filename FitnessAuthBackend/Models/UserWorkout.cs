
using System.Text.Json.Serialization;

namespace FitnessAuthBackend.Models
{
    public class UserWorkout
    {

        public int UserWorkoutId { get; set; }
        public string UserId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<UserExercise> UserExercises { get; set; }
    }
}

