using System.Collections.Generic;

namespace ServiceConfigurator.Entities
{
    public class DependencyConfiguration
    {
        public string MasterName { get; set; }
        public IList<ServiceConfiguration> SlaveConfigurations { get; set; } 
    }
}