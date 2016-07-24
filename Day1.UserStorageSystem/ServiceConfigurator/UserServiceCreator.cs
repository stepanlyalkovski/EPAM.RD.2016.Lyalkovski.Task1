using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //public static UserService CreateService(ServiceConfiguration configuration)
        //{
        //    var domain = AppDomain.CreateDomain(configuration.Name, null, null);
        //    var type = typeof(DomainServiceLoader);
        //    var loader = (DomainServiceLoader)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(type).FullName, type.FullName);
        //    Console.WriteLine("Creating service " + configuration.Name);
        //    var assemblies = domain.GetAssemblies();
        //    Console.WriteLine("Assemblies: ");
        //    foreach (var assembly in assemblies)
        //    {
        //        Console.WriteLine(assembly.FullName);
        //    }
        //    Console.WriteLine(RemotingServices.IsTransparentProxy(loader));
        //    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Task1.StorageSystem.dll");
        //    return loader.LoadService(path, configuration);
        //}

        public static IEnumerable<UserService> CreateServices(IEnumerable<ServiceConfiguration> configurations)
        {
            var serviceConfigurations = configurations as ServiceConfiguration[] ?? configurations.ToArray();
            var services = new List<UserService>();
            MasterUserService master = null;
            DomainServiceLoader masterLoader = null;

            foreach (var serviceConfiguration in serviceConfigurations)
            {
                var domain = AppDomain.CreateDomain(serviceConfiguration.Name, null, null);
                var type = typeof(DomainServiceLoader);
                var loader = (DomainServiceLoader)domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(type).FullName, type.FullName);
                Debug.WriteLine("Creating service " + serviceConfiguration.Name);
                var assemblies = domain.GetAssemblies();
                Debug.WriteLine("Assemblies: ");
                foreach (var assembly in assemblies)
                {
                    Debug.WriteLine(assembly.FullName);
                }
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Task1.StorageSystem.dll");

                var service = loader.LoadService(path, serviceConfiguration); // create service

                var userService = service as MasterUserService;
                if (userService != null)
                {
                    master = userService;
                    masterLoader = loader;
                }

                services.Add(service);
            }

            if (master != null)
            {
                masterLoader.ConnectMaster(master, serviceConfigurations.Where(c => c.Type == ServiceType.Slave).ToList());
            }
                 
            return services;
        }
    }
}
