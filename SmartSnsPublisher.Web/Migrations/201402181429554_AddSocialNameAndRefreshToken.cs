namespace SmartSnsPublisher.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSocialNameAndRefreshToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteInfoes", "SocilaName", c => c.String());
            AddColumn("dbo.SiteInfoes", "RefreshToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SiteInfoes", "RefreshToken");
            DropColumn("dbo.SiteInfoes", "SocilaName");
        }
    }
}
