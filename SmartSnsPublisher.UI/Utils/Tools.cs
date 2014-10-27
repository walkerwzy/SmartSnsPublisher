using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartSnsPublisher.UI.Utils
{
    public class Tools
    {
        /// <summary>
        /// get user real ip address
        /// </summary>
        /// <returns></returns>
        public static string GetRealIp()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null ?
                HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0] :
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}