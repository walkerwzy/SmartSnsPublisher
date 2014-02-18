using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Entity
{
    public interface IAccessToken
    {
        /// <summary>
        /// 社交网站ID
        /// </summary>
        string UserId { get; set; }
        /// <summary>
        /// 社交网站用户名
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 社交网站令牌
        /// </summary>
        string AccessToken { get; set; }
        /// <summary>
        /// 社交网站刷新令牌
        /// </summary>
        string RefreshToken { get; set; }
        /// <summary>
        /// accesstoken失效时间
        /// </summary>
        int Expire { get; set; }
        string Error { get; set; }
    }
}
