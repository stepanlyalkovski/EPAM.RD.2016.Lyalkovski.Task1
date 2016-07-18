using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceConfigurator.CustomSections.Services;
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
        public static IEnumerable<UserService> InitializeServices()
        {
            var serviceSection = GetServiceSection();           
            INumGenerator generator = new EvenIdGenerator();
            ValidatorBase<User> validator = new SimpleUserValidator();
            IRepository<User> repository;
            IUserXmlFileWorker worker = null;
            bool masterExist = false;
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
                    if (masterExist || serviceCount > 1)
                    {
                        throw new ConfigurationErrorsException("Not allowed to create more than one master");
                    }

                    masterExist = true;
                    services.Add(new MasterUserService(generator, validator, repository, loggingSwitch.Enabled));

                }

                if (serviceType == "slave")
                {
                    for (int j = 0; j < serviceCount; j++)
                    {
                        services.Add(new SlaveUserService(generator, validator, repository, loggingSwitch.Enabled));
                    }
                }
            }

            return services;         
        }

        private static UserServicesConfigSection GetServiceSection()
        {
            return (UserServicesConfigSection)ConfigurationManager.GetSection("UserServices");
        }

    }
}
