using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Web.Models
{
    public class SiteDbContext : DbContext
    {
        public SiteDbContext()
            : base("DefaultConnection")
        {

        }

        public DbSet<SiteInfo> Sites { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SiteDbContext, Migrations.Configuration>());
            base.OnModelCreating(modelBuilder);
        }
    }


    public class SiteInfo : Entity
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        [JsonProperty("sitename")]
        public string SiteName { get; set; }
        [JsonProperty("socialid")]
        public string SocialId { get; set; }
        [JsonProperty("socialname")]
        public string SocilaName { get; set; }
        [JsonProperty("des")]
        public string Description { get; set; }
        [JsonProperty("token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh")]
        public string RefreshToken { get; set; }
        [JsonProperty("expires")]
        public DateTime ExpireDate { get; set; }
    }
}