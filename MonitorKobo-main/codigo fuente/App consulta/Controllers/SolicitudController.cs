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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace App_consulta.Controllers
{
    public class SolicitudController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _env;
        private readonly MongoDatabaseService mdb;


        public SolicitudController(ApplicationDbContext context, 
            UserManager<ApplicationUser> _userManager,
            IWebHostEnvironment env,
            MongoDatabaseService mongoService)
        {
            db = context;
            userManager = _userManager;
            _env = env;
            mdb = mongoService;
        }

        // GET: EncuestadorController
        [Authorize(Policy = "Solicitud.Crear")]
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

        [Authorize(Policy = "Solicitud.Crear")]
        public async Task<ActionResult> ListAjax()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var solicitudes = await db.RequestUser.Where(n => user.Id == n.IdUser)
                .Select(n => new
                {
                    n.Id,
                    Asunto = n.Request,
                    Registro = n.RecordNumber != null && n.RecordNumber != "" ? "Si" : "No",
                    Adjunto = n.File != null && n.File != "" ? "Si" : "No",
                    Fecha = n.CreateDate,
                    Estado = n.State,
                    Alerta = n.AlertUser
                }).OrderByDescending(n => n.Fecha).ToListAsync();
            return Json(solicitudes);
        }


        [Authorize(Policy = "Solicitud.Crear")]
        public async Task<ActionResult> Create(string id = "", int project = 0)
        {
            KoGenericData registro = null;
            var projectObj = await db.KoProject.FindAsync(project);

            if (projectObj != null && projectObj.Validable) { 
                registro = await mdb.Find(projectObj.Collection, id);
            }

            ViewBag.registro = registro;
            ViewBag.project = project;

            return View();
        }

        // POST: EncuestadorController/Create
        [Authorize(Policy = "Solicitud.Crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormFile fileTemp, RequestUser item, string time = "")
        {
            String error = "";

            //Valida el archivo
            var adjunto = "";
            if (fileTemp != null && fileTemp.Length > 0)
            {
                try
                {
                    var _path = Path.Combine(_env.ContentRootPath, "Storage");
                    //Crea la carpeta
                    var _pathFolder = Path.Combine(_path, "Adjuntos");
                    if (!Directory.Exists(_pathFolder))
                    {
                        Directory.CreateDirectory(_pathFolder);
                    }
                    //Carga el archivo
                    var _fileName = time + "-" + Path.GetFileName(fileTemp.FileName);
                    var _pathFile = Path.Combine(_pathFolder, _fileName);
                    using (var fileStream = new FileStream(_pathFile, FileMode.Create))
                    {
                        await fileTemp.CopyToAsync(fileStream);
                    }
                    adjunto = Path.Combine("Adjuntos", _fileName);
                }
                catch (Exception e) { error = "Error archivo: " + e.Message; }
            }
          
            //Crea la solicitud
            try
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);

                item.File = adjunto;        
                item.IdUser = user.Id;
                item.CreateDate = DateTime.Now;
                item.State = RequestUser.ESTADO_NUEVA;
                item.AlertAdmin = true;

                db.RequestUser.Add(item);
                await db.SaveChangesAsync();
            }
            catch (Exception e) { error = "Error solicitud: " + e.Message; }

            return Json(error);
        }


        [Authorize(Policy = "Solicitud.Crear")]
        public async Task<ActionResult> DetailsAlert(int id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            var solicitud = await db.RequestUser.Where(n => n.Id == id && n.IdUser == user.Id).FirstOrDefaultAsync();
            if (solicitud == null) { return NotFound(); }

            solicitud.AlertUser = false;
            db.Entry(solicitud).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = solicitud.Id });
        }

        [Authorize(Policy = "Solicitud.Crear")]
        public async Task<ActionResult> Details(int id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            var solicitud = await db.RequestUser.Where(n => n.Id == id && n.IdUser == user.Id).FirstOrDefaultAsync();
            if (solicitud == null) { return NotFound(); }

            ViewBag.registro = solicitud.RecordNumber;

            return View(solicitud);
        }


        // GET: EncuestadorController
        [Authorize(Policy = "Solicitud.Administrar")]
        public ActionResult IndexAdmin()
        {
            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }
            return View();
        }

        [Authorize(Policy = "Solicitud.Administrar")]
        public async Task<ActionResult> ListAjaxAdmin()
        {
            var solicitudes = await db.RequestUser
                .Select(n => new RequestViewModel { 
                    Id = n.Id,
                    State = n.State,
                    Request = n.Request,
                    UserId = n.IdUser,
                    UserName = "",
                    File = n.File != null && n.File != "" ? "Si" : "No",
                    AlertAdmin = n.AlertAdmin ? "Si" : "No",
                    AlertUser = n.AlertUser ? "Si" : "No",
                    AlertEmail = n.AlertUser ? "Si" : "No",
                    CreateDate = n.CreateDate,
                    ValidationDate = n.AdminName != null ? n.ValidationDate.ToString() : "",
                    AdminName = n.AdminName,
                    RecordId = n.RecordId,
                    RecordProject = n.RecordProject,
                    RecordNumber = n.RecordNumber,
                    Message = n.Message
                }).ToListAsync();

            var idsUsuarios = solicitudes.Select(n => n.UserId).Distinct().ToList();
            var usuarios = await db.Users.Where(n => idsUsuarios.Contains(n.Id))
                .Select(n => new
                {
                    n.Id,
                    Nombre = n.Nombre + " " + n.Apellido
                })
                .ToDictionaryAsync(n => n.Id, n => n.Nombre);
            
            foreach(var item in solicitudes)
            {
                if (usuarios.ContainsKey(item.UserId))
                {
                    item.UserName = usuarios[item.UserId];
                }
            }
            
            return Json(solicitudes);
        }

        [Authorize(Policy = "Solicitud.Administrar")]
        public async Task<ActionResult> Edit(int id)
        {
            var solicitud = await db.RequestUser.FindAsync(id);
            if (solicitud == null) { return NotFound(); }

            var dataForm = new RequestUserDataForm { Id = solicitud.Id, Response = solicitud.Response, State = solicitud.State };

            var estados = new Dictionary<int, string>{
                {  RequestUser.ESTADO_NUEVA, "Nuevo" },
                {  RequestUser.ESTADO_EN_PROCESO, "En proceso" },
                {  RequestUser.ESTADO_SOLUCIONADA, "Completo" },
                {  RequestUser.ESTADO_CANCELADA, "Cancelado" },
            };
            ViewBag.Estados = new SelectList(estados, "Key", "Value", solicitud.State);

            ViewBag.Solicitud = solicitud;

            var user = await userManager.FindByIdAsync(solicitud.IdUser);
            ViewBag.User = user;

            ViewBag.registro = solicitud.RecordNumber;

            return View(dataForm);
        }

        [Authorize(Policy = "Solicitud.Administrar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RequestUserDataForm data)
        {
            var original = await db.RequestUser.FindAsync(data.Id);
            if (original == null)
            {
                ModelState.AddModelError(string.Empty, "Solicitud no registrada.");
            }

            if (ModelState.IsValid)
            {
                //Agrega los cambios de formulario
                original.Response = data.Response;
                original.State = data.State;

                //Agrega metadatos del usuario actual
                var adminUser = await userManager.FindByNameAsync(User.Identity.Name);
                original.AdminName = adminUser.Nombre + " " + adminUser.Apellido;
                original.ValidationDate = DateTime.Now;

                //Cambia las alertyas
                original.AlertAdmin = false;
                original.AlertUser = true;

                db.Entry(original).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("IndexAdmin");
            }

            var estados = new Dictionary<int, string>{
                {  RequestUser.ESTADO_NUEVA, "Nuevo" },
                {  RequestUser.ESTADO_EN_PROCESO, "En proceso" },
                {  RequestUser.ESTADO_SOLUCIONADA, "Terminado" },
                {  RequestUser.ESTADO_CANCELADA, "Cancelado" },
            };
            ViewBag.Estados = new SelectList(estados, "Key", "Value", data.State);

            ViewBag.Solicitud = original;

            var user = await userManager.FindByIdAsync(original.IdUser);
            ViewBag.User = user;
            //TODO revisar que funcione
            ViewBag.registro = original.RecordNumber;

            return View(data);
        }

        [Authorize(Policy = "Solicitud.Administrar")]
        [HttpPost]
        public async Task<ActionResult> QuickResponse(int id)
        {
            RespuestaAccion r = new();

            var solicitud = await db.RequestUser.FindAsync(id);
            if (solicitud != null)
            {
                var projectObj = await db.KoProject.FindAsync(solicitud.RecordProject);
                if (projectObj != null && projectObj.Validable)
                {
                    try
                    {
                        var item = await mdb.FindViewModel(projectObj.Collection, solicitud.RecordId);
                        if (item != null)
                        {
                            if (item.State != KoGenericData.ESTADO_BORRADOR)
                            {

                                var user = await userManager.FindByNameAsync(User.Identity.Name);
                                var update = Builders<KoExtendData>.Update.Set("edit_user", user.Id);
                                var datetime = DateTime.Now;
                                update = update.Set("edit_date", datetime);
                                update = update.Set("state", KoGenericData.ESTADO_BORRADOR);

                                var filter = Builders<KoExtendData>.Filter.Eq(n => n.Id, item.Id);
                                var save = await mdb.Update(projectObj.Collection, update, filter);

                                if (save)
                                {
                                    solicitud.Response = "FORMALIZACIÓN PASA A BORRADOR.";
                                    solicitud.State = RequestUser.ESTADO_SOLUCIONADA;
                                    solicitud.AdminName = user.Nombre + " " + user.Apellido;
                                    solicitud.ValidationDate = DateTime.Now;
                                    solicitud.AlertAdmin = false;
                                    solicitud.AlertUser = true;

                                    db.Entry(solicitud).State = EntityState.Modified;
                                    await db.SaveChangesAsync();

                                    r.Success = true;
                                }
                                else { r.Message = "Error: No fue posible cambiar el estado del registro."; }

                            }
                            else { r.Message = "ERROR: Formalización ya esta en estado borrador."; }
                        }
                        else { r.Message = "ERROR: Registro no encontrado."; }

                    }
                    catch (Exception e) { r.Message = "ERROR: " + e.Message; }
                }
                else { r.Message = "ERROR: Proyecto no encontrado."; }
            }
            else { r.Message = "ERROR: Solicitud no encontrada."; }

            return Json(r);
        }

        [HttpGet]
        public FileStreamResult ViewAdjunto(string filename = "noFound.jpg")
        {
            var permiso = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Crear" && c.Value == "1"));
            var permisoAdmin = User.HasClaim(c => (c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Administrar" && c.Value == "1"));
            if (!permiso && !permisoAdmin) { filename = "lock.jpg"; }

            if (filename == null) { filename = "noFound.jpg"; }

            string filepath = Path.Combine(_env.ContentRootPath, "Storage", filename);

            byte[] data = System.IO.File.ReadAllBytes(filepath);

            new FileExtensionContentTypeProvider().TryGetContentType(filename, out string contentType);
            var mime = contentType ?? "application/octet-stream";

            Stream stream = new MemoryStream(data);
            return new FileStreamResult(stream, mime);
        }
    }
}
