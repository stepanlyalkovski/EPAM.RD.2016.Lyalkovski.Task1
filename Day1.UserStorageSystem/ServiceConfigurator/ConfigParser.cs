using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using ServiceConfigurator.CustomSections.Files;
using ServiceConfigurator.CustomSections.Services;
using ServiceConfigurator.Entities;

namespace ServiceConfigurator
{
    public class ConfigParser
    {
        public static IEnumerable<ServiceConfiguration> ParseAppConfig()
        {
            var serviceSection = GetServiceSection();
            IList<ServiceConfiguration> serviceConfigurations =
                    new List<ServiceConfiguration>(serviceSection.FileItems.Count);

            for (int i = 0; i < serviceSection.FileItems.Count; i++)
            {
                var serviceType = serviceSection.FileItems[i].ServiceType;
                ServiceType type = serviceType.ToLower() == "master" ? ServiceType.Master : ServiceType.Slave;
                var serviceName = serviceSection.FileItems[i].ServiceName;
                string filePath = GetXmlFilePath();
                BooleanSwitch loggingSwitch = new BooleanSwitch("loggingSwitch", "Switch in config file");

                var address = serviceSection.FileItems[i].IpAddress;
                int port = serviceSection.FileItems[i].Port;
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


        private static UserServicesConfigSection GetServiceSection()
        {
            return (UserServicesConfigSection)ConfigurationManager.GetSection("UserServices");
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