using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAplikacia.Models
{
    public class DayModel
    {
        // The date for this day
        public DateTime Date { get; set; }

        // Indicates if this day is selected in the UI
        public bool IsSelected { get; set; }

        // Optional: Indicates if this day has notes (useful for highlighting in the UI)
        public bool HasNotes { get; set; }
    }
}
