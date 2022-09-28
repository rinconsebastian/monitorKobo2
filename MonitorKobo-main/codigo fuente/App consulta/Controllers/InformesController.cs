﻿using App_consulta.Data;
using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class InformesController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly MongoDatabaseService mdb;
        private readonly IWebHostEnvironment _env;

        public InformesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> _userManager,
            IWebHostEnvironment env,
            MongoDatabaseService mongoService)
        {
            db = context;
            userManager = _userManager;
            mdb = mongoService;
            _env = env;
        }

        [Authorize(Policy = "Encuestas.Listado")]
        public async Task<IActionResult> Listado(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null) { return NotFound(); }

            var fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableReport)
                .Select(n => new
                {
                    data = n.NameDB ?? n.Name,
                    caption = n.TableTitle,
                    order = n.TableOrder,
                    priority = n.TablePriority,
                    type = n.TableType,
                    width = n.WidthTableReport
                }).OrderBy(n => n.order).ToListAsync();

            var grid = "gridReport_" + project.Id;

            var config = new List<Object>
            {
                new
                {
                    grid = "#"+grid,
                    source = "Informes/GetEncuestas/" + project.Id,
                    projectId = project.Id,
                    showUser = true,
                    reportName = project.Name,
                    columns = fields
                }
            };

            ViewBag.config = JsonConvert.SerializeObject(config);
            ViewBag.gridId = grid;
            ViewBag.project = project;

            return View();
        }

        [Authorize(Policy = "Registro.Listado")]
        public async Task<IActionResult> Validacion(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null || !project.Validable) { return NotFound(); }

            var fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableValidation)
                .Select(n => new
                {
                    data = n.NameDB ?? n.Name,
                    caption = n.TableTitle,
                    order = n.TableOrder,
                    priority = n.TablePriority,
                    type = n.TableType,
                    width = n.WidthTableValidation
                }).OrderBy(n => n.order).ToListAsync();


            var grid = "gridValidation_" + project.Id;

            var config = new List<Object>
            {
                new
                {
                    grid = "#"+grid,
                    source = "Informes/GetEncuestasValidate/" + project.Id ,
                    projectId = project.Id,
                    showUser = true,
                    reportName = project.Name,
                    columns = fields
                }
            };

            ViewBag.config = JsonConvert.SerializeObject(config);
            ViewBag.gridId = grid;
            ViewBag.project = project;

            return View();
        }

        /**
         * Json: encuestas por usuario
         */
        [Authorize(Policy = "Encuestas.Usuario")]
        public async Task<ActionResult> GetEncuestasUsuario(int id, string code)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null) { return NotFound(); }

            var serializeList = await GetData(project, code);
            return Json(serializeList);
        }

        /**
         * Json: encuestas para todos los usuarios
         */
        [Authorize(Policy = "Encuestas.Listado")]
        public async Task<ActionResult> GetEncuestas(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null) { return NotFound(); }

            var serializeList = await GetData(project, null, false);
            return Json(serializeList);
        }

        /**
        * Json: registros validados para todos los usuarios
        */
        [Authorize(Policy = "Registro.Listado")]
        public async Task<ActionResult> GetEncuestasValidate(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null || !project.Validable) { return NotFound(); }

            var serializeList = await GetData(project, null, true);
            return Json(serializeList);
        }

        private async Task<List<Object>> GetData(KoProject project, string userCode = null, bool isValidable = false)
        {
            var resp = new List<Object>();

            //dependencia del usuario actual
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env, mdb);
            var responsables = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            //Encuestadores relacionados a la dependencia actual
            var encuestadores = await db.Pollster.Where(n => responsables.Contains(n.IdResponsable))
                .Select(n => new
                {
                    cedula = n.DNI.ToString(),
                    nombre = n.Name
                }).ToListAsync();

            var nombresEncuestadores = encuestadores.ToDictionary(n => n.cedula, n => n.nombre);
            var cedulasEncuestadores = encuestadores.Select(n => n.cedula).ToList();

            if (userCode != null && !cedulasEncuestadores.Contains(userCode)) { return resp; }

            //Consulta los campos 

            List<string> fields;
            if (userCode != null)
                fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableUser && n.NameDB != null)
                    .Select(n => n.NameDB).ToListAsync();
            else if (isValidable)
                fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableValidation && n.NameDB != null)
                .Select(n => n.NameDB).ToListAsync();
            else
                fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableReport && n.NameDB != null)
                .Select(n => n.NameDB).ToListAsync();

            //Configuración proyecto

            fields.Add("state");
            fields.Add("hidden");
            fields.Add("formato");
            if (fields.Contains("tipo_acuicultor"))
            {
                fields.Add("ip_812");
                fields.Add("ip_813");
                fields.Add("ip_814");
            }

            //Estados 
            var estados = await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n.Label);
            var estadosEstilo = await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n.Class);

            //Consulta los datos filtrados

            var filter = userCode != null ? Builders<BsonDocument>.Filter.Eq("user", userCode) : Builders<BsonDocument>.Filter.In("user", cedulasEncuestadores);
            //filter = Builders<BsonDocument>.Filter.Ne("user", BsonNull.Value);
            filter &= (Builders<BsonDocument>.Filter.Eq("hidden", BsonNull.Value) | Builders<BsonDocument>.Filter.Eq("hidden", false));
            if (fields.Contains("location"))
                filter &= Builders<BsonDocument>.Filter.Ne("location", BsonNull.Value);
            if (isValidable)
                filter &= Builders<BsonDocument>.Filter.Gte("state", KoGenericData.ESTADO_BORRADOR);

            var dataFiltered = await mdb.GetWithFilter(project.Collection, fields, filter, false); //!project.Validable

            if (dataFiltered.Count > 0)
            {
                //Consulta ubicaciones si es requerido
                Dictionary<String, LocationViewModel> locations = new();

                if (fields.Contains("location"))
                {
                    var codesLocation = new List<string>();
                    foreach (var item in dataFiltered)
                    {
                        codesLocation.Add((String)item["location"]);
                    }
                    codesLocation = codesLocation.Distinct().ToList();
                    locations = await db.Location.Where(n => codesLocation.Contains(n.Code2))
                        .Select(n => new LocationViewModel
                        {
                            Code = n.Code2,
                            Name = n.Name,
                            Parent = n.LocationParent != null ? n.LocationParent.Name : ""
                        }).ToDictionaryAsync(n => n.Code, n => n);
                }

                //Consulta dependencias
                var dependencias = await db.Responsable.ToDictionaryAsync(n => n.Id, n => n.Nombre);

                //Permisos de columnas
                var verValidacion = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Ver" && c.Value == "1"));

                foreach (var item in dataFiltered)
                {
                    //Convierte el ObjectId a String
                    if (item.Contains("_id"))
                    {
                        item["_id"] = ((ObjectId)item["_id"]).ToString();
                    }

                    //Completa la información del encuestador
                    if (item.Contains("user"))
                    {
                        var userItem = (String)item["user"];
                        var nombreEncuestador = nombresEncuestadores.ContainsKey(userItem) ? nombresEncuestadores[userItem] : "";
                        item.Add("user_name", nombreEncuestador);
                    }

                    //Completa la información del tipo de acuicultor
                    if (item.Contains("ip_812") && item.Contains("ip_813") && item.Contains("ip_814")
                        && item["ip_812"] != BsonNull.Value && item["ip_813"] != BsonNull.Value && item["ip_814"] != BsonNull.Value)
                    {
                        var tipo = item.Contains("tipo_acuicultor") && item["tipo_acuicultor"] != BsonNull.Value ? (String)item["tipo_acuicultor"] : "";
                        var asociado = (String)item["ip_812"] == "2" && (String)item["ip_813"] == "2" && (String)item["ip_814"] == "2";
                        item["tipo_acuicultor"] = tipo == "" && asociado ? "Asociado" : tipo;
                    }

                    //Completa los municipios y departamentos
                    if (item.Contains("location"))
                    {
                        var locationItem = (String)item["location"];
                        if (locations.ContainsKey(locationItem))
                        {
                            var aux = locations[locationItem];
                            item["location"] = aux.Name;
                            item.Add("location_level", aux.Parent);
                        }
                    }

                    if (item.Contains("dependence"))
                    {
                        var dependenceItem = (int)item["dependence"];
                        if (dependencias.ContainsKey(dependenceItem))
                        {
                            var aux = dependencias[dependenceItem];
                            item["dependence"] = aux;
                        }
                    }

                    //Completa información para estado y opciones
                    if (project.Validable && verValidacion)
                    {
                        var estado = item.Contains("state") && item["state"].BsonType != BsonType.Null ? (int)item["state"] : 0;
                        item.Add("state_name", estados.ContainsKey(estado) ? estados[estado] : "No definido");
                        item.Add("state_class", estadosEstilo.ContainsKey(estado) ? estadosEstilo[estado] : "");
                    }
                }
            }

            return dataFiltered.ConvertAll(BsonTypeMapper.MapToDotNetValue);
        }

        public async Task<ActionResult> ContadorAnexos()
        {
            var project = await db.KoProject.FindAsync(2);
            if (project == null) { return NotFound(); }

            var estadosValidos = new int[] { 2, 3, 4, 6 };
            
            var fields = await db.KoField.Where(n => n.IdProject == 2 && n.ShowPrint).ToListAsync();
            var fieldsImages = fields.Where(n => n.FormType == KoField.TYPE_IMG && !n.NameDB.Contains("cedula") && n.NameDB != "foto_encuestado")
                .OrderBy(n => n.PrintTitle).ToList();


            var fieldsFiles = fields.Where(n => (n.FormType == KoField.TYPE_FILE || n.FormType == KoField.TYPE_IMG)
                && !n.NameDB.Contains("cedula") && n.NameDB != "foto_encuestado")
            .OrderBy(n => n.PrintTitle).ToList();

            var dataFiltered = await mdb.Get(project.Collection);

            int valido = 0, images = 0, suma = 0, archivos = 0;

            foreach (var item in dataFiltered) {
                if (estadosValidos.Contains(item.State)) {
                    valido++;
                    suma++;
                    foreach (var f in fieldsImages)
                    {
                        if (item.DynamicProperties.ContainsKey(f.NameDB) && item.DynamicProperties[f.NameDB] != null) {
                            images++;
                            suma++;
                        }
                    }

                    foreach (var f in fieldsFiles)
                    {
                        if (item.DynamicProperties.ContainsKey(f.NameDB) && item.DynamicProperties[f.NameDB] != null)
                        {
                            archivos++;
                        }
                    }
                }
            }

            var r = new
            {
                total = dataFiltered.Count,
                validos = valido,
                imagenes = images,
                imagenes_y_archivos = archivos,
                hojas = suma
            };

            return Json(r);
        }

        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<IActionResult> ListadoGestion(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null) { return NotFound(); }

            var fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableReport)
                .Select(n => new
                {
                    data = n.NameDB ?? n.Name,
                    caption = n.TableTitle,
                    order = n.TableOrder,
                    priority = n.TablePriority,
                    type = n.TableType,
                    width = n.WidthTableReport
                }).OrderBy(n => n.order).ToListAsync();

            var grid = "gridReportG_" + project.Id;

            var config = new List<Object>
            {
                new
                {
                    grid = "#"+grid,
                    source = "Informes/GetEncuestasGestion/" + project.Id,
                    projectId = project.Id,
                    showUser = true,
                    reportName = project.Name,
                    columns = fields
                }
            };

            ViewBag.config = JsonConvert.SerializeObject(config);
            ViewBag.gridId = grid;
            ViewBag.project = project;

            return View();
        }

        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<ActionResult> GetEncuestasGestion(int id)
        {
            var project = await db.KoProject.FindAsync(id);
            if (project == null) { return NotFound(); }

            var resp = new List<Object>();

            //Consulta los campos 

            List<string> fields = await db.KoField.Where(n => n.IdProject == project.Id && n.ShowTableReport && n.NameDB != null)
                .Select(n => n.NameDB).ToListAsync();

            //Encuestadores relacionados a la dependencia actual
            var encuestadores = await db.Pollster.Select(n => new{cedula = n.DNI.ToString(),nombre = n.Name}).ToListAsync();

            var nombresEncuestadores = encuestadores.ToDictionary(n => n.cedula, n => n.nombre);
            

            //Configuración proyecto

            fields.Add("state");
            fields.Add("hidden");
            fields.Add("formato");
            if (fields.Contains("tipo_acuicultor"))
            {
                fields.Add("ip_812");
                fields.Add("ip_813");
                fields.Add("ip_814");
            }

            //Estados 
            var estados = await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n.Label);

            //Consulta los datos filtrados
            var filter = Builders<BsonDocument>.Filter.Ne("user", BsonNull.Value);
            if (fields.Contains("location"))
                filter &= Builders<BsonDocument>.Filter.Ne("location", BsonNull.Value);
          
            var dataFiltered = await mdb.GetWithFilter(project.Collection, fields, filter, false); 

            if (dataFiltered.Count > 0)
            {
                //Consulta ubicaciones si es requerido
                Dictionary<String, LocationViewModel> locations = new();

                if (fields.Contains("location"))
                {
                    var codesLocation = new List<string>();
                    foreach (var item in dataFiltered)
                    {
                        codesLocation.Add((String)item["location"]);
                    }
                    codesLocation = codesLocation.Distinct().ToList();
                    locations = await db.Location.Where(n => codesLocation.Contains(n.Code2))
                        .Select(n => new LocationViewModel
                        {
                            Code = n.Code2,
                            Name = n.Name,
                            Parent = n.LocationParent != null ? n.LocationParent.Name : ""
                        }).ToDictionaryAsync(n => n.Code, n => n);
                }

                foreach (var item in dataFiltered)
                {
                    //Convierte el ObjectId a String
                    if (item.Contains("_id"))
                    {
                        item["_id"] = ((ObjectId)item["_id"]).ToString();
                    }

                    //Completa la información del encuestador
                    if (item.Contains("user"))
                    {
                        var userItem = (String)item["user"];
                        var nombreEncuestador = nombresEncuestadores.ContainsKey(userItem) ? nombresEncuestadores[userItem] : "";
                        item.Add("user_name", nombreEncuestador);
                    }

                    //Completa la información del tipo de acuicultor
                    if (item.Contains("ip_812") && item.Contains("ip_813") && item.Contains("ip_814")
                        && item["ip_812"] != BsonNull.Value && item["ip_813"] != BsonNull.Value && item["ip_814"] != BsonNull.Value)
                    {
                        var tipo = item.Contains("tipo_acuicultor") && item["tipo_acuicultor"] != BsonNull.Value ? (String)item["tipo_acuicultor"] : "";
                        var asociado = (String)item["ip_812"] == "2" && (String)item["ip_813"] == "2" && (String)item["ip_814"] == "2";
                        item["tipo_acuicultor"] = tipo == "" && asociado ? "Asociado" : tipo;
                    }

                    //Completa los municipios y departamentos
                    if (item.Contains("location"))
                    {
                        var locationItem = (String)item["location"];
                        if (locations.ContainsKey(locationItem))
                        {
                            var aux = locations[locationItem];
                            item["location"] = aux.Name;
                            item.Add("location_level", aux.Parent);
                        }
                    }

                    var estado = item.Contains("state") && item["state"].BsonType != BsonType.Null ? (int)item["state"] : 0;
                    item.Add("state_name", estados.ContainsKey(estado) ? estados[estado] : "No definido");
                }
            }

            var serializeList = dataFiltered.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            return Json(serializeList);
        }
    }
}
