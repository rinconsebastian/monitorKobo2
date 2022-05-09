using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class FormalizationConfig
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Nombre DB")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Field { get; set; }

        [Display(Name = "Nombre Kobo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Value { get; set; }

        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int Group { get; set; }
    }
}
