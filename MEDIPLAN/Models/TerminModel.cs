using System;
using System.ComponentModel.DataAnnotations;
using MEDIPLAN.Models;


namespace MEDIPLAN.Models
{
    public class TerminModel
    {
        public int Id { get; set; }  // DODAJ OVU LINIJU

        [Required]
        public int DoktorId { get; set; }

        [Required]
        public DateTime? Datum { get; set; }

        [Required]
        public int Lokacija { get; set; }
    }
}
