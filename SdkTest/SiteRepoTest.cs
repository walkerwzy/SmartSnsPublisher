using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSnsPublisher.Web.Models;
using SmartSnsPublisher.Web.Repository;

namespace SdkTest
{
    [TestClass]
    public class SiteRepoTest
    {
        private readonly SiteInfoRepository repo = new SiteInfoRepository();

        [TestMethod]
        public void TestGetList()
        {
            //repo.AddConnectSite(new SiteInfo
            //{
            //    AccessToken = "accesstoken",
            //    Description = "des",
            //    ExpireDate = DateTime.Now,
            //    Id = 1,
            //    SiteName = "sitename",
            //    UserId = "aabc"
            //});
            //repo.AddConnectSite(new SiteInfo
            //{
            //    AccessToken = "accesstoken",
            //    Description = "des",
            //    ExpireDate = DateTime.Now,
            //    Id = 1,
            //    SiteName = "sitename2",
            //    UserId = "aabc"
            //});
            Console.WriteLine(repo.UserConnectedSites("aabc").Count());
        }
    }
}
