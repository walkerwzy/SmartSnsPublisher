using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Entity
{
    public class SinaAccessToken : IAccessToken
    {
        /// <summary>
        /// sina accesstoken response do not have this property
        /// </summary>
        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// sina accesstoken response do not have this property
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("remind_in")]
        public int RemindIn { get; set; }
        [JsonProperty("expires_in")]
        public int Expire { get; set; }
        [JsonProperty("uid")]
        public string UserId { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
