using Inventory_Management_iTransition.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.ViewModel
{
    public class CustomFieldManagementViewModel
    {
        public int InventoryId { get; set; }
        public List<CustomField> Fields { get; set; }
        public CustomFieldViewModel NewField { get; set; } 
    }

    public class CustomFieldViewModel
    {
        public int InventoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public FieldType FieldType { get; set; }

        [Display(Name = "Show in Item Table?")]
        public bool ShowInItemTable { get; set; }
    }
}