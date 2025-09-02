using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Inventory_Management_iTransition.Models
{
	public class ApplicationUser: IdentityUser
	{
		public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<InventoryUserAccess> AccessibleInventories { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public ApplicationUser()
        {
            Inventories = new HashSet<Inventory>();
            AccessibleInventories = new HashSet<InventoryUserAccess>();
            Likes = new HashSet<Like>();
            Comments = new HashSet<Comment>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}