using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            var users = await UserManager.Users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Email = u.Email,
                IsBlocked = u.LockoutEnabled && u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > System.DateTime.UtcNow,
                IsAdmin = false
            }).ToListAsync();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new InMContext()));
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                foreach (var user in users)
                {
                    user.IsAdmin = await UserManager.IsInRoleAsync(user.Id, "Admin");
                }
            }

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Block(string id)
        {
            await UserManager.SetLockoutEndDateAsync(id, System.DateTime.UtcNow.AddYears(100));
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unblock(string id)
        {
            await UserManager.SetLockoutEndDateAsync(id, DateTimeOffset.MinValue);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                await UserManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakeAdmin(string id)
        {
            await UserManager.AddToRoleAsync(id, "Admin");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAdmin(string id)
        {
            await UserManager.RemoveFromRoleAsync(id, "Admin");
            return RedirectToAction("Index");
        }
    }
}