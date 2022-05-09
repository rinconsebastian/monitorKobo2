using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class RequestUser
    {

        public const int ESTADO_NUEVA = 0;
        public const int ESTADO_SOLUCIONADA = 1;
        public const int ESTADO_EN_PROCESO = 2;
        public const int ESTADO_CANCELADA = 3;

        [Required]
        [Key]
        public int Id { get; set; }
        [Display(Name = "Asunto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Request { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int State { get; set; }

        [Display(Name = "Mensaje")]
        [JsonIgnore]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Message { get; set; }


        [Display(Name = "Formalización")]
        public int FormalizationId { get; set; }

        [Display(Name = "Adjunto")]
        [JsonIgnore]
        public string File { get; set; }


        [Display(Name = "Respuesta")]
        [JsonIgnore]
        public string Response { get; set; }


        [Display(Name = "Aleta usuario")]
        public bool AlertUser { get; set; }

        [Display(Name = "Alerta administrar")]
        public bool AlertAdmin { get; set; }


        [Display(Name = "Usuario registro")]
        public string IdUser { get; set; }


        [Display(Name = "Administrador")]
        public string AdminName { get; set; }


        [Display(Name = "Fecha creación")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Fecha validación")]
        public DateTime ValidationDate { get; set; }
    }

    public class RequestUserDataForm
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int Id { get; set; }

        [Display(Name = "Respuesta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Response { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int State { get; set; }
    }
}
