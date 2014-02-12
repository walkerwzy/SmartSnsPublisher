using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartSnsPublisher.Web.Controllers
{
    public class CbController : Controller
    {
        //
        // GET: /Cb/
        public ActionResult Index()
        {
            return Content("index");
        }

        // for live account login callback
        // GET: /cb/live
        public ActionResult Live()
        {
            return Content(Request.RawUrl);
        }

        // for twitter login callback
        // GET: /cb/tw
        public ActionResult Tw()
        {
            return Content(Request.RawUrl);
        }

        // for facebook login callback
        // GET: /cb/fb
        public ActionResult Fb()
        {
            return Content(Request.RawUrl);
        }

        // for google login callback
        // GET: /signin-google
        public ActionResult Google()
        {
            return Content(Request.RawUrl);
        }
	}
}