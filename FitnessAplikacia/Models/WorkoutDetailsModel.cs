using System;
using System.Collections.Generic;

namespace FitnessAplikacia.Models
{
    public class UserWorkoutDetailsModel
    {
        public int UserWorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserExerciseModel> Exercises { get; set; }
    }

}
