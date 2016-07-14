using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.FileConfigurator
{
    public class FileElement : ConfigurationElement
    {

        [ConfigurationProperty("fileType", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string FileType
        {
            get { return ((string)base["fileType"]); }
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
