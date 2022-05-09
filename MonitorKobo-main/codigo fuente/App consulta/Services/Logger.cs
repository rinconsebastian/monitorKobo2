using App_consulta.Data;
using App_consulta.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace App_consulta.Services
{
    public class Logger
    {
        private readonly ApplicationDbContext db;

        public Logger(ApplicationDbContext context)
        {
            db = context;

        }

        public async Task<string> Registrar(RegistroLog registro,Type oType, int id)
        {
            // await Task.Run(() => {});

            var error = "";

            var anterior = registro.ValAnterior != null ? JsonConvert.SerializeObject(registro.ValAnterior) : "";
            var nuevo = registro.ValNuevo != null ? JsonConvert.SerializeObject(registro.ValNuevo) : "";

            if (oType != null && anterior != "" && nuevo != "")
            {
                var arrAnterior = new Dictionary<String, String>();
                var arrNuevo = new Dictionary<String, String>();

                var props = oType.GetProperties()
                    .Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute))).ToArray();

                foreach (var oProperty in props)
                {

                    var oOldValue = oProperty.GetValue(registro.ValAnterior, null);
                    var oNewValue = oProperty.GetValue(registro.ValNuevo, null);

                    if (!object.Equals(oOldValue, oNewValue))
                    {
                        var sOldValue = oOldValue == null ? "null" : oOldValue.ToString();
                        var sNewValue = oNewValue == null ? "null" : oNewValue.ToString();

                        arrAnterior.Add(oProperty.Name, sOldValue);
                        arrNuevo.Add(oProperty.Name, sNewValue);
                    }
                }
                anterior = arrAnterior.Count > 0 ? JsonConvert.SerializeObject(arrAnterior) : "";
                nuevo = arrNuevo.Count > 0 ? JsonConvert.SerializeObject(arrNuevo) : "";
            }
            if (anterior != "" || nuevo != "")
            {
                try
                {
                    LogModel logn = new LogModel
                    {

                        Usuario = registro.Usuario,
                        Fecha = DateTime.Now,
                        Accion = registro.Accion,
                        Modelo = registro.Modelo + " " + id,
                        ValAnterior = anterior,
                        ValNuevo = nuevo
                    };

                    db.Add(logn);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }

            return error;
        }


        public async Task<string> RegistrarDirecto(RegistroLog registro)
        {
            var error = "";
            try
            {
                LogModel logn = new LogModel
                {
                    Usuario = registro.Usuario,
                    Fecha = DateTime.Now,
                    Accion = registro.Accion,
                    Modelo = registro.Modelo,
                    ValAnterior = JsonConvert.SerializeObject(registro.ValAnterior),
                    ValNuevo = JsonConvert.SerializeObject(registro.ValNuevo)
                };
                db.Add(logn);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return error;
        }
    }
}
