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
        private readonly IdFormatService _idFormatService;

        public ItemController()
        {
            db = new InMContext();
            _idFormatService = new IdFormatService();
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

            var viewModel = new ItemFormViewModel
            {
                InventoryId = inventory.Id,
                InventoryTitle = inventory.Title,
                FieldValues = inventory.CustomFields.Select(cf => new CustomFieldValueViewModel
                {
                    CustomFieldId = cf.Id,
                    FieldName = cf.Title,
                    FieldType = cf.Type
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

            var inventory = await db.Inventories.Include(i => i.CustomIdElements).FirstOrDefaultAsync(i => i.Id == viewModel.InventoryId);
            if (inventory == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            if (inventory.OwnerId != userId && !User.IsInRole("Admin") &&
                !await db.InventoryUserAccesses.AnyAsync(a => a.InventoryId == viewModel.InventoryId && a.UserId == userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You don't have permission to add items to this inventory.");
            }

            var newItem = new Item
            {
                InventoryId = viewModel.InventoryId,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (inventory.CustomIdElements.Any(e => e.Type == IdElementType.Sequence))
                    {
                        var maxSeq = await db.Items
                            .Where(i => i.InventoryId == inventory.Id)
                            .Select(i => (int?)i.SequenceNumber)
                            .DefaultIfEmpty(0)
                            .MaxAsync();
                        newItem.SequenceNumber = (maxSeq ?? 0) + 1;
                    }

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
                        ModelState.AddModelError("", "Could not generate a unique item ID. Please try again.");
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

                    if (viewModel.FieldValues != null)
                    {
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
                    }

                    transaction.Commit();
                    return RedirectToAction("Details", "Inventory", new { id = viewModel.InventoryId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "An error occurred while saving the item: " + ex.Message);
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
            }
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