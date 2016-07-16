using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConfigurator.CustomSections.Services
{
    public class ServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("type", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string ServiceType
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("count", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Count
        {
            get { return (string)base["count"]; }
            set { base["count"] = value; }
        }
    }
}
