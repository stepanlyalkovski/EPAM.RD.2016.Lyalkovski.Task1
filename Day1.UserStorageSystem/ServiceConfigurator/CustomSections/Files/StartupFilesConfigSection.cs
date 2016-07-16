using System.Configuration;

namespace ConfigGenerator.CustomSections.Files
{
    public class StartupFilesConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Files")]
        public FilesCollection FileItems => (FilesCollection)base["Files"];
    }
}
