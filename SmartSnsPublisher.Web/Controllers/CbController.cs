using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly SiteInfoRepository _repository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public CbController()
        {
            _repository = new SiteInfoRepository();
        }

        // sina api callback handler
        // GET: /Cb/process/id?
        // code for oauth 2.0
        // openid, openkey for qq
        // oauth_token, oauth_verifier for twitter oauth 1.1
        public async Task<ActionResult> Process(string id, string code = "", string openid = "", string openkey = "", string oauth_token = "", string oauth_verifier = "")
        {
            var sitename = id.ToLower();
            if (id != "twitter" && string.IsNullOrEmpty(code.Trim()))
                throw new Exception("illegal entrance");
            try
            {
                IAccountFacade srv;
                string des;
                switch (sitename)
                {
                    case "sina":
                        srv = new SinaService();
                        des = "新浪微博";
                        break;
                    case "qq":
                        srv = new TencentService();
                        des = "腾讯微博";
                        break;
                    case "twitter":
                        srv = new TwitterService();
                        des = "twitter";
                        code = Request.Url.ToString();
                        break;
                    default:
                        throw new Exception("bad callback");
                }
                var token = await srv.GetAccessTokenAsync(code);
                if (!string.IsNullOrEmpty(token.Error))
                    throw new Exception(token.Error);

                var site = new SiteInfo
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpireDate = DateTime.UtcNow.AddHours(8).AddSeconds(token.Expire),
                    SocialId = token.UserId,
                    SocilaName = token.UserName,
                    SiteName = sitename,
                    UserId = User.Identity.GetUserId(),
                    Description = des
                };
                //腾讯微博需要openid和openkey
                if (sitename == "qq")
                {
                    var ext = string.Format(@"{{""openid"":""{0}"",""openkey"":""{1}""}}", openid, openkey);
                    site.ExtInfo = ext;
                }
                else if (sitename == "twitter")
                {
                    var ext = string.Format(@"{{""secret"":""{0}""}}", ((TwitterAccessToken) token).AccessTokenSecret);
                    site.ExtInfo = ext;
                }
                _repository.AddConnectSite(site);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}