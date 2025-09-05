namespace Inventory_Management_iTransition.Migrations
{
    using Inventory_Management_iTransition.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<Inventory_Management_iTransition.Context.InMContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Inventory_Management_iTransition.Context.InMContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            // --- Create Roles if they don't exist ---
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new IdentityRole("User"));
            }

            // --- Create Users if they don't exist ---
            var adminUser = userManager.FindByEmail("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                userManager.Create(adminUser, "Password123!");
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            var regularUser = userManager.FindByEmail("user@example.com");
            if (regularUser == null)
            {
                regularUser = new ApplicationUser { UserName = "user@example.com", Email = "user@example.com" };
                userManager.Create(regularUser, "Password123!");
                userManager.AddToRole(regularUser.Id, "User");
            }

            context.SaveChanges(); // Save users to get their Ids

            // --- Seed Categories ---
            var categories = new List<Category>
            {
                new Category { Name = "Office Equipment" },
                new Category { Name = "Library Books" },
                new Category { Name = "HR Documents" },
                new Category { Name = "IT Assets" },
                new Category { Name = "Furniture" }
            };
            categories.ForEach(c => context.Categories.AddOrUpdate(p => p.Name, c));
            context.SaveChanges();

            // --- Seed Tags ---
            var tags = new List<Tag>
            {
                new Tag { Name = "electronics" },
                new Tag { Name = "laptop" },
                new Tag { Name = "documentation" },
                new Tag { Name = "fiction" },
                new Tag { Name = "non-fiction" },
                new Tag { Name = "desk" },
                new Tag { Name = "chair" }
            };
            tags.ForEach(t => context.Tags.AddOrUpdate(p => p.Name, t));
            context.SaveChanges();

            // --- Seed Inventories ---
            var officeInventory = new Inventory
            {
                Title = "Head Office Laptops",
                Description = "A complete inventory of all laptops assigned to employees at the head office.",
                CategoryId = context.Categories.First(c => c.Name == "IT Assets").Id,
                OwnerId = adminUser.Id,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var libraryInventory = new Inventory
            {
                Title = "Fiction Section - First Floor",
                Description = "All fiction books available on the first floor of the central library.",
                CategoryId = context.Categories.First(c => c.Name == "Library Books").Id,
                OwnerId = regularUser.Id,
                IsPublic = false,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            context.Inventories.AddOrUpdate(i => i.Title, officeInventory, libraryInventory);
            context.SaveChanges();

            // --- Add Tags to Inventories ---
            officeInventory.InventoryTags.Add(new InventoryTag { TagId = tags.First(t => t.Name == "electronics").Id });
            officeInventory.InventoryTags.Add(new InventoryTag { TagId = tags.First(t => t.Name == "laptop").Id });
            libraryInventory.InventoryTags.Add(new InventoryTag { TagId = tags.First(t => t.Name == "fiction").Id });
            context.SaveChanges();

            // --- Seed Custom Fields for Office Inventory ---
            var laptopModelField = new CustomField { InventoryId = officeInventory.Id, Title = "Model Name", Type = FieldType.SingleLineText, Description = "e.g., Dell XPS 15", DisplayInItemTable = true };
            var serialNumberField = new CustomField { InventoryId = officeInventory.Id, Title = "Serial Number", Type = FieldType.SingleLineText, Description = "The unique serial number", DisplayInItemTable = false };
            var purchasePriceField = new CustomField { InventoryId = officeInventory.Id, Title = "Purchase Price", Type = FieldType.Numeric, Description = "Price in USD", DisplayInItemTable = true };
            var underWarrantyField = new CustomField { InventoryId = officeInventory.Id, Title = "Under Warranty", Type = FieldType.Checkbox, Description = "Is the device still under warranty?", DisplayInItemTable = false };
            context.CustomFields.AddOrUpdate(cf => cf.Title, laptopModelField, serialNumberField, purchasePriceField, underWarrantyField);
            context.SaveChanges();

            // --- Seed Items for Office Inventory ---
            var item1 = new Item { InventoryId = officeInventory.Id, CreatedById = adminUser.Id, CustomId = "LAP-2025-001" };
            var item2 = new Item { InventoryId = officeInventory.Id, CreatedById = regularUser.Id, CustomId = "LAP-2025-002" };
            context.Items.AddOrUpdate(i => i.CustomId, item1, item2);
            context.SaveChanges();

            // --- Seed Custom Field Values for Items ---
            context.CustomFieldValues.AddOrUpdate(cfv => new { cfv.ItemId, cfv.CustomFieldId },
                new CustomFieldValue { ItemId = item1.Id, CustomFieldId = laptopModelField.Id, Value = "Dell XPS 15 9530" },
                new CustomFieldValue { ItemId = item1.Id, CustomFieldId = serialNumberField.Id, Value = "SN-ABC-123" },
                new CustomFieldValue { ItemId = item1.Id, CustomFieldId = purchasePriceField.Id, Value = "2100" },
                new CustomFieldValue { ItemId = item1.Id, CustomFieldId = underWarrantyField.Id, Value = "true" },
                new CustomFieldValue { ItemId = item2.Id, CustomFieldId = laptopModelField.Id, Value = "MacBook Pro 16" },
                new CustomFieldValue { ItemId = item2.Id, CustomFieldId = serialNumberField.Id, Value = "SN-DEF-456" },
                new CustomFieldValue { ItemId = item2.Id, CustomFieldId = purchasePriceField.Id, Value = "2500" },
                new CustomFieldValue { ItemId = item2.Id, CustomFieldId = underWarrantyField.Id, Value = "false" }
            );
            context.SaveChanges();

            // --- Seed Likes ---
            context.Likes.AddOrUpdate(l => new { l.UserId, l.ItemId },
                new Like { UserId = regularUser.Id, ItemId = item1.Id },
                new Like { UserId = adminUser.Id, ItemId = item2.Id }
            );
            context.SaveChanges();
        }
    }
}
