namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "LastName", c => c.String());
            AddColumn("dbo.Logs", "FirsName", c => c.String());
            AddColumn("dbo.Logs", "OtherNames", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "OtherNames");
            DropColumn("dbo.Logs", "FirsName");
            DropColumn("dbo.Logs", "LastName");
        }
    }
}
