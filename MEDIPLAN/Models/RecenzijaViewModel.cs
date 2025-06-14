using System.ComponentModel.DataAnnotations;
using MEDIPLAN.Models;

namespace MEDIPLAN.Models
{
    public class RecenzijaViewModel
    {
        public int TerminId { get; set; }
        public int DoktorId { get; set; }

        [Range(1, 5)]
        public int OcjenaDoktor { get; set; }

        [Range(1, 5)]
        public int OcjenaKlinika { get; set; }

        [Required]
        [StringLength(1000)]
        public string Tekst { get; set; } = string.Empty;

        public string DoktorIme { get; set; } = string.Empty;
    }
}
