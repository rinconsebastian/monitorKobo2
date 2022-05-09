using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Display(Name = "Dependencia")]
        public int IDDependencia { get; set; }

        [Display(Name = "Correo electrónico confirmado")]
        public override bool EmailConfirmed { get; set; }

        [Display(Name = "Correo electrónico")]
        public override string Email { get; set; }


        [Display(Name = "Número de teléfono")]
        public override string PhoneNumber { get; set; }


        [Display(Name = "Bloqueo habilitado")]
        public override bool LockoutEnabled { get; set; }


        [Display(Name = "Recuento de errores de acceso")]
        public override int AccessFailedCount { get; set; }


        [Display(Name = "Nombre de usuario")]
        public override string UserName { get; set; }


        /* public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
         {
             // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
             var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
             // Add custom user claims here

             return userIdentity;
         }*/
    }
}
