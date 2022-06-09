using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            db = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var config = await db.Configuracion.FirstOrDefaultAsync();
            ViewBag.config = config;

            var encuestas = await db.KoProject.Select(n => new
            {
                n.Id,
                n.Name
            }).ToDictionaryAsync(n => n.Id, n => n.Name);
            ViewBag.encuestas = encuestas;

            var registros = await db.KoProject.Where(n => n.Validable)
                .Select(n => new
                {
                    n.Id,
                    Name = n.ValidationName
                }).ToDictionaryAsync(n => n.Id, n => n.Name);

            ViewBag.registros = registros;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
