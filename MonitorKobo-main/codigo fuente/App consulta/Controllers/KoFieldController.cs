using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App_consulta.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App_consulta.Controllers
{
    public class KoFieldController : Controller
    {

        private readonly ApplicationDbContext db;
       
        public KoFieldController(ApplicationDbContext context)
        {
            db = context;
           
        }

        public string SqlErrorHandler(Exception exception)
        {

            string mensaje = "";
            if (exception is DbUpdateConcurrencyException)
            {
                mensaje = "Error no identificado";
            }

            if (exception is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null
                        && dbUpdateEx.InnerException.InnerException != null)
                {
                    if (dbUpdateEx.InnerException.InnerException is SqlException sqlException)
                    {
                        mensaje = sqlException.Number switch
                        {
                            // Unique constraint error
                            2627 => "Ya existe un elemento con el mismo identificador unico",
                            // Constraint check violation
                            547 => "No se puede eliminar este item por que tiene elementos que dependen de el",
                            // Duplicated key row error
                            2601 => "Ya existe un elemento con el mismo identificador unico",
                            _ => "Error en la base de datos",// A custom exception of yours for other DB issues
                        };
                    }
                    else
                    {
                        mensaje = dbUpdateEx.InnerException.ToString();
                    }


                }
            }

            return mensaje;
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<IActionResult> Index(int idProject = 0, int order = 0)
        {
            var project = await db.KoProject.FindAsync(idProject);
            if (project == null) { return NotFound(); }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            List<KoField> items = order switch
            {
                //formulario
                1 => await db.KoField.Where(n => n.IdProject == idProject && n.ShowForm)
                                        .OrderBy(n => n.FormOrder).ToListAsync(),
                //reporte general
                2 => await db.KoField.Where(n => n.IdProject == idProject && n.ShowTableReport)
                                        .OrderBy(n => n.TableOrder).ToListAsync(),
                //reporte usuario
                3 => await db.KoField.Where(n => n.IdProject == idProject && n.ShowTableUser)
                                        .OrderBy(n => n.TableOrder).ToListAsync(),
                //reporte validacion
                4 => await db.KoField.Where(n => n.IdProject == idProject && n.ShowTableValidation)
                                        .OrderBy(n => n.TableOrder).ToListAsync(),
                //all
                _ => await db.KoField.Where(n => n.IdProject == idProject).ToListAsync(),
            };
            ViewBag.project = project;
            ViewBag.Order = order;
            return View(items);
        }

        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Create(int idProject)
        {
            var project = await db.KoProject.FindAsync(idProject);
            if (project == null) { return NotFound(); }

            ViewBag.ItemTypes = GetOptions();
            ViewBag.project = project;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Create(int idProject, KoField koField)
        {
            var project = await db.KoProject.FindAsync(idProject);
            if (project == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                db.KoField.Add(koField);
                await db.SaveChangesAsync();

                return RedirectToAction("Index", new { idProject });
            }

            ViewBag.ItemTypes = GetOptions();
            ViewBag.project = project;

            return View(koField);
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(int id)
        {
            KoField field = await db.KoField.FindAsync(id);
            if (field == null) { return NotFound(); }

            ViewBag.ItemTypes = GetOptions();
            ViewBag.project = field.Project;
            return View(field);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(KoField koField)
        {
            var project = await db.KoProject.FindAsync(koField.IdProject);
            if (project == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                db.Entry(koField).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index", new { idProject = koField.IdProject });
            }

            ViewBag.ItemTypes = GetOptions();
            ViewBag.project = project;
            return View(koField);
        }



        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Delete(int id)
        {
           var item = await db.KoField.FindAsync(id);
            if (item == null) { return NotFound(); }
            return View(item);
        }


        [Authorize(Policy = "Configuracion.General")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var controlConfiguracion = new ConfiguracionsController(db);
            var item = await db.KoField.FindAsync(id);
            try
            {
                db.KoField.Remove(item);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = controlConfiguracion.SqlErrorHandler(ex);
                HttpContext.Session.SetComplex("error", error);
            }

            return RedirectToAction("Index", new { idProject = item.IdProject });
        }

        private static List<SelectListItem> GetOptions() {
            var types = new List<SelectListItem>() {
                new SelectListItem() { Text = "Texto", Value = KoField.TYPE_TEXT.ToString() },
                new SelectListItem() { Text = "Imagen", Value = KoField.TYPE_IMG.ToString() },
                new SelectListItem() { Text = "PDF / Archivo", Value = KoField.TYPE_FILE.ToString() },
                new SelectListItem() { Text = "Selección", Value = KoField.TYPE_SELECT_ONE.ToString() },
                new SelectListItem() { Text = "Multiple", Value = KoField.TYPE_SELECT_MULTIPLE.ToString() },
                new SelectListItem() { Text = "Multiple Especial", Value = KoField.TYPE_SELECT_EXTRA.ToString() },
            };

            return types;
        }

        [NonAction]
        public static string OptionToLabel(int option)
        {
            var label = option switch
            {
                KoField.TYPE_TEXT => "Texto",
                KoField.TYPE_IMG => "Imagen",
                KoField.TYPE_FILE => "PDF / Archivo",
                KoField.TYPE_SELECT_ONE => "Selección",
                KoField.TYPE_SELECT_MULTIPLE => "Multiple",
                KoField.TYPE_SELECT_EXTRA => "Especial",
                _ => "-",
            };
            return label;
        }
    }
}
