using System.Configuration;

namespace ServiceConfigurator.CustomSections.Files
{
    [ConfigurationCollection(typeof(FileElement))]
    public class FilesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).Path;
        }

        public FileElement this[int idx] => (FileElement) this.BaseGet(idx);
    }
}
