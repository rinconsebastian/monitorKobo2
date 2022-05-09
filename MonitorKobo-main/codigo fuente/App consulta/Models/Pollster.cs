using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace App_consulta.Models
{
    public class Pollster
    {

        [Required]
        [Key]
        public int Id { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = " El campo {0}  no es un correo electrónico válido.")]
        public string Email { get; set; }

        
        [Display(Name = "Cedula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        [Remote(action: "VerifyDNI", controller: "Encuestador", AdditionalFields = nameof(Id) )]
        public int DNI { get; set; }

        [Display(Name = "Código")]
        public string Code { get; set; }

        [NotMapped]
        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdLocationParent { get; set; }


        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdLocation { get; set; }
        [ForeignKey("IdLocation")]
        public virtual Location Location { get; set; }


        [Display(Name = "Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }
        [ForeignKey("IdResponsable")]
        public virtual Responsable Responsable { get; set; }


        [Display(Name = "Usuario registro")]
        public string IdUser { get; set; }
        [ForeignKey("IdUser")]
        public virtual ApplicationUser User { get; set; }

        [Display(Name = "Fecha registro")]
        public DateTime CreationDate { get; set; }
    }
}
