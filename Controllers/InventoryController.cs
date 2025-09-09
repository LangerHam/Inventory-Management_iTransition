using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
                .Include(i => i.Comments.Select(c => c.Author)) 
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
            var categories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
            var viewModel = new InventoryFormViewModel
            {
                Categories = new SelectList(categories, "Id", "Name")
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


                if (!string.IsNullOrWhiteSpace(viewModel.Tags))
                {
                    var tagNames = viewModel.Tags.Split(',').Select(t => t.Trim().ToLower()).Where(t => !string.IsNullOrEmpty(t));
                    foreach (var tagName in tagNames)
                    {
                        var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName) ?? new Tag { Name = tagName };
                        inventory.InventoryTags.Add(new InventoryTag { Tag = tag });
                    }
                }

                db.Inventories.Add(inventory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var allCategories = await db.Categories.OrderBy(c => c.Name).ToListAsync();
            viewModel.Categories = new SelectList(allCategories, "Id", "Name", viewModel.CategoryId);
            return View(viewModel);
        }

        public async Task<ActionResult> _Settings(int inventoryId)
        {
            var inventory = await db.Inventories.FindAsync(inventoryId);
            if (inventory == null) { return HttpNotFound(); }

            var currentUserId = User.Identity.GetUserId();
            if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var viewModel = new InventoryFormViewModel
            {
                Id = inventory.Id,
                Title = inventory.Title,
                Description = inventory.Description,
                CategoryId = inventory.CategoryId,
                IsPublic = inventory.IsPublic,
                RowVersion = inventory.RowVersion,
                Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", inventory.CategoryId)
            };

            return PartialView("_Settings", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(InventoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", viewModel.CategoryId);
                return PartialView("_Settings", viewModel);
            }

            var inventoryToUpdate = await db.Inventories.FindAsync(viewModel.Id);
            if (inventoryToUpdate == null) { return HttpNotFound(); }

            var currentUserId = User.Identity.GetUserId();
            if (inventoryToUpdate.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            inventoryToUpdate.Title = viewModel.Title;
            inventoryToUpdate.Description = viewModel.Description;
            inventoryToUpdate.CategoryId = viewModel.CategoryId;
            inventoryToUpdate.IsPublic = viewModel.IsPublic;

            db.Entry(inventoryToUpdate).OriginalValues["RowVersion"] = viewModel.RowVersion;

            try
            {
                await db.SaveChangesAsync();

                var newViewModel = new InventoryFormViewModel
                {
                    Id = inventoryToUpdate.Id,
                    Title = inventoryToUpdate.Title,
                    Description = inventoryToUpdate.Description,
                    CategoryId = inventoryToUpdate.CategoryId,
                    IsPublic = inventoryToUpdate.IsPublic,
                    RowVersion = inventoryToUpdate.RowVersion,
                    Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", inventoryToUpdate.CategoryId),
                    SaveMessage = "All changes saved."
                };
                return PartialView("_Settings", newViewModel);
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value. Your edit operation was canceled.");
                viewModel.Categories = new SelectList(await db.Categories.ToListAsync(), "Id", "Name", viewModel.CategoryId);
                return PartialView("_Settings", viewModel);
            }
        }

        [ChildActionOnly]
        public async Task<ActionResult> _Discussion(int inventoryId)
        {
            var inventory = await db.Inventories
                .Include(i => i.Comments.Select(c => c.Author))
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null) return HttpNotFound();

            return PartialView("_Discussion", inventory);
        }
    }
}