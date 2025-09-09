using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class Item
	{
		[Key]
		public int Id { get; set; }
        [Required]
        [StringLength(255)]
        [Index("IX_Inventory_CustomId", 2, IsUnique = true)]
        public string CustomId { get; set; }

        [Required]
        [Index("IX_Inventory_CustomId", 1, IsUnique = true)]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; }

        [Required]
        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public int? SequenceNumber { get; set; }

        public Item()
        {
            CustomFieldValues = new HashSet<CustomFieldValue>();
            Likes = new HashSet<Like>();
        }
    }
}