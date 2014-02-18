using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Entity
{
    public class TencentAccessToken : IAccessToken
    {
        [JsonProperty("uid")]
        public string UserId { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int Expire { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("name")]
        public string UserName{get;set;}
    }
}
