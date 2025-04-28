using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia.Models
{
    public class FilterDataDto
    {
        public List<MuscleGroupDto> MuscleGroups { get; set; }
        public List<DifficultyDto> Difficulties { get; set; }
    }
}
