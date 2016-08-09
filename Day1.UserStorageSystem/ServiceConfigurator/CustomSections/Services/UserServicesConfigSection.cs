using System.Configuration;

namespace ServiceConfigurator.CustomSections.Services
{
    public class UserServicesConfigSection : ConfigurationSection
    {
        
        [ConfigurationProperty("Services")]
        public UserServicesCollection ServiceItems => (UserServicesCollection)base["Services"];
    }
}
