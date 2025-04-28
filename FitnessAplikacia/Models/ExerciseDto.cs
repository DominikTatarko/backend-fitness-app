using System;
using System.Collections.Generic;

namespace FitnessAplikacia.Models
{
    public class ExerciseDto
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public string MuscleGroupName { get; set; }
        public string DifficultyName { get; set; }
    }
}
