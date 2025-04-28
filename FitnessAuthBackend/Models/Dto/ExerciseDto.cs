namespace FitnessAuthBackend.Models.Dto
{
    public class ExerciseDto
    {
        public int UserExerciseId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; }
        public string Rest { get; set; }
        public string MuscleGroupName { get; set; }
        public string DifficultyName { get; set; }
    }
}
