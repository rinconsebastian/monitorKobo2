using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App_consulta.Data;
using App_consulta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.ViewComponents
{
    public class DependenciaNombre : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public DependenciaNombre(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var item = await GetItemsAsync(id);
            return View(item);
        }
        private Task<Responsable> GetItemsAsync(int id)
        {
            return db.Responsable.Where(n => n.Id == id).FirstOrDefaultAsync();
        }
    }
}
