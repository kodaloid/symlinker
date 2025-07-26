using System.Runtime.InteropServices;

namespace SymLinker.Includes
{
    public class SymLinkHelper
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes );
    }
}