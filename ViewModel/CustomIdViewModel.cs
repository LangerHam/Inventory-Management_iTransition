using Inventory_Management_iTransition.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.ViewModel
{
    public class CustomIdManagementViewModel
    {
        public int InventoryId { get; set; }
        public List<CustomIdElement> Elements { get; set; }
        public CustomIdElementViewModel NewElement { get; set; }
    }

    public class CustomIdElementViewModel
    {
        public int InventoryId { get; set; }

        [Required]
        [Display(Name = "Element Type")]
        public IdElementType ElementType { get; set; }

        [Display(Name = "Value / Format (optional)")]
        [StringLength(100)]
        public string Value { get; set; }
    }
}