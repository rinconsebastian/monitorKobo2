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
                    LogModel logn = new()
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

        public async Task<string> RegistrarProps(RegistroLog registro, Type oType, int id)
        {

            var dicAnterior = new Dictionary<string, object>();
            var dicNuevo = new Dictionary<string, object>();

            var error = "";
            var anteriorResp = new Dictionary<String, String>(); ;
            var nuevoResp = new Dictionary<String, String>(); ;


            var props = oType.GetProperties()
                    .Where(pi => !Attribute.IsDefined(pi, typeof(JsonIgnoreAttribute))).ToArray();

            foreach (var oProperty in props)
            {
                if (oProperty.Name == "Props" || oProperty.Name == "Multiple") { continue;  }

                var oOldValue = oProperty.GetValue(registro.ValAnterior, null);
                var oNewValue = oProperty.GetValue(registro.ValNuevo, null);

                if (oProperty.Name == "DynamicProperties")
                {
                    dicAnterior = (Dictionary<string, object>)oOldValue;
                    dicNuevo = (Dictionary<string, object>)oNewValue;
                    continue;
                }
              
                if (!object.Equals(oOldValue, oNewValue))
                {
                    var sOldValue = oOldValue == null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue == null ? "null" : oNewValue.ToString();

                    anteriorResp.Add(oProperty.Name, sOldValue);
                    nuevoResp.Add(oProperty.Name, sNewValue);
                }
            }

            var propsDynamic = dicAnterior.Keys.Concat(dicNuevo.Keys).Distinct().ToList();

            foreach (var key in propsDynamic)
            {
                var oOldValue = dicAnterior.ContainsKey(key) ? dicAnterior[key] :null ;
                var oNewValue = dicNuevo.ContainsKey(key) ? dicNuevo[key] : null;

                if(oOldValue != null)
                {
                    var typeOld = oOldValue.GetType().Name;
                    if (typeOld == "String[]" || typeOld.StartsWith("List"))
                    {
                        oOldValue = JsonConvert.SerializeObject(oOldValue);
                    }
                }

                if (oNewValue != null)
                {
                    var typeNew = oNewValue.GetType().Name;
                    if (typeNew == "String[]" || typeNew.StartsWith("List")) {
                        oNewValue = JsonConvert.SerializeObject(oNewValue);
                    }
                }

             

                if (!object.Equals(oOldValue, oNewValue))
                {
                    var sOldValue = oOldValue == null ? "null" : oOldValue.ToString();
                    var sNewValue = oNewValue == null ? "null" : oNewValue.ToString();

                    anteriorResp.Add(key, sOldValue);
                    nuevoResp.Add(key, sNewValue);
                }
            }

            try
            {
                var stringAnterior = anteriorResp.Count > 0 ? JsonConvert.SerializeObject(anteriorResp) : "";
                var stringNuevo = nuevoResp.Count > 0 ? JsonConvert.SerializeObject(nuevoResp) : "";
                LogModel logn = new()
                {

                    Usuario = registro.Usuario,
                    Fecha = DateTime.Now,
                    Accion = registro.Accion,
                    Modelo = registro.Modelo + " " + id,
                    ValAnterior = stringAnterior,
                    ValNuevo = stringNuevo
                };
                db.Add(logn);
                await db.SaveChangesAsync();
            }
            catch (Exception e) { error = e.Message; }

            return error;
        }

        public async Task<string> RegistrarDirecto(RegistroLog registro)
        {
            var error = "";
            try
            {
                LogModel logn = new()
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
