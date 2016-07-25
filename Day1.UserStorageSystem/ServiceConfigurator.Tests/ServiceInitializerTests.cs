using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete.Services;

namespace ServiceConfigurator.Tests
{
    [TestFixture]
    class ServiceInitializerTests
    {
        private ServiceConfiguration configuration;

        //[Test]
        //public void InitializeServices_AddOneMasterAndTwoSlavesToAppConfig_ReturnedRequiredNumberOfServices()
        //{
        //    configuration = new ServiceConfiguration
        //    {
        //        Type = ServiceType.Master,
        //        Name = "testName"
        //    };

        //    var service = UserServiceCreator.CreateService(configuration);

        //    Assert.IsTrue(service is MasterUserService);
        //}

        //[Test]
        //public void InitializeServices_GetMasterAndSlavesFromAppConfig_ServiceIsTransparentProxy()
        //{
        //    int serviceCount = 3;

        //    var service = UserServiceInitializer.InitializeServices().First();

        //    Assert.IsTrue(RemotingServices.IsTransparentProxy(service));
        //}
    }
}
