using Inventory_Management_iTransition.Context;
using Inventory_Management_iTransition.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Inventory_Management_iTransition.Controllers
{
    public class HomeController : Controller
    {
        private InMContext db;

        public HomeController()
        {
            db = new InMContext();
        }

        public async Task<ActionResult> Index()
        {
            var latestInventories = await db.Inventories
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .Include(i => i.Owner) 
                .ToListAsync();

            var popularInventories = await db.Inventories
                .OrderByDescending(i => i.Items.Count)
                .Take(5)
                .Include(i => i.Owner)
                .ToListAsync();

            var tags = await db.Tags.ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestInventories = latestInventories,
                PopularInventories = popularInventories,
                TagCloud = tags
            };

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";

            return View();
        }
    }
}
