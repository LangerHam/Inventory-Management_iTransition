using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Management_iTransition.Models
{
	public class InventoryUserAccess
	{
        [Key, Column(Order = 0)]
        public int InventoryId { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
		public virtual Inventory Inventory { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}