using SymLinker.Includes;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SymLinker.Windows
{
    /// <summary>
    /// Interaction logic for ImportLinksDialog.xaml
    /// </summary>
    public partial class ImportLinksDialog : Window
    {
        public ObservableCollection<FsEntry> LinksFound { get; private set; } = [];
        public ObservableCollection<SymLink> LinksSelected { get; private set; } = [];


        public ImportLinksDialog(string path)
        {
            InitializeComponent();

            foreach (var dir in Directory.GetDirectories(path))
            {
                if (SymLink.TryParse(dir) is SymLink link)
                {
                    LinksFound.Add(link.Source);
                }
            }

            foreach (var file in Directory.GetFiles(path))
            {
                if (SymLink.TryParse(file) is SymLink link)
                {
                    LinksFound.Add(link.Source);
                }
            }
            ListFoundLinks.ItemsSource = LinksFound;
        }


        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            LinksSelected.Clear();
            FsEntry[] entries = [..ListFoundLinks.SelectedItems.Cast<FsEntry>()];
            foreach (FsEntry entry in entries)
            {
                if (SymLink.TryParse(entry) is SymLink link)
                {
                    LinksSelected.Add(link);
                }
            }
            DialogResult = true;
        }


        private void ListFoundLinks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonOK.IsEnabled = ListFoundLinks.SelectedItems.Count > 0;
        }
    }
}