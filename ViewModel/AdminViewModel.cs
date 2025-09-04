using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory_Management_iTransition.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
    }
}