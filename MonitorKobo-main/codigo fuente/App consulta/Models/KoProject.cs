using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class KoProject
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Nombre DB")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Collection { get; set; }

        [Display(Name = "Validable")]
        public bool Validable { get; set; }

        [Display(Name = "Informe")]
        public string ValidationName { get; set; }

        [Display(Name = "Campo")]
        public string ValidationField { get; set; }

        [Display(Name = "Value")]
        public string ValidationValue{ get; set; }

        [Display(Name = "KPI Url")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string KoboKpiUrl { get; set; }

        [Display(Name = "Api Token")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string KoboApiToken { get; set; }

        [Display(Name = "Asset UID")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string KoboAssetUid { get; set; }

        [Display(Name = "Attachment URL")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string KoboAttachment { get; set; }

        [Display(Name = "Attachment User")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string KoboUsername { get; set; }

        [Display(Name = "Última actualización")]
        public DateTime LastUpdate { get; set; }
    }
}
