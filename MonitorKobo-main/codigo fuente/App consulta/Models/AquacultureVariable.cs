using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace App_consulta.Models
{
    public class AquacultureVariable
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Grupo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        [StringLength(50, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string Group { get; set; }

        [Display(Name = "Clave")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        [StringLength(50, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string Key { get; set; }

        [Display(Name = "Valor")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        [StringLength(100, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string Value { get; set; }
    }

    public class AqFieldSelectViewModel
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public int Columns { get; set; }
        public bool Selected { get; set; }
    }
}
