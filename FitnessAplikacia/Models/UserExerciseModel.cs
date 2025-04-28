using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia.Models
{
    public class UserExerciseModel
    {
        public int ExerciseId { get; set; }
        public int UserExerciseId { get; set; }

        public int UserWorkoutId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Rest { get; set; }
    }

}
