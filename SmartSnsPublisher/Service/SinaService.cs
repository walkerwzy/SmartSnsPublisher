﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSnsPublisher.Utility;
using Newtonsoft.Json;

namespace SmartSnsPublisher.Service
{
    public class SinaService : IAccountFacade
    {
        private string _appkey;
        private static readonly Dictionary<string, string> _resources = new Dictionary<string, string>
        {
            {"authorize","https://api.weibo.com/oauth2/authorize"}, //请求授权
            {"accesstoken","https://api.weibo.com/oauth2/access_token"},//获取授权
            {"tokeninfo","https://api.weibo.com/oauth2/get_token_info"},//授权查询
            {"revoke","https://api.weibo.com/oauth2/revokeoauth2"}//授权回收
        };


        public SinaService()
        {
            _appkey = AppKey;
        }
        public string AppKey
        {
            get
            {
                if (string.IsNullOrEmpty(_appkey))
                {
                    _appkey = ConfigurationManager.AppSettings["appkey:sina"];
                }
                return _appkey;
            }
            set
            {
                _appkey = value;
            }
        }

        public void Authorization()
        {
            var param = new Dictionary<string, string>
            {
                {"client_id",AppKey},
                {"rediredt_uri",""},
                {"scope","all"},
                {"state",""},
                {"display",""}, //defalut, mobile, wap, client, apponweibo
                {"forcelogin",""},
                {"language",""}
            };
            string resp = HelperWebRequest.DoGet(_resources["authorize"], param);
            var respObj = JsonConvert.DeserializeObject(resp);
            //store code and state
        }

        public void GetAccessToken()
        {
            throw new NotImplementedException();
        }

        public void Post(string message)
        {
            throw new NotImplementedException();
        }

        public void Post(string message, byte[] attachment)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #region helper


        #endregion
    }
}