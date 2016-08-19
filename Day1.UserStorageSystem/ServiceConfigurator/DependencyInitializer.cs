using System.Linq;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete.Services;

namespace ServiceConfigurator
{
    public class DependencyInitializer
    {
        public static void InitalizeDependencies(MasterUserService master, DependencyConfiguration configuration)
        {
            if (master == null)
            {
                return;
            }

            if (master.Name != configuration.MasterName)
            {
                return;
            }

            if (configuration.SlaveConfigurations.Count == 0)
            {
                return;                
            }

            master.Communicator.ConnectGroup(configuration.SlaveConfigurations.Select(c => c.IpEndPoint).ToList());
        }
    }
}