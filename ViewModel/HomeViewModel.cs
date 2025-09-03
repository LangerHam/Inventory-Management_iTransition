using Inventory_Management_iTransition.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.ViewModel
{
	public class HomeViewModel
	{
        public IEnumerable<Inventory> LatestInventories { get; set; }
        public IEnumerable<Inventory> PopularInventories { get; set; }
        public IEnumerable<Tag> TagCloud { get; set; }
    }
}