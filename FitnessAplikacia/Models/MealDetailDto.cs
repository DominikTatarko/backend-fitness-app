using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessAplikacia.Services;
namespace FitnessAplikacia.Models
{
    public class MealDetailDto
    {
        public int JedloId { get; set; }
        public string NazovJedla { get; set; }
        public int TypJedlaId { get; set; }
        public int Kalorie { get; set; }
        public int Sacharidy { get; set; }
        public int Tuky { get; set; }
        public string TypJedla { get; set; }
        public string NazovObrJedlo { get; set; }

        public List<KrokDto> Kroky { get; set; }
        public string ImagePath { get; set; }
    }
}
