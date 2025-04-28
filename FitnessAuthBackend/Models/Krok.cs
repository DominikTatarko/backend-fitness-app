namespace FitnessAuthBackend.Models
{
    public class Krok
    {
        public int KrokId { get; set; }
        public int JedloId { get; set; }
        public int CisloKroku { get; set; }
        public string PopisKroku { get; set; }

        // Navigation property
        public Jedlo Jedlo { get; set; }
    }

}
