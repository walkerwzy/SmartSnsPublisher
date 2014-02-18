using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Utility;

namespace SmartSnsPublisher.Service
{
    public class TencentService:IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private static readonly Dictionary<string, string> _auth_resources = new Dictionary<string, string>
        {
            {"authorize","https://open.t.qq.com/cgi-bin/oauth2/authorize"} //请求授权
            //{"accesstoken","https://api.weibo.com/oauth2/access_token"},//获取授权
            //{"tokeninfo","https://api.weibo.com/oauth2/get_token_info"},//授权查询
            //{"revoke","https://api.weibo.com/oauth2/revokeoauth2"}//授权回收
        };
        private static readonly Dictionary<string, string> _post_resources = new Dictionary<string, string>
        {
            {"update","https://api.weibo.com/2/statuses/update.json"},
            {"post","https://upload.api.weibo.com/2/statuses/upload.json"}
        };

        public TencentService()
        {
            _appkey = ConfigurationManager.AppSettings["app:qq:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:qq:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:qq:secret"];
        }

        public string GetAuthorizationUrl()
        {
            var param = new Dictionary<string, object>
            {
                {"client_id",_appkey},
                {"redirect_uri",_redirectUrl},
                {"wap",""},
                {"scope","all"},
                {"state",""},
                {"display",""}, //defalut, mobile, wap, client, apponweibo
                {"forcelogin",""},
                {"language",""},
                {"response_type","code"}
            };

            return Common.GetUrl(param, _auth_resources["authorize"]);
        }

        public Task<Entity.SinaAccessToken> GetAccessTokenAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAsync(string token, string message, string ip, string latitude, string longitude)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "0.0", string longitude = "0.0")
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
