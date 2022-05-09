using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class Location
    {

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Territorio padre")]
        public int? IdParent { get; set; }
        [ForeignKey("IdParent")]
        public virtual Location LocationParent { get; set; }

        [Display(Name = "Nombre")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Código")]
        [Required]
        public int Code { get; set; }


        [Display(Name = "Código2")]
		[Required]
        public string Code2 { get; set; }

        [Required]
        [Display(Name = "Nivel")]
        public int IdLevel { get; set; }
        [ForeignKey("IdLevel")]
        public virtual LocationLevel LocationLevel { get; set; }

    }
}
