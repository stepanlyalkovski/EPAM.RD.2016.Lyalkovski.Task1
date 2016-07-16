using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    }
}
