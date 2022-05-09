using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Models
{
    public class RegistroLog
    {
        public string Usuario { get; set; }


       

        public string Accion { get; set; }

        public string Modelo { get; set; }

        public Object ValAnterior { get; set; }

        public Object ValNuevo { get; set; }
    }
}
