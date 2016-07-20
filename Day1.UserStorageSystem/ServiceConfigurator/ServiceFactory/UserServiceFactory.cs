using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator.ServiceFactory
{
    static class UserServiceFactory
    {
        public static UserService CreateUserService(string serviceType, INumGenerator numGenerator, ValidatorBase<User> validator, 
                                                        IRepository<User> repository, bool loggingEnabled)
        {
            switch (serviceType.ToLower())
            {
                case "master": return new MasterUserService(numGenerator, validator, repository, loggingEnabled);
                case "slave": return new SlaveUserService(numGenerator, validator, repository, loggingEnabled);
                default:
                    return null;
            }
        }
    }
}
