using System.ComponentModel.DataAnnotations.Schema;

namespace MEDIPLAN.Models
{
    public class Notifikacije
    {
        public int Id { get; set; }
        public int DoktorId { get; set; }
        public string Poruka { get; set; } = null!;
        public DateTime VrijemeKreiranja { get; set; }
        public bool Procitano { get; set; }

        [ForeignKey("DoktorId")]
        public virtual Korisnici Doktor { get; set; } = null!;
    }
}
