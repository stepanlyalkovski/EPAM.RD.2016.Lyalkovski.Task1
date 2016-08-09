using System.Configuration;

namespace ServiceConfigurator.CustomSections.Dependencies
{
    public class DependencyConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("MasterService")]
        public MasterServiceCollection MasterServices => (MasterServiceCollection) base["MasterService"];
    }
    
}
