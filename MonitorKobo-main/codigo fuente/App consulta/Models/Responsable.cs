using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class Responsable
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Dependencia padre")]
        public int? IdJefe { get; set; }
        [ForeignKey("IdJefe")]
        public virtual Responsable ResponsableJefe { get; set; }

        [Display(Name = "Responsable")]
        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Editar { get; set; }

        public virtual ICollection<Responsable> Hijos { get; set; }
    }
}
