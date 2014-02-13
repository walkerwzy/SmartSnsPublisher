using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartSnsPublisher.Web.Models;
using SmartSnsPublisher.Web.Repository;

namespace SmartSnsPublisher.Web.Controllers
{
    public class SiteController : ApiController
    {
        private readonly SiteInfoRepository _repository;

        public SiteController()
        {
            _repository = new SiteInfoRepository();
        }

        // get user linked sites
        // GET: /api/site/id
        [HttpGet]
        public IList<SiteInfo> UserSites(string id)
        {
            return _repository.UserConnectedSites(id).ToList();
        }
    }
}
