using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class ResponsablesController : Controller
    {
        private readonly ApplicationDbContext db;

        public ResponsablesController(ApplicationDbContext context)
        {
            db = context;
          
        }

        [Authorize(Policy = "Configuracion.Responsable")]
        public async Task<IActionResult> Index2()
        {
            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            var responsables = await db.Responsable.ToListAsync();
            return View(responsables);
        }

        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Details(int id)
        {
            Responsable responsable = await db.Responsable.FindAsync(id);
            if (responsable == null) { return NotFound(); }
            return View(responsable);
        }


        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Create()
        {
            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Create(Responsable responsable)
        {

            if (ModelState.IsValid)
            {
                db.Responsable.Add(responsable);
                await db.SaveChangesAsync();
                return RedirectToAction("Index2");
            }

            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre");
            return View(responsable);
        }

        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Edit(int id)
        {
            Responsable responsable = await db.Responsable.FindAsync(id);
            if (responsable == null) { return NotFound(); }
            ViewBag.Responsables = new SelectList(await db.Responsable.Where(n => n.Id != id).ToListAsync(), "Id", "Nombre");
            return View(responsable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Edit(Responsable responsable)
        {


            if (ModelState.IsValid)
            {
                db.Entry(responsable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index2");
            }
            ViewBag.Responsables = new SelectList(await db.Responsable.Where(n => n.Id != responsable.Id).ToListAsync(), "Id", "Nombre");
            return View(responsable);
        }


        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> Delete(int id)
        {
            Responsable responsable = await db.Responsable.FindAsync(id);
            if (responsable == null) { return NotFound(); }
            return View(responsable);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Responsable.Editar")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            string error = "";
            ConfiguracionsController controlConfiguracion = new ConfiguracionsController(db);

            Responsable responsable = await db.Responsable.FindAsync(id);
            try
            {
                db.Responsable.Remove(responsable);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                error = controlConfiguracion.SqlErrorHandler(ex);
                HttpContext.Session.SetComplex("error", error);
            }

            return RedirectToAction("Index2");
        }


        public List<int> GetAllIdsFromResponsable(int id)
        {
            Responsable responsable = db.Responsable.Find(id);
            List<int> ids = new List<int>();
            ids.Add(responsable.Id);

            List<Responsable> responsablesIn = new List<Responsable>();
            List<Responsable> responsablesOut = new List<Responsable>();
            responsablesIn.Add(responsable);

            while (responsablesIn.Count() > 0)
            {
                responsablesOut.Clear();
                foreach (Responsable respIn in responsablesIn)
                {
                    var responsablesX = db.Responsable.Where(n => n.IdJefe == respIn.Id).ToList();
                    foreach (var respX in responsablesX)
                    {
                        responsablesOut.Add(respX);
                        ids.Add(respX.Id);
                    }
                }
                responsablesIn.Clear();
                responsablesIn.AddRange(responsablesOut);

            }

            return ids;
        }
    }
}
