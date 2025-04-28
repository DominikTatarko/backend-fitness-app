using System;

namespace FitnessAplikacia.Models
{
    public class UserWorkoutModel
    {
        public int UserWorkoutId { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
