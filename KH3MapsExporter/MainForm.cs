using KH3MapsExporter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KH3MapsExporter {



    public partial class MainForm : Form {

        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
        

        public MainForm()
        {
            InitializeComponent();
            TXT_UVIewerPath.Text = Settings.Default.UViewerPath;
            TXT_NoesisPath.Text = Settings.Default.NoesisPath;
            TXT_TempFolderPath.Text = Settings.Default.TempPath;
        }

        private void setConfigPath(ref TextBox TXT,Action<string> onValue) {
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "exe";
            openFileDialog.Filter = "exe files (*.exe)|*.exe";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                TXT.Text = openFileDialog.FileName;
                onValue(openFileDialog.FileName);
                Settings.Default.Save();
                Settings.Default.Reload();
            }
        }

        private void BTN_UViewerPath_Click(object sender, EventArgs e)
        {
            setConfigPath(ref TXT_UVIewerPath, (v)=> Settings.Default.UViewerPath = v);
        }

        private void BTN_NoesisPath_Click(object sender, EventArgs e)
        {
            setConfigPath(ref TXT_NoesisPath, (v) => Settings.Default.NoesisPath = v);
        }

        private void BTN_TempFolderPath_Click(object sender, EventArgs e)
        {
            setConfigPath(ref TXT_TempFolderPath, (v) => Settings.Default.TempPath = v);
        }

        private void scanFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            var mapsDir = "";
            if (string.IsNullOrEmpty(mapsDir)) {
                DialogResult result = openFolderDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(openFolderDialog.SelectedPath))
                {
                    var allDirectories = Directory.GetDirectories(openFolderDialog.SelectedPath, "*", System.IO.SearchOption.AllDirectories);
                    mapsDir = allDirectories.ToList().Find(dir => Path.GetFileName(dir) == "Maps");
                }
            }

            if (mapsDir != null)
            {
                var directories = Directory.GetDirectories(mapsDir);
                Debug.WriteLine(mapsDir);
                CMB_MapsIDsList.Items.Clear();
                CMB_MapsIDsList.Items.AddRange(
                    directories.Select(dir => new UMapFile { Name = Path.GetFileName(dir), Value = dir }).ToArray()
                );
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CMB_MapsIDsList_SelectedValueChanged(object sender, EventArgs e)
        {
            var item = CMB_MapsIDsList.SelectedItem as UMapFile;
            var mapFolder = item.Value;
            var mapFiles = Directory.GetFiles(mapFolder, "*.umap", SearchOption.AllDirectories);
            TRV_Maps.Nodes.Clear();
            mapFiles.ToList().ForEach(mapFile => {
                TRV_Maps.Nodes.Add(new TreeNode() { Text = Path.GetFileName(mapFile) });
            });
        }
    }
}
