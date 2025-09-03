using Inventory_Management_iTransition.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.ViewModel
{
    public class InventoryIndexViewModel
    {
        public IEnumerable<Inventory> OwnedInventories { get; set; }
        public IEnumerable<Inventory> AccessibleInventories { get; set; }
    }

    public class InventoryDetailViewModel
    {
        public Inventory Inventory { get; set; }
    }

    public class InventoryFormViewModel
    {
        public int? Id { get; set; } 

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description (Markdown supported)")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Make Inventory Public?")]
        public bool IsPublic { get; set; }

        public string Tags { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public InventoryFormViewModel()
        {
            Id = 0;
        }
    }
}