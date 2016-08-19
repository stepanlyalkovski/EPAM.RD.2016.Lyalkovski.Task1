using System.Configuration;

namespace ServiceConfigurator.CustomSections.Dependencies
{
    public class MasterServiceCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string MasterServiceName
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        public DependencyElement this[int idx] => (DependencyElement)BaseGet(idx);

        protected override ConfigurationElement CreateNewElement()
        {
            return new DependencyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DependencyElement)element).ServiceName;
        }
    }
}