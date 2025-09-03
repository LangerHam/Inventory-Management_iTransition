using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private InMContext db;

        public InventoryController()
        {
            db = new InMContext();
        }

        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var ownedInventories = await db.Inventories
                .Where(i => i.OwnerId == userId)
                .ToListAsync();
            var accessibleInventories = await db.InventoryUserAccesses
                .Where(iua => iua.UserId == userId)
                .Select(iua => iua.Inventory)
                .ToListAsync();
            var viewModel = new InventoryIndexViewModel
            {
                OwnedInventories = ownedInventories,
                AccessibleInventories = accessibleInventories
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var inventory = await db.Inventories
                .Include(i => i.Owner)
                .Include(i => i.Items.Select(item => item.CreatedBy))
                .Include("Comments.User")
                .Include(i => i.CustomFields)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            var viewModel = new InventoryDetailViewModel
            {
                Inventory = inventory,
            };

            return View(viewModel);
        }

        public async Task<ActionResult> Create()
        {
            var viewModel = new InventoryFormViewModel
            {
                Categories = await db.Categories.ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InventoryFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var inventory = new Inventory
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    CategoryId = viewModel.CategoryId,
                    IsPublic = viewModel.IsPublic,
                    OwnerId = User.Identity.GetUserId(),
                };


                db.Inventories.Add(inventory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            viewModel.Categories = await db.Categories.ToListAsync();
            return View(viewModel);
        }
    }
}