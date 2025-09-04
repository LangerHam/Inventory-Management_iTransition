using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class CustomIdController : Controller
    {
        private InMContext db;

        public CustomIdController()
        {
            db = new InMContext();
        }

        public async Task<ActionResult> _IdManager(int inventoryId)
        {
            var inventory = await db.Inventories
                .Include(i => i.CustomIdElements)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var viewModel = new CustomIdManagementViewModel
            {
                InventoryId = inventoryId,
                Elements = inventory.CustomIdElements.OrderBy(e => e.Order).ToList()
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomIdElementViewModel elementViewModel)
        {
            if (ModelState.IsValid)
            {
                var inventory = await db.Inventories.Include(i => i.CustomIdElements)
                                    .FirstOrDefaultAsync(i => i.Id == elementViewModel.InventoryId);
                var currentUserId = User.Identity.GetUserId();

                if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                var newElement = new CustomIdElement
                {
                    InventoryId = elementViewModel.InventoryId,
                    Type = elementViewModel.ElementType,
                    ValueOrFormat = elementViewModel.Value,
                    Order = inventory.CustomIdElements.Any() ? inventory.CustomIdElements.Max(e => e.Order) + 1 : 1
                };

                db.CustomIdElements.Add(newElement);
                await db.SaveChangesAsync();

                return RedirectToAction("_IdManager", new { inventoryId = elementViewModel.InventoryId });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var element = await db.CustomIdElements.Include(e => e.Inventory).FirstOrDefaultAsync(e => e.Id == id);
            if (element == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            if (element.Inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            int inventoryId = element.InventoryId;
            db.CustomIdElements.Remove(element);
            await db.SaveChangesAsync();

            return RedirectToAction("_IdManager", new { inventoryId = inventoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reorder(int inventoryId, List<int> orderedIds)
        {
            var inventory = await db.Inventories.FindAsync(inventoryId);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (orderedIds != null)
            {
                var elementsToUpdate = await db.CustomIdElements
                                                     .Where(e => e.InventoryId == inventoryId)
                                                     .ToListAsync();

                for (int i = 0; i < orderedIds.Count; i++)
                {
                    var element = elementsToUpdate.FirstOrDefault(e => e.Id == orderedIds[i]);
                    if (element != null)
                    {
                        element.Order = i + 1;
                    }
                }
                await db.SaveChangesAsync();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}