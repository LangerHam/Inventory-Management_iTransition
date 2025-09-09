using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.Models;
using Inventory_Management_iTransition.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory_Management_iTransition.Controllers
{
    [Authorize]
    public class LikeController : Controller
    {
        private InMContext db;

        public LikeController()
        {
            db = new InMContext();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Toggle(int itemId)
        {
            var userId = User.Identity.GetUserId();
            var existingLike = await db.Likes
                .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == userId);

            if (existingLike != null)
            {
                db.Likes.Remove(existingLike);
            }
            else
            {
                var newLike = new Like
                {
                    ItemId = itemId,
                    UserId = userId
                };
                db.Likes.Add(newLike);
            }

            await db.SaveChangesAsync();

            var likeCount = await db.Likes.CountAsync(l => l.ItemId == itemId);

            return Json(new LikeViewModel { NewLikeCount = likeCount });
        }
    }
}