using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using ServiceConfigurator.CustomSections.Dependencies;
using ServiceConfigurator.CustomSections.Files;
using ServiceConfigurator.CustomSections.Services;
using ServiceConfigurator.Entities;

namespace ServiceConfigurator
{
    public class ConfigParser
    {
        public static IEnumerable<ServiceConfiguration> ParseServiceConfigSection()
        {
            var serviceSection = GetServiceSection();
            IList<ServiceConfiguration> serviceConfigurations =
                    new List<ServiceConfiguration>(serviceSection.ServiceItems.Count);

            for (int i = 0; i < serviceSection.ServiceItems.Count; i++)
            {
                var serviceType = serviceSection.ServiceItems[i].ServiceType;
                ServiceType type = serviceType.ToLower() == "master" ? ServiceType.Master : ServiceType.Slave;
                var serviceName = serviceSection.ServiceItems[i].ServiceName;
                string filePath = GetXmlFilePath();
                BooleanSwitch loggingSwitch = new BooleanSwitch("loggingSwitch", "Switch in config file");

                var address = serviceSection.ServiceItems[i].IpAddress;
                int port = serviceSection.ServiceItems[i].Port;
                IPAddress ipAddress;
                bool parsed = IPAddress.TryParse(address, out ipAddress);
                IPEndPoint endPoint = null;
                if (parsed)
                {
                    endPoint = new IPEndPoint(IPAddress.Parse(address), port);
                }

                serviceConfigurations.Add(new ServiceConfiguration
                {
                    Name = serviceName,
                    Type = type,
                    FilePath = filePath,
                    LoggingEnabled = loggingSwitch.Enabled,
                    IpEndPoint = endPoint
                });
            }

            return serviceConfigurations;
        }

        public static DependencyConfiguration ParseDependencyConfiguration()
        {
            var section = GetDependencySection();
            var config = new DependencyConfiguration
            {
                MasterName = section.MasterServices.MasterServiceName
            };
            int dependencyCount = section.MasterServices.Count;
            config.SlaveConfigurations = new List<ServiceConfiguration>(dependencyCount);
            for (int i = 0; i < dependencyCount; i++)
            {
                var dependency = section.MasterServices[i];
                IPAddress address;
                bool parsed = IPAddress.TryParse(dependency.IpAddress, out address);
                if (!parsed)
                {
                    throw new ArgumentException("Address is not valid");
                }

                var slaveConfig = new ServiceConfiguration
                {
                    IpEndPoint = new IPEndPoint(address, dependency.Port),
                    Name = dependency.ServiceName
                };
                config.SlaveConfigurations.Add(slaveConfig);
            }

            return config;
        }

        private static UserServicesConfigSection GetServiceSection()
        {
            return (UserServicesConfigSection)ConfigurationManager.GetSection("UserServices");
        }

        private static DependencyConfigSection GetDependencySection()
        {
            return (DependencyConfigSection)ConfigurationManager.GetSection("MasterDependencies");
        }

        #region Files Section

        /// <summary>
        /// Search for files in App.config that have 'xml-storage' type
        /// </summary>
        /// <returns>Path to XML file</returns>
        private static string GetXmlFilePath()
        {
            var config = GetFileConfig();

            for (int i = 0; i < config.FileItems.Count; i++)
            {
                var fileItem = config.FileItems[i];
                if (fileItem.FileType == "xml-storage")
                {
                    return fileItem.Path;
                }
            }

            return null;
        }

        private static StartupFilesConfigSection GetFileConfig()
        {
            return (StartupFilesConfigSection)ConfigurationManager.GetSection("StartupFiles");
        }

#endregion
    }
}