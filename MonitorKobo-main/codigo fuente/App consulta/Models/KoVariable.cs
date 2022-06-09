using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace App_consulta.Models
{
    public class KoVariable
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Group { get; set; }

        [Display(Name = "Clave")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Key { get; set; }

        [Display(Name = "Valor")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Value { get; set; }
    }
}
