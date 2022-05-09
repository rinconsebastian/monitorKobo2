using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class LogsController : Controller
    {

        private readonly ApplicationDbContext db;
      


        public LogsController(ApplicationDbContext context)
        {
            db = context;
          
        }

        [Authorize(Policy = "Configuracion.Logs")]
        public ActionResult Index()
        {
            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }
            return View();
        }

        [Authorize(Policy = "Configuracion.Logs")]
        public async Task<ActionResult> Ajax()
        {
            var logs = await db.Log//.Take(1000)
                .Select(n => new {
                    n.Id,
                    n.Usuario,
                    n.Modelo,
                    n.Fecha,
                    n.Accion
                }).OrderByDescending(n => n.Fecha).ToListAsync();
            return Json(logs);
        }


        [Authorize(Policy = "Configuracion.Logs")]
        public async Task<ActionResult> Details(int Id)
        {
            LogModel log = await db.Log.FindAsync(Id);
            if (log == null) { return NotFound(); }
            var oldVal = log.ValAnterior;
            var newVal = log.ValNuevo;
            try
            {
                if(oldVal != null && oldVal != "")
                {
                    oldVal = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(log.ValAnterior), Formatting.Indented);
                }
            }
            catch (Exception) { }

            try
            {
                if (newVal != null && newVal != "")
                {
                    newVal = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(log.ValNuevo), Formatting.Indented);
                }
            }
            catch (Exception) { }

            ViewBag.Old = oldVal;
            ViewBag.New = newVal;
            return View(log);
        }
    }
}
