namespace ServiceConfigurator.CustomSections.Files
{
    public class TempFileInitializer
    {
        public static string GetFilePath(StartupFilesConfigSection section)
        {
            return section.FileItems[0].Path;
        }
    }
}
