namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "PatientHosptalNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "PatientHosptalNo");
        }
    }
}
