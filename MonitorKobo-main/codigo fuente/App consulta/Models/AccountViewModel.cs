using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App_consulta.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [EmailAddress(ErrorMessage = " El campo {0}  no es un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El {0}  debe contener al menos {2} caracteres.", MinimumLength = 3)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0}  debe contener al menos {2} caracteres.", MinimumLength = 3)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }


        [Display(Name = "Dependencia")]
        public int Dependencias { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe contener al menos {2} Caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }


    public class EditViewModel
    {
        [Required]
        [Display(Name = "Id")]

        public string Id { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [EmailAddress(ErrorMessage = " El campo {0}  no es un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]

        public string Email { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El {0}  debe contener al menos {2} caracteres.", MinimumLength = 3)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El {0}  debe contener al menos {2} caracteres.", MinimumLength = 3)]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Display(Name = "Dependencia")]
        public int IDDependencia { get; set; }

        [StringLength(100, ErrorMessage = "El {0} debe contener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden. ")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email confirmado")]
        public bool EmailConfirmed { get; set; }


        [Display(Name = "Bloqueado")]
        public bool LockoutEnabled { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }
    }
}
