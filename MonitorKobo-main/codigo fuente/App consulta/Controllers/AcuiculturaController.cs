using App_consulta.Data;
using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class AcuiculturaController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly MongoDatabaseService mdb;
        private readonly IWebHostEnvironment _env;

        public AcuiculturaController(
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

        [Authorize(Policy = "Acuicultura.Imprimir")]
        public async Task<ActionResult> Details(string id, int project)
        {
            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }

            var item = await mdb.Find(projectObj.Collection, id);
            if (item == null) { return NotFound(); }

            if (!item.DynamicProperties.ContainsKey("formato")) { return NotFound(); }
            
            var fields = await db.AquacultureField.ToListAsync();
            ViewBag.Fields = fields.ToDictionary(n => n.NameDB, n => n);

            //Locations
            var codes = new List<string>();

            var fieldsLocation = fields.Where(n => n.Type == AquacultureField.TYPE_LOCATION && n.IdParent == null).ToList();
            foreach (var f in fieldsLocation)
            {
                if(item.DynamicProperties[f.NameDB] != null)
                    codes.Add((String)item.DynamicProperties[f.NameDB]);
            }

            var groupLocationInner = fields.Where(n => n.Type == AquacultureField.TYPE_LOCATION && n.IdParent != null)
                .GroupBy(n => (int)n.IdParent)
                .ToDictionary(n => n.Key, n => n.Select(a => a.NameDB).ToList());

            var idsGroups = groupLocationInner.Keys;
            var namesLocationsInner = await db.AquacultureField.Where(n => idsGroups.Contains(n.Id))
                .ToDictionaryAsync(n => n.Id, n => n.NameDB);

            foreach (var g in groupLocationInner)
            {
                var groupName = namesLocationsInner[g.Key];
                if (item.DynamicProperties[groupName] == null) continue;
                var subitems = (List<object>) item.DynamicProperties[groupName];
                foreach(var obj in subitems)
                {
                    var json = JsonConvert.SerializeObject(obj);
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    foreach (var nF in g.Value)
                    {
                        if (dic.ContainsKey(nF) && dic[nF] != null)
                            codes.Add((String)dic[nF]);
                    }
                }
                
            }

            codes = codes.Distinct().ToList();
            var locationsList = await db.Location.Where(n => codes.Contains(n.Code2))
                .Select(n => new { 
                    codeTemp = n.Code2 + "_" + n.IdLevel,
                    name = n.Name
                })
                .ToListAsync();
            locationsList = locationsList.Distinct().ToList();
            var locations   = locationsList.ToDictionary(n => n.codeTemp, n => n.name);
            ViewBag.Locations = locations;

            //Tipo de vista detalles
            var viewName =  "Details";
            var props = item.DynamicProperties;
            if (props.ContainsKey("ip_812") && props.ContainsKey("ip_813") && props.ContainsKey("ip_814"))
            {
                var asociado = (String)props["ip_812"] == "2" && (String)props["ip_813"] == "2" && (String)props["ip_814"] == "2";
                viewName = asociado ? "Simple" : "Details";
            }
            return View(viewName,item);


        }

        [Authorize(Policy = "Acuicultura.Editar")]
        public async Task<ActionResult> Edit(string id, int project)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env, mdb);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }
            ViewBag.project = projectObj;

            var item = await mdb.FindViewModel(projectObj.Collection, id);
            if (item == null ) { return NotFound(); }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            var validTypes = new int[] { 1, 2, 3 };
            var ignoreFields = new int[] { 3,4,16,18 };
            var allFields = await db.AquacultureField.Where(n => n.NameKobo != null && n.IdParent == null 
            && validTypes.Contains(n.Type) && !ignoreFields.Contains(n.Id) && n.Id < 143 ).ToListAsync();

            var keysExists = item.DynamicProperties.Keys.ToList();
            var fieldsValids = allFields.Where(n => keysExists.Contains(n.NameDB)).ToList();
            ViewBag.fieldsValids = fieldsValids;

            var variables = await db.AquacultureVariable.ToListAsync();
            ViewBag.Variables = variables.GroupBy(n => n.Group).ToDictionary(n => n.Key, n => n.ToList());

            return View(item);
        }

        [Authorize(Policy = "Acuicultura.Editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(KoDataViewModel item, int project)
        {
            var projectObj = await db.KoProject.FindAsync(project);
            if (projectObj == null || !projectObj.Validable) { return NotFound(); }
            ViewBag.project = projectObj;

            //Registros para log
            var original = await mdb.FindViewModel(projectObj.Collection, item.Id);

            var anterior = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);
            var nuevo = new KoDataViewModel(original.Id, original.IdKobo, original.State, original.User, original.IdResponsable, original.IdLastEditByUser, original.LastEditDate);

            var validTypes = new int[] { 1, 2, 3 };
            var ignoreFields = new int[] { 3, 4, 16, 18 };
            var allFields = await db.AquacultureField.Where(n => n.NameKobo != null && n.IdParent == null
            && validTypes.Contains(n.Type) && !ignoreFields.Contains(n.Id) && n.Id < 143).ToListAsync();

            var keysExists = original.DynamicProperties.Keys.ToList();
            var fieldsValids = allFields.Where(n => keysExists.Contains(n.NameDB)).ToList();
            ViewBag.fieldsValids = fieldsValids;

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                nuevo.IdLastEditByUser = user.Id;

                foreach (var aField in fieldsValids)
                {
                    var uField = aField.NameDB;
                    var IsValid = true;
                    if (aField.Type == 3)
                    {
                        IsValid = item.Multiple.ContainsKey(uField);
                        if (IsValid)
                        {
                            string[] aValue = item.Multiple[uField];
                            update = update.Set(uField, aValue);
                            nuevo.DynamicProperties.Add(uField, aValue);
                        }
                    }
                    else
                    {
                        IsValid = item.Props.ContainsKey(uField);
                        if (IsValid)
                        {
                            string uValue = item.Props[uField];
                            update = update.Set(uField, uValue);
                            nuevo.DynamicProperties.Add(uField, uValue);
                        }
                    }
                    anterior.DynamicProperties.Add(uField, original.DynamicProperties[uField]);

                    if (!IsValid)
                    {
                        update = update.Set(uField, MongoDB.Bson.BsonNull.Value);
                        nuevo.DynamicProperties.Add(uField, null);
                    }
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

            var variables = await db.AquacultureVariable.ToListAsync();
            ViewBag.Variables = variables.GroupBy(n => n.Group).ToDictionary(n => n.Key, n => n.ToList());

            return View(item);
        }


    }
}
