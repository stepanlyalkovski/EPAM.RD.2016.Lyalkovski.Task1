using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator
{
    public class DomainServiceLoader : MarshalByRefObject
    {
        public T LoadService<T>(string assemblyString,INumGenerator numGenerator, 
            ValidatorBase<User> validator, IRepository<User> repository, bool loggingEnabled) where T: UserService
        {
            var assembly = Assembly.LoadFrom(assemblyString);
            var types = assembly.GetTypes();
            Console.WriteLine("Current Domain Name: " + Thread.GetDomain().FriendlyName);
            var serviceType = typeof (T);
            object[] ctorArgs = { numGenerator, validator, repository, loggingEnabled };
            var service = (T)Activator.CreateInstance(serviceType, ctorArgs);
            return service;
        }
    }
}