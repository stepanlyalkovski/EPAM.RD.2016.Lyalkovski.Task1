using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
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
            UserServiceCommunicator communicator = null;
            switch (configuration.Type)
            {
                    case ServiceType.Master:
                    {
                        Sender<User> sender = new Sender<User>();
                        communicator = new UserServiceCommunicator(sender);
                        result = new MasterUserService(generator, validator,
                            repository, configuration.LoggingEnabled);
                    }
                    
                    break;
                    case ServiceType.Slave:
                    {
                        Receiver<User> receiver = new Receiver<User>(configuration.IpEndPoint.Address, 
                                                                        configuration.IpEndPoint.Port);
                       
                        communicator = new UserServiceCommunicator(receiver);
                        result = new SlaveUserService(generator, validator, repository, configuration.LoggingEnabled);
                        Task task = receiver.AcceptConnection();
                        task.ContinueWith((t) => communicator.RunReceiver());
                    }    break;
                default: throw new ArgumentException("Unknown ServiceType");
            }
            result.AddCommunicator(communicator);

            result.Name = AppDomain.CurrentDomain.FriendlyName;

            return result;
        }

        public void ConnectMaster(MasterUserService master, IEnumerable<ServiceConfiguration> slaveConfigurations)
        {
            master.Communicator.Connect(slaveConfigurations.Where(c => c.IpEndPoint != null)
                                                           .Select(c => c.IpEndPoint));

        }
    }


}