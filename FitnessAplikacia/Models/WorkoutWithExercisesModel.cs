using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia.Models
{
    public class WorkoutWithExercisesModel
    {
        public int UserWorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserExerciseModel> Exercises { get; set; }
    }

}
