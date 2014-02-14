using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NLog;
using SmartSnsPublisher.Service;
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
                if (string.IsNullOrEmpty(token.Error))
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
                return JavaScript("alert('authorization successfull!');window.close();");
            }
            catch (Exception ex)
            {
                err = ex.Message;
            }
            return await CreateAsyncResult(err);
        }

        public ActionResult Test(string id)
        {
            logger.Debug("Debugging Message");
            logger.Info("Info message");
            logger.Warn("Warning Message");
            return Content(id);
        }

        private async Task<ActionResult> CreateAsyncResult(string message)
        {
            return await Task.Run(() => Content("authorization failure: " + message));
        }


        //public async Task<ActionResult> Sina(string code = "", string access_token = "", int expires_in = -1, string uid = "")
        //{
        //    if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(access_token))
        //        return await Task.Run(() => Content("illegal entrance"));

        //    var srv = new SinaService();

        //    if (string.IsNullOrEmpty(code)) return await Task.Run(() => Content("illegal entrance"));
        //    string s = await srv.GetAccessToken(code);
        //    return Content(s);
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
        //}
    }
}