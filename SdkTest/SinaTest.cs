﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartSnsPublisher;
using SmartSnsPublisher.Service;

namespace SdkTest
{
    [TestClass]
    public class SinaTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            SinaService sina = new SinaService();
            Console.WriteLine(sina.AppKey);
        }
    }
}
