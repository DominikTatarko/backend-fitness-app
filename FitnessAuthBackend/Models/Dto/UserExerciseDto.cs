namespace FitnessAuthBackend.Models
{
    public class UserExerciseDto
    {
        public int UserWorkoutId { get; set; }  // The ID of the workout
        public int ExerciseId { get; set; }  // The name of the exercise (not ExerciseId)
        public int Sets { get; set; }  // Number of sets
        public string Reps { get; set; }  // Repetitions (can be a string like "10-12")
        public string Rest { get; set; }  // Rest time between sets (e.g., "60s")
    }
}
