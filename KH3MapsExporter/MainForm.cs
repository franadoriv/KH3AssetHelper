using KH3AssetHelper.Helpers;
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
using UE;


namespace KH3MapsExporter {



    public partial class MainForm : Form {

        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
        private List<UMapFile> mapsFiles = new List<UMapFile>();
        private ContextMenuStrip ctxMenuForTreeView = new ContextMenuStrip();
        private UAssetParser parser = new UAssetParser(new UAParserConfig());

        public MainForm()
        {
            InitializeComponent();
            TXT_UVIewerPath.Text = Settings.Default.UViewerPath;
            TXT_NoesisPath.Text = Settings.Default.NoesisPath;
            TXT_TempFolderPath.Text = Settings.Default.TempPath;
            TXT_GameFolderPath.Text = Settings.Default.GamePath;
            TXT_BlueprintDBPath.Text = Settings.Default.BlueprintDBPath;
            TXT_MaterialDBPath.Text = Settings.Default.MaterialDBPath;
            TRV_Maps.ContextMenuStrip = ctxMenuForTreeView;
        }

        private void setConfigFilePath(ref TextBox TXT,Action<string> onValue,string defaultExt = "exe",string filter = "exe files (*.exe)|*.exe") {
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = defaultExt;
            openFileDialog.Filter = filter;
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                TXT.Text = openFileDialog.FileName;
                onValue(openFileDialog.FileName);
                Settings.Default.Save();
                Settings.Default.Reload();
            }
        }

