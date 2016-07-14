using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator.FileConfigurator
{
    public class TempFileInitializer
    {
        public static string GetFilePath(StartupFilesConfigSection section)
        {
            return section.FileItems[0].Path;
        }
    }
}
