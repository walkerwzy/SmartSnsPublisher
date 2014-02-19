namespace SmartSnsPublisher.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnExtinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteInfoes", "ExtInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteInfoes", "ExtInfo");
        }
    }
}
