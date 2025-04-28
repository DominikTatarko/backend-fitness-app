namespace FitnessAuthBackend.Models
{
    public class Performance
    {
        public int PerformanceId { get; set; }
        public string UserId { get; set; }
        public int PushUpCount { get; set; } // Push-ups done today
        public int PullUpCount { get; set; } // Pull-ups done today
        public DateTime Date { get; set; } // The date of the performance
    }

}
