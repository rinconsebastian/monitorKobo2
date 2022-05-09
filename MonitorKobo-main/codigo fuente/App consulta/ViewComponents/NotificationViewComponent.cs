using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        public NotificationViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> _userManager
)
        {
            db = context;
            userManager = _userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool admin)
        {

            var request = new List<NotificationViewModel>();
            var count = 0;

            var user = await userManager.FindByNameAsync(User.Identity.Name);

            if (admin)
            {
                count = await db.RequestUser.Where(n => n.AlertAdmin).CountAsync();
                request = await db.RequestUser.Where(n => n.AlertAdmin)
                    .OrderBy(n => n.CreateDate)
                    .Select(n => new NotificationViewModel
                    {
                        id = n.Id,
                        texto = n.Request
                    }).Take(5).ToListAsync();
            }
            else
            {
                count = await db.RequestUser.Where(n => n.IdUser == user.Id && n.AlertUser == true).CountAsync();
                request = await db.RequestUser.Where(n => n.IdUser == user.Id && n.AlertUser == true)
                    .Select(n => new NotificationViewModel
                    {
                        id = n.Id,
                        texto = n.Request
                    }).Take(5).ToListAsync();
            }
            ViewBag.Count = count;
            ViewBag.Admin = admin;
            return View(request);
        }

    }
}
