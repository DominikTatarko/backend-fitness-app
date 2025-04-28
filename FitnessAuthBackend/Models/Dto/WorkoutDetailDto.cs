namespace FitnessAuthBackend.Models.Dto
{
    public class WorkoutDetailsDto
    {
        public int UserWorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ExerciseDto> Exercises { get; set; }
    }
}
