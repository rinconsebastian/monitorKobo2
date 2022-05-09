using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
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


        public SolicitudController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager, IWebHostEnvironment env)
        {
            db = context;
            userManager = _userManager;
            _env = env;
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
                    Id = n.Id,
                    Asunto = n.Request,
                    Formalizacion = n.FormalizationId > 0 ? "Si" : "No",
                    Adjunto = n.File != null && n.File != "" ? "Si" : "No",
                    Fecha = n.CreateDate,
                    Estado = n.State,
                    Alerta = n.AlertUser
                }).OrderByDescending(n => n.Fecha).ToListAsync();
            return Json(solicitudes);
        }

        [Authorize(Policy = "Solicitud.Crear")]
        public async Task<ActionResult> Create(int id = 0)
        {
            Formalization formalizacion = null;
            if(id > 0)
            {
                formalizacion = await db.Formalization.FindAsync(id);
            }
            ViewBag.Formalizacion = formalizacion;
            return View();
        }

        // POST: EncuestadorController/Create
        [Authorize(Policy = "Solicitud.Crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormFile fileTemp, string Request, string Message, string FormalizationId = "", string time = "")
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
                var solicitud = new RequestUser();

                solicitud.Request = Request;
                solicitud.Message = Message;
                solicitud.FormalizationId = FormalizationId != "" ? Int32.Parse(FormalizationId) : 0;
                solicitud.File = adjunto;

                var user = await userManager.FindByNameAsync(User.Identity.Name);
                solicitud.IdUser = user.Id;
                solicitud.CreateDate = DateTime.Now;
                solicitud.State = RequestUser.ESTADO_NUEVA;
                solicitud.AlertAdmin = true;

                db.RequestUser.Add(solicitud);
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

            Formalization formalizacion = null;
            if (solicitud.FormalizationId > 0)
            {
                formalizacion = await db.Formalization.FindAsync(solicitud.FormalizationId);
            }
            ViewBag.Formalizacion = formalizacion;

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
                    IdUser = n.IdUser,
                    File = n.File != null && n.File != "" ? "Si" : "No",
                    AlertAdmin = n.AlertAdmin ? "Si" : "No",
                    AlertUser = n.AlertUser ? "Si" : "No",
                    CreateDate = n.CreateDate,
                    ValidationDate = n.AdminName != null ? n.ValidationDate.ToString() : "",
                    AdminName = n.AdminName,
                    FormalizationId = n.FormalizationId,
                    FormalizationNumber = ""
                }).ToListAsync();

            var idsUsuarios = solicitudes.Select(n => n.IdUser).Distinct().ToList();
            var usuarios = await db.Users.Where(n => idsUsuarios.Contains(n.Id))
                .Select(n => new
                {
                    Id = n.Id,
                    Nombre = n.Nombre + " " + n.Apellido
                })
                .ToDictionaryAsync(n => n.Id, n => n.Nombre);

            var idsFormalizaciones = solicitudes.Select(n => n.FormalizationId).Distinct().ToList();
            var formalizaciones = await db.Formalization.Where(n => idsFormalizaciones.Contains(n.Id))
               .Select(n => new
               {
                   Id = n.Id,
                   Numero = n.NumeroRegistro
               })
               .ToDictionaryAsync(n => n.Id, n => n.Numero);

            foreach(var item in solicitudes)
            {
                if (usuarios.ContainsKey(item.IdUser))
                {
                    item.NameUser = usuarios[item.IdUser];
                }
                if (item.FormalizationId > 0 && formalizaciones.ContainsKey(item.FormalizationId))
                {
                    item.FormalizationNumber = formalizaciones[item.FormalizationId];
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

            Formalization formalizacion = null;
            if (solicitud.FormalizationId > 0)
            {
                formalizacion = await db.Formalization.FindAsync(solicitud.FormalizationId);
            }
            ViewBag.Formalizacion = formalizacion;

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

            Formalization formalizacion = null;
            if (original.FormalizationId > 0)
            {
                formalizacion = await db.Formalization.FindAsync(original.FormalizationId);
            }
            ViewBag.Formalizacion = formalizacion;

            return View(data);
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

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filename, out contentType);
            var mime = contentType ?? "application/octet-stream";

            Stream stream = new MemoryStream(data);
            return new FileStreamResult(stream, mime);
        }
    }
}
