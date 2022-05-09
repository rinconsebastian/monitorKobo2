using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.ViewComponents
{
    public class UsuariosFromResponsableViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext db;
      
        public UsuariosFromResponsableViewComponent(ApplicationDbContext context)
        {
            db = context;
          
        }

        public async Task<IViewComponentResult> InvokeAsync(int idResponsable)
        {
            var items = await GetItemsAsync(idResponsable);
            return View(items);
        }
        private Task<List<ApplicationUser>> GetItemsAsync(int idResponsable)
        {

            return db.Users.Where(n => n.IDDependencia == idResponsable).OrderBy(n => n.Nombre).ToListAsync();

        }
    }
}
