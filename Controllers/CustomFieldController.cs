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

            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomFieldViewModel fieldViewModel)
        {
            if (ModelState.IsValid)
            {
                var inventory = await db.Inventories.FindAsync(fieldViewModel.InventoryId);
                var currentUserId = User.Identity.GetUserId();

                if (inventory.OwnerId != currentUserId && !User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                var customField = new CustomField
                {
                    InventoryId = fieldViewModel.InventoryId,
                    Title = fieldViewModel.Title,
                    Type = fieldViewModel.FieldType,
                    DisplayInItemTable = fieldViewModel.ShowInItemTable
                };

                db.CustomFields.Add(customField);
                await db.SaveChangesAsync();

                return RedirectToAction("_FieldList", new { inventoryId = fieldViewModel.InventoryId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid data submitted.");
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