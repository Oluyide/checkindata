namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Logs", "PatientHosptalNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Logs", "PatientHosptalNo", c => c.String());
        }
    }
}
