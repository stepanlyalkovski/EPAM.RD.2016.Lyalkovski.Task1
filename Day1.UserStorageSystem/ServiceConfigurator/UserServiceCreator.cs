using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceConfigurator.DomainServiceLoading;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator
{
    public static class UserServiceCreator
    {
        public static UserService CreateService(ServiceConfiguration configuration)
        {
            var domain = AppDomain.CreateDomain(configuration.Name, null, null);
            var type = typeof(DomainServiceLoader);
            var loader = (DomainServiceLoader)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(type).FullName, type.FullName);
            Console.WriteLine("Creating service " + configuration.Name);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Task1.StorageSystem.dll");
            return loader.LoadService(path, configuration);
        }
    }
}
