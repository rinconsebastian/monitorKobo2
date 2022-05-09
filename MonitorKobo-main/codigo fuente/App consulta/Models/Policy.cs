using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class Policy
    {
        [Required]
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Claim")]
        public string claim { get; set; }

        public int group { get; set; }
    }
}
