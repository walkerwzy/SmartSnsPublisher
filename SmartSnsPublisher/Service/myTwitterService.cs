using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SmartSnsPublisher.Service
{
    public class myTwitterService:IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private readonly Logger _logger;

        private static readonly Dictionary<string, string> _auth_resources = new Dictionary<string, string>
        {
            {"authorize","https://api.twitter.com/oauth/request_token"}, //请求授权
            {"accesstoken","https://api.twitter.com/oauth/authenticate"}, //获取授权
            {"tokeninfo",""},//授权查询
            {"revoke",""}//授权回收
        };
        private static readonly Dictionary<string, string> _post_resources = new Dictionary<string, string>
        {
            {"update","https://open.t.qq.com/api/t/add"}, //post text
            {"post","https://open.t.qq.com/api/t/add_pic"} //post text with picture
        };

        public myTwitterService()
        {
            _appkey = ConfigurationManager.AppSettings["app:twitter:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:twitter:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:twitter:secret"];

            _logger = LogManager.GetCurrentClassLogger();
        }

        public string GetAuthorizationUrl()
        {
            // Authorization Header:
            // OAuth oauth_nonce="K7ny27JTpKVsTgdyLdDfmQQWVLERj2zAK5BslRsqyw", oauth_callback="http%3A%2F%2Fmyapp.com%3A3005%2Ftwitter%2Fprocess_callback", oauth_signature_method="HMAC-SHA1", oauth_timestamp="1300228849", oauth_consumer_key="OqEqJeafRSF11jBMStrZz", oauth_signature="Pc%2BMLdv028fxCErFyi8KXFM%2BddU%3D", oauth_version="1.0"

            // Response:
            // oauth_token=Z6eEdO8MOmk394WozF5oKyuAv855l4Mlqo7hhlSLik&oauth_token_secret=Kd75W4OQfb2oJTV0vzGzeXftVAwgMnEK9MumzYcM&oauth
            throw new NotImplementedException();
        }

        public Task<Entity.IAccessToken> GetAccessTokenAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
