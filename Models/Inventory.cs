using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class Inventory
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsPublic { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<CustomField> CustomFields { get; set; }
        public virtual ICollection<CustomIdElement> CustomIdElements { get; set; }
        public virtual ICollection<InventoryTag> InventoryTags { get; set; }
        public virtual ICollection<InventoryUserAccess> WriteAccessUsers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public Inventory()
        {
            Items = new HashSet<Item>();
            CustomFields = new HashSet<CustomField>();
            CustomIdElements = new HashSet<CustomIdElement>();
            InventoryTags = new HashSet<InventoryTag>();
            WriteAccessUsers = new HashSet<InventoryUserAccess>();
            Comments = new HashSet<Comment>();
        }
    }
}