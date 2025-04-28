namespace FitnessAuthBackend.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string UserId { get; set; } // FK to identify the user
        public DateTime Date { get; set; }
        public string Content { get; set; }
    }
}
