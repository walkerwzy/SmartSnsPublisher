using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Utility
{
    public static class HelperDictionary
    {
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, object>> dict, bool ignoreBlank = true)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, object>> dem = dict.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value.ToString();
                // 如果设置为忽略参数名或参数值为空的参数
                if(ignoreBlank && (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))) continue;
                if (hasParam)
                {
                    postData.Append("&");
                }

                postData.Append(name);
                postData.Append("=");
                postData.Append(Uri.EscapeDataString(value));
                hasParam = true;
            }

            return postData.ToString();
        }
    }
}
