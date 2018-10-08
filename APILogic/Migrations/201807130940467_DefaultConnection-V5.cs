namespace APILogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnectionV5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacilityIdentities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProviderId = c.Int(nullable: false),
                        Key = c.Int(nullable: false),
                        ProviderName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FacilityIdentities");
        }
    }
}
