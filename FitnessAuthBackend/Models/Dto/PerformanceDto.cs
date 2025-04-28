namespace FitnessAuthBackend.Models.Dto
{
    public class PerformanceDto
    {
        public string UserId { get; set; }  // The user's unique identifier
        public DateTime Date { get; set; }   // The date for the performance entry
        public int PushUpCount { get; set; } // Number of push-ups
        public int PullUpCount { get; set; } // Number of pull-ups
    }


}
