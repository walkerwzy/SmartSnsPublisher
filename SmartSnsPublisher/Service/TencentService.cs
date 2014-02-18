using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Utility;

namespace SmartSnsPublisher.Service
{
    public class TencentService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private static readonly Dictionary<string, string> _auth_resources = new Dictionary<string, string>
        {
            {"authorize","https://open.t.qq.com/cgi-bin/oauth2/authorize"}, //请求授权
            {"accesstoken","https://open.t.qq.com/cgi-bin/oauth2/access_token"} //获取授权
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
                {"response_type","code"},
                {"wap",""},
                {"state",""},
                {"forcelogin",""}
            };

            return Common.GetUrl(param, _auth_resources["authorize"]);
        }

        public async Task<IAccessToken> GetAccessTokenAsync(string code)
        {
            var postData = new Dictionary<string, string>
            {
                {"client_id",_appkey},
                {"client_secret",_appsecret},
                {"redirect_uri",_redirectUrl},
                {"grant_type", "authorization_code"},
                {"code",code}
            };
            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(postData.ToList()))
            using (var task = client.PostAsync(_auth_resources["accesstoken"], content))
            {
                var response = task.Result;
                //response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                await Task.Run(() => HelperLogger.Debug(result));
                if (result.Contains("errorCode"))
                {
                    //dynamic rtn = JsonConvert.DeserializeObject(result);
                    string error = result.Substring(result.LastIndexOf("=") + 2).TrimEnd('\'');
                    return new TencentAccessToken { Error = error };
                }
                return !response.IsSuccessStatusCode
                    ? new TencentAccessToken { Error = "Response code: " + response.StatusCode }
                    : JsonConvert.DeserializeObject<TencentAccessToken>(result);
            }
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
