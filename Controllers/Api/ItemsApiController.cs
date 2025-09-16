using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace Inventory_Management_iTransition.Controllers.Api
{
    public class CreateItemApiDto
    {
        public int InventoryId { get; set; }
        public string CustomId { get; set; }
    }
    [RoutePrefix("api/items")]
    public class ItemsApiController : ApiController
    {
        private InMContext db;
        public ItemsApiController()
        {
            db = new InMContext();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateItem(CreateItemApiDto itemDto)
        {
            var apiKey = Request.Headers.FirstOrDefault(h => h.Key.Equals("X-API-KEY", StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault();
            var validApiKey = ConfigurationManager.AppSettings["PowerAutomateApiKey"];

            if (string.IsNullOrEmpty(validApiKey) || apiKey != validApiKey)
            {
                return Unauthorized();
            }

            // --- Main Logic ---
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventory = await db.Inventories.FindAsync(itemDto.InventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            var isDuplicate = await db.Items.AnyAsync(i => i.InventoryId == itemDto.InventoryId && i.CustomId == itemDto.CustomId);
            if (isDuplicate)
            {
                return Conflict();
            }

            var newItem = new Item
            {
                InventoryId = itemDto.InventoryId,
                CustomId = itemDto.CustomId,
                CreatedById = inventory.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.Items.Add(newItem);
            await db.SaveChangesAsync();

            return Ok(new { Message = "Item created successfully.", ItemId = newItem.Id });
        }
    }
}
