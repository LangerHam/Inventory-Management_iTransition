using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class Like
	{
        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public int ItemId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Item Item { get; set; }
    }
}