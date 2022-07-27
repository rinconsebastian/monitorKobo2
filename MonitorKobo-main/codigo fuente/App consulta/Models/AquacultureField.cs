
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace App_consulta.Models
{
    /**
     * Mapeo de variables
     * FORMULARIO DE CARACTERIZACIÓN DE UNIDADES DE PRODUCCIÓN DE ACUICULTURA
     * FT-IV-046
     */
    public class AquacultureField
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre formulario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Nombre Kobo")]
        [StringLength(255, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string NameKobo { get; set; }

        [Display(Name = "Nombre DB")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [StringLength(100, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string NameDB { get; set; }

        [Display(Name = "Grupo Mapeo")]
        [StringLength(50, ErrorMessage = "El valor {0} no puede superar los {1} caracteres.")]
        public string GroupMap { get; set; }

        [Display(Name = "Id Padre")]
        public int? IdParent { get; set; }

        [Display(Name = "Tipo")]
        public int Type { get; set; }
    }

}
