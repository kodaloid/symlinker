using Microsoft.Win32;
using SymLinker.Includes;
using SymLinker.Windows;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SymLinker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SymAppSettings Settings = new();

        // icon thanks to https://www.hiclipart.com/free-transparent-background-png-clipart-diqyr

        private void CanExecuteAlways(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        private void CanExecuteOnSelection(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ListLinks.SelectedItems.Count > 0;


        public MainWindow() => InitializeComponent();


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Settings = await SymAppSettings.Load() ?? new();
            ListLinks.ItemsSource = Settings.Links;
        }


        private async void FileMenu_CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Command == Commands.MenuAddFileLink)
                {
                    AddLinkDialog dlg = new(true) { Owner = this };
                    if (dlg.ShowDialog() == true && dlg.Result is SymLink link)
                    {
                        Settings.Links.Add(link);
                        await Settings.Save();
                    }
                }
                else if (e.Command == Commands.MenuAddDirectoryLink)
                {
                    AddLinkDialog dlg = new(false) { Owner = this };
                    if (dlg.ShowDialog() == true && dlg.Result is SymLink link)
                    {
                        Settings.Links.Add(link);
                        await Settings.Save();
                    }
                }
                else if (e.Command == Commands.MenuImportLinks)
                {
                    OpenFolderDialog dlg = new() { DereferenceLinks = false, Title = "Choose directory that contains symlinks" };
                    if (dlg.ShowDialog(this) == true)
                    {
                        ImportLinksDialog dlg2 = new(dlg.FolderName) { Owner = this };
                        if (dlg2.ShowDialog() == true)
                        {
                            foreach (SymLink entry in dlg2.LinksSelected)
                            {
                                Settings.Links.Add(entry);
                            }
                        }
                        await Settings.Save();
                    }
                }
                else if (e.Command == Commands.MenuRemoveSelected)
                {
                    if (ListLinks.SelectedItem is SymLink link)
                    {
                        string message = "Also delete the actual symlink?";
                        if (link.Type == SymLinkType.Hard) message = "Also delete the actual symlink? If this is a hardlink, the destination will also be removed.";
                        if (link.Source.Exists && MessageBox.Show(this, message, "Remove Selected", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            if (link.Source.Type == FsEntryType.File) File.Delete(link.Source.Path);
                            if (link.Source.Type == FsEntryType.Directory) Directory.Delete(link.Source.Path);
                        }
                        Settings.Links.Remove(link);
                        await Settings.Save();
                    }
                }
                else if (e.Command == Commands.MenuExit) Close();
                else if (e.Command == Commands.MenuAbout)
                {
                    AboutDialog dlg = new() { Owner = this };
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error when running a command,\n{ex.Message}", "Command Error");
            }
        }
    }
}