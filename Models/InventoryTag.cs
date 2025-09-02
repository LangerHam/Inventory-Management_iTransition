using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class InventoryTag
	{
        [Key, Column(Order = 0)]
        public int InventoryId { get; set; }

        [Key, Column(Order = 1)]
        public int TagId { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual Tag Tag { get; set; }
    }
}