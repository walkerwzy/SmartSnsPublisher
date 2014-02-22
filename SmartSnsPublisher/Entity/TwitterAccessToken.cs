using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Entity
{
    public class TwitterAccessToken:IAccessToken
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Expire { get; set; }
        public string Error { get; set; }

        public string AccessTokenSecret { get; set; }
    }
}
