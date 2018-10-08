namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacilityIdentities", "ProviderEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacilityIdentities", "ProviderEmail");
        }
    }
}
