using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class InformesController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment _env;


        public InformesController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager, IWebHostEnvironment env)
        {
            db = context;
            userManager = _userManager;
            _env = env;
        }


        [Authorize(Policy = "Encuestas.Listado")]
        public ActionResult Encuestados()
        {
            var kobo = new KoboController(db, userManager, _env);

            ViewBag.DataTime = kobo.GetDatetimeData(KoboController.FILE_CARACTERIZACION);

            return View();
        }

        [Authorize(Policy = "Encuestas.Listado")]
        public ActionResult Asociaciones()
        {
            var kobo = new KoboController(db, userManager, _env);

            ViewBag.DataTimeAssoc = kobo.GetDatetimeData(KoboController.FILE_ASOCIACION);

            return View();
        }

    }
}
