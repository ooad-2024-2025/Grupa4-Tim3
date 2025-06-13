using System;
using System.ComponentModel.DataAnnotations;

namespace MEDIPLAN.Models
{
    public class TerminModel
    {
        [Required]
        public int DoktorId { get; set; }

        [Required]
        public DateTime? Datum { get; set; }

        [Required]
        public int Lokacija { get; set; }

        [Required]
        public int MedicinskeUslugeId { get; set; }
    }
}
