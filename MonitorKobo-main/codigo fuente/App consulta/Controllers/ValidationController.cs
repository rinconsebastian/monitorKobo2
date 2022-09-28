using App_consulta.Data;
using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class ValidationController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly MongoDatabaseService mdb;
        private readonly IWebHostEnvironment _env;

        public ValidationController(
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

        [Authorize(Policy = "Registro.Validar")]
        [HttpPost]
        public async Task<IActionResult> Load(string id, int project)
        {
            var r = new RespuestaAccion();

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return Json(r); }

            var previo = await mdb.Find(projectObj.Collection, id);
            if (previo != null && previo.State < KoGenericData.ESTADO_BORRADOR)
            {
                var kobo = new KoBoDataController(db, userManager, _env, mdb);
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                r = await kobo.LoadValidation(project, id, user);
                return Json(r);
            }

            r.Url = "Validation/Edit/" + id + "?project=" + project;
            r.Success = true;
            return Json(r);
        }

        [Authorize(Policy = "Registro.Ver")]
        public async Task<ActionResult> Details(string id, int project)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env, mdb);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null) { return NotFound(); }
            ViewBag.project = projectObj;

            var item = await mdb.FindExt(projectObj.Collection, id);
            if(item == null || item.State < KoGenericData.ESTADO_BORRADOR || (item.IdResponsable != 0 && !respRelacionado.Contains(item.IdResponsable))) { return NotFound(); }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            var allFields = await db.KoField.Where(n => n.IdProject == project && n.ShowForm).ToListAsync();

            //Agregar ubicación simple 
            await AddLocation(allFields, item.DynamicProperties);

            var keysExists = item.DynamicProperties.Keys.ToList();
            var fieldsValids = allFields.Where(n => keysExists.Contains(n.NameDB)).ToList();

            var validStates = new List<int> { 
                KoField.TYPE_TEXT, 
                KoField.TYPE_SELECT_ONE,
                KoField.TYPE_SELECT_MULTIPLE,
                KoField.TYPE_SELECT_EXTRA
            };

            var fieldsText = fieldsValids.Where(n => validStates.Contains(n.FormType)).OrderBy(n => n.FormOrder).ToList();
            ViewBag.fieldsText = fieldsText;

            var fieldsFile = fieldsValids.Where(n => n.FormType == KoField.TYPE_IMG || n.FormType == KoField.TYPE_FILE)
                .OrderBy(n => n.FormOrder).ToList();
            ViewBag.fieldsFile = fieldsFile;

            

            var estado = await db.KoDataState.FindAsync(item.State);
            ViewBag.estado = estado;

            ViewBag.Responsable = await db.Responsable.FindAsync(item.IdResponsable);
            ViewBag.CreateByUser = await db.Users.FindAsync(item.IdCreateByUser);
            ViewBag.LastEditByUser = await db.Users.FindAsync(item.IdLastEditByUser);

            return View(item);
        }

        [HttpGet]
        public FileStreamResult ViewImage(string filename = "noFound.jpg")
        {
            var permiso = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Ver" && c.Value == "1"));
            if (!permiso) { filename = "lock.jpg"; }

            if (filename == null) { filename = "noFound.jpg"; }

            new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);

            string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);

            byte[] data = new byte[1];
            try
            {
                data = System.IO.File.ReadAllBytes(filepath);
            }
            catch (Exception) { }

            Stream stream = new MemoryStream(data);
            return new FileStreamResult(stream, contentType);
        }

        [Authorize(Policy = "Registro.Validar")]
        public async Task<ActionResult> Edit(string id, int project)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env, mdb);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }
            ViewBag.project = projectObj;

            var item = await mdb.FindViewModel(projectObj.Collection, id);
            if (item == null || !respRelacionado.Contains(item.IdResponsable)) { return NotFound(); }

            var permisoCambiarEstado = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Cancelar" && c.Value == "1"));

            if (item.State != KoGenericData.ESTADO_BORRADOR && !permisoCambiarEstado)
            {
                return RedirectToAction("Details", new { id = item.Id, project });
            }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            var allFields = await db.KoField.Where(n => n.IdProject == project && n.ShowForm).ToListAsync();

            //Agregar ubicación simple 
            await AddLocation(allFields, item.DynamicProperties);

            var keysExists = item.DynamicProperties.Keys.ToList();
            var fieldsValids = allFields.Where(n => keysExists.Contains(n.NameDB)).ToList();


            var validStates = new List<int> {
                KoField.TYPE_TEXT,
                KoField.TYPE_SELECT_ONE,
                KoField.TYPE_SELECT_MULTIPLE,
                KoField.TYPE_SELECT_EXTRA
            };

            var fieldsText = fieldsValids.Where(n => validStates.Contains(n.FormType)).OrderBy(n => n.FormOrder).ToList();
            ViewBag.fieldsText = fieldsText;

            var fieldsFile = fieldsValids.Where(n => n.FormType == KoField.TYPE_IMG || n.FormType == KoField.TYPE_FILE)
                .OrderBy(n => n.FormOrder).ToList();
            ViewBag.fieldsFile = fieldsFile;

            //estado del registro
            var estado = await db.KoDataState.FindAsync(item.State);
            ViewBag.estado = estado;

            //Campos de seleccion
            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", item.IdResponsable);
            ViewBag.Estados = new SelectList(await db.KoDataState.Where(n => n.Id > 2).ToListAsync(), "Id", "Label", item.State);

            var variables = await db.KoVariable.ToListAsync();
            ViewBag.Variables = variables.GroupBy(n => n.Group).ToDictionary(n => n.Key, n => n.ToList());

            return View(item);
        }
 
        [Authorize(Policy = "Registro.Validar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(KoDataViewModel item, int project)
        {
            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }
            ViewBag.project = projectObj;

            //Registros para log
            var original = await mdb.FindViewModel(projectObj.Collection, item.Id);
           
            //Consulta los parametros editables
            var allFields = await db.KoField.Where(n => n.IdProject == project && n.ShowForm).ToListAsync();

            //Agregar ubicación simple 
            await AddLocation(allFields, original.DynamicProperties);

            var anterior = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);
            var nuevo = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);

            var keysExists = original.DynamicProperties.Keys.ToList();
            var fieldsValids = allFields.Where(n => keysExists.Contains(n.NameDB)).ToList();

         
            var validStates = new List<int> {
                KoField.TYPE_TEXT,
                KoField.TYPE_SELECT_ONE,
                KoField.TYPE_SELECT_MULTIPLE,
                KoField.TYPE_SELECT_EXTRA
            };

            var fieldsText = fieldsValids.Where(n => validStates.Contains(n.FormType)).OrderBy(n => n.FormOrder).ToList();
            var fieldsEditables = fieldsText.Where(n => n.Editable).Select(n => n.NameDB).ToList();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                nuevo.IdLastEditByUser = user.Id;

                var permisoEditar = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Editar" && c.Value == "1"));
                if (permisoEditar)
                {
                    foreach (var uField in fieldsEditables)
                    {
                        update = update.Set(uField, item.Props[uField]);
                        nuevo.DynamicProperties.Add(uField, item.Props[uField]);
                        anterior.DynamicProperties.Add(uField, original.DynamicProperties[uField]);
                    }
                }
                if (item.State > 0) { 
                    update = update.Set("state", item.State);
                    nuevo.State = item.State;
                }
                if (item.IdResponsable > 0) {
                    update = update.Set("dependence", item.IdResponsable);
                    nuevo.IdResponsable = item.IdResponsable;
                }
                var now = DateTime.Now;
                update = update.Set("edit_date", now);
                nuevo.LastEditDate = now;

                var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, item.Id);
                var save = await mdb.Update(projectObj.Collection, update, filter);
                if (save)
                {
                    var log = new Logger(db);
                    var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "Edit", Modelo = projectObj.ValidationName, ValAnterior = anterior, ValNuevo = nuevo };
                    await log.RegistrarProps(registro, typeof(KoDataViewModel), Int32.Parse(original.IdKobo));

                    HttpContext.Session.SetComplex("error", "Los datos fueron actualizados correctamente.");
                    return RedirectToAction("Edit", new { id = item.Id, project });
                }
                else { HttpContext.Session.SetComplex("error", "No fue posible actualizar el registro."); }
            }

            ViewBag.fieldsText = fieldsText;

            var fieldsFile = fieldsValids.Where(n => n.FormType == KoField.TYPE_IMG || n.FormType == KoField.TYPE_FILE)
                .OrderBy(n => n.FormOrder).ToList();
            ViewBag.fieldsFile = fieldsFile;

            var estado = await db.KoDataState.FindAsync(original.State);
            ViewBag.estado = estado;

            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", original.IdResponsable);
            ViewBag.Estados = new SelectList(await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n.Label), "Id", "Label", original.State);
            
            return View(item);
        }


        [Authorize(Policy = "Registro.Validar")]
        [HttpPost]
        public async Task<RespuestaAccion> CambiarEstado(string id, int project, int estado)
        {
            var r = new RespuestaAccion();

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { r.Message = "Proyecto no encontrado."; return r; }
            ViewBag.project = projectObj;

            var original = await mdb.FindViewModel(projectObj.Collection, id);
            if (original == null) { r.Message = "Registro no encontrado."; return r; }

            try
            {
                var anterior = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);
                var nuevo = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);

                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                var datetime = DateTime.Now;
                update = update.Set("edit_date", datetime);
                update = update.Set("state", estado);


                var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, original.Id);
                var save = await mdb.Update(projectObj.Collection, update, filter);

                if (save)
                {
                    nuevo.State = estado;
                    nuevo.LastEditDate = datetime;
                    nuevo.IdLastEditByUser = user.Id;

                    var log = new Logger(db);
                    var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "ChangeStatus",
                        Modelo = projectObj.ValidationName, ValAnterior = anterior, ValNuevo = nuevo };
                    await log.RegistrarProps(registro, typeof(KoDataViewModel), Int32.Parse(original.IdKobo));

                    r.Success = true;
                }
                else { r.Message = "No fue posible cambiar el estado del registro."; return r; }
            }
            catch (Exception e) { r.Message = e.Message; }
            return r;
        }


        [Authorize(Policy = "Registro.Imprimir")]
        public async Task<ActionResult> Print(string[] ids, int project)
        {
            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }
            ViewBag.project = projectObj;

            //estados 
            var estadosValidos = await db.KoDataState.Where(n => n.Print).Select(n => n.Id).ToListAsync();
            ViewBag.EstadosValidos = estadosValidos;
            var estados = await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n);
            ViewBag.Estados = estados;

            //Información registros
            var items = await mdb.FindMany(projectObj.Collection, ids);
            var idsValidos = items.Where(n => estadosValidos.Contains(n.State)).Select(n => n.Id).ToList();
            ViewBag.Ids = JsonConvert.SerializeObject(idsValidos);

            //Campos dinamicos 
            var fields = await db.KoField.Where(n => n.IdProject == project && n.ShowPrint).ToListAsync();

            var fieldsText = fields.Where(n => n.FormType != KoField.TYPE_IMG && n.FormType != KoField.TYPE_FILE)
                .OrderBy(n => n.PrintTitle).ToList();
            ViewBag.fieldsText = fieldsText;

            var fieldsImages = fields.Where(n => n.FormType == KoField.TYPE_IMG).OrderBy(n => n.PrintTitle).ToList();
            ViewBag.fieldsImages = fieldsImages;

            var fieldsFiles= fields.Where(n => (n.FormType == KoField.TYPE_FILE || n.FormType == KoField.TYPE_IMG)
                   && !n.NameDB.Contains("cedula") && n.NameDB != "foto_encuestado")
                .OrderBy(n => n.PrintTitle).ToList();
            ViewBag.fieldsFiles = fieldsFiles;

            //Agregar ubicación multiple
            Dictionary<String, LocationViewModel> locations = new();
            var fieldsTextName = fieldsText.Select(n => n.NameDB).ToList();
            if (fieldsTextName.Contains("location"))
            {
                var codesLocation = new List<string>();
                foreach (var item in items)
                {
                    codesLocation.Add((String)item.DynamicProperties["location"]);
                }
                codesLocation = codesLocation.Distinct().ToList();
                locations = await db.Location.Where(n => codesLocation.Contains(n.Code2))
                    .Select(n => new LocationViewModel
                    {
                        Code = n.Code2,
                        Name = n.Name,
                        Parent = n.LocationParent != null ? n.LocationParent.Name : ""
                    }).ToDictionaryAsync(n => n.Code, n => n);

                foreach(var item in items)
                {
                    if (item.DynamicProperties.ContainsKey("location"))
                    {
                        var locationItem = (String)item.DynamicProperties["location"];
                        if (locations.ContainsKey(locationItem))
                        {
                            var aux = locations[locationItem];
                            item.DynamicProperties["location"] = aux.Name;
                            item.DynamicProperties.Add("location_level", aux.Parent);
                        }
                    }
                }
            }

            return View(items);
        }

        [Authorize(Policy = "Registro.Imprimir")]
        [HttpPost]
        public async Task<int> Imprimir(string ids, int project)
        {
            var success = 0;

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return success; }

            if (ids == "") { return success; }
            var idsList = ids.Split(',');

            var items = await mdb.FindManyViewModel(projectObj.Collection, idsList);
           
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var datetime = DateTime.Now;

            var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
            update = update.Set("edit_date", datetime);
       

            foreach (var original in items)
            {
                try
                {
                    var anterior = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);
                    var nuevo = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);

                    var estado = original.State == 10 ? 10: 6; //Si estado entregado mantiene estado, si no impreso.
                    update = update.Set("state", estado);

                    var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, original.Id);
                    var save = await mdb.Update(projectObj.Collection, update, filter);

                    if (save)
                    {
                        nuevo.State = estado;
                        nuevo.LastEditDate = datetime;
                        nuevo.IdLastEditByUser = user.Id;

                        var log = new Logger(db);
                        var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "Print", Modelo = projectObj.ValidationName, ValAnterior = anterior, ValNuevo = nuevo };
                        await log.RegistrarProps(registro, typeof(KoDataViewModel), Int32.Parse(original.IdKobo));

                        success++;
                    }                   
                }
                catch (Exception) { }
            }
            return success;
        }

        [HttpPost]
        [Authorize(Policy = "Registro.Validar")]
        public async Task<IActionResult> UpdateImage(IFormFile file, string filename)
        {
            var r = await UpdateFile(file, filename);
            return Json(r);
        }


        [HttpPost]
        [Authorize(Policy = "Registro.Imagen.Cambiar")]
        public async Task<IActionResult> LoadFile(IFormFile file, string id, int projectid, string name)
        {
            var r = new RespuestaAccion();

            var project = await db.KoProject.FindAsync(projectid);
            if (project == null || !project.Validable) { r.Message = "Error: proyecto no encontrado."; return Json(r); }

            var item = await mdb.Find(project.Collection, id);
            if (item == null) { r.Message = "Error: Item no encontrado."; return Json(r); }

            string filename = null;
            if (item.DynamicProperties.ContainsKey(name))
                filename = item.DynamicProperties[name] != null ? (String)item.DynamicProperties[name] : null;
            
            var actionNew = false;
            if (filename == null)
            {
                var extension = (Path.GetExtension(file.FileName)).ToLower();
                filename = Path.Combine(project.Collection, item.IdKobo, name + extension);
                actionNew = true;
            }
            r = await UpdateFile(file, filename);

            if (r.Success)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                var datetime = DateTime.Now;
                update = update.Set("edit_date", datetime);
                update = update.Set(name, r.Message);

                var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, item.Id);
                var save = await mdb.Update(project.Collection, update, filter);

                if (save)
                {
                    r.Success = true;
                    var log = new Logger(db);
                    var registro = new RegistroLog
                    {
                        Usuario = User.Identity.Name,
                        Accion = actionNew ? "LoadNewFile" : "LoadFile",
                        Modelo = project.ValidationName + " " + item.IdKobo,
                        ValAnterior = actionNew ? "" : name + ": " + filename,
                        ValNuevo = name + ": " + r.Message
                    };
                    await log.RegistrarDirecto(registro);
                }
                else { r.Message = "No fue posible actualizar el registro asociado al archivo."; return Json(r); }
            }
            return Json(r);
        }

        [HttpPost]
        [Authorize(Policy = "Registro.Imagen.Cambiar")]
        public async Task<IActionResult> DeleteNewFile(string id, int projectid, string name)
        {
            var r = new RespuestaAccion();

            var project = await db.KoProject.FindAsync(projectid);
            if (project == null || !project.Validable) { r.Message = "Error: proyecto no encontrado."; return Json(r); }

            var item = await mdb.Find(project.Collection, id);
            if (item == null) { r.Message = "Error: Item no encontrado."; return Json(r); }

            string filename = null;
            if (item.DynamicProperties.ContainsKey(name))
                filename = item.DynamicProperties[name] != null ? (String)item.DynamicProperties[name] : null;

            if (filename != null)
            {
                string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);

                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                    var datetime = DateTime.Now;
                    update = update.Set("edit_date", datetime);
                    update = update.Set(name, MongoDB.Bson.BsonNull.Value);

                    var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, item.Id);
                    var save = await mdb.Update(project.Collection, update, filter);
                    if (save)
                    {
                        r.Success = true;
                        var log = new Logger(db);
                        var registro = new RegistroLog
                        {
                            Usuario = User.Identity.Name,
                            Accion = "DeleteNewFile",
                            Modelo = project.ValidationName + " " + item.IdKobo,
                            ValAnterior = name + ": " + filename,
                            ValNuevo = ""
                        };
                        await log.RegistrarDirecto(registro);
                    }
                    else { r.Message = "No fue posible actualizar el registro asociado al archivo."; return Json(r); }
                }
                else { r.Message = "Error: El archivo no es válido."; }
            }
            else { r.Message = "Error: Archivo no encontrado."; }
            return Json(r);
        }

        private async Task<RespuestaAccion> UpdateFile(IFormFile file, string filename)
        {
            var r = new RespuestaAccion();

            if (file != null && file.Length > 0)
            {
                string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);
                //busca y elimina el archivo actual
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                //actualiza la extensión 
                var extension = (Path.GetExtension(file.FileName)).ToLower();
                if(extension != "")
                    filepath = Path.ChangeExtension(filepath, extension);
                //carga el nuevo archivo
                try
                {
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    r.Message = Path.ChangeExtension(filename, extension);
                    r.Success = true;
                }
                catch (Exception e) { r.Message = "Error: " + e.Message; }

            }
            else { r.Message = "Error: El archivo no es válido."; }
            return r;
        }

        private async Task AddLocation(List<KoField> fields, IDictionary<string, object> DynamicProperties)
        {
            var fieldsTextName = fields.Select(n => n.NameDB).ToList();
            if (fieldsTextName.Contains("location"))
            {
                var codeLocation = (String)DynamicProperties["location"];
                var location = await db.Location.Where(n => n.Code2 == codeLocation)
                    .Select(n => new LocationViewModel
                    {
                        Name = n.Name,
                        Parent = n.LocationParent != null ? n.LocationParent.Name : ""
                    }).FirstOrDefaultAsync();

                if (location != null)
                {
                    DynamicProperties["location"] = location.Name;
                    DynamicProperties.Add("location_level", location.Parent);
                }
            }
        }

        [Authorize(Policy = "Registro.Borrar")]
        [HttpPost]
        public async Task<int> Delete(string ids, int project)
        {
            var success = 0;

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return success; }

            if (ids == "") { return success; }
            var idsList = ids.Split(',');
            try
            {
                success = await mdb.DeleteMany(projectObj.Collection, idsList); ;
            }
            catch (Exception) { }
            return success;
        }
    }
}
