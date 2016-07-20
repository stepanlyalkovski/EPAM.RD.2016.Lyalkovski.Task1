using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ServiceConfigurator.Tests
{
    [TestFixture]
    class ServiceInitializerTests
    {
        [Test]
        public void InitializeServices_AddOneMasterAndTwoSlavesToAppConfig_ReturnedRequiredNumberOfServices()
        {
            int serviceCount = 3;

            var services = ServiceInitializer.InitializeServices();

            Assert.AreEqual(serviceCount, services.Count());
        }

        [Test]
        public void InitializeServices_GetMasterAndSlavesFromAppConfig_ServiceIsTransparentProxy()
        {
            int serviceCount = 3;

            var service = ServiceInitializer.InitializeServices().First();
            
            Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
        }
    }
}
