using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class LogModel
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Acción")]
        public string Accion { get; set; }

        [Display(Name = "Relacionado")]
        public string Modelo { get; set; }

        [Display(Name = "Valor anterior")]
        public string ValAnterior { get; set; }

        [Display(Name = "Valor nuevo")]
        public string ValNuevo { get; set; }

    }
}
