namespace FitnessAuthBackend.Models
{
    public class TypJedla
    {
        public int Id { get; set; }
        public string TypJedlaNazov { get; set; }

        // Navigation property
        public ICollection<Jedlo> Jedla { get; set; }
    }
}
