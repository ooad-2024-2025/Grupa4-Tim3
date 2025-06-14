using System;
using System.ComponentModel.DataAnnotations;

namespace MEDIPLAN.Models
{
    public class TerminiEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DoktorId { get; set; }


        [Required]
        public Lokacija Lokacija { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DatumVrijemePocetak { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DatumVrijemeKraj { get; set; }
    }
}
