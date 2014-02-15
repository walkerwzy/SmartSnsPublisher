using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using NLog;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Service;
using SmartSnsPublisher.Utility;
using SmartSnsPublisher.Web.Models;
using SmartSnsPublisher.Web.Repository;

namespace SmartSnsPublisher.Web.Controllers
{
    [Authorize]
    public class CbController : Controller
    {
        private readonly SiteInfoRepository repository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public CbController()
        {
            repository = new SiteInfoRepository();
        }

        // sina api callback handler
        // GET: /Cb/
        public async Task<ActionResult> Sina(string code)
        {
            if (string.IsNullOrEmpty(code.Trim()))
                return await CreateAsyncResult("illegal entrance");
            string err;
            try
            {
                var srv = new SinaService();
                var token = await srv.GetAccessTokenAsync(code);
                if (!string.IsNullOrEmpty(token.Error))
                    return await CreateAsyncResult(token.Error);

                var site = new SiteInfo
                {
                    AccessToken = token.AccessToken,
                    ExpireDate = DateTime.UtcNow.AddHours(8).AddSeconds(token.Expire),
                    SocialId = token.UserId,
                    SiteName = "sina",
                    UserId = User.Identity.GetUserId()
                };
                repository.AddConnectSite(site);
                //return JavaScript("<script>alert('authorization successfull!');window.close();</script>");
                return Redirect("/");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> update(string id = "hello world")
        {
            try
            {
                var srv = new SinaService();
                var token = repository.UserConnectedSites(User.Identity.GetUserId())
                    .Single(m => m.SiteName == "sina").AccessToken;
                var rtn = await srv.UpdateAsync(token, id);
                return Content(rtn);
            }
            catch (Exception ex)
            {
                var s = "err:" + ex.Message;
                while (ex.InnerException != null)
                {
                    s += ex.Message + "<br/>";
                    ex = ex.InnerException;
                }
                return Content(s);
            }
        }

        private async Task<ActionResult> CreateAsyncResult(string message)
        {
            return await Task.Run(() => Content("authorization failure: " + message));
        }
    }
}