using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia.Models
{
    public class UserExerciseDto
    {
        public int UserWorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Sets { get; set; }
        public string Reps { get; set; }
        public string Rest { get; set; }
    }

}
