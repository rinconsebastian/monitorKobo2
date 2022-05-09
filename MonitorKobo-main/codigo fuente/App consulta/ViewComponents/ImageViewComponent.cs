using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.ViewComponents
{
    public class ImageViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _env;

        public ImageViewComponent(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<IViewComponentResult> InvokeAsync(string file, string text = "", string css = "", string id = "")
        {
            var time = "";
            try
            {
                var _path = Path.Combine(_env.ContentRootPath, "Storage");
                DateTime date = System.IO.File.GetLastWriteTime(Path.Combine(_path, file));
                time = date.ToString("u");
            }
            catch (Exception) { }

            ViewBag.Path = file;
            ViewBag.Text = text;
            ViewBag.Css = css;
            ViewBag.Time = time;
            ViewBag.Id = id;

            return View();
        }
    }
}
