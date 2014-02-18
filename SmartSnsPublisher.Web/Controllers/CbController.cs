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
        // GET: /Cb/process/id?&code=
        public async Task<ActionResult> Process(string id, string code)
        {
            if (string.IsNullOrEmpty(code.Trim()))
                throw new Exception("illegal entrance");
            try
            {
                IAccountFacade srv;
                string sitename = id.ToLower();
                switch (sitename)
                {
                    case "sina":
                        srv = new SinaService();
                        break;
                    case "qq":
                        srv=new TencentService();
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
                    ExpireDate = DateTime.UtcNow.AddHours(8).AddSeconds(token.Expire),
                    SocialId = token.UserId,
                    SiteName = sitename,
                    UserId = User.Identity.GetUserId()
                };
                _repository.AddConnectSite(site);
                //return JavaScript("<script>alert('authorization successfull!');window.close();</script>");
                return Redirect("/");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //for test
        //public async Task<ActionResult> Update(string id = "hello world")
        //{
        //    try
        //    {
        //        var srv = new SinaService();
        //        var token = _repository.UserConnectedSites(User.Identity.GetUserId())
        //            .Single(m => m.SiteName == "sina").AccessToken;
        //        var rtn = await srv.UpdateAsync(token, id);
        //        return Content(rtn);
        //    }
        //    catch (Exception ex)
        //    {
        //        var s = "err:" + ex.Message;
        //        while (ex.InnerException != null)
        //        {
        //            s += ex.Message + "<br/>";
        //            ex = ex.InnerException;
        //        }
        //        return Content(s);
        //    }
        //}

        //public async Task<ActionResult> Upload(string id)
        //{
        //    try
        //    {
        //        id = "upload image test: " + DateTime.Now.Ticks.ToString("x");
        //        var srv = new SinaService();
        //        var imgurl = @"c:\users\walker\desktop\test.jpg";
        //        //using (var img=Image.FromFile(imgurl))
        //        //{
        //        //    var ba = new ImageConverter().ConvertTo(img, typeof (byte[]));
        //        //}
        //        byte[] ba;
        //        using (var stream = System.IO.File.OpenRead(imgurl))
        //        {
        //            var fileLength = (int)stream.Length;
        //            ba = new byte[fileLength];
        //            stream.Read(ba, 0, fileLength);
        //        }
        //        var token = _repository.UserConnectedSites(User.Identity.GetUserId())
        //            .Single(m => m.SiteName == "sina").AccessToken;
        //        var rtn = await srv.PostAsync(token, id, ba);
        //        return Content(rtn);

        //    }
        //    catch (Exception ex)
        //    {
        //        var s = "err:" + ex.Message;
        //        while (ex.InnerException != null)
        //        {
        //            s += ex.Message + "<br/>";
        //            ex = ex.InnerException;
        //        }
        //        return Content(s);
        //    }
        //}

        //private async Task<ActionResult> CreateAsyncResult(string message)
        //{
        //    return await Task.Run(() => Content("authorization failure: " + message));
        //}
    }
}