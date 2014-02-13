using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Utility;

namespace SmartSnsPublisher.Utility
{
    public static class HelperWebRequest
    {

        #region get/post/和带文件的post

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url)
        {
            HttpWebRequest req = GetWebRequest(url);
            //req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            var rsp = (HttpWebResponse)req.GetResponse();
            var encoding = Encoding.UTF8;
            if (rsp.CharacterSet != null) encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }
        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">用于构造请求参数的键值对字典</param>
        /// <returns>HTTP响应</returns>
        public static string DoGet(string url, IDictionary<string, object> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                if (url.Contains("?"))
                {
                    url = url + "&" + parameters.ToQueryString();
                }
                else
                {
                    url = url + "?" + parameters.ToQueryString();
                }
            }
            return DoGet(url);
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数，用于构建请求体的健值对字典</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, IDictionary<string, object> parameters)
        {
            return DoPost(url, parameters.ToQueryString());
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求体</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, string body)
        {
            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            byte[] postData = Encoding.UTF8.GetBytes(body);
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            var encoding = Encoding.UTF8;
            if (rsp.CharacterSet != null) encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行带文件上传的HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <param name="fileInfo">请求文件参数</param>
        /// <returns>HTTP响应</returns>
        public static string DoPost(string url, IDictionary<string, object> textParams, IDictionary<string, FileInfo> fileInfo)
        {
            // 如果没有文件参数，则走普通POST请求
            if (fileInfo == null || fileInfo.Count == 0)
            {
                return DoPost(url, textParams);
            }

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线

            HttpWebRequest req = GetWebRequest(url, "POST");
            req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

            Stream reqStream = req.GetRequestStream();
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 组装文本请求参数
            const string textTemplate = "Content-Disposition:form-data;name=\"{0}\"\r\nContent-Type:text/plain\r\n\r\n{1}";
            IEnumerator<KeyValuePair<string, object>> textEnum = textParams.GetEnumerator();
            while (textEnum.MoveNext())
            {
                string textEntry = string.Format(textTemplate, textEnum.Current.Key, textEnum.Current.Value);
                byte[] itemBytes = Encoding.UTF8.GetBytes(textEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);
            }

            // 组装文件请求参数
            const string fileTemplate = "Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            IEnumerator<KeyValuePair<string, FileInfo>> fileEnum = fileInfo.GetEnumerator();
            while (fileEnum.MoveNext())
            {
                string key = fileEnum.Current.Key;
                FileItem fileItem = new FileItem(fileEnum.Current.Value);
                string fileEntry = string.Format(fileTemplate, key, fileItem.GetFileName(), fileItem.GetMimeType());
                byte[] itemBytes = Encoding.UTF8.GetBytes(fileEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);

                byte[] fileBytes = fileItem.GetContent();
                reqStream.Write(fileBytes, 0, fileBytes.Length);
            }

            reqStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            var encoding = Encoding.UTF8;
            if (rsp.CharacterSet != null) encoding = Encoding.GetEncoding(rsp.CharacterSet);
            return GetResponseAsString(rsp, encoding);
        }

        #endregion

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            StringBuilder result = new StringBuilder();
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                if (stream == null) return string.Empty;
                reader = new StreamReader(stream, encoding);

                // 按字符读取并写入字符串缓冲
                int ch = -1;
                while ((ch = reader.Read()) > -1)
                {
                    // 过滤结束符
                    char c = (char)ch;
                    if (c != '\0')
                    {
                        result.Append(c);
                    }
                }
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }

            return result.ToString();
        }

        public static HttpWebRequest GetWebRequest(string url, string method = "GET", bool keepAlive = true, int timeout = 100000, bool expect100Continue = false)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = expect100Continue;
            req.Method = method;
            req.KeepAlive = keepAlive;
            req.Timeout = timeout;
            return req;
        }

    }
}
