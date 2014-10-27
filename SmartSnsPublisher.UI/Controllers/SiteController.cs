using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using SmartSnsPublisher.UI.Filters;
using SmartSnsPublisher.UI.Models;
using SmartSnsPublisher.UI.Repository;

namespace SmartSnsPublisher.UI.Controllers
{
    public class SiteController : ApiController
    {
        private readonly SiteInfoRepository _repository;

        public SiteController()
        {
            _repository = new SiteInfoRepository();
        }

        // get user linked sites
        // GET: /api/site
        [Authorize]
        [HttpGet]
        public async Task<IList<SiteInfo>> UserSites()
        {
            return await _repository.UserConnectedSites(User.Identity.GetUserId()).ToListAsync();
        }

        // unlink user site
        // Delete: /api/site/id
        [HttpDelete]
        [Authorize]
        [MyValidateAntiForgeryToken]
        public string DeleteSite(int id)
        {
            _repository.RemoveConnectSite(id);
            return "ok";
        }
    }
}
