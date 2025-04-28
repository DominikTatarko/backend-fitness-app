namespace FitnessAuthBackend.Models
{
    public class UserExercise
    {
        public int UserExerciseId { get; set; }
        public int UserWorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; }
        public string Rest { get; set; }

        public UserWorkout UserWorkout { get; set; }
        public Exercise Exercise { get; set; }
    }
}
