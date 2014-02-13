using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Utility;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Service
{
    public class SinaService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private static readonly Dictionary<string, string> Resources = new Dictionary<string, string>
        {
            {"authorize","https://api.weibo.com/oauth2/authorize"}, //请求授权
            {"accesstoken","https://api.weibo.com/oauth2/access_token"},//获取授权
            {"tokeninfo","https://api.weibo.com/oauth2/get_token_info"},//授权查询
            {"revoke","https://api.weibo.com/oauth2/revokeoauth2"}//授权回收
        };


        public SinaService()
        {
            _appkey = ConfigurationManager.AppSettings["app:sina:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:sina:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:sina:secret"];
        }

        public string GetAuthorizationUrl()
        {
            var param = new Dictionary<string, object>
            {
                {"client_id",_appkey},
                {"redirect_uri",_redirectUrl},
                {"scope","all"},
                {"state",""},
                {"display",""}, //defalut, mobile, wap, client, apponweibo
                {"forcelogin",""},
                {"language",""},
                {"response_type","code"}
            };

            return GetUrl(param, Resources["authorize"]);
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _appkey),
                new KeyValuePair<string, string>("client_secret", _appsecret),
                new KeyValuePair<string, string>("redirect_uri", _redirectUrl),
                new KeyValuePair<string, string>("grant_type", "authorization_code"), //hard code
                new KeyValuePair<string, string>("code", code)
            };
            var client = new HttpClient();
            var response = client.PostAsync(Resources["accesstoken"],
                new FormUrlEncodedContent(postData)).Result;
            if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
            return await Task.Run(() => "error");
        }

        public void Post(string message)
        {
            throw new NotImplementedException();
        }

        public void Post(string message, byte[] attachment)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #region helper

        /// <summary>
        /// Compose resource url and params
        /// </summary>
        /// <param name="param"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetUrl(Dictionary<string, object> param, string url)
        {
            var query = param.ToQueryString();
            if (url.Contains('?')) url = url + '&' + query;
            else url = url + '?' + query;
            return url;
        }

        #endregion
    }
}
