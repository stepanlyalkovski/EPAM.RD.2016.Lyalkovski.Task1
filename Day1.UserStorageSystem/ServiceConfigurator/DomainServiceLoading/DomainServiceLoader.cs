using System;
using NetworkServiceCommunication;
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
        public UserService LoadDomainService(string assemblyString, ServiceConfiguration configuration)
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

            UserService domainService;
            UserServiceCommunicator communicator;
            switch (configuration.Type)
            {
                    case ServiceType.Master:
                    {
                        domainService = new MasterUserService(generator, validator,
                            repository, configuration.LoggingEnabled);

                        communicator = this.GetMasterCommunicator();                        
                    }      
                    break;

                    case ServiceType.Slave:
                    {
                        domainService = new SlaveUserService(generator, validator, repository,
                                                                    configuration.LoggingEnabled);

                        Receiver<User> receiver = new Receiver<User>(configuration.IpEndPoint.Address, 
                                                                        configuration.IpEndPoint.Port);
                       
                        communicator = new UserServiceCommunicator(receiver);
                    }    break;

                default: throw new ArgumentException("Unknown ServiceType");
            }
            domainService.AddCommunicator(communicator);

            domainService.Name = AppDomain.CurrentDomain.FriendlyName;

            return domainService;
        }

        private UserServiceCommunicator GetMasterCommunicator()
        {
            var sender = new Sender<User>();
            return new UserServiceCommunicator(sender);
        }
    }


}