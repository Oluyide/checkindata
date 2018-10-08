namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "FirstName", c => c.String());
            DropColumn("dbo.Logs", "FirsName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Logs", "FirsName", c => c.String());
            DropColumn("dbo.Logs", "FirstName");
        }
    }
}
