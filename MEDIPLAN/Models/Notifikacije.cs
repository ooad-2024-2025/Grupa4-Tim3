namespace MEDIPLAN.Models
{
    public class Notifikacije
    {
        public int Id { get; set; }
        public int DoktorId { get; set; }
        public string Poruka { get; set; } = string.Empty; 

        public Korisnici? Doktor { get; set; } 
    }
}