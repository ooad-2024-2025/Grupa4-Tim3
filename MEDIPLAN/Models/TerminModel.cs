using System.ComponentModel.DataAnnotations;
using MEDIPLAN.Models;

namespace MEDIPLAN.Models
{
    public class TerminModel
    {
        [Required(ErrorMessage = "Morate odabrati doktora.")]
        public int DoktorId { get; set; }  

        [Required(ErrorMessage = "Morate odabrati datum.")]
        public DateTime? Datum { get; set; }
    }
}