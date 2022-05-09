using App_consulta.Data;
using App_consulta.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_consulta.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UsersController(ApplicationDbContext context,
            UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager
            )
        {
            db = context;
            userManager = _userManager;
            signInManager = _signInManager;
        }

        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Index(string resultado)
        {
            string error = (string)HttpContext.Session.GetComplex<string>("error");
            if (error != "")
            {
                ViewBag.error = error;
                HttpContext.Session.Remove("error");
            }
            var usuarios = await db.Users.ToListAsync();
            return View(usuarios);
        }

        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Details(string Id)
        {
            ApplicationUser usuario = await userManager.FindByIdAsync(Id);
            if (usuario == null) { return NotFound(); }

            var roles = await db.UserRoles.Where(n => n.UserId == Id).Select(n => n.RoleId).ToListAsync();
            ViewBag.Roles = await db.Roles.Where(n => roles.Contains(n.Id)).ToListAsync();
            return View(usuario);
        }


        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await db.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    IDDependencia = model.Dependencias
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    if (model.UserRoles != null)
                    {
                        foreach (var rol in model.UserRoles)
                        {
                            await userManager.AddToRoleAsync(user, rol);
                        }
                    }
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            ViewBag.Roles = new SelectList(await db.Roles.ToListAsync(), "Name", "Name");
            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre");

            return View(model);
        }


        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Edit(string id)
        {

            ApplicationUser usuario = await userManager.FindByIdAsync(id);
            if (usuario == null) { return NotFound(); }

            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre", usuario.IDDependencia);

            var idRoles = await db.UserRoles.Where(n => n.UserId == usuario.Id).Select(n => n.RoleId).ToListAsync();
            var roles = await db.Roles.Where(n => idRoles.Contains(n.Id)).Select(n => n.Name).ToListAsync();
            ViewBag.Roles = new SelectList(await db.Roles.ToListAsync(), "Name", "Name");

            EditViewModel Modelo = new EditViewModel()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                IDDependencia = usuario.IDDependencia,
                EmailConfirmed = usuario.EmailConfirmed,
                LockoutEnabled = usuario.LockoutEnabled,
                UserRoles = roles,
            };

            return View(Modelo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Usuario.Editar")]

        public async Task<ActionResult> Edit(EditViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var original = await userManager.FindByIdAsync(usuario.Id);

                original.Nombre = usuario.Nombre;
                original.Apellido = usuario.Apellido;
                if (original.Email != usuario.Email)
                {
                    original.Email = usuario.Email;
                    original.UserName = usuario.Email;
                }
                original.EmailConfirmed = usuario.EmailConfirmed;
                original.LockoutEnabled = usuario.LockoutEnabled;
                original.IDDependencia = usuario.IDDependencia;

                var updateResult = await userManager.UpdateAsync(original);

                if (updateResult.Succeeded)
                {
                    // INICIO - Guarda los roles
                    var idsRolesOriginal = await db.UserRoles.Where(n => n.UserId == original.Id).Select(n => n.RoleId).ToListAsync();
                    var namesRolesOriginal = await db.Roles.Where(n => idsRolesOriginal.Contains(n.Id)).Select(n => n.Name).ToListAsync();
                    var rolesRemover = namesRolesOriginal;
                    if (usuario.UserRoles != null && usuario.UserRoles.Count > 0)
                    {
                        var rolesAgregar = usuario.UserRoles.Except(namesRolesOriginal).ToList();
                        rolesRemover = namesRolesOriginal.Except(usuario.UserRoles).ToList();
                        if (rolesAgregar.Count > 0)
                        {
                            await userManager.AddToRolesAsync(original, rolesAgregar);
                        }
                    }
                    if (rolesRemover.Count > 0)
                    {
                        await userManager.RemoveFromRolesAsync(original, rolesRemover);
                    }
                    // FIN - Guarda los roles

                    // INICIO - Actualizar contraseña
                    if (usuario.Password != null && usuario.ConfirmPassword != null)
                    {
                        if (usuario.Password.Length > 0 && usuario.ConfirmPassword.Length > 0 && usuario.Password == usuario.ConfirmPassword)
                        {
                            var code = await userManager.GeneratePasswordResetTokenAsync(original);
                            var result = await userManager.ResetPasswordAsync(original, code, usuario.Password);
                            if (!result.Succeeded)
                            {
                                HttpContext.Session.SetComplex("error", "Imposible actualizar Contraseña");
                            }
                        }
                    }
                    // FIN - Actualizar contraseña
                }
                else
                {
                    var errorMsg = "";
                    foreach (var error in updateResult.Errors)
                    {
                        errorMsg += error.Description + "<br>";
                    }
                    HttpContext.Session.SetComplex("error", errorMsg);
                }
                return RedirectToAction("Index");
            }

            ViewBag.Responsables = new SelectList(await db.Responsable.ToListAsync(), "Id", "Nombre", usuario.IDDependencia);

            var idRoles = await db.UserRoles.Where(n => n.UserId == usuario.Id).Select(n => n.RoleId).ToListAsync();
            var roles = await db.Roles.Where(n => idRoles.Contains(n.Id)).Select(n => n.Name).ToListAsync();
            ViewBag.Roles = new SelectList(await db.Roles.ToListAsync(), "Name", "Name");

            return View(usuario);
        }

        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> Delete(string Id)
        {
            ApplicationUser usuario = await userManager.FindByIdAsync(Id);
            if (usuario == null) { return NotFound(); }
            return View(usuario);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Usuario.Editar")]
        public async Task<ActionResult> DeleteConfirmedAsync(string Id)
        {
            string error = "";
            ConfiguracionsController controlConfiguracion = new ConfiguracionsController(db);

            var user = await userManager.FindByIdAsync(Id);

            try
            {
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var errorA in result.Errors)
                    {
                        error += errorA.Description + "<br>";
                    }
                }
            }
            catch (Exception ex)
            {
                error = controlConfiguracion.SqlErrorHandler(ex);
            }
            HttpContext.Session.SetComplex("error", error);

            return RedirectToAction("Index");
        }
    }
}
