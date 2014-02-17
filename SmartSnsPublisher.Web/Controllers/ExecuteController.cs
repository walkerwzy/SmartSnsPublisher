using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SmartSnsPublisher.Service;
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
        public ActionResult UploadFile()
        {
            HttpPostedFileBase file = Request.Files[0];
            if (null == file) throw new Exception("no files selected");
            //storefile to disk
            var filename = GetFileStorePath(User.Identity.GetUserName(), true);
            file.SaveAs(filename);
            return Json("ok");
        }

        //
        // POST: /execute/deletefile
        [HttpPost]
        public ActionResult DeleteFile()
        {
            GetFileStorePath(User.Identity.GetUserName(), true);
            return Json("ok");
        }

        // 
        // POST: /execute/post
        [HttpPost]
        public async Task<ActionResult> Post(string msg)
        {
            var userid = User.Identity.GetUserId();

            // async post to sina
            var sina = new SinaService();
            var token = _repository.UserConnectedSites(userid)
                .Single(m => m.SiteName == "sina").AccessToken;
            var rtn = await sina.UpdateAsync(token, msg, Tools.GetRealIp());
            var sinaStatus = new PostStatus { SiteName = "sina", Message = rtn };

            // async post to qq

            // async post to fanfou

            var col = new List<PostStatus> { sinaStatus };

            return Json(col);
        }

        // 
        // POST: /execute/postwithimage
        [HttpPost]
        public async Task<ActionResult> PostWithImage(string msg)
        {
            var userid = User.Identity.GetUserId();
            var username = User.Identity.GetUserName();
            var filename = GetFileStorePath(username);
            try
            {
                byte[] buffer = await GetFileStreamFormDisk(filename);

                // async post to sina
                var sina = new SinaService();
                var token = _repository.UserConnectedSites(userid).Single(m => m.SiteName == "sina").AccessToken;
                var rtn = await sina.PostAsync(token, msg, buffer, Tools.GetRealIp());
                var sinaStatus = new PostStatus { SiteName = "sina", Message = rtn };

                 //async post to qq

                 //async post to fanfou

                var col = new List<PostStatus> { sinaStatus };

                return Json(col);
            }
            finally
            {
                // delete used files
                fileOP.Delete(filename);
            }
        }

        private string GetFileStorePath(string username, bool deleteExist = false)
        {
            // use username as filename, thus one people got one image storage.
            //var randomName = Path.GetRandomFileName();
            if (!Directory.Exists(_tempFileDir)) Directory.CreateDirectory(_tempFileDir);
            var fileName = Path.Combine(_tempFileDir, username + ".rb");
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
    }
}