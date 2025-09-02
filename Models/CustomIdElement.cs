using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.Models
{
    public enum IdElementType
    {
        FixedText,
        Random20Bit,
        Random32Bit,
        Random6Digit,
        Random9Digit,
        Guid,
        DateTime,
        Sequence
    }
    public class CustomIdElement
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; }

        [Required]
        public IdElementType Type { get; set; }

        public string ValueOrFormat { get; set; }

        [Required]
        public int Order { get; set; }
    }
}