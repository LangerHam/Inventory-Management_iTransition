namespace Inventory_Management_iTransition.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemsChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "SequenceNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "SequenceNumber");
        }
    }
}
