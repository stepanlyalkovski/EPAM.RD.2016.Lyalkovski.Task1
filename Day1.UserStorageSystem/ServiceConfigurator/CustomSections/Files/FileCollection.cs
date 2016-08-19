using System.Configuration;

namespace ServiceConfigurator.CustomSections.Files
{
    [ConfigurationCollection(typeof(FileElement))]
    public class FilesCollection : ConfigurationElementCollection
    {
        public FileElement this[int idx] => (FileElement)BaseGet(idx);

        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).Path;
        }
    }
}
