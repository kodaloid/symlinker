using Microsoft.Win32;
using SymLinker.Includes;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SymLinker.Windows
{
    /// <summary>
    /// Interaction logic for AddLinkDialog.xaml
    /// </summary>
    public partial class AddLinkDialog : Window
    {
        bool IsFile;
        public SymLink? Result { get; private set; } = null;


        public AddLinkDialog(bool file)
        {
            InitializeComponent();
            IsFile = file;
        }


        private void ChooseLink_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button)
            {
                string? oldPath = TextSource.Text;
                string filename = "";

                retry:;
                SaveFileDialog dlg = new()
                {
                    Filter = "SymLink|*.*",
                    Title = "Choose where to place the symlink",
                    OverwritePrompt = false,
                    FileName = filename
                };

                if (!string.IsNullOrWhiteSpace(oldPath))
                {
                    dlg.InitialDirectory = oldPath;
                }

                if (dlg.ShowDialog(this) == true)
                {
                    if (File.Exists(dlg.FileName))
                    {
                        filename = dlg.FileName;
                        MessageBox.Show(this, "Error file already exists, you can not overwrite a symlink, pick a new name.");
                        goto retry;
                    }
                    TextSource.Text = dlg.FileName;
                }
            }
        }


        private void ChooseTarget_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button)
            {
                if (IsFile)
                {
                    string oldFile = TextTarget.Text;

                    OpenFileDialog dlg = new() { Filter = "All File(s)|*.*", FileName = oldFile };
                    if (dlg.ShowDialog(this) == true)
                    {
                        TextTarget.Text = dlg.FileName;
                    }
                }
                else
                {
                    string? oldPath = TextTarget.Text;

                    OpenFolderDialog dlg = new();
                    if (!string.IsNullOrWhiteSpace(oldPath)) dlg.InitialDirectory = oldPath;

                    if (dlg.ShowDialog(this) == true)
                    {
                        TextTarget.Text = dlg.FolderName;
                    }
                }
            }
        }


        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, $"Error: {message}", "Error When Creating Symlink", MessageBoxButton.OK);
        }


        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            SymLinkType type = 0;
            if (RadioHardLink.IsChecked == true) type = SymLinkType.Hard;

            SymLink symLink = new(TextSource.Text, TextTarget.Text, type);
            symLink.Validate();

            if (symLink.ValidatedState == SymValidatedLinkState.Untested)
            {
                ShowErrorMessage("Test failed, rejecting creation to be safe.");
                return;
            }

            if (symLink.ValidatedState == SymValidatedLinkState.WrongDestination)
            {
                ShowErrorMessage("You attempted to overwrite a link to point to another destination, this is not allowed here");
                return;
            }

            if (symLink.ValidatedState == SymValidatedLinkState.Exists)
            {
                ShowErrorMessage("This link already exists.");
                return;
            }

            if (symLink.ValidatedState == SymValidatedLinkState.NotLinkable || symLink.ValidatedState == SymValidatedLinkState.SamePath)
            {
                ShowErrorMessage("This you cant use a real location as a symlink.");
                return;
            }

            if (symLink.ValidatedState == SymValidatedLinkState.InvalidDestination)
            {
                ShowErrorMessage("This link destination is invalid or does not exist.");
                return;
            }

            try
            {
                if (RadioSoftLink.IsChecked == true)
                {
                    if (IsFile)
                    {
                        File.CreateSymbolicLink(TextSource.Text, TextTarget.Text);
                    }
                    else
                    {
                        Directory.CreateSymbolicLink(TextSource.Text, TextTarget.Text);
                    }
                }
                else
                {
                    if (!SymLinkHelper.CreateHardLink(TextSource.Text, TextTarget.Text, IntPtr.Zero))
                    {
                        throw new Exception("Failed to create hard symlink.");
                    }
                }

                Result = SymLink.TryParse(TextSource.Text);
                if (Result == null) throw new Exception("Failed to create symlink.");
            }
            catch (Exception ex) { ShowErrorMessage(ex.Message); }

            DialogResult = true;
        }
    }
}