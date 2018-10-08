namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV7 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Logs", "User");
            DropColumn("dbo.Logs", "ModuleName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Logs", "ModuleName", c => c.String());
            AddColumn("dbo.Logs", "User", c => c.String());
        }
    }
}
