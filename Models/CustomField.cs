using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
    public enum FieldType
    {
        SingleLineText,
        MultiLineText,
        Numeric,
        DocumentLink,
        Checkbox
    }
    public class CustomField
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; } 

        [Required]
        public FieldType Type { get; set; }

        public bool DisplayInItemTable { get; set; }

        public int Order { get; set; }

        public virtual ICollection<CustomFieldValue> Values { get; set; }

        public CustomField()
        {
            Values = new HashSet<CustomFieldValue>();
        }
    }
}