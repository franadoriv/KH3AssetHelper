using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.InteropServices;

using UE;
using System.IO;
using UE.UE4;
using KH3AssetHelper.Helpers;
using KH3AHUI.Properties;

namespace UKH3
{
    public partial class MainForm : Form
    {
        UAssetParserResult readResult;
        UActor currentActor;
        ContextMenuStrip docMenu = new ContextMenuStrip();

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void Initialize()
        {
            var rawDataFolder = "K:/GameRipping/PS4/KH3/Unpacked\u0020RAWs";

            TXT_UAssetPath.Text = @$"K:\GameRipping\PS4\KH3\Unpacked PAKs\Game\Maps\tt\umap\tt_01\tt_01_areaA.umap";
            TXT_BlueprintDB.Text = $"{rawDataFolder}/blueprint.db";
            TXT_MaterialDB.Text = $"{rawDataFolder}/material.db";
            TXT_StaticMeshFolder.Text = $"{rawDataFolder}/staticmesh.raw";
            TXT_SkeletalMeshFolder.Text = $"{rawDataFolder}/skeletalmesh.raw";
            TXT_TexturesFolder.Text = $"{rawDataFolder}/textures.dds";
            TXT_ExportFolder.Text = $"{rawDataFolder}/temp_exports";
            TXT_BlenderPath.Text = @"C:\Program Files\Blender Foundation\Blender 2.83\blender.exe";
            TXT_NoesisPath.Text = @"K:\GameRipping\Tools\noesisv4431\Noesis.exe";
            TXT_UViewerPath.Text = @"K:\GameRipping\Tools\umodel_win32\umodel.exe";

            TRV_uAssets.ContextMenuStrip = docMenu;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void BTN_Start_Click(object sender, EventArgs e)
        {
            var config = new UAParserConfig()
            {
                MaterialDBPath = TXT_MaterialDB.Text,
                BlueprintDBPath = TXT_BlueprintDB.Text,
                StaticMeshRawFolder = TXT_StaticMeshFolder.Text,
                SkeletalMeshRawFolder = TXT_SkeletalMeshFolder.Text,
                TexturesFolder = TXT_TexturesFolder.Text,
                ExportFolder = TXT_ExportFolder.Text,
                BlenderPath = TXT_BlenderPath.Text
            };

            var parser = new UAssetParser(config);

            parser.OnDebugMessage = (txt) => {
                //Console.Write(txt);
            };
            Console.WriteLine("Parsing...");
            this.readResult = parser.ParseUAsset(TXT_UAssetPath.Text);
            Console.WriteLine("Parsed " + this.readResult.uAssets.Count());
            LoadTreeExplorer(this.readResult.uAssets);
            
        }

        public void LoadTreeExplorer(List<UAsset> uAssets)
        {
            TRV_uAssets.Nodes.Clear();
            TRV_uAssets.ImageList = new ImageList();
            TRV_uAssets.ImageList.Images.Add("mapIcon", Resources.mapIcon);
            TRV_uAssets.ImageList.Images.Add("actorIcon", Resources.actorIcon);
            TRV_uAssets.ImageList.Images.Add("meshIcon", Resources.meshIcon);
            TRV_uAssets.ImageList.Images.Add("textureIcon", Resources.textureIcon);
            uAssets.ToList().ForEach(uAsset => {
                var NodeMap = new TreeNode(uAsset.name) { Tag = uAsset,ImageKey= "mapIcon" };
                var staticMeshActorNode = new TreeNode("Static mesh actors");
                staticMeshActorNode.Nodes.AddRange(uAsset.staticMeshActorList.Select(actor => {
                    var node = new TreeNode(actor.name) { Tag = actor,ImageKey="actorIcon" };
                    node.Nodes.AddRange(actor.materials.Select(materialName => {
                        if (!uAsset.uMaterials.ContainsKey(materialName))
                            return null;
                        var m = uAsset.uMaterials[materialName];
                        var smNode = new TreeNode(m.name) { Tag = materialName,ImageKey="meshIcon" };
                        smNode.Nodes.AddRange(m.texturesList.Select(tex => new TreeNode(tex) {Name= tex, Tag = "TEXTURE",ImageKey="textureIcon"}).ToArray());
                        return smNode;
                    }).Where(x=>x!=null).ToArray());
                    return node;
                }).ToArray());
                NodeMap.Nodes.Add(staticMeshActorNode);
                TRV_uAssets.Nodes.Add(NodeMap);
            });
        }

        private void TRV_uAssets_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var details = "";
            switch (e.Node.Tag)
            {
                case UStaticMeshActor actor:
                    currentActor = actor;
                    details += $"-- UStaticMeshActor --\r\n";
                    details += $"id: {actor.id}\r\n";
                    details += $"name: {actor.name}\r\n";
                    details += $"Static mesh file: {actor.staticMeshFile}\r\n";
                    details += $"Position: X:{actor.relativeLocation.X} Y:{actor.relativeLocation.Y} Z:{actor.relativeLocation.Z}\r\n";
                    details += $"Rotation: X:{actor.relativeRotation.X} Y:{actor.relativeRotation.Y} Z:{actor.relativeRotation.Z}\r\n";
                    details += $"Scale: X:{actor.relativeScale3D.X} Y:{actor.relativeScale3D.Y} Z:{actor.relativeScale3D.Z}\r\n";
                    details += $"Overmat: {string.Join(',', actor.overmat)}\r\n";
                    if (this.readResult != null)
                    {
                        details += "SubMeshes materials:\r\n";
                        details += string.Join("\r\n", actor.materials.Select((m,i) => $" {i}) {m}"));
                    }
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
            TXT_ItemDetails.Text = details;
        }

        private void TRV_uAssets_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            docMenu.Items.Clear();
            var options = new List<ToolStripMenuItem>();
            var nodeObj = e.Node.Tag;
            Action<string,Action> createOption = (string name, Action onClick) =>
            {
                options.Add(new ToolStripMenuItem() { Text = name});
                options[options.Count - 1].Click += (obj, ev) => onClick.Invoke();
            };

            switch (nodeObj)
            {
                case UStaticMeshActor subMesh:
                    createOption("Export to ASCII", () => {
                        var saveDlg = new SaveFileDialog()
                        {
                            Filter = "ASCII file (*.ascii)|*.ascii",
                            Title = "Save an ASCII model"
                        };
                        saveDlg.ShowDialog();
                        if (saveDlg.FileName != "")
                        {
                            Console.WriteLine(saveDlg.FileName);
                            var materialDB = readResult.materialDB;
                            readResult.exportHelper.ExportMeshToASCII(subMesh, saveDlg.FileName);
                        }
                    });
                    createOption("Export to FBX", () => {
                        var saveDlg = new SaveFileDialog()
                        {
                            Filter = "ASCII file (*.fbx)|*.fbx",
                            Title = "Save an FBX model"
                        };
                        saveDlg.ShowDialog();
                        if (saveDlg.FileName != "")
                        {
                            Console.WriteLine(saveDlg.FileName);
                            var materialDB = readResult.materialDB;
                            readResult.exportHelper.ASCII2FBX(subMesh, saveDlg.FileName);
                        }
                    });
                    break;
                case UAsset uMapAsset:
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
                        using (var fbd = new FolderBrowserDialog()) {
                            DialogResult result = fbd.ShowDialog();
                            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                                this.readResult.exportHelper.uMapAssetExport(@"K:\GameRipping\PS4\KH3\Unpacked PKGs", uMapAsset, fbd.SelectedPath);
                            }
                        }
                    });
                    break;
                case "TEXTURE":
                    createOption("Export to PNG", () => { });
                    break;
            }
            docMenu.Items.AddRange(options.ToArray());

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ExportHelper.DDS2TGA(@"C:\Users\Adolfo\Desktop\FinalRip\textures\T_tt_Arc_BuildingDAh_D.dds", @"C:\Users\Adolfo\Desktop\omg.png");
            // ExportHelper.uAssetMdl2FBX(TXT_ExportFolder.Text);
            ExportHelper.uModelConvert(@"K:\GameRipping\PS4\KH3\Unpacked PKGs", "*/SM_tt_Arc_BridgeB.uasset", @"C:\Users\Adolfo\Desktop\FinalRip\export");
        }
    }
}
