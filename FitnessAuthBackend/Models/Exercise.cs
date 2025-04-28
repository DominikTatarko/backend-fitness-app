using System.ComponentModel.DataAnnotations;

namespace FitnessAuthBackend.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; } // Primary key
        public string ExerciseName { get; set; } // Name of the exercise
        public string ShortYouTubeDemo { get; set; } // URL for a short demonstration video
        public string InDepthYouTubeExplanation { get; set; } // URL for an in-depth explanation video
        public int DifficultyId { get; set; } // Difficulty level (e.g., Beginner, Intermediate, Advanced)
        public Difficulty Difficulty { get; set; }
        public int MuscleGroupId { get; set; }
        public MuscleGroup MuscleGroup { get; set; }

        // Navigation property for the relationship with ProgramWorkout
        public ICollection<ProgramWorkout> ProgramWorkouts { get; set; }
    }
}
