using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceConfigurator.CustomSections.Services;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator
{
    public static class ServiceInitializer
    {
        //TODO create interface with Settings so you'll be able to test creator
        public static IEnumerable<UserService> InitializeServices()
        {           
            var serviceConfigurations = ParseAppConfig();
            IList<UserService> services = new List<UserService>();

            foreach (var serviceConfiguration in serviceConfigurations)
            {
                var service = UserServiceCreator.CreateService(serviceConfiguration);
                Console.WriteLine("-----Services has been created");
                services.Add(service);
            }

            var master = (MasterUserService)services.FirstOrDefault(s => s is MasterUserService);

            if (master == null)
            {
                throw new ConfigurationErrorsException("Master is not exist");
            }

            var slaves = services.OfType<SlaveUserService>().ToList();
            SubscribeServices(master, slaves);
            ThreadInitializer.InitializeThreads(master, slaves);

            return services;     
        }

        private static IEnumerable<ServiceConfiguration> ParseAppConfig()
        {
            var serviceSection = GetServiceSection();
            IList<ServiceConfiguration> serviceConfigurations = 
                    new List<ServiceConfiguration>(serviceSection.FileItems.Count);

            for (int i = 0; i < serviceSection.FileItems.Count; i++)
            {
                var serviceType = serviceSection.FileItems[i].ServiceType;
                ServiceType type = serviceType.ToLower() == "master" ? ServiceType.Master : ServiceType.Slave;
                var serviceName = serviceSection.FileItems[i].ServiceName;
                string filePath = FileInitializer.GetXmlFilePath();
                BooleanSwitch loggingSwitch = new BooleanSwitch("loggingSwitch", "Switch in config file");

                serviceConfigurations.Add(new ServiceConfiguration
                {
                    Name = serviceName,
                    Type = type,
                    FilePath = filePath,
                    LoggingEnabled = loggingSwitch.Enabled
                });
            }

            return serviceConfigurations;
        }

        private static UserServicesConfigSection GetServiceSection()
        {
            return (UserServicesConfigSection)ConfigurationManager.GetSection("UserServices");
        }

        private static void SubscribeServices(MasterUserService master, IEnumerable<SlaveUserService> slaves)
        {
            foreach (var slave in slaves)
            {
                slave.Subscribe(master);
            }
        }


        private static void WrapInDomains(MasterUserService master, IEnumerable<SlaveUserService> slaves)
        {
            //var domain = AppDomain.CreateDomain($"Service_{j}", null, null);
            //services.Add(service);

        }
    }
}
