using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Inventory_Management_iTransition.Models;

namespace Inventory_Management_iTransition.ViewModel
{
    public class ItemFormViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; }

        public List<CustomFieldValueViewModel> FieldValues { get; set; }
    }
    public class CustomFieldValueViewModel
    {
        public int CustomFieldId { get; set; }
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }

        [Display(Name = "Value")]
        public string Value { get; set; }
    }
}