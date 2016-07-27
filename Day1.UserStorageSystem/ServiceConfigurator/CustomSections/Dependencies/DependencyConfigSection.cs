using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceConfigurator.CustomSections.Services;

namespace ServiceConfigurator.CustomSections.Dependencies
{
    public class DependencyConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("MasterService")]
        public MasterServiceCollection MasterServices => (MasterServiceCollection) base["MasterService"];
    }
    
}
