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
using Inventory_Management_iTransition.Services;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private InMContext db;
        private readonly IdFormatService _idFormatService = new IdFormatService();

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
                var inventoryForForm = await db.Inventories.Include(i => i.CustomFields).FirstOrDefaultAsync(i => i.Id == viewModel.InventoryId);
                viewModel.InventoryTitle = inventoryForForm.Title;
                viewModel.FieldValues = inventoryForForm.CustomFields.Select(cf => new CustomFieldValueViewModel
                {
                    CustomFieldId = cf.Id,
                    FieldName = cf.Title,
                    FieldType = cf.Type
                }).ToList();
                return View(viewModel);
            }

            var userId = User.Identity.GetUserId();
            var newItem = new Item
            {
                InventoryId = viewModel.InventoryId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var inventory = await db.Inventories.Include(i => i.CustomIdElements).FirstOrDefaultAsync(i => i.Id == viewModel.InventoryId);
            if (inventory == null) return HttpNotFound();

            for (int i = 0; i < 5; i++)
            {
                var generatedId = await _idFormatService.GenerateIdAsync(inventory, db);
                var isDuplicate = await db.Items.AnyAsync(it => it.InventoryId == viewModel.InventoryId && it.CustomId == generatedId);

                if (!isDuplicate)
                {
                    newItem.CustomId = generatedId;
                    break;
                }
            }

            if (string.IsNullOrEmpty(newItem.CustomId))
            {
                ModelState.AddModelError("", "Could not generate a unique item ID. The inventory may be very busy. Please try again.");
                var inventoryForForm = await db.Inventories.Include(i => i.CustomFields).FirstOrDefaultAsync(i => i.Id == viewModel.InventoryId);
                viewModel.InventoryTitle = inventoryForForm.Title;
                viewModel.FieldValues = inventoryForForm.CustomFields.Select(cf => new CustomFieldValueViewModel
                {
                    CustomFieldId = cf.Id,
                    FieldName = cf.Title,
                    FieldType = cf.Type
                }).ToList();
                return View(viewModel);
            }

            db.Items.Add(newItem);
            await db.SaveChangesAsync();

            foreach (var fieldValueModel in viewModel.FieldValues)
            {
                var customFieldValue = new CustomFieldValue
                {
                    ItemId = newItem.Id,
                    CustomFieldId = fieldValueModel.CustomFieldId,
                    Value = fieldValueModel.Value ?? (fieldValueModel.FieldType == FieldType.Checkbox ? "false" : "")
                };
                db.CustomFieldValues.Add(customFieldValue);
            }

            await db.SaveChangesAsync();

            return RedirectToAction("Details", "Inventory", new { id = viewModel.InventoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMultiple(IEnumerable<int> itemIds, int inventoryId)
        {
            if (itemIds != null && itemIds.Any())
            {
                var itemsToDelete = await db.Items
                    .Where(i => itemIds.Contains(i.Id) && i.InventoryId == inventoryId)
                    .ToListAsync();

                var inventory = await db.Inventories.FindAsync(inventoryId);
                var currentUserId = User.Identity.GetUserId();
                if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                db.Items.RemoveRange(itemsToDelete);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Inventory", new { id = inventoryId });
        }
    }
}