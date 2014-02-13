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
        public ActionResult Sina()
        {
            return Content(string.Format("rawurl:{0},path_query{1}", Request.RawUrl, Request.Url.PathAndQuery));
        }
    }
}