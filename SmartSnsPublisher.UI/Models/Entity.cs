using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SmartSnsPublisher.UI.Models
{
    public abstract class Entity
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}