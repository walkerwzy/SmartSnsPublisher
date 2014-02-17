using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    [System.Web.Mvc.Authorize]
    public class ExecuteController : Controller
    {
        private readonly SiteInfoRepository _repository;
        private const string PicSessionkey = "_postedFile";
        private const string TempFileDir = "~/Upload";

        public ExecuteController()
        {
            _repository = new SiteInfoRepository();
        }

        //
        // POST: /execute/uploadfile/
        [HttpPost]
        public ActionResult UploadFile()
        {
            DeleteSessionFile();
            HttpPostedFileBase file = Request.Files[0];
            if (null == file) throw new Exception("no files selected");
            //storefile to disk
            var filename = getFileStorePath();
            file.SaveAs(filename);
            Session[PicSessionkey] = filename;
            return Json("ok");
        }

        //
        // POST: /execute/deletefile
        [HttpPost]
        public ActionResult DeleteFile()
        {
            Session.Remove(PicSessionkey);
            Session.Abandon();
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
            try
            {
                var userid = User.Identity.GetUserId();
                byte[] buffer = await GetFileStreamFormDisk();

                //return await CreateAsyncResult("aaa");
                // async post to sina
                var sina = new SinaService();
                var token = _repository.UserConnectedSites(userid)
                    .Single(m => m.SiteName == "sina").AccessToken;
                var rtn = await sina.PostAsync(token, msg, buffer, Tools.GetRealIp());
                var sinaStatus = new PostStatus {SiteName = "sina", Message = rtn};

                // async post to qq

                // async post to fanfou

                var col = new List<PostStatus> {sinaStatus};

                return Json(col);
            }
            finally
            {
                DeleteSessionFile();
            }
        }

        private async Task<ActionResult> CreateAsyncResult(string message)
        {
            return await Task.Run(() => Json(new { error = "action failure: " + message }));
        }

        private string getFileStorePath()
        {
            var physicDir = Server.MapPath(TempFileDir);
            var randomName = Path.GetRandomFileName();
            if (!Directory.Exists(physicDir)) Directory.CreateDirectory(physicDir);
            var fileName = Path.Combine(physicDir, randomName);
            if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
            return fileName;
        }

        private async Task<byte[]> GetFileStreamFormDisk()
        {
            var fileName = Session[PicSessionkey] as string;
            if (string.IsNullOrEmpty(fileName)) throw new Exception("file upload fail");
            using (var stream = fileOP.OpenRead(fileName))
            {
                var length = (int)stream.Length;
                var buffer = new byte[length];
                await stream.ReadAsync(buffer, 0, length);
                return buffer;
            }
        }

        /// <summary>
        /// 确保session之前上传的图片删除了
        /// </summary>
        private void DeleteSessionFile()
        {
            var fileName = Session[PicSessionkey] as string;
            if (string.IsNullOrEmpty(fileName)) return;
            if (fileOP.Exists(fileName)) fileOP.Delete(fileName);
            Session.Remove(PicSessionkey);
            Session.Abandon();
        }
    }
}