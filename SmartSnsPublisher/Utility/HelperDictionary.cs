using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                if (ignoreBlank && (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))) continue;
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

        public static string QueryStringToJson(this string query)
        {
            query = query.TrimStart('?');
            var kvs = query.Split('&');
            if (kvs.Length == 1)
            {
                var kvp = kvs[0].Split('=');
                if (kvp.Length == 2) return string.Format(@"{{""{0}"":""{1}""}}", kvp[0], kvp[1]);
                return "{}";
            }
            var builder = new StringBuilder("{");
            //format: {"name":"value","name2":"value2"}
            const string format = "\"{0}\":\"{1}\"";
            foreach (var pair in kvs.Select(kv => kv.Split('=')))
            {
                if (pair.Length == 2) builder.AppendFormat(format, pair[0], pair[1]);
                builder.Append(",");
            }
            return builder.ToString().TrimEnd(',') + "}";
        }

        public static IDictionary<string, string> QueryStringToDict(this string query)
        {
            query = query.TrimStart('?');
            var kvs = query.Split('&');
            if (kvs.Length == 1)
            {
                var kvp = kvs[0].Split('=');
                if (kvp.Length == 2) return new Dictionary<string, string> { { kvp[0], kvp[1] } };
                return new Dictionary<string, string>();
            }
            return kvs.Select(kv => kv.Split('='))
                    .Where(pair => pair.Length == 2)
                    .ToDictionary(pair => pair[0], pair => pair[1]);
        }
    }
}