        private void setConfigDirPath(ref TextBox TXT, Action<string> onValue)
        {
            openFolderDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFolderDialog.SelectedPath))
            {
                TXT.Text = openFolderDialog.SelectedPath;
                onValue(openFolderDialog.SelectedPath);
                Settings.Default.Save();
                Settings.Default.Reload();
            }
        }

        private void BTN_UViewerPath_Click(object sender, EventArgs e)
        {
            setConfigFilePath(ref TXT_UVIewerPath, (v)=> Settings.Default.UViewerPath = v);
        }

        private void BTN_NoesisPath_Click(object sender, EventArgs e)
        {
            setConfigFilePath(ref TXT_NoesisPath, (v) => Settings.Default.NoesisPath = v);
        }

        private void BTN_TempFolderPath_Click(object sender, EventArgs e)
        {
            setConfigDirPath(ref TXT_TempFolderPath, (v) => Settings.Default.TempPath = v);
        }

        private void BTN_GameFolderPath_Click(object sender, EventArgs e)
        {
            setConfigDirPath(ref TXT_GameFolderPath, (v) => Settings.Default.GamePath = v);
        }

        private void BTN_MaterialDBPath_Click(object sender, EventArgs e)
        {
            setConfigFilePath(ref TXT_MaterialDBPath, (v) => Settings.Default.MaterialDBPath = v, "db", "db files(*.db)|*.db");
        }

        private void BTN_BlueprintDBPath_Click(object sender, EventArgs e)
        {
            setConfigFilePath(ref TXT_BlueprintDBPath, (v) => Settings.Default.BlueprintDBPath = v,"db","db files(*.db)|*.db");
        }

        private static string LikeToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("_", ".").Replace("%", ".*") + "$";
        }

        private void scanFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Console.WriteLine("Starting");
            var ext = new List<string> { "umap" };
            var mapsFilesPaths = Directory
                .EnumerateFiles(Settings.Default.GamePath, "*.*", SearchOption.AllDirectories)
                .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()) && Regex.IsMatch(s, LikeToRegular(@"%\Game\Maps\%\umap\%"))).ToList();
            mapsFilesPaths.ForEach(p => Console.WriteLine(p));
            Console.WriteLine("Completed " + mapsFilesPaths.Count());

            var mapCodes = mapsFilesPaths.Select(path => Path.GetFileNameWithoutExtension(path).Split('_')[0]).Distinct().ToArray();
            
            CMB_MapsIDsList.Items.Clear();
            CMB_MapsIDsList.Items.AddRange(mapCodes);
            mapsFiles = mapsFilesPaths.Select(mf => new UMapFile
            {
                mapNamespace = Path.GetFileNameWithoutExtension(mf).Split('_')[0],
                name = Path.GetFileNameWithoutExtension(mf),
                path = mf
            }).ToList();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CMB_MapsIDsList_SelectedValueChanged(object sender, EventArgs e)
        {
            var mapid = CMB_MapsIDsList.SelectedItem as string;
            Console.WriteLine(mapid);
            TRV_Maps.Nodes.Clear();
            TRV_Maps.Nodes.AddRange(mapsFiles.Where(mapFile => {
                return mapFile.mapNamespace == mapid;
            }).Select(mapFile=>new TreeNode() { Text= mapFile.name,Tag=mapFile }).ToArray());
        }

        private void updateConfig() {
            parser.config.TempFolder = Settings.Default.TempPath;
            parser.config.BlueprintDBPath = Settings.Default.BlueprintDBPath;
            parser.config.MaterialDBPath = Settings.Default.MaterialDBPath;
            parser.config.UModelPath = Settings.Default.UViewerPath;
            parser.config.NoesisPath = Settings.Default.NoesisPath;
            parser.config.StaticMeshRawFolder = @"C:\Users\Frank\Desktop\Extra\staticmesh.raw";
        }

        public void fillMapNode(TreeNode node, List<UAsset> uAssets)
        {
            //TRV_Maps.Nodes.Clear();
            //TRV_Maps.ImageList = new ImageList();
            //TRV_Maps.ImageList.Images.Add("mapIcon", Resources.mapIcon);
            //TRV_Maps.ImageList.Images.Add("actorIcon", Resources.actorIcon);
            //TRV_Maps.ImageList.Images.Add("meshIcon", Resources.meshIcon);
            //TRV_Maps.ImageList.Images.Add("textureIcon", Resources.textureIcon);
            uAssets.ForEach(uAsset => { 
                var NodeMap = new TreeNode(uAsset.name) { Tag = uAsset, ImageKey = "mapIcon" };
                var staticMeshActorNode = new TreeNode("Static mesh actors");
                staticMeshActorNode.Nodes.AddRange(uAsset.staticMeshActorList.Select(actor => {
                    var actorNode = new TreeNode(actor.name) { Tag = actor, ImageKey = "actorIcon" };
                    actorNode.Nodes.AddRange(actor.materials.Select(materialName => {
                        if (!uAsset.uMaterials.ContainsKey(materialName))
                            return null;
                        var m = uAsset.uMaterials[materialName];
                        var smNode = new TreeNode(m.name) { Tag = materialName, ImageKey = "meshIcon" };
                        smNode.Nodes.AddRange(m.texturesList.Select(tex => new TreeNode(tex) { Name = tex, Tag = "TEXTURE", ImageKey = "textureIcon" }).ToArray());
                        return smNode;
                    }).Where(x => x != null).ToArray());
                    return actorNode;
                }).ToArray());
                NodeMap.Nodes.Add(staticMeshActorNode);
                node.Nodes.Add(NodeMap);
            });
        }

        private void TRV_Maps_RightClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            ctxMenuForTreeView.Items.Clear();
            var options = new List<ToolStripMenuItem>();
            var nodeObj = e.Node.Tag;
            Action<string, Action> createOption = (string name, Action onClick) =>
            {
                options.Add(new ToolStripMenuItem() { Text = name });
                options[options.Count - 1].Click += (obj, ev) => onClick.Invoke();
            };

            switch (nodeObj)
            {
                case UMapFile uMapFile:
                    if(!uMapFile.loaded)
                        createOption("Load", () => {
                            updateConfig();
                            try
                            {
                                uMapFile.parsedResult = parser.ParseUAsset(uMapFile.path);
                                //uMapFile.parsedResult.config = parser.config;
                                fillMapNode(e.Node, uMapFile.parsedResult.uAssets);
                                uMapFile.loaded = true;
                            }
                            catch (Exception ex) {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                uMapFile.loaded = false;
                            }
                        });
                    break;
                case UAsset uMapAsset:
                    var mapFile = e.Node.Parent.Tag as UMapFile;
                    createOption("Export map metadata", () => {
                        var saveDlg = new SaveFileDialog()
                        {
                            Filter = "uAsset meta file (*.json)|*.json",
                            Title = "Save an JSON file"
                        };
                        saveDlg.ShowDialog();
                        if (saveDlg.FileName != "")
                            ExportHelper.uAsset2Json(uMapAsset, saveDlg.FileName);

                    });
                    createOption("Export map", () => {
                        using (var fbd = new FolderBrowserDialog())
                        {
                            DialogResult result = fbd.ShowDialog();
                            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                            {
                                mapFile.parsedResult.exportHelper.uMapAssetExport(Settings.Default.GamePath, uMapAsset, fbd.SelectedPath);
                            }
                        }
                    });
                    break;
            }
            ctxMenuForTreeView.Items.AddRange(options.ToArray());

        }

        private void TRV_Maps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var details = "";
            switch (e.Node.Tag)
            {
                case UStaticMeshActor actor:
                    var  currentActor = actor;
                    details += $"-- UStaticMeshActor --\r\n";
                    details += $"id: {actor.id}\r\n";
                    details += $"name: {actor.name}\r\n";
                    details += $"Static mesh file: {actor.staticMeshFile}\r\n";
                    details += $"Position: X:{actor.relativeLocation.X} Y:{actor.relativeLocation.Y} Z:{actor.relativeLocation.Z}\r\n";
                    details += $"Rotation: X:{actor.relativeRotation.X} Y:{actor.relativeRotation.Y} Z:{actor.relativeRotation.Z}\r\n";
                    details += $"Scale: X:{actor.relativeScale3D.X} Y:{actor.relativeScale3D.Y} Z:{actor.relativeScale3D.Z}\r\n";
                    //details += $"Overmat: {string.Join(',', actor.overmat.ToString())}\r\n";
                   /* if (this.readResult != null)
                    {
                        details += "SubMeshes materials:\r\n";
                        details += string.Join("\r\n", actor.materials.Select((m, i) => $" {i}) {m}"));
                    }*/
                    break;
                case UMaterial material:
                    details += $"-- UMaterial --\r\n";
                    details += $"id: {material.id}\r\n";
                    details += $"name: {material.name}\r\n";
                    details += $"textureBaseName: {material.textureBaseName}\r\nr\n";
                    break;
                case "TEXTURE":
                    details += $"-- TEXTURE --\r\n";
                    details += $"name: {e.Node.Name}\r\n";
                    break;
            }
            LBL_TreeViewItemDetails.Text = details;
        }
    }
}
