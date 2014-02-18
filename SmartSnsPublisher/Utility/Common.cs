using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Utility
{
    public class Common
    {
        /// <summary>
        /// Compose resource url and params
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrl(Dictionary<string, object> param, string url)
        {
            var query = param.ToQueryString();
            if (url.Contains('?')) url = url + '&' + query;
            else url = url + '?' + query;
            return url;
        }

        public static string encodeURL(string source)
        {
            // microsoft encode the blank  to '+', we change it back
            return WebUtility.UrlEncode(source).Replace("+", " ");
        }
    }
}
