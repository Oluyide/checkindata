namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "PatientHospitalNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "PatientHospitalNo");
        }
    }
}
