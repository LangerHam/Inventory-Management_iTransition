using Inventory_Management_iTransition.Context;
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
using Inventory_Management_iTransition.Models;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class CustomFieldController : Controller
    {
        private InMContext db;

        public CustomFieldController()
        {
            db = new InMContext();
        }

        public async Task<ActionResult> _FieldList(int inventoryId)
        {
            var inventory = await db.Inventories
                .Include(i => i.CustomFields)
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

            var viewModel = new CustomFieldManagementViewModel
            {
                InventoryId = inventoryId,
                Fields = inventory.CustomFields.ToList()
            };

            return PartialView("_FieldList", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomFieldManagementViewModel viewModel)
        {
            var fieldViewModel = viewModel.NewField;

            if (ModelState.IsValid)
            {
                var inventory = await db.Inventories.FindAsync(viewModel.InventoryId);
                if (inventory == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Inventory not found.");
                }

                var currentUserId = User.Identity.GetUserId();
                if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                var existingFieldsCount = await db.CustomFields
                    .CountAsync(cf => cf.InventoryId == viewModel.InventoryId && cf.Type == fieldViewModel.FieldType);

                if (existingFieldsCount >= 3)
                {
                    ModelState.AddModelError("NewField.Title", $"You cannot add more than 3 fields of type '{fieldViewModel.FieldType}'.");
                    return await _FieldList(viewModel.InventoryId);
                }

                var newField = new CustomField
                {
                    InventoryId = viewModel.InventoryId,
                    Title = fieldViewModel.Title,
                    Type = fieldViewModel.FieldType,
                    DisplayInItemTable = fieldViewModel.ShowInItemTable
                };

                db.CustomFields.Add(newField);
                await db.SaveChangesAsync();
            }

            return await _FieldList(viewModel.InventoryId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var customField = await db.CustomFields.Include(cf => cf.Inventory).FirstOrDefaultAsync(cf => cf.Id == id);
            if (customField == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            if (customField.Inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            int inventoryId = customField.InventoryId;
            db.CustomFields.Remove(customField);
            await db.SaveChangesAsync();

            return RedirectToAction("_FieldList", new { inventoryId = inventoryId });
        }
    }
}