using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using SmartSnsPublisher.Service;
using SmartSnsPublisher.Web.Filters;
using SmartSnsPublisher.Web.Models;
using SmartSnsPublisher.Web.Repository;
using Microsoft.AspNet.Identity;
using SmartSnsPublisher.Web.Utils;
using fileOP = System.IO.File;

namespace SmartSnsPublisher.Web.Controllers
{
    [Authorize]
    public class ExecuteController : Controller
    {
        private readonly SiteInfoRepository _repository;
        private const string PicSessionkey = "_postedFileDir";
        private readonly string _tempFileDir;

        public ExecuteController()
        {
            _repository = new SiteInfoRepository();
            _tempFileDir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        }

        //
        // POST: /execute/uploadfile/
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files[0];
            if (null == file) throw new Exception("no files selected");
            //storefile to disk
            var filename = GetFileStorePath(User.Identity.GetUserName());
            file.SaveAs(filename);
            return Json("ok");
        }

        //
        // POST: /execute/deletefile
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteFile()
        {
            GetFileStorePath(User.Identity.GetUserName());
            return Json("ok");
        }

        // 
        // POST: /execute/post
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> Post(string msg, string sync)
        {
            var syncStatus = sync.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (syncStatus.Length == 0) throw new Exception("no account selected");
            var userid = User.Identity.GetUserId();
            var result = new Dictionary<string, string>();
            var userSites = await _repository.UserConnectedSites(userid).ToListAsync();

            //todo: with geoinfo

            // async post to sina
            var sina = userSites.SingleOrDefault(m => _checkSite(m.SiteName, "sina"));
            if (null != sina && syncStatus.Contains("sina"))
            {
                var sinaSrv = new SinaService();
                var sinaToken = sina.AccessToken;
                var sinaRtn = await sinaSrv.UpdateAsync(sinaToken, msg, ip: Tools.GetRealIp());
                result.Add("sina", sinaRtn);
            }

            // async post to qq
            var qq = userSites.SingleOrDefault(m => _checkSite(m.SiteName, "qq"));
            if (null != qq && syncStatus.Contains("qq"))
            {
                var qqSrv = new TencentService();
                var qqToken = qq.AccessToken;
                var qqExt = JsonConvert.DeserializeObject(qq.ExtInfo);
                var qqRtn = await qqSrv.UpdateAsync(qqToken, msg, Tools.GetRealIp(), ext: qqExt);
                result.Add("qq", qqRtn);
            }

            var twitter = userSites.SingleOrDefault(m => _checkSite(m.SiteName, "twitter"));
            if (null != twitter && syncStatus.Contains(("twitter")))
            {
                var twSrv = new TwitterService();
                var twToken = twitter.AccessToken;
                dynamic twExt = JsonConvert.DeserializeObject(twitter.ExtInfo);
                var twRtn = await twSrv.UpdateAsync(twToken, msg, ext: twExt);
                await Task.Run(() => { });
                result.Add("twitter", twRtn);
            }

            // async post to fanfou

            return Json(result);
        }

        // 
        // POST: /execute/postwithimage
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public async Task<ActionResult> PostWithImage(string msg, string sync)
        {
            var syncStatus = sync.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (syncStatus.Length == 0) throw new Exception("no account selected");
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            var filename = GetFileStorePath(username);
            var result = new Dictionary<string, string>();
            var userSites = await _repository.UserConnectedSites(userid).ToListAsync();
            byte[] buffer = await GetFileStreamFormDisk(filename);

            // async post to sina
            var sina = userSites.SingleOrDefault(m => _checkSite(m.SiteName, "sina"));
            if (null != sina || syncStatus.Contains("sina"))
            {
                var sinaSrv = new SinaService();
                var sinaToken = sina.AccessToken;
                var sinaRtn = await sinaSrv.PostAsync(sinaToken, msg, buffer, Tools.GetRealIp());
                result.Add("sina", sinaRtn);
            }

            //async post to qq
            var qq = userSites.SingleOrDefault(m => _checkSite(m.SiteName, "qq"));
            if (null != qq || syncStatus.Contains("qq"))
            {
                var qqSrv = new TencentService();
                var qqToken = qq.AccessToken;
                var qqExt = JsonConvert.DeserializeObject(qq.ExtInfo);
                var qqRtn = await qqSrv.PostAsync(qqToken, msg, buffer, Tools.GetRealIp(), ext: qqExt);
                result.Add("qq", qqRtn);
            }

            //async post to fanfou

            return Json(result);
        }

        private string GetFileStorePath(string username)
        {
            // use username as filename, thus one people got one image storage.
            //var randomName = Path.GetRandomFileName();
            //if (!Directory.Exists(_tempFileDir)) Directory.CreateDirectory(_tempFileDir);
            var fileName = Path.Combine(_tempFileDir, username + ".rb");
            // appharbor app can only write app_data directory, and can't delete file,
            //if (deleteExist && System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
            return fileName;
        }

        private async Task<byte[]> GetFileStreamFormDisk(string filename)
        {
            using (var stream = fileOP.OpenRead(filename))
            {
                var length = (int)stream.Length;
                var buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, length);
                return buffer;
            }
        }

        private static bool _checkSite(string dbname, string sitename)
        {
            return string.Compare(dbname, sitename, StringComparison.OrdinalIgnoreCase) == 0x0;
        }
    }
}