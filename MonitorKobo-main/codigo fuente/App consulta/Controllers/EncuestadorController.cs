using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class EncuestadorController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _env;


        public EncuestadorController(ApplicationDbContext context,UserManager<ApplicationUser> _userManager, IWebHostEnvironment env)
        {
            db = context;
            userManager = _userManager;
            _env = env;
        }

        // GET: EncuestadorController
        [Authorize(Policy = "Encuestador.Ver")]
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

        [Authorize(Policy = "Encuestador.Ver")]
        public async Task<ActionResult> ListAjax()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var respRelacionado = await GetResponsablesbyIdParent(user.IDDependencia, 1,3);

            KoboController kobo = new KoboController(db, userManager, _env);

            var encuestadores = await db.Pollster.Where(n => respRelacionado.Contains(n.IdResponsable)).
                Select(n => new EncuestadorViewModel{
                    ID = n.Id,
                    Cedula = n.DNI,
                    Nombre = n.Name,
                    Municipio = n.Location != null ? n.Location.Name : "",
                    Departamento = (n.Location != null && n.Location.LocationParent != null) ? n.Location.LocationParent.Name : "",
                    Coordinacion = n.Responsable != null ? n.Responsable.Nombre : "",
                    Telefono = n.PhoneNumber,
                    Email = n.Email,
                    NumeroEncuestas = 0,
                    NumeroAsociaciones = 0
                }).ToListAsync();

            var totales = kobo.GetTotalEncuestas();
            var asociaciones = kobo.GetTotalAsociaciones();
            foreach(var enc in encuestadores)
            {
                var idUser = enc.Cedula.ToString();
                if (totales.ContainsKey(idUser))
                {
                    enc.NumeroEncuestas = totales[idUser];
                }
                if (asociaciones.ContainsKey(idUser))
                {
                    enc.NumeroAsociaciones = asociaciones[idUser];
                }
            }
            return Json(encuestadores);
        }


        // GET: EncuestadorController/Details/5
        [Authorize(Policy = "Encuestador.Ver")]
        public async Task<ActionResult> Details(int id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var respRelacionado = await GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            Pollster encuestador = await db.Pollster.Where(n => n.Id == id && respRelacionado.Contains(n.IdResponsable)).FirstOrDefaultAsync();

            var kobo = new KoboController(db,userManager,_env);

            ViewBag.DataTime = kobo.GetDatetimeData(KoboController.FILE_CARACTERIZACION);
            ViewBag.DataTimeAssoc = kobo.GetDatetimeData(KoboController.FILE_ASOCIACION);

            if (encuestador == null) { return NotFound(); }
            return View(encuestador);
        }

        // GET: EncuestadorController/Create
        [Authorize(Policy = "Encuestador.Editar")]
        public async Task<ActionResult> Create()
        {
            ResponsablesController controlResponsable = new ResponsablesController(db);
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var ids = controlResponsable.GetAllIdsFromResponsable(user.IDDependencia);

            ViewBag.Departamentos = new SelectList(await db.Location.Where(n => n.IdLevel== 2).OrderBy(n => n.Name).ToListAsync(), "Id", "Name");
            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n=> ids.Contains(n.Id) && n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre",ids.FirstOrDefault());
            return View();
        }

        // POST: EncuestadorController/Create
        [Authorize(Policy = "Encuestador.Editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Pollster encuestador)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
               
                //Si no define la coordinación (dependencia) encargada se usa la del usuario actual
                if (encuestador.IdResponsable == 0) { encuestador.IdResponsable = user.IDDependencia; }

                //Usuario y fecha de creación
                encuestador.User = user;
                encuestador.CreationDate = DateTime.Now;

                encuestador.Code = await GenerateCode(encuestador.DNI);

                db.Pollster.Add(encuestador);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = encuestador.Id });
            }


            ResponsablesController controlResponsable = new ResponsablesController(db);
                        
            var ids = controlResponsable.GetAllIdsFromResponsable(user.IDDependencia);

            ViewBag.Departamentos = new SelectList(await db.Location.Where(n => n.IdLevel == 2).OrderBy(n => n.Name).ToListAsync(), "Id", "Name");
            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => ids.Contains(n.Id) && n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", encuestador.IdResponsable);
            return View(encuestador);
        }

        // GET: EncuestadorController/Edit/5
        [Authorize(Policy = "Encuestador.Editar")]
        public async Task<ActionResult> Edit(int id)
        {
            ResponsablesController controlResponsable = new ResponsablesController(db);
            //Valida que el usuario tenga permisos sobre el encuestador
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var respRelacionado = await GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var ids = controlResponsable.GetAllIdsFromResponsable(user.IDDependencia);

            Pollster encuestador = await db.Pollster.Where(n => n.Id == id && respRelacionado.Contains(n.IdResponsable)).FirstOrDefaultAsync();
            if (encuestador == null) { return NotFound(); }

            if(encuestador.Location != null)
            {
                encuestador.IdLocationParent = (int)encuestador.Location.IdParent;
            }
            ViewBag.Departamentos = new SelectList(await db.Location.Where(n => n.IdLevel == 2).OrderBy(n => n.Name).ToListAsync(), "Id", "Name", encuestador.IdLocationParent);
            ViewBag.Municipios = new SelectList(await db.Location.Where(n => n.IdLevel == 3 && n.IdParent == encuestador.IdLocationParent).OrderBy(n => n.Name).ToListAsync(), "Id", "Name", encuestador.IdLocation);
            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => ids.Contains(n.Id) && n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", encuestador.IdResponsable);

            return View(encuestador);

    
        }


        // POST: EncuestadorController/Edit/5
        [Authorize(Policy = "Encuestador.Editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Pollster encuestador)
        {
            var original = await db.Pollster.FindAsync(encuestador.Id);
            if (original == null)
            {
                ModelState.AddModelError(string.Empty, "Encuestador no registrado.");
            }
            if (ModelState.IsValid)
            {
                original.DNI = encuestador.DNI;
                original.Name = encuestador.Name;
                original.PhoneNumber = encuestador.PhoneNumber;
                original.Email = encuestador.Email;
                if(encuestador.IdLocation > 0)
                {
                    original.IdLocation = encuestador.IdLocation;
                }
                if (encuestador.IdResponsable > 0)
                {
                    original.IdResponsable = encuestador.IdResponsable;
                }
                original.Code = await GenerateCode(original.DNI);

                db.Entry(original).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = original.Id });
            }
            ResponsablesController controlResponsable = new ResponsablesController(db);
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var ids = controlResponsable.GetAllIdsFromResponsable(user.IDDependencia);

            ViewBag.Departamentos = new SelectList(await db.Location.Where(n => n.IdLevel == 2).OrderBy(n => n.Name).ToListAsync(), "Id", "Name", encuestador.IdLocation);
            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => ids.Contains(n.Id) && n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", encuestador.IdResponsable);
            return View(encuestador);
        }

        // GET: EncuestadorController/Delete/5
        [Authorize(Policy = "Encuestador.Administrar")]
        public async Task<ActionResult> Delete(int id)
        {
            Pollster encuestador = await db.Pollster.FindAsync(id);
            if (encuestador == null) { return NotFound(); }
            return View(encuestador);
        }

        // POST: EncuestadorController/Delete/5
        [Authorize(Policy = "Encuestador.Administrar")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var controlConfiguracion = new ConfiguracionsController(db);
            Pollster encuestador = await db.Pollster.FindAsync(id);
            try
            {
                db.Pollster.Remove(encuestador);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = controlConfiguracion.SqlErrorHandler(ex);
                HttpContext.Session.SetComplex("error", error);
            }

            return RedirectToAction("Index");
        }

        public async Task<string> GenerateCode(int DNI)
        {
            int codigo = await db.Configuracion.Select(n => n.CodeEncuestador).FirstOrDefaultAsync();
            int final = DNI % 100000;

            int clave = final > codigo ? final - codigo : codigo - final;
            return clave.ToString("00000");
        }


        public async Task<List<int>> GetResponsablesbyIdParent(int idParent, int level, int maxLevel)
        {
            var resp = new List<int> { idParent };
            if (level == maxLevel) { return resp; }
            
            var childs = await db.Responsable.Where(n => n.IdJefe == idParent).Select(n => n.Id).ToListAsync();
            foreach (int r in childs)
            {
                var aux = await GetResponsablesbyIdParent(r, level +1, maxLevel);
                if (aux != null && aux.Count > 0)
                {
                    resp.AddRange(aux);
                }
            }

            return resp;
        }


        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyDNI(int DNI, int Id)
        {
            if (db.Pollster.Where(n => n.DNI == DNI && n.Id != Id).Any())
            {
                return Json($"La cedula {DNI} ya ha sido registrada.");
            }
            return Json(true);
        }



        [Authorize(Policy = "Encuestador.Editar")]
        [HttpPost]
        public async Task<ActionResult> LocationsAjax(int IdParent)
        {
            var locations = await db.Location.Where(n => n.IdParent == IdParent).Select(n => new {
                n.Id,
                n.Name
            }).OrderBy(n => n.Name).ToListAsync();
            return Json(locations);
        }


        

    }
}
