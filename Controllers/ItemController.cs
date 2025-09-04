using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private InMContext db;

        public ItemController()
        {
            db = new InMContext();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = await db.Items
                .Include(i => i.Inventory)
                .Include(i => i.CreatedBy)
                .Include(i => i.Likes)
                .Include(i => i.CustomFieldValues.Select(cfv => cfv.CustomField))
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }


        public async Task<ActionResult> Create(int inventoryId)
        {
            var inventory = await db.Inventories
                .Include(i => i.CustomFields)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            // TODO: Add security check here.
            // Verify if the current user has write access to this inventory.
            // (IsOwner, IsAdmin, IsPublic, or in InventoryUserAccess list)

            var viewModel = new ItemFormViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                FieldValues = inventory.CustomFields.Select(cf => new CustomFieldValueViewModel
                {
                    CustomFieldId = cf.Id,
                    FieldName = cf.Title,
                    FieldType = cf.Type,
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ItemFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var inventory = await db.Inventories.FindAsync(viewModel.InventoryId);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            // TODO: Add security check here (same as GET)

            var userId = User.Identity.GetUserId();
            var newItem = new Item
            {
                InventoryId = viewModel.InventoryId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                RowVersion = new byte[8], 
                // TODO: Implement the actual custom ID generation logic here.

                CustomId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()
            };

            newItem.CustomFieldValues = new List<CustomFieldValue>();
            foreach (var fieldValueModel in viewModel.FieldValues)
            {
                var customFieldValue = new CustomFieldValue
                {
                    CustomFieldId = fieldValueModel.CustomFieldId,
                    Item = newItem,
                    Value = fieldValueModel.Value
                };
                newItem.CustomFieldValues.Add(customFieldValue);
            }

            db.Items.Add(newItem);
            await db.SaveChangesAsync();

            return RedirectToAction("Details", "Inventory", new { id = viewModel.InventoryId });
        }
    }
}