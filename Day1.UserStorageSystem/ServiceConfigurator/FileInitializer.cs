using System.Configuration;
using ServiceConfigurator.CustomSections.Files;

namespace ServiceConfigurator
{
    public class FileInitializer
    {
        private static StartupFilesConfigSection GetFileConfig()
        {
            return (StartupFilesConfigSection)ConfigurationManager.GetSection("StartupFiles");
        }

        public static string GetFilePath()
        {
            var section = GetFileConfig();
            return section.FileItems[0].Path;
        }

        /// <summary>
        /// Search for files in App.config that have 'xml-storage' type
        /// </summary>
        /// <returns>Path to XML file</returns>
        public static string GetXmlFilePath()
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
    }
}
