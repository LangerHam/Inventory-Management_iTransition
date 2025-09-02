using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
	public class CustomFieldValue
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [Required]
        public int CustomFieldId { get; set; }

        [ForeignKey("CustomFieldId")]
        public virtual CustomField CustomField { get; set; }

        public string Value { get; set; }
    }
}