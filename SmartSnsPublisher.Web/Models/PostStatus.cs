using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Web.Models
{
    public class PostStatus
    {
        [JsonProperty("sitename")]
        public string SiteName { get; set; }
        /// <summary>
        /// 如果不是"ok"，则表示错误消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

    }
}