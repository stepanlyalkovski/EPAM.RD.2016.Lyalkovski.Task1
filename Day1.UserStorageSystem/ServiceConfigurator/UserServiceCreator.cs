using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator
{
    public static class UserServiceCreator
    {
        //TODO user IoC container
        public static T CreateService<T>(string domainName, INumGenerator numGenerator, ValidatorBase<User> validator,
                                IRepository<User> repository, bool loggingEnabled) where T : UserService
        {
            var domain = AppDomain.CreateDomain(domainName, null, null);
            var type = typeof(DomainServiceLoader);
            var loader = (DomainServiceLoader)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(type).FullName, type.FullName);
            Console.WriteLine(RemotingServices.IsTransparentProxy(loader));
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Task1.StorageSystem.dll");

            var service = loader.LoadService<T>(path, numGenerator, validator, repository, loggingEnabled);

            Console.WriteLine(RemotingServices.IsTransparentProxy(service));
            return service;
        }
    }
}
