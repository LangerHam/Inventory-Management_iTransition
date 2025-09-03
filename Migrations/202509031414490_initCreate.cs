namespace Inventory_Management_iTransition.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        IsPublic = c.Boolean(nullable: false),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        AuthorId = c.String(nullable: false, maxLength: 128),
                        InventoryId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .Index(t => t.AuthorId)
                .Index(t => t.InventoryId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.InventoryUserAccesses",
                c => new
                    {
                        InventoryId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.InventoryId, t.UserId })
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.InventoryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Likes",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ItemId })
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomId = c.String(nullable: false, maxLength: 255),
                        InventoryId = c.Int(nullable: false),
                        CreatedById = c.String(nullable: false, maxLength: 128),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .Index(t => new { t.InventoryId, t.CustomId }, unique: true, name: "IX_Inventory_CustomId")
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.CustomFieldValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        CustomFieldId = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomFields", t => t.CustomFieldId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.CustomFieldId);
            
            CreateTable(
                "dbo.CustomFields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InventoryId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        DisplayInItemTable = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inventories", t => t.InventoryId)
                .Index(t => t.InventoryId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CustomIdElements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InventoryId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        ValueOrFormat = c.String(),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .Index(t => t.InventoryId);
            
            CreateTable(
                "dbo.InventoryTags",
                c => new
                    {
                        InventoryId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.InventoryId, t.TagId })
                .ForeignKey("dbo.Inventories", t => t.InventoryId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.InventoryId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.InventoryTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.InventoryTags", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.CustomIdElements", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.CustomFields", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.Comments", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Likes", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Items", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.CustomFieldValues", "ItemId", "dbo.Items");
            DropForeignKey("dbo.CustomFieldValues", "CustomFieldId", "dbo.CustomFields");
            DropForeignKey("dbo.Items", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Inventories", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InventoryUserAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.InventoryUserAccesses", "InventoryId", "dbo.Inventories");
            DropForeignKey("dbo.Inventories", "CategoryId", "dbo.Categories");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Tags", new[] { "Name" });
            DropIndex("dbo.InventoryTags", new[] { "TagId" });
            DropIndex("dbo.InventoryTags", new[] { "InventoryId" });
            DropIndex("dbo.CustomIdElements", new[] { "InventoryId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.CustomFields", new[] { "InventoryId" });
            DropIndex("dbo.CustomFieldValues", new[] { "CustomFieldId" });
            DropIndex("dbo.CustomFieldValues", new[] { "ItemId" });
            DropIndex("dbo.Items", new[] { "CreatedById" });
            DropIndex("dbo.Items", "IX_Inventory_CustomId");
            DropIndex("dbo.Likes", new[] { "ItemId" });
            DropIndex("dbo.Likes", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.InventoryUserAccesses", new[] { "UserId" });
            DropIndex("dbo.InventoryUserAccesses", new[] { "InventoryId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Comments", new[] { "InventoryId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Inventories", new[] { "CategoryId" });
            DropIndex("dbo.Inventories", new[] { "OwnerId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Tags");
            DropTable("dbo.InventoryTags");
            DropTable("dbo.CustomIdElements");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.CustomFields");
            DropTable("dbo.CustomFieldValues");
            DropTable("dbo.Items");
            DropTable("dbo.Likes");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.InventoryUserAccesses");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Comments");
            DropTable("dbo.Inventories");
            DropTable("dbo.Categories");
        }
    }
}
