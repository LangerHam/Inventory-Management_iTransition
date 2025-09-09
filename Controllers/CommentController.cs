using Inventory_Management_iTransition.Hubs;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Inventory_Management_iTransition.Controllers
{
    public class CommentController : Controller
    {
        private InMContext db;

        public CommentController()
        {
            db = new InMContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int inventoryId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return new HttpStatusCodeResult(400, "Message cannot be empty.");
            }

            var userId = User.Identity.GetUserId();
            var comment = new Comment
            {
                InventoryId = inventoryId,
                AuthorId = userId,
                Text = message,
                CreatedAt = DateTime.UtcNow
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<CommentHub>();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            hubContext.Clients.Group(inventoryId.ToString()).addNewMessageToPage(
                user.UserName,
                message,
                comment.CreatedAt.ToString("g")
            );

            return new HttpStatusCodeResult(200); 
        }
    }
}