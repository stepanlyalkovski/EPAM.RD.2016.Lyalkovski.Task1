using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConfigurator.CustomSections.Services
{
    public class UserServicesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Services")]
        public UserServicesCollection FileItems => (UserServicesCollection)base["Services"];
    }
}
