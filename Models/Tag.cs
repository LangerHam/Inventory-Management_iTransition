using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class Tag
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<InventoryTag> InventoryTags { get; set; }

        public Tag()
        {
            InventoryTags = new HashSet<InventoryTag>();
        }
    }
}