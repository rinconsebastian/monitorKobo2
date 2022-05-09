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
    public class UsersByRoleViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext db;
       
        public UsersByRoleViewComponent(ApplicationDbContext context)
        {
            db = context;
          
        }

        public async Task<IViewComponentResult> InvokeAsync(string idRole)
        {
            var items = await GetItemsAsync(idRole);
            return View(items);
        }
        private Task<List<ApplicationUser>> GetItemsAsync(string idRole)
        {
            var ids = db.UserRoles.Where(n => n.RoleId == idRole).Select(n => n.UserId).ToList();
            return db.Users.Where(n => ids.Contains(n.Id)).ToListAsync();
        }
    }
}
