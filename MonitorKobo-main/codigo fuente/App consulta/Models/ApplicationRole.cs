using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Display(Name = "Nombre")]
        [Required]
        public override string Name { get; set; }

    }
}
