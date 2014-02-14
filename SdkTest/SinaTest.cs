using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SmartSnsPublisher;
using SmartSnsPublisher.Entity;
using SmartSnsPublisher.Service;

namespace SdkTest
{
    [TestClass]
    public class SinaTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var source =
                "{\"access_token\":\"2.007_jWKBTRPn9C20f1c8c347ge1dRC\",\"remind_in\":\"157679999\",\"expires_in\":157679999,\"uid\":\"1071696872\",\"scope\":\"follow_app_official_microblog\"}";
            var o = JsonConvert.DeserializeObject<SinaAccessToken>(source);
            Console.WriteLine("ok");
        }
    }
}
