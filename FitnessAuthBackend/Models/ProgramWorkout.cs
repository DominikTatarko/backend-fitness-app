namespace FitnessAuthBackend.Models
{
    public class ProgramWorkout
    {
        public int ProgramWorkoutId { get; set; }
        public int ProgramId { get; set; }
        public int? ExerciseId { get; set; }
        public int Day { get; set; }
        public int? ExerciseOrder { get; set; }
        public int? Sets { get; set; }
        public string Reps { get; set; }
        public string Rest { get; set; }

        public FitnessProgram Program { get; set; }
        public Exercise Exercise { get; set; }
        public ICollection<UserWorkout> UserWorkouts { get; set; }
    }
}
