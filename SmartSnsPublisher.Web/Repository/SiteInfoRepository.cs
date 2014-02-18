using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartSnsPublisher.Web.Models;

namespace SmartSnsPublisher.Web.Repository
{
    public class SiteInfoRepository
    {
        private readonly SiteDbContext _context;

        public SiteInfoRepository()
        {
            _context = new SiteDbContext();
        }

        /// <summary>
        /// Retrive user's connected site list
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<SiteInfo> UserConnectedSites(string userid)
        {
            return _context.Sites.Where(m => m.UserId == userid);
        }

        /// <summary>
        /// Add a new site
        /// </summary>
        /// <param name="info"></param>
        public void AddConnectSite(SiteInfo info)
        {
            //unique userid
            var old = _context.Sites.Where(m => m.UserId == info.UserId && string.Compare(m.SiteName, info.SiteName, StringComparison.OrdinalIgnoreCase) == 0);
            if (old.Any()) _context.Sites.Remove(old.First());
            _context.Sites.Add(info);
            _context.SaveChanges();
        }


        public void RemoveConnectSite(int siteid)
        {
            _context.Sites.Remove(_context.Sites.Single(m => m.Id == siteid));
            _context.SaveChanges();
        }
    }
}