

using System.ComponentModel.DataAnnotations;

namespace App_consulta.Models
{
    public class EncuestaMap
    {
        public string IdKobo { get; set; }
        public string User { get; set; }
        public string DNI { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public string Datetime { get; set; }
        public string Validation { get; set; }
        public string Carnet { get; set; }
    }

    public class EncuestaDataModel
    {
        public string IdKobo { get; set; }
        public string User { get; set; }
        public string UserName { get; set; }
        public string DNI { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public string Dep { get; set; }
        public string Mun { get; set; }
        public string Datetime { get; set; }
        public bool Validation { get; set; }
        public int FormalizacionId { get; set; }
        public int FormalizacionEstado { get; set; }
        public string Carnet { get; set; }
        public string Status { get; set; }

    }


    public class AsociacionMap
    {
        public string IdKobo { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public string Datetime { get; set; }
    }

    public class AsociacionDataModel
    {
        public string IdKobo { get; set; }
        public string User { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LocationCode { get; set; }
        public string Dep { get; set; }
        public string Mun { get; set; }
        public string Datetime { get; set; }
    }


    public class FormalizacionPostModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre y apellidos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Name { get; set; }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string Cedula { get; set; }

        [Display(Name = "Área de pesca")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string AreaPesca { get; set; }

        [Display(Name = "Artes de pesca")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public string ArtesPesca { get; set; }

        [Display(Name = "Nombre asociación")]
        public string NombreAsociacion { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int Estado { get; set; }

        [Display(Name = "Coordinación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio. ")]
        public int IdResponsable { get; set; }

    }
}
