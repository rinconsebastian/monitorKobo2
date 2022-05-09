using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class Configuracion
    {
        [Required]
        [Key]
        public int id { get; set; }


       
        [Display(Name = "Imagen logo")]
        public string Logo { get; set; }

        [Display(Name = "Favicon")]
        public string Favicon { get; set; }

        [Display(Name = "Imagen encabezado")]
        public string ImgHeader { get; set; }

        [Display(Name = "Imagen fondo")]
        public string ImgBackgroud { get; set; }


        [Display(Name = "Color Texto Cabecera")]
        public string colorTextoHeader { get; set; }

        [Display(Name = "Color  Principal")]
        public string colorPrincipal { get; set; }

        [Display(Name = "Color Texto Principal")]
        public string colorTextoPrincipal { get; set; }

        [Required]
        [Display(Name = "Email  contacto")]
        public string contacto { get; set; }

        [Required]
        [Display(Name = "Sistema activo")]
        public bool activo { get; set; }

        [Required]
        [Display(Name = "Nombre entidad")]
        public string Entidad { get; set; }

        [Required]
        [Display(Name = "Nombre plan estratégico")]
        public string NombrePlan { get; set; }

       [Required]
        [Display(Name = "Libre")]
        public bool libre { get; set; }

        [Required]
        [Range(0, 99999,ErrorMessage = "El valor del {0} debe estar entre {1} y {2}.")]
        [Display(Name = "Código Encuestador")]
        public int CodeEncuestador { get; set; }

        [Display(Name = "KPI Url")]
        public string KoboKpiUrl { get; set; }

        [Display(Name = "Api Token")]
        public string KoboApiToken { get; set; }

        [Display(Name = "Asset UID (Caracterización)")]
        public string KoboAssetUid { get; set; }

        [Display(Name = "Asset UID (Asociaciones)")]
        public string KoboAssetUidAssociation { get; set; }

        [Display(Name = "Attachment URL")]
        public string KoboAttachment { get; set; }

        [Display(Name = "Attachment User")]
        public string KoboUsername { get; set; }

        [Display(Name = "Params Map")]
        public string KoboParamsMap { get; set; }

        [Display(Name = "Params Map (Asociaciones)")]
        public string KoboParamsMapAssociation { get; set; }

    }
}
