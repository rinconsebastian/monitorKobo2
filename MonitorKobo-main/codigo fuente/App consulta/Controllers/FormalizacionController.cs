using System;
using System.Collections.Generic;
using System.Linq;
using App_consulta.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace App_consulta.Controllers
{
    public class FormalizacionController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _env;

        public FormalizacionController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager, IWebHostEnvironment env)
        {
            db = context;
            userManager = _userManager;
            _env = env;
        }

        [Authorize(Policy = "Formalizacion.Validar")]
        [HttpPost]
        public async Task<IActionResult> Cargar(string idKobo)
        {
            var r = new RespuestaAccion();

            var formalizacion = await db.Formalization.Where(n => n.IdKobo == idKobo).FirstOrDefaultAsync();

            if (formalizacion == null)
            {
                var kobo = new KoboController(db, userManager, _env);
                r = await kobo.LoadFormalizacion(idKobo, User.Identity.Name);
                return Json(r);
            }

            r.Url = "Formalizacion/Edit/" + formalizacion.Id;
            r.Success = true;
            return Json(r);
        }


        [Authorize(Policy = "Formalizacion.Listado")]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Policy = "Formalizacion.Listado")]
        public async Task<IActionResult> Ajax()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var formalizaciones = await db.Formalization.Where(n => respRelacionado.Contains(n.IdResponsable))
                .Select(n => new FormalizacionViewModel()
                {
                    ID = n.Id,
                    Registro = n.NumeroRegistro,
                    Cedula = n.Cedula,
                    Nombre = n.Name,
                    Departamento = n.Departamento,
                    Municipio = n.Municipio,
                    Fecha = n.FechaSolicitud,
                    Estado = n.Estado,
                    Coordinacion = n.Responsable.Nombre
                })
                .ToListAsync();
            foreach (var formalizacion in formalizaciones)
            {
                formalizacion.NombreEstado = GetEstado(formalizacion.Estado);
            }

            return Json(formalizaciones);
        }


        [Authorize(Policy = "Formalizacion.Ver")]
        public async Task<ActionResult> Details(int id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var formalizacion = await db.Formalization.Where(n => n.Id == id && respRelacionado.Contains(n.IdResponsable)).FirstOrDefaultAsync();
            if (formalizacion == null) { return NotFound(); }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            return View(formalizacion);
        }

        [Authorize(Policy = "Formalizacion.Imprimir")]
        public async Task<ActionResult> Print(int[] ids)
        {
            var formalizaciones = await db.Formalization.Where(n => ids.Contains(n.Id)).ToListAsync();
            var estadosValidos = new List<int> { Formalization.ESTADO_COMPLETO, Formalization.ESTADO_IMPRESO };
            ViewBag.EstadosValidos = estadosValidos;
            var idsValidos = formalizaciones.Where(n => estadosValidos.Contains(n.Estado)).Select(n => n.Id).ToList();
            ViewBag.Ids = JsonConvert.SerializeObject( idsValidos );
            return View(formalizaciones);
        }



        // GET: EncuestadorController/Edit/5
        [Authorize(Policy = "Formalizacion.Validar")]
        public async Task<ActionResult> Edit(int id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var encuestadorControl = new EncuestadorController(db, userManager, _env);
            var respRelacionado = await encuestadorControl.GetResponsablesbyIdParent(user.IDDependencia, 1, 3);

            var formalizacion = await db.Formalization.Where(n => n.Id == id && respRelacionado.Contains(n.IdResponsable)).FirstOrDefaultAsync();
            if (formalizacion == null) { return NotFound(); }

            var permisoCancelar = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Cancelar" && c.Value == "1"));

            if (formalizacion.Estado != Formalization.ESTADO_BORRADOR && !permisoCancelar)
            {
                return RedirectToAction("Details", new { id = formalizacion.Id });
            }

            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }

            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", formalizacion.IdResponsable);

            var estados = new Dictionary<int, string>{
                {  Formalization.ESTADO_BORRADOR, "Borrador" },
                {  Formalization.ESTADO_COMPLETO, "Completo" },
                {  Formalization.ESTADO_CANCELADO, "Cancelado" },
                {  Formalization.ESTADO_IMPRESO, "Impreso" },
                {  Formalization.ESTADO_CARNET_VIGENTE, "Carnet vigente" }
            };

            ViewBag.Estados = new SelectList(estados, "Key", "Value", formalizacion.Estado);

            ViewBag.Formalizacion = formalizacion;
            var postModel = new FormalizacionPostModel() {
                Id = formalizacion.Id,
                AreaPesca = formalizacion.AreaPesca,
                ArtesPesca = formalizacion.ArtesPesca,
                Cedula = formalizacion.Cedula,
                Estado = formalizacion.Estado,
                IdResponsable = formalizacion.IdResponsable,
                Name = formalizacion.Name,
                NombreAsociacion = formalizacion.NombreAsociacion
            };

            return View(postModel);

        }

        [Authorize(Policy = "Formalizacion.Validar")]
        [HttpPost]
        public async Task<RespuestaAccion> CambiarEstado(int id, int estado)
        {
            var r = new RespuestaAccion();
            var formalizacion = await db.Formalization.FindAsync(id);
            if (formalizacion == null) { r.Message = "Formalización no registrada."; return r; }

            try
            {
                var anterior = await db.Formalization.AsNoTracking().Where(n => n.Id == formalizacion.Id).FirstOrDefaultAsync();

                formalizacion.Estado = estado;
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                formalizacion.LastEditDate = DateTime.Now;
                formalizacion.LastEditByUser = user;

                db.Entry(formalizacion).State = EntityState.Modified;
                await db.SaveChangesAsync();

                var log = new Logger(db);
                var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "ChangeStatus", Modelo = "Formalization", ValAnterior = anterior, ValNuevo = formalizacion };
                await log.Registrar(registro, typeof(Formalization), formalizacion.Id);

                //var nombreEstado = GetEstado(estado).ToUpper();
                //r.Message = "El estado de la formalización fue cambiado ha " + nombreEstado + " correctamente.";
                r.Success = true;            
            }
            catch (Exception e)
            {
                r.Message = e.Message;
            }


            return r;
        }

        [Authorize(Policy = "Formalizacion.Imprimir")]
        [HttpPost]
        public async Task<int> Imprimir(string ids)
        {
            var success = 0;

            if(ids == "") { return success; }
            var idsList = ids.Split(',').Select(n => Convert.ToInt32(n)).ToList();

            //&& n.Estado != Formalization.ESTADO_IMPRESO   solo marcar primera impresion
            var formalizaciones = await db.Formalization.Where(n => idsList.Contains(n.Id) ).ToListAsync();
            var anteriores = await db.Formalization.AsNoTracking().Where(n => idsList.Contains(n.Id) && n.Estado != Formalization.ESTADO_IMPRESO).ToListAsync();

            foreach (var formalizacion in formalizaciones)
            {
                try
                {
                    var anterior = anteriores.Where(n => n.Id == formalizacion.Id).FirstOrDefault();

                    formalizacion.Estado = Formalization.ESTADO_IMPRESO;

                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    formalizacion.LastEditDate = DateTime.Now;
                    formalizacion.LastEditByUser = user;

                    db.Entry(formalizacion).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    var log = new Logger(db);
                    var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "Print", Modelo = "Formalization", ValAnterior = anterior, ValNuevo = formalizacion };
                    await log.Registrar(registro, typeof(Formalization), formalizacion.Id);

                    success++;
                }
                catch (Exception) { }
            }


            return success;
        }

        [Authorize(Policy = "Formalizacion.Validar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormalizacionPostModel formalizacion)
        {
            var original = await db.Formalization.FindAsync(formalizacion.Id);
            if (original == null)
            {
                ModelState.AddModelError(string.Empty, "Formalización no registrada.");
            }
            if (ModelState.IsValid)
            {
                var permisoEditar = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Editar" && c.Value == "1"));
                if (permisoEditar)
                {
                    original.Name = formalizacion.Name;
                    original.Cedula = formalizacion.Cedula;
                    original.NombreAsociacion = formalizacion.NombreAsociacion;
                    original.AreaPesca = formalizacion.AreaPesca;
                    original.ArtesPesca = formalizacion.ArtesPesca;
                }
                if(formalizacion.Estado > 0) { 
                    original.Estado = formalizacion.Estado;
                }
                if (formalizacion.IdResponsable > 0)
                {
                    original.IdResponsable = formalizacion.IdResponsable;
                }

                var anterior = await db.Formalization.AsNoTracking().Where(n => n.Id == formalizacion.Id).FirstOrDefaultAsync();

                var user = await userManager.FindByNameAsync(User.Identity.Name);
                original.LastEditDate = DateTime.Now;
                original.LastEditByUser = user;

                db.Entry(original).State = EntityState.Modified;
                await db.SaveChangesAsync();



                var log = new Logger(db);
                var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "Edit", Modelo = "Formalization", ValAnterior = anterior, ValNuevo = original };
                await log.Registrar(registro, typeof(Formalization), formalizacion.Id);

                HttpContext.Session.SetComplex("error", "Los datos fueron actualizados correctamente.");

                return RedirectToAction("Edit", new { id = formalizacion.Id });
            }

            ViewBag.Coordinaciones = new SelectList(await db.Responsable.Where(n => n.Nombre.StartsWith("[CDR]")).OrderBy(n => n.Nombre).ToListAsync(), "Id", "Nombre", formalizacion.IdResponsable);

            var estados = new Dictionary<int, string>{
                {  Formalization.ESTADO_BORRADOR, "Borrador" },
                {  Formalization.ESTADO_COMPLETO, "Completo" },
                {  Formalization.ESTADO_CANCELADO, "Cancelado" },
                {  Formalization.ESTADO_IMPRESO, "Impreso" },
                {  Formalization.ESTADO_CARNET_VIGENTE, "Carnet vigente" }

            };

            ViewBag.Estados = new SelectList(estados, "Key", "Value", formalizacion.Estado);

            ViewBag.Formalizacion = formalizacion;
            var postModel = new FormalizacionPostModel()
            {
                Id = formalizacion.Id,
                AreaPesca = formalizacion.AreaPesca,
                ArtesPesca = formalizacion.ArtesPesca,
                Cedula = formalizacion.Cedula,
                Estado = formalizacion.Estado,
                IdResponsable = formalizacion.IdResponsable,
                Name = formalizacion.Name,
                NombreAsociacion = formalizacion.NombreAsociacion
            };

            return View(postModel);
        }


        [HttpGet]
        public FileStreamResult ViewImage(string filename = "noFound.jpg")
        {
            var permiso = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Ver" && c.Value == "1"));
            if (!permiso) { filename = "lock.jpg"; }

            if (filename == null) { filename = "noFound.jpg"; }

            string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);

            byte[] data = new byte[1];
            try
            {
                data = System.IO.File.ReadAllBytes(filepath);
            }
            catch (Exception) { }

            Stream stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/jpeg");
        }


        [HttpPost]
        [Authorize(Policy = "Formalizacion.Validar")]
        public async Task<IActionResult> UpdateImage(IFormFile file, string filename)
        {
            var r = await UpdateImageGen(file, filename);
            return Json(r);
        }

        [HttpPost]
        [Authorize(Policy = "Formalizacion.Imagen.Cambiar")]
        public async Task<IActionResult> LoadImage(IFormFile file, string filename, string formalizacion = "", string name = "")
        {
            var r = await UpdateImageGen(file, filename);

            var log = new Logger(db);
            var registro = new RegistroLog { Usuario = User.Identity.Name, Accion = "LoadImage", Modelo = "Formalization " + formalizacion, ValAnterior = "", ValNuevo = name + ": " + filename };
            await log.RegistrarDirecto(registro);

            return Json(r);
        }

        private async Task<RespuestaAccion> UpdateImageGen(IFormFile file, string filename)
        {
            var r = new RespuestaAccion();

            if (file != null && file.Length > 0)
            {
                string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                
                try
                    {
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        r.Success = true;
                    }
                    catch (Exception e) { r.Message = "Error: " + e.Message; }
               
            }
            else { r.Message = "Error: El archivo no es válido."; }
            return r;
        }


        public string GetEstado(int s)
        {
            var r = s.ToString();
            switch (s)
            {
                case Formalization.ESTADO_BORRADOR:
                    r = "Borrador";
                    break;
                case Formalization.ESTADO_COMPLETO:
                    r = "Completo";
                    break;
                case Formalization.ESTADO_CANCELADO:
                    r = "Cancelado";
                    break;
                case Formalization.ESTADO_IMPRESO:
                    r = "Impreso";
                    break;
                case Formalization.ESTADO_CARNET_VIGENTE:
                    r = "Carnet vigente";
                    break;
            }
            return r;
        }
    }
}
