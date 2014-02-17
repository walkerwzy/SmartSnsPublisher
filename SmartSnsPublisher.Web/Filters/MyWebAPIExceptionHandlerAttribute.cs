using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using NLog;

namespace SmartSnsPublisher.Web.Filters
{
    public class MyWebAPIExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;
            _logger.ErrorException("[" + context.Request.RequestUri + "]" + ex.Message, ex);
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}