using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Utility;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Service
{
    public class SinaService : IAccountFacade
    {
        private string _appkey;
        private string _redirectUrl;
        private static readonly Dictionary<string, string> _resources = new Dictionary<string, string>
        {
            {"authorize","https://api.weibo.com/oauth2/authorize"}, //请求授权
            {"accesstoken","https://api.weibo.com/oauth2/access_token"},//获取授权
            {"tokeninfo","https://api.weibo.com/oauth2/get_token_info"},//授权查询
            {"revoke","https://api.weibo.com/oauth2/revokeoauth2"}//授权回收
        };


        public SinaService()
        {
            _appkey = AppKey;
        }
        public string AppKey
        {
            get
            {
                if (string.IsNullOrEmpty(_appkey))
                {
                    _appkey = ConfigurationManager.AppSettings["app:sina:key"];
                }
                return _appkey;
            }
            set
            {
                _appkey = value;
            }
        }

        public string RedirectUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_redirectUrl))
                {
                    _appkey = ConfigurationManager.AppSettings["app:sina:redirect"];
                }
                return _appkey;
            }
            set
            {
                _redirectUrl = value;
            }
        }
        public string GetAuthorizationUrl()
        {
            var param = new Dictionary<string, object>
            {
                {"client_id",AppKey},
                {"rediredt_uri",""},
                {"scope","all"},
                {"state",""},
                {"display",""}, //defalut, mobile, wap, client, apponweibo
                {"forcelogin",""},
                {"language",""}
            };

            var query = param.ToQueryString();
            var url = _resources["authorize"];
            if (url.Contains('?')) url = url + '&' + query;
            else url = url + '?' + query;
            //string resp = HelperWebRequest.DoGet(_resources["authorize"], param);
            //var respObj = JsonConvert.DeserializeObject(resp);

            // instead of request the url directly, we retun it to browser
            return url;
        }

        public void GetAccessToken()
        {
            throw new NotImplementedException();
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


        #endregion
    }
}
