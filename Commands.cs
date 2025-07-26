using System.Windows.Input;

namespace SymLinker
{
    public class Commands
    {
        private static readonly Type ownerType = typeof(object);
        private static RoutedUICommand GenCommand(string text, string name) => new(text, name, ownerType);

        public static readonly RoutedUICommand MenuAddFileLink = GenCommand("New _File Link...", nameof(MenuAddFileLink));

        public static readonly RoutedUICommand MenuAddDirectoryLink = GenCommand("New _Directory Link...", nameof(MenuAddDirectoryLink));

        public static readonly RoutedUICommand MenuImportLinks = GenCommand("Import...", nameof(MenuImportLinks));

        public static readonly RoutedUICommand MenuRemoveSelected = GenCommand("_Remove Selected", nameof(MenuRemoveSelected));

        public static readonly RoutedUICommand MenuExit = GenCommand("E_xit", nameof(MenuExit));

        public static readonly RoutedUICommand MenuAbout = GenCommand("_About", nameof(MenuAbout));
    }
}