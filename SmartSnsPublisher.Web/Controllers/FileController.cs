using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSnsPublisher.Web.Controllers
{
    public class FileController : Controller
    {
        //
        // POST: /File/
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            System.Threading.Thread.Sleep(3000);
            var s = Request.Files;
            return Json("ok");
        }
	}
}