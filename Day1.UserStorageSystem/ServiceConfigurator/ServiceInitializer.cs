using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using ServiceConfigurator.CustomSections.Services;
using ServiceConfigurator.ServiceFactory;
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
            var serviceSection = GetServiceSection();           
            INumGenerator generator = new EvenIdGenerator();
            ValidatorBase<User> validator = new SimpleUserValidator();
            IRepository<User> repository;
            IUserXmlFileWorker worker = null;
            MasterUserService master = null;
            IList<SlaveUserService> slaves = new List<SlaveUserService>();
            IList<UserService> services = new List<UserService>();
            string filePath = FileInitializer.GetXmlFilePath();
            BooleanSwitch loggingSwitch = new BooleanSwitch("loggingSwitch", "Switch in config file");

            if (filePath != null)
            {
                worker = new UserXmlFileWorker();
            }
            repository = new UserRepository(worker, filePath);

            for (int i = 0; i < serviceSection.FileItems.Count; i++)
            {
                var serviceType = serviceSection.FileItems[i].ServiceType;
                var serviceCount = Int32.Parse(serviceSection.FileItems[i].Count);

                if (serviceType == "master")
                {
                    if (master != null || serviceCount > 1)
                    {
                        throw new ConfigurationErrorsException("Not allowed to create more than one master");
                    }

                    master = UserServiceCreator.CreateService<MasterUserService>("master_domain", generator, 
                        validator, repository, loggingSwitch.Enabled);
                    services.Add(master);
                    Console.WriteLine(RemotingServices.IsTransparentProxy(master));
                }

                if (serviceType == "slave")
                {
                    for (int j = 0; j < serviceCount; j++)
                    {
                        var service = UserServiceCreator.CreateService<SlaveUserService>($"Slave_{j}_Domain", generator,
                            validator, repository, loggingSwitch.Enabled);
                        services.Add(service);
                    }
                }

            }

            if (master == null)
            {
                throw new ConfigurationErrorsException("Master is not exist");
            }

            SubscribeServices(master, slaves);

            return services;         
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
