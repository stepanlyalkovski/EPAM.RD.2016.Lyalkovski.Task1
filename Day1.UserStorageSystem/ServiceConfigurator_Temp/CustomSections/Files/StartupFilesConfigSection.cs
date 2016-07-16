using System.Configuration;

namespace ServiceConfigurator.CustomSections.Files
{
    public class StartupFilesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Files")]
        public FilesCollection FileItems => (FilesCollection)base["Files"];
    }
}
