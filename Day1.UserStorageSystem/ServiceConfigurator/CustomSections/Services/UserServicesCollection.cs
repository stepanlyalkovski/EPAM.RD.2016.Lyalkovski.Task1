using System.Configuration;

namespace ServiceConfigurator.CustomSections.Services
{
    [ConfigurationCollection(typeof(ServiceElement))]
    public class UserServicesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceElement) element).ServiceName;
        }

        public ServiceElement this[int idx] => (ServiceElement)BaseGet(idx);
    }
}
