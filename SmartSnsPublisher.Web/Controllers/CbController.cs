using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartSnsPublisher.Service;
using SmartSnsPublisher.Web.Models;
using SmartSnsPublisher.Web.Repository;

namespace SmartSnsPublisher.Web.Controllers
{
    [Authorize]
    public class CbController : Controller
    {
        private readonly SiteInfoRepository repository;

        public CbController()
        {
            repository = new SiteInfoRepository();
        }

        // sina api callback handler
        // GET: /Cb/
        public async Task<ActionResult> Sina(string code = "", string access_token = "", int expires_in = -1, string uid = "")
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(access_token))
            {
                await Task.Run(() => { });
                return Content("illegal entrance");
            }

            var srv = new SinaService();
            //if (string.IsNullOrEmpty(access_token))
            //{
            //    //return Redirect(srv.GetAccessTokenUrl(code));
            //    SmartSnsPublisher.Utility.HelperWebRequest.DoPost(srv.GetAccessTokenUrl(code), null);
            //}
            //else if (expires_in == -1 || string.IsNullOrEmpty(uid)) return Content("invalid request");
            //var site = new SiteInfo
            //{
            //    AccessToken = access_token,
            //    ExpireDate = DateTime.UtcNow.AddHours(8).AddSeconds(expires_in),
            //    SocialId = uid,
            //    SiteName = "sina",
            //    UserId = User.Identity.GetUserId()
            //};
            //repository.AddConnectSite(site);
            //return JavaScript("alert('authorization successfull!');window.close();");

            if (!string.IsNullOrEmpty(code))
            {
                string s = await srv.GetAccessTokenUrl(code);
                return Content(s);

            }
            else
            {
                await Task.Run(() => { });
                return Content("illegal entrance");
            }
        }
    }
}