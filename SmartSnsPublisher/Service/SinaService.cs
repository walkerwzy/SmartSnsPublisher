using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Utility;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Service
{
    public class SinaService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private static readonly Dictionary<string, string> _auth_resources = new Dictionary<string, string>
        {
            {"authorize","https://api.weibo.com/oauth2/authorize"}, //请求授权
            {"accesstoken","https://api.weibo.com/oauth2/access_token"},//获取授权
            {"tokeninfo","https://api.weibo.com/oauth2/get_token_info"},//授权查询
            {"revoke","https://api.weibo.com/oauth2/revokeoauth2"}//授权回收
        };
        private static readonly Dictionary<string, string> _post_resources = new Dictionary<string, string>
        {
            {"update","https://api.weibo.com/2/statuses/update.json"},
            {"post","https://upload.api.weibo.com/2/statuses/upload.json"}
        };

        /*
            Response Error Format
            {
	            "request" : "/statuses/home_timeline.json",
	            "error_code" : "20502",
	            "error" : "Need you follow uid."
            }
        */

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

            return Common.GetUrl(param, _auth_resources["authorize"]);
        }

        public async Task<SinaAccessToken> GetAccessTokenAsync(string code)
        {
            //var postData = new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("client_id", _appkey),
            //    new KeyValuePair<string, string>("client_secret", _appsecret),
            //    new KeyValuePair<string, string>("redirect_uri", _redirectUrl),
            //    new KeyValuePair<string, string>("grant_type", "authorization_code"), //hard code
            //    new KeyValuePair<string, string>("code", code)
            //};
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
                if (result.Contains("error_code"))
                {
                    dynamic rtn = JsonConvert.DeserializeObject(result);
                    return new SinaAccessToken { Error = rtn.error };
                }
                return !response.IsSuccessStatusCode
                    ? new SinaAccessToken { Error = "Response code: " + response.StatusCode }
                    : JsonConvert.DeserializeObject<SinaAccessToken>(result);
            }
        }

        /// <summary>
        /// 发布一条文字微博
        /// </summary>
        /// <see cref="http://open.weibo.com/wiki/2/statuses/update"/>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns>"ok" for success, or error string</returns>
        public async Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "0.0", string longitude = "0.0")
        {
            var postData = new Dictionary<string, string>
            {
                {"access_token",token},
                {"status",Common.encodeURL(message)}, //urlencode withou system.web 
                //{"visible","0"},//微博的可见性，0：所有人能看，1：仅自己可见，2：密友可见，3：指定分组可见，默认为0。
                //{"list_id",""},//微博的保护投递指定分组ID，只有当visible参数为3时生效且必选。
                {"lat",latitude},
                {"long",longitude},
                //{"annotations","\"from walker's sns sync app\""}, //json
                {"rip",ip}
            };
            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(postData.ToList())) //application/x-www-form-urlencoded
            using (var task = client.PostAsync(_post_resources["update"], content))
            {
                var response = task.Result;
                //response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                if (result.Contains("error_code"))
                {
                    dynamic rtn = JsonConvert.DeserializeObject(result);
                    return rtn.error_code + ": " + rtn.error;
                }
                await Task.Run(() => HelperLogger.Debug(result));
                return "ok";
                /*
                 {"created_at":"Sat Feb 15 02:08:28 +0800 2014","id":3678060027245767,"mid":"3678060027245767","idstr":"3678060027245767","text":"hello+world","source":"<a href=\"http://open.weibo.com\" rel=\"nofollow\">未通过审核应用</a>","favorited":false,"truncated":false,"in_reply_to_status_id":"","in_reply_to_user_id":"","in_reply_to_screen_name":"","pic_urls":[],"geo":null,"user":{"id":1071696872,"idstr":"1071696872","class":1,"screen_name":"walkerwzy","name":"walkerwzy","province":"42","city":"1","location":"湖北 武汉","description":"fuck away...","url":"http://www.dig-music.com","profile_image_url":"http://tp1.sinaimg.cn/1071696872/50/5596300400/1","profile_url":"walkerwzy","domain":"walkerwzy","weihao":"","gender":"m","followers_count":114,"friends_count":202,"statuses_count":1628,"favourites_count":25,"created_at":"Mon Mar 15 18:23:41 +0800 2010","following":false,"allow_all_act_msg":false,"geo_enabled":true,"verified":false,"verified_type":-1,"remark":"","ptype":0,"allow_all_comment":true,"avatar_large":"http://tp1.sinaimg.cn/1071696872/180/5596300400/1","avatar_hd":"http://tp1.sinaimg.cn/1071696872/180/5596300400/1","verified_reason":"","follow_me":false,"online_status":0,"bi_followers_count":24,"lang":"zh-cn","star":0,"mbtype":0,"mbrank":0,"block_word":0},"reposts_count":0,"comments_count":0,"attitudes_count":0,"mlevel":0,"visible":{"type":0,"list_id":0}}
                 */
            }
        }

        /// <summary>
        /// 发布一条带图片的微博
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        /// <param name="ip"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <see cref="http://open.weibo.com/wiki/2/statuses/upload"/>
        /// <returns>"ok" for success, or error string</returns>
        public async Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "0.0", string longitude = "0.0")
        {
            var postData = new Dictionary<string, string>
            {
                {"access_token",token},
                {"status",Common.encodeURL(message)}, //urlencode withou system.web 
                //{"visible","0"},//微博的可见性，0：所有人能看，1：仅自己可见，2：密友可见，3：指定分组可见，默认为0。
                //{"list_id",""},//微博的保护投递指定分组ID，只有当visible参数为3时生效且必选。
                {"lat",latitude},
                {"long",longitude},
                //{"annotations","\"from walker's sns sync app\""}, //json
                {"rip",ip}
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
                string extname;
                imgContent.Headers.ContentType = MediaTypeHeaderValue.Parse(HelperFileInfo.GetImageMIMEType(attachment, out extname));
                //Content-Disposition: form-data; name=pic; filename=aaa.png\r\nContent-Type: image/png\r\n\r\n
                requestContent.Add(imgContent, "pic", DateTime.Now.Ticks.ToString("X") + extname);
                using (var task = client.PostAsync(_post_resources["post"], requestContent))
                {
                    var response = task.Result;
                    //response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    if (result.Contains("error_code"))
                    {
                        dynamic rtn = JsonConvert.DeserializeObject(result);
                        return rtn.error_code + ": " + rtn.error;
                    }
                    await Task.Run(() => HelperLogger.Debug(result));
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
