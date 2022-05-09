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
    public class ConfiguracionsController : Controller
    {

        private readonly ApplicationDbContext db;
       



        public ConfiguracionsController(ApplicationDbContext context)
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
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<IActionResult> Index2()
        {
            PropertyInfo[] propertyInfo = typeof(Configuracion).GetProperties();
            ViewBag.Props = propertyInfo;
            var config = await db.Configuracion.FirstAsync();
            if (config == null) { return NotFound(); }
            return View(config);
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<IActionResult> Edit(int id)
        {
            Configuracion configuracion = await db.Configuracion.FindAsync(id);
            if (configuracion == null) { return NotFound(); }
            return View(configuracion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Configuracion.General")]
        public async Task<IActionResult> Edit(Configuracion configuracion)
        {
            var log = new Logger(db);

            var original = await db.Configuracion.AsNoTracking().Where(n => n.id == configuracion.id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                db.Entry(configuracion).State = EntityState.Modified;
                await db.SaveChangesAsync();

                var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "Edit", Modelo = "Configuracion", ValAnterior = original, ValNuevo = configuracion };
                await log.Registrar(registro, typeof(Configuracion), 0);

                return RedirectToAction("Index2");
            }
            return View(configuracion);
        }
    }
}
