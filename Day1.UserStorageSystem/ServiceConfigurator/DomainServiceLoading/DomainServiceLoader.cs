using System;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator.DomainServiceLoading
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
            UserService result = null;
            switch (configuration.Type)
            {
                    case ServiceType.Master:
                    result = new MasterUserService(generator, validator, 
                                                                repository, configuration.LoggingEnabled);
                    break;
                    case ServiceType.Slave:
                        result = new SlaveUserService(generator, validator,
                                                               repository, configuration.LoggingEnabled);
                    break;
            }

            if (result != null)
            {
                result.Name = AppDomain.CurrentDomain.FriendlyName;
            }

            return result;
        }
     }


}