using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SmartSnsPublisher.Entity;
using Tweetinvi;
using TweetinviCore.Events;
using TweetinviCore.Interfaces.Credentials;
using TweetinviCore.Interfaces.oAuth;
using TweetinviCredentials;
using TweetinviLogic.TwitterEntities;
using CredentialsCreator = Tweetinvi.CredentialsCreator;

namespace SmartSnsPublisher.Service
{
    public class TwitterService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private readonly Logger _logger;
        private static ITemporaryCredentials _applicationCredentials;

        public TwitterService()
        {
            _appkey = ConfigurationManager.AppSettings["app:twitter:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:twitter:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:twitter:secret"];

            _logger = LogManager.GetCurrentClassLogger();

            if (_applicationCredentials == null) _applicationCredentials = CredentialsCreator.GenerateApplicationCredentials(_appkey, _appsecret);
        }

        public string GetAuthorizationUrl()
        {
            return CredentialsCreator.GetAuthorizationURLForCallback(_applicationCredentials, _redirectUrl);
        }

        public async Task<IAccessToken> GetAccessTokenAsync(string code)
        {
            var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL(code, _applicationCredentials);
            if (ExceptionHandler.GetExceptions().Any())
            {
                var ex = ExceptionHandler.GetLastException();
                await Task.Run(() => _logger.ErrorException(ex.TwitterDescription, ex.WebException));
                throw new Exception(ex.TwitterDescription);
            }
            IAccessToken token = null;
            await Task.Run(() =>
            {
                token = new TwitterAccessToken
                {
                    AccessToken = newCredentials.AccessToken,
                    AccessTokenSecret = newCredentials.AccessTokenSecret
                };
            });
            TwitterCredentials.ApplicationCredentials = TwitterCredentials.CreateCredentials(
                newCredentials.AccessToken, newCredentials.AccessTokenSecret, _appkey, _appsecret);
            return token;
        }

        public async Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            if (TwitterCredentials.Credentials == null)
                TwitterCredentials.Credentials = TwitterCredentials.CreateCredentials(
                    token,
                    ext.secret.ToString(),
                    _appkey,
                    _appsecret);
            var twitter = Tweet.CreateTweet(message);
            await Task.Run(() => { twitter.Publish(); });
            if (ExceptionHandler.GetExceptions().Any())
            {
                var ex = ExceptionHandler.GetLastException();
                await Task.Run(() => _logger.ErrorException(ex.TwitterDescription, ex.WebException));
                throw new Exception(ex.TwitterDescription);
            }
            return twitter.IsTweetPublished ? "ok" : "fail";
        }

        public Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
