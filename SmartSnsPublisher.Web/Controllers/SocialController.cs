using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SmartSnsPublisher.Service;
using SmartSnsPublisher.Web.Repository;

namespace SmartSnsPublisher.Web.Controllers
{
    public class SocialController : Controller
    {
        // Gen get oauth2 code url, and then redirect to it
        // GET: /Social/getcode/id
        public ActionResult Getcode(string id)
        {
            var url = "";
            id = id.ToLower();
            switch (id)
            {
                case "sina":
                    var sinasrv = new SinaService();
                    url = sinasrv.GetAuthorizationUrl();
                    break;
                default:
                    break;
            }
            return Redirect(url);
        }
	}
}