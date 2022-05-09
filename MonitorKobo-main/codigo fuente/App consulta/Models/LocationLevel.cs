using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class LocationLevel
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nivel Territorial")]
        [Required]
        public string Nombre { get; set; }

    }
}
