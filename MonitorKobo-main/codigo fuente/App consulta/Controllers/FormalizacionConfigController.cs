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

namespace App_consulta.Controllers
{
    public class FormalizacionConfigController : Controller
    {

        private readonly ApplicationDbContext db;
       
        public FormalizacionConfigController(ApplicationDbContext context)
        {
            db = context;
           
        }

        public string SqlErrorHandler(Exception exception)
        {

            string mensaje = "";
            DbUpdateConcurrencyException concurrencyEx = exception as DbUpdateConcurrencyException;
            if (concurrencyEx != null)
            {
                mensaje = "Error no identificado";
            }

            DbUpdateException dbUpdateEx = exception as DbUpdateException;
            if (dbUpdateEx != null)
            {
                if (dbUpdateEx.InnerException != null
                        && dbUpdateEx.InnerException.InnerException != null)
                {
                    SqlException sqlException = dbUpdateEx.InnerException.InnerException as SqlException;
                    if (sqlException != null)
                    {
                        switch (sqlException.Number)
                        {
                            case 2627:  // Unique constraint error
                                mensaje = "Ya existe un elemento con el mismo identificador unico";
                                break;
                            case 547:   // Constraint check violation
                                mensaje = "No se puede eliminar este item por que tiene elementos que dependen de el";
                                break;
                            case 2601:  // Duplicated key row error
                                mensaje = "Ya existe un elemento con el mismo identificador unico";
                                break;

                            default:
                                // A custom exception of yours for other DB issues
                                mensaje = "Error en la base de datos";
                                break;
                        }
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
        public async Task<IActionResult> Index()
        {
            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }
            var items = await db.FormalizationConfig.ToListAsync();
            return View(items);
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(int id)
        {
            FormalizationConfig config = await db.FormalizationConfig.FindAsync(id);
            if (config == null) { return NotFound(); }

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(FormalizationConfig config)
        {

            if (ModelState.IsValid)
            {
                db.Entry(config).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(config);
        }

    }
}
