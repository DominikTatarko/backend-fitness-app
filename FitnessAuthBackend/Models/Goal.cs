namespace FitnessAuthBackend.Models
{
    public class Goal
    {
        public int GoalId { get; set; }
        public string UserId { get; set; }
        public int PushUpGoal { get; set; } // Target push-ups
        public int PullUpGoal { get; set; } // Target pull-ups
        public DateTime TargetDate { get; set; } // Date by which goals should be achieved
    }

}
