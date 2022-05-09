using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class Formalization
    {
        public const int ESTADO_BORRADOR = 1;
        public const int ESTADO_COMPLETO = 2;
        public const int ESTADO_CANCELADO = 3;
        public const int ESTADO_IMPRESO = 4;
        public const int ESTADO_CARNET_VIGENTE= 5;


        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Id Kobo")]
        public string IdKobo { get; set; }

        [Display(Name = "Número registro")]
        public string NumeroRegistro { get; set; }

        [Display(Name = "Nombre y apellidos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Fecha solicitud")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string FechaSolicitud { get; set; }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Cedula { get; set; }


        [Display(Name = "Departamento")]
        public string Departamento { get; set; }


        [Display(Name = "Municipio")]
        public string Municipio { get; set; }


        [Display(Name = "Área de pesca")]
        public string AreaPesca { get; set; }


        [Display(Name = "Artes de pesca")]
        public string ArtesPesca { get; set; }


        [Display(Name = "Nombre asociación")]
        public string NombreAsociacion { get; set; }


        [Display(Name = "Fotografía pescador")]
        public string ImgPescador { get; set; }

        [Display(Name = "Solicitud de carnetización")]
        public string ImgSolicitudCarnet { get; set; }

        [Display(Name = "Certificación")]
        public string ImgCertificacion { get; set; }

        [Display(Name = "Foto Cédula (Anverso)")]
        public string ImgCedulaAnverso { get; set; }

        [Display(Name = "Foto Cédula (Reverso)")]
        public string ImgCedulaReverso { get; set; }

        [Display(Name = "Firma digital")]
        public string ImgFirmaDigital { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; }

        [Display(Name = "Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }
        [ForeignKey("IdResponsable")]
        [JsonIgnore]
        public virtual Responsable Responsable { get; set; }

        [Display(Name = "Creado por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdCreateByUser { get; set; }
        [ForeignKey("IdCreateByUser")]
        [JsonIgnore]
        public virtual ApplicationUser CreateByUser { get; set; }

        [Display(Name = "Última edición por")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string IdLastEditByUser { get; set; }
        [ForeignKey("IdLastEditByUser")]
        [JsonIgnore]
        public virtual ApplicationUser LastEditByUser { get; set; }

        [Display(Name = "Fecha creación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Fecha última edición")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public DateTime LastEditDate { get; set; }

        [Display(Name = "Encuestador")]
        public String Encuestador { get; set; }
    }
}
