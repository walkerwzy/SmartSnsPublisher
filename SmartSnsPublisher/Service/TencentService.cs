using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Utility;

namespace SmartSnsPublisher.Service
{
    public class TencentService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private readonly Logger _logger;

        private static readonly Dictionary<string, string> _auth_resources = new Dictionary<string, string>
        {
            {"authorize","https://open.t.qq.com/cgi-bin/oauth2/authorize"}, //请求授权
            {"accesstoken","https://open.t.qq.com/cgi-bin/oauth2/access_token"}, //获取授权
            {"tokeninfo",""},//授权查询
            {"revoke",""}//授权回收
        };
        private static readonly Dictionary<string, string> _post_resources = new Dictionary<string, string>
        {
            {"update","https://open.t.qq.com/api/t/add"}, //post text
            {"post","https://open.t.qq.com/api/t/add_pic"} //post text with picture
        };

        public TencentService()
        {
            _appkey = ConfigurationManager.AppSettings["app:qq:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:qq:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:qq:secret"];

            _logger = LogManager.GetCurrentClassLogger();
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
                result = result.QueryStringToJson();
                if (result.Contains("errorCode"))
                {
                    //errorCode=xxx&errorMsg=xxx
                    //var respDict = result.QueryStringToDict();
                    //string error;
                    //respDict.TryGetValue("errorMsg", out error);
                    //return new TencentAccessToken { Error = "error: " + error };

                    // return raw error message instead
                    await Task.Run(() => _logger.Error(result));
                    return new TencentAccessToken { Error = result };
                }
                await Task.Run(() => _logger.Debug(result));
                //access_token=e0586ec1d1e2a8b26e8d5703a99d7eea&expires_in=8035200&refresh_token=3af8d4909d7ebbae08dbbb5825029a92&openid=f3c7e92e9b1f8b065f6154d7f5569981&name=walkerwzy&nick=walker&state=
                return !response.IsSuccessStatusCode
                    ? new TencentAccessToken { Error = "Response code: " + response.StatusCode }
                    : JsonConvert.DeserializeObject<TencentAccessToken>(result);
            }
        }

        /// <summary>
        /// 发布一条文字微博
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ext">e.g. {openid:xxx,openkey:xxx}</param>
        /// <returns></returns>
        public async Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            var postData = new Dictionary<string, string>
            {
                //oauth
                {"openid",ext.openid.ToString()},
                {"openkey",ext.openkey.ToString()},
                {"oauth_version","2.a"},
                {"oauth_consumer_key",_appkey},
                {"access_token",token},
                {"scope","all"},
                //content
                {"format","json"},//json, xml
                {"content",Common.encodeURL(message)}, //urlencode withou system.web 
                {"latitude",latitude},
                {"longitude",longitude},
                {"clientip",ip},
                {"compatibleflag","0"},//容错，超出140字正常发
                {"empty","0"}
            };
            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(postData.ToList())) //application/x-www-form-urlencoded
            using (var task = client.PostAsync(_post_resources["update"], content))
            {
                var response = task.Result;
                //response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                // success: {ret:0,errcode:0,msg:"ok",data:{id:"",timestamp:""},seqid:"xxx"}
                // fail:    {ret:"1",errcode:"1",msg:"error clientip",data:{},detailerrinfo:{...}}
                dynamic dy = JsonConvert.DeserializeObject(result);
                if (dy.ret.ToString() != "0")
                {
                    await Task.Run(() => _logger.Error(result));
                    return string.Format("ret:{0},code:{1},msg:{2}", dy.ret, dy.errcode, dy.msg);
                }
                await Task.Run(() => _logger.Debug(result));
                return "ok";
                /*
                 {"created_at":"Sat Feb 15 02:08:28 +0800 2014","id":xxx,"mid":"xxx","idstr":"xxx","text":"xxx","source":"xxx","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","pic_urls":[],"geo":null,
                 * "user":{...},
                 * "reposts_count":0,"comments_count":0,"attitudes_count":0,"mlevel":0,"visible":{"type":0,"list_id":0}}
                 */
            }
        }

        public async Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            var postData = new Dictionary<string, string>
            {
                //oauth
                {"openid",ext.openid.ToString()},
                {"openkey",ext.openkey.ToString()},
                {"oauth_version","2.a"},
                {"oauth_consumer_key",_appkey},
                {"access_token",token},
                {"scope","all"},
                //content
                {"format","json"},//json, xml
                {"content",Common.encodeURL(message)}, //urlencode withou system.web 
                {"latitude",latitude},
                {"longitude",longitude},
                {"clientip",ip},
                {"compatibleflag","0"},//容错，超出140字正常发
                {"empty","0"}
            };
            using (var client = new HttpClient())
            using (var requestContent = new MultipartFormDataContent()) // default use a guid as boundary, or you can set a custom one
            using (var imgContent = new ByteArrayContent(attachment))
            {
                //Content-Disposition: form-data; name={key}\r\n\r\n{value}
                foreach (var item in postData)
                {
                    requestContent.Add(new StringContent(item.Value), item.Key);
                }
                //Content-Disposition: form-data; name=pic; filename=aaa.png\r\nContent-Type: image/png\r\n\r\n
                string extname;
                imgContent.Headers.ContentType = MediaTypeHeaderValue.Parse(HelperFileInfo.GetImageMIMEType(attachment, out extname));
                requestContent.Add(imgContent, "pic", DateTime.Now.Ticks.ToString("X") + extname);

                //client.DefaultRequestHeaders.ExpectContinue = true;
                //ServicePointManager.SecurityProtocol=SecurityProtocolType.Ssl3;

                using (var task = client.PostAsync(_post_resources["post"], requestContent))
                {
                    var response = task.Result;
                    //response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    // success: {ret:0,errcode:0,msg:"ok",data:{id:"",timestamp:""},seqid:"xxx"}
                    // fail:    {ret:"1",errcode:"1",msg:"error clientip",data:{},detailerrinfo:{...}}
                    dynamic dy = JsonConvert.DeserializeObject(result);
                    if (dy.ret.ToString() != "0")
                    {
                        await Task.Run(() => _logger.Error(result));
                        return string.Format("ret:{0},code:{1},msg:{2}", dy.ret, dy.errcode, dy.msg);
                    }
                    await Task.Run(() => _logger.Debug(result));
                    return "ok";
                }
            }
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
