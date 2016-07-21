using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
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
    public class DomainServiceLoader : MarshalByRefObject
    {
        public UserService LoadService(string assemblyString, ServiceConfiguration configuration)
        {
            //ServiceConfigurator includes Service dll so we don't need to Load in explicitly
            //var assembly = Assembly.LoadFrom(assemblyString); 
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Console.WriteLine("Assemblies: ");
            foreach (var assembly in assemblies)
            {
                Console.WriteLine(assembly.FullName);
            }
            //temporary way to initialize components
            INumGenerator generator = new EvenIdGenerator();
            ValidatorBase<User> validator = new SimpleUserValidator();
            IUserXmlFileWorker worker = null;
            if (configuration.FilePath != null)
            {
                worker = new UserXmlFileWorker();
            }
            IRepository<User> repository = new UserRepository(worker, configuration.FilePath);

            switch (configuration.Type)
            {
                    case ServiceType.Master: return new MasterUserService(generator, validator, 
                                                                repository, configuration.LoggingEnabled);
                    case ServiceType.Slave: return new SlaveUserService(generator, validator, 
                                                                repository, configuration.LoggingEnabled);
                default:
                    return null;
            }
        }


    }
}