using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartSnsPublisher.Service;
using SmartSnsPublisher.UI.Repository;

namespace SmartSnsPublisher.Web.Controllers
{
    public class SocialController : Controller
    {
        private IAccountFacade _srv;

        // Gen get oauth2 code url, and then redirect to it
        // GET: /Social/getcode/id
        public ActionResult Getcode(string id)
        {
            id = id.ToLower();
            switch (id)
            {
                case "sina":
                    _srv=new SinaService();
                    break;
                case "qq":
                    _srv = new TencentService();
                    break;
                case "twitter":
                    _srv=new TwitterService();
                    break;
                default:
                    throw new Exception("illage call");
            }
            return Redirect(_srv.GetAuthorizationUrl());
        }
	}
}