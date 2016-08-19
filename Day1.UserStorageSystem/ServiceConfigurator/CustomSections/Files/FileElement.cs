using System.Configuration;

namespace ServiceConfigurator.CustomSections.Files
{
    public class FileElement : ConfigurationElement
    {
        [ConfigurationProperty("fileType", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string FileType
        {
            get { return (string)base["fileType"]; }
            set { base["fileType"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }
    }
}
