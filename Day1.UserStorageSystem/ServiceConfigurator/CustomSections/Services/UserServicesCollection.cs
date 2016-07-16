using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceConfigurator.CustomSections.Files;

namespace ServiceConfigurator.CustomSections.Services
{
    [ConfigurationCollection(typeof(FileElement))]
    public class UserServicesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceElement) element).ServiceType;
        }

        public ServiceElement this[int idx] => (ServiceElement) BaseGet(idx);
    }
}
