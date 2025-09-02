namespace Inventory_Management_iTransition.Context
{
    using Inventory_Management_iTransition.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class InMContext : IdentityDbContext<ApplicationUser>
    {
        // Your context has been configured to use a 'InMContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Inventory_Management_iTransition.Context.InMContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'InMContext' 
        // connection string in the application configuration file.
        public InMContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<InventoryTag> InventoryTags { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }
        public DbSet<CustomIdElement> CustomIdElements { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<InventoryUserAccess> InventoryUserAccesses { get; set; }

        public static InMContext Create()
        {
            return new InMContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryTag>()
                .HasKey(it => new { it.InventoryId, it.TagId });

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.ItemId });

            modelBuilder.Entity<InventoryUserAccess>()
                .HasKey(iua => new { iua.InventoryId, iua.UserId });

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Inventories)
                .WithRequired(i => i.Owner)
                .HasForeignKey(i => i.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.AccessibleInventories)
                .WithRequired(iua => iua.User)
                .HasForeignKey(iua => iua.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Item>()
                .HasRequired(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .WillCascadeOnDelete(false);
        }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}


}