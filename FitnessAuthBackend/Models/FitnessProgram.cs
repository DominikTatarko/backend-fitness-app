using System.ComponentModel.DataAnnotations;

namespace FitnessAuthBackend.Models
{
    public class FitnessProgram
    {
        [Key]
        public int ProgramId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string NazovObrProgram { get; set; }
        

        // Navigation properties
        public ICollection<ProgramWorkout> ProgramWorkouts { get; set; }
        
    }
}
