﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Entity;

namespace SmartSnsPublisher
{
    public interface IAccountFacade
    {

        #region 授权

        /// <summary>
        /// 从用户获取授权
        /// </summary>
        string GetAuthorizationUrl();

        /// <summary>
        /// 用户授权后，从资源方获取令牌
        /// </summary>
        Task<IAccessToken> GetAccessTokenAsync(string code);

        #endregion

        #region 操作

        /// <summary>
        /// 发布一条文字微博
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ext">用于个别网站的特定规则，比如腾讯微博，在oauth2.0的规则之外还需要传递openid和openkey</param>
        /// <returns></returns>
        Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null);

        /// <summary>
        /// 发布一条带图片的微博
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        /// <param name="ip"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ext">用于个别网站的特定规则，比如腾讯微博，在oauth2.0的规则之外还需要传递openid和openkey</param>
        /// <returns>"ok" for success, or error string</returns>
        Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null);
        /// <summary>
        /// 删除一条微博
        /// </summary>
        void Delete();

        // 关注，取消关注，回复，随便查看，私信等暂时不在关注范围内，此项目名为Publisher
        // 全功能SDK／API未在进行中

        #endregion

    }
}
