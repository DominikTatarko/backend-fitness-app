namespace FitnessAuthBackend.Models
{
    public class Jedlo
    {
        public int JedloId { get; set; }
        public string NazovJedla { get; set; }
        public int TypJedlaId { get; set; }
        public double Kalorie { get; set; }
        public double Bielkoviny { get; set; }
        public double Tuky { get; set; }
        public double Sacharidy { get; set; }
        public string NazovObrJedlo { get; set; }
        // Navigation properties
        public TypJedla TypJedla { get; set; }
        public ICollection<Krok> Kroky { get; set; }
    }
}
