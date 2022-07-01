using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class KoField
    {

        public const int  TYPE_TEXT = 1;
        public const int TYPE_IMG = 2;
        public const int TYPE_FILE = 3;
        public const int TYPE_SELECT_ONE = 4;
        public const int TYPE_SELECT_MULTIPLE = 5;
        public const int TYPE_SELECT_EXTRA= 6;

        [Required]
        [Key]
        public int Id { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [Display(Name = "Nombre Kobo")]
        public string NameKobo { get; set; }

        [Display(Name = "Nombre DB")]
        public string NameDB { get; set; }

        [Display(Name = "Validación")]
        public bool Validable { get; set; }


        [Display(Name = "Activar Formulario")]
        public bool ShowForm { get; set; }


        [Display(Name = "Editable")]
        public bool Editable { get; set; }

        [Display(Name = "Etiqueta")]
        public string FormLabel { get; set; }

        [Display(Name = "Grupo")]
        public string FormGroup { get; set; }

        [Display(Name = "Orden")]
        public int? FormOrder { get; set; }

        [Display(Name = "Tipo campo")]
        public int FormType { get; set; }

        [Display(Name = "Grupo selección")]
        public string FormGroupSelect { get; set; }



        [Display(Name = "Reporte general")]
        public bool ShowTableReport { get; set; }

        [Display(Name = "Reporte Usuario")]
        public bool ShowTableUser { get; set; }

        [Display(Name = "Reporte validación")]
        public bool ShowTableValidation { get; set; }

        [Display(Name = "Reporte impresión")]
        public bool ShowPrint { get; set; }

        [Display(Name = "Titulo impresión")]
        public string PrintTitle { get; set; }


        [Display(Name = "Ancho (general)")]
        public int? WidthTableReport { get; set; }

        [Display(Name = "Ancho (general)")]
        public int? WidthTableUser { get; set; }

        [Display(Name = "Ancho (general)")]
        public int? WidthTableValidation { get; set; }


        [Display(Name = "Titulo")]
        public string TableTitle { get; set; }

        [Display(Name = "Orden")]
        public int? TableOrder { get; set; }

        [Display(Name = "Tipo")]
        public string TableType { get; set; }

        [Display(Name = "Prioridad")]
        public int? TablePriority { get; set; }


        [Required]
        [Display(Name = "Proyecto")]
        public int IdProject { get; set; }
        [ForeignKey("IdProject")]
        public virtual KoProject Project { get; set; }
    }

    public class KoFieldViewModel
    {
        public string NameDB { get; set; }
        public string FormLabel { get; set; }
        public string FormGroup { get; set; }
        public int? FormOrder { get; set; }
        public int FormType { get; set; }
        public string FormGroupSelect { get; set; }
    }
}
