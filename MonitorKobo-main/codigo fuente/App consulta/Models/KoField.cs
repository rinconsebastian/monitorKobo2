using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class KoField
    {

        public const string TYPE_TEXT = "1";
        public const string TYPE_IMG = "2";
        public const string TYPE_LABEL = "3";

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Nombre Kobo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Value { get; set; }

        [Display(Name = "Nombre DB")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Field { get; set; }

        [Display(Name = "Etiqueta (formulario)")]
        public string Label { get; set; }

        [Display(Name = "Grupo (formulario)")]
        public string Group { get; set; }

        [Display(Name = "Tipo campo")]
        public int Type { get; set; }


        [Display(Name = "Título (tabla)")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string TitleTable { get; set; }

        [Display(Name = "Título (tabla resumen)")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string TitleTableSummary { get; set; }

        [Display(Name = "Validación")]
        public bool Validable { get; set; }

        [Display(Name = "Mostrar (formulario)")]
        public bool ShowForm { get; set; }

        [Display(Name = "Mostrar (tabla)")]
        public bool ShowTable { get; set; }

        [Display(Name = "Mostrar (tabla resumen)")]
        public bool ShowTableSummary{ get; set; }


        [Required]
        [Display(Name = "Proyecto")]
        public int IdProject { get; set; }
        [ForeignKey("IdProject")]
        public virtual KoProject Project { get; set; }
    }
}
