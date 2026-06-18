using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ShapeDatTorrent.ConsoleApp.UI
{
    public enum FileBrowserMode
    {
        File,
        Folder
    }

    public partial class FileSystemBrowser : UserControl
    {
        private string _path;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path {
            get { return PathTB.Text; }
            set { PathTB.Text = value; }
        }



        [Category("Appearance")]
        [Description("The text to display in the File/Folder Dialog window.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // Change to Visible or remove
        public string Prompt { get; set; } = "Please choose a file/folder";

        [Category("Behavior")]
        [Description("Determines if the browser targets a file or a directory.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public FileBrowserMode Mode { get; set; } = FileBrowserMode.File;

        [Category("Behavior")]
        [Description("The filter string for the OpenFileDialog (e.g., 'Text files (*.txt)|*.txt').")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ExtensionFilter { get; set; } = "All files (*.*)|*.*";

        public FileSystemBrowser()
        {
            InitializeComponent();
        }



        private void BrowseBT_Click(object sender, EventArgs e)
        {
            if (Mode == FileBrowserMode.File)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = Prompt;
                openFileDialog.Filter = ExtensionFilter;

                var result = openFileDialog.ShowDialog();

                if (result != DialogResult.OK) return;

                Path = openFileDialog.FileName;
            }
            else if (Mode == FileBrowserMode.Folder)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = Prompt;

                var result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;

                Path = dialog.SelectedPath;
            }
        }

        private void ClearBT_Click(object sender, EventArgs e)
        {
            Path = String.Empty;
        }
    }
}
