using System;
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
        Task<SinaAccessToken> GetAccessTokenAsync(string code);

        #endregion

        #region 操作

        /// <summary>
        /// 发布一条文字微博
        /// </summary>
        /// <param name="message"></param>
        void Post(string message);
        /// <summary>
        /// 发布一条带图片的微博
        /// </summary>
        /// <param name="message"></param>
        /// <param name="attachment"></param>
        void Post(string message, byte[] attachment);
        /// <summary>
        /// 删除一条微博
        /// </summary>
        void Delete();

        // 关注，取消关注，回复，随便查看，私信等暂时不在关注范围内，此项目名为Publisher
        // 全功能SDK／API未在进行中

        #endregion

    }
}
