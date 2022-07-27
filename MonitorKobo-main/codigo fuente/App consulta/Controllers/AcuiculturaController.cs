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

        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<ActionResult> Index()
        {
            var project = await db.KoProject.FindAsync(2);
            ViewBag.LastUpdate = project.LastUpdate;
            return View();
        }

        [Authorize(Policy = "Acuicultura.Listado")]
        public async Task<ActionResult> Ajax()
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

            //Estados 
            var estados = await db.KoDataState.ToDictionaryAsync(n => n.Id, n => n.Label);

            //Consulta los campos y filtra
            var fields = new List<string> { "kobo_id", "user", "state", "a_04", "i_05" };
            var filter = Builders<BsonDocument>.Filter.In("user", cedulasEncuestadores);
            filter = Builders<BsonDocument>.Filter.Ne("user", BsonNull.Value);
            var dataFiltered = await mdb.GetWithFilter("aquaculture", fields, filter, false);

            if (dataFiltered.Count > 0)
            {
                //Consulta ubicaciones si es requerido
                Dictionary<String, LocationViewModel> locations = new();

                //Consulta dependencias
                var dependencias = await db.Responsable.ToDictionaryAsync(n => n.Id, n => n.Nombre);

                foreach (var item in dataFiltered)
                {
                    item["_id"] = ((ObjectId)item["_id"]).ToString();

                    var userItem = (String)item["user"];
                    var nombreEncuestador = nombresEncuestadores.ContainsKey(userItem) ? nombresEncuestadores[userItem] : "";
                    item.Add("user_name", nombreEncuestador);

                    var estado = item.Contains("state") && item["state"].BsonType != BsonType.Null ? (int)item["state"] : 0;
                    item.Add("state_name", estados.ContainsKey(estado) ? estados[estado] : "No definido");
                }
            }

            resp = dataFiltered.ConvertAll(BsonTypeMapper.MapToDotNetValue);
            
            return Json(resp);
        }

        [Authorize(Policy = "Acuicultura.Imprimir")]
        public async Task<ActionResult> Details(string id)
        {
            var item = await mdb.Find("aquaculture", id);
            if (item == null) { return NotFound(); }

            var fields = await db.AquacultureField.ToListAsync();
            ViewBag.Fields = fields.ToDictionary(n => n.NameDB, n => n);

            //Locations
            var codes = new List<string>();

            var fieldsLocation = fields.Where(n => n.Type == 4 && n.IdParent == null).ToList();
            foreach (var f in fieldsLocation)
            {
                if(item.DynamicProperties[f.NameDB] != null)
                    codes.Add((String)item.DynamicProperties[f.NameDB]);
            }

            var groupLocationInner = fields.Where(n => n.Type == 4 && n.IdParent != null)
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
            var locations = await db.Location.Where(n => codes.Contains(n.Code2))
                .ToDictionaryAsync(n => n.Code2 + "_" + n.IdLevel, n => n.Name);
            ViewBag.Locations = locations;

            return View(item);


        }
    }
}
