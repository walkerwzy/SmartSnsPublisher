using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace SmartSnsPublisher.Web.Filters
{
    public class MyExceptionHandlerAttribute:HandleErrorAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;
            _logger.ErrorException(ex.Message, ex);

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult result = new JsonResult
                {
                    Data = ex.Message
                };
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.Result = result;
            }
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
        }
    }
}