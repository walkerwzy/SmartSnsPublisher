using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SmartSnsPublisher.Web.Models
{
    public class SiteDbContext : DbContext
    {
        public SiteDbContext()
            : base("DefaultConnection")
        {
            
        }

        public DbSet<SiteInfo> Sites { get; set; }
    }


    public class SiteInfo : Entity
    {
        public string UserId { get; set; }
        public string SiteName { get; set; }
        public string Description { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}