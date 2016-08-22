using System.Linq;
using ServiceConfigurator.Entities;
using Task1.StorageSystem.Concrete.Services;

namespace ServiceConfigurator
{
    public class DependencyInitializer
    {
        /// <summary>
        /// Initializes slaves that master will conntect to.
        /// </summary>
        /// <param name="master">master service that will connect to dependencies</param>
        /// <param name="configuration">settings from app.config</param>
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