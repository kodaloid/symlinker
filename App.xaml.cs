using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using static System.Environment;

namespace SymLinker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static string RelPath(string path = "")
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //This will strip just the working path name:
            //C:\Program Files\MyApplication

            string basePath = System.IO.Path.GetDirectoryName(strExeFilePath) ?? "/";
            return basePath.TrimEnd('/') + $"/{path}";
        }


        internal static string AppDataPath(string path = "")
        {
            string basePath = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "SymLinker");
            string fullPath = basePath.TrimEnd('\\') + $"\\{path}";

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            return fullPath;
        }
    }
}