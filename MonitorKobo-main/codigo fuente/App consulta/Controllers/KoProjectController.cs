﻿using App_consulta.Data;
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
    public class KoProjectController : Controller
    {

        private readonly ApplicationDbContext db;
       
        public KoProjectController(ApplicationDbContext context)
        {
            db = context;
           
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
            var items = await db.KoProject.ToListAsync();
            return View(items);
        }

        [Authorize(Policy = "Configuracion.General")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Create(KoProject config)
        {
            if (ModelState.IsValid)
            {
                db.KoProject.Add(config);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(config);
        }


        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(int id)
        {
            var config = await db.KoProject.FindAsync(id);
            if (config == null) { return NotFound(); }

            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Policy = "Configuracion.General")]
        public async Task<ActionResult> Edit(KoProject config)
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
