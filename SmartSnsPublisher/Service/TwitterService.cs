using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SmartSnsPublisher.Entity;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Credentials;

namespace SmartSnsPublisher.Service
{
    public class TwitterService : IAccountFacade
    {
        private readonly string _appkey;
        private readonly string _redirectUrl;
        private readonly string _appsecret;
        private readonly Logger _logger;
        private static ITemporaryCredentials _tempCredentials;

        public TwitterService()
        {
            _appkey = ConfigurationManager.AppSettings["app:twitter:key"];
            _redirectUrl = ConfigurationManager.AppSettings["app:twitter:redirect"];
            _appsecret = ConfigurationManager.AppSettings["app:twitter:secret"];

            _logger = LogManager.GetCurrentClassLogger();

            if (_tempCredentials == null) _tempCredentials = CredentialsCreator.GenerateApplicationCredentials(_appkey, _appsecret);
        }

        public string GetAuthorizationUrl()
        {
            return CredentialsCreator.GetAuthorizationURLForCallback(_tempCredentials, _redirectUrl);
        }

        public async Task<IAccessToken> GetAccessTokenAsync(string code)
        {
            var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL(code, _tempCredentials);
            _tempCredentials = null;
            if (ExceptionHandler.GetExceptions().Any())
            {
                var ex = ExceptionHandler.GetLastException();
                await Task.Run(() => _logger.ErrorException(ex.TwitterDescription, ex.WebException));
                throw new Exception(ex.TwitterDescription);
            }
            IAccessToken token = null;
            await Task.Run(() =>
            {
                _setCredentials(newCredentials.AccessToken, newCredentials.AccessTokenSecret);
                var user = User.GetLoggedUser();
                token = new TwitterAccessToken
                {
                    UserId = user.IdStr,
                    UserName = user.ScreenName,
                    AccessToken = newCredentials.AccessToken,
                    AccessTokenSecret = newCredentials.AccessTokenSecret
                };
            });
            return token;
        }

        public async Task<string> UpdateAsync(string token, string message, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            _setCredentials(token, ext.secret.ToString());
            var twitter = Tweet.CreateTweet(message);
            await Task.Run(() =>
            {
                double lat, lon;
                if (double.TryParse(latitude, out lat) && double.TryParse(longitude, out lon))
                    twitter.PublishWithGeo(lon, lat);
                else twitter.Publish();
            });
            if (ExceptionHandler.GetExceptions().Any())
            {
                var ex = ExceptionHandler.GetLastException();
                await Task.Run(() => _logger.ErrorException(ex.TwitterDescription, ex.WebException));
                throw new Exception(ex.TwitterDescription);
            }
            return twitter.IsTweetPublished ? "ok" : "fail";
        }

        public async Task<string> PostAsync(string token, string message, byte[] attachment, string ip = "127.0.0.1", string latitude = "", string longitude = "", dynamic ext = null)
        {
            _setCredentials(token, ext.secret.ToString());
            var twitter = Tweet.CreateTweet(message);
            await Task.Run(() =>
            {
                //todo: upload photo
                double lat, lon;
                if (double.TryParse(latitude, out lat) && double.TryParse(longitude, out lon))
                    twitter.PublishWithGeo(lon, lat);
                else twitter.Publish();
            });
            if (ExceptionHandler.GetExceptions().Any())
            {
                var ex = ExceptionHandler.GetLastException();
                await Task.Run(() => _logger.ErrorException(ex.TwitterDescription, ex.WebException));
                throw new Exception(ex.TwitterDescription);
            }
            return twitter.IsTweetPublished ? "ok" : "fail";
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #region helper

        private void _setCredentials(string token, string secret)
        {
            if (TwitterCredentials.Credentials == null)
                TwitterCredentials.Credentials = TwitterCredentials.CreateCredentials(
                    token, secret, _appkey, _appsecret);
        }

        #endregion
    }
}
