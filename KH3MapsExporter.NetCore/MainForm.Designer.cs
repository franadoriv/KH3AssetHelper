using System;

namespace UKH3
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            
            this.LBL_CnfBlueprintDB = new System.Windows.Forms.Label();
            this.LBL_CnfMaterialDB = new System.Windows.Forms.Label();
            this.LBL_CnfStaticMesh = new System.Windows.Forms.Label();
            this.LBL_CnfTextureFolder = new System.Windows.Forms.Label();
            this.LBL_CnfSkeletalMesh = new System.Windows.Forms.Label();
            this.LBL_CnfExportFolder = new System.Windows.Forms.Label();

            this.TXT_MaterialDB = new System.Windows.Forms.TextBox();
            this.TXT_BlenderPath = new System.Windows.Forms.TextBox();
            this.TXT_BlueprintDB = new System.Windows.Forms.TextBox();
            this.TXT_StaticMeshFolder = new System.Windows.Forms.TextBox();
            this.TXT_SkeletalMeshFolder = new System.Windows.Forms.TextBox();
            this.TXT_TexturesFolder = new System.Windows.Forms.TextBox();
            this.TXT_ExportFolder = new System.Windows.Forms.TextBox();
            this.TXT_ItemDetails = new System.Windows.Forms.TextBox();
            this.TXT_UAssetPath = new System.Windows.Forms.TextBox();
            this.TXT_NoesisPath = new System.Windows.Forms.TextBox();
            this.TXT_UViewerPath = new System.Windows.Forms.TextBox();

            this.BTN_SelectBlueprintDB = new System.Windows.Forms.Button();
            this.BTN_SelectRawMeshFolder = new System.Windows.Forms.Button();
            this.BTN_SelectMaterialDB = new System.Windows.Forms.Button();
            this.BTN_SelectSkeletalMeshFolder = new System.Windows.Forms.Button();
            this.BTN_SelectTextureFolder = new System.Windows.Forms.Button();
            this.BTN_SelectExportFolder = new System.Windows.Forms.Button();
            this.BTN_Start = new System.Windows.Forms.Button();
            this.BTN_ReadUAsset = new System.Windows.Forms.Button();
            this.BTN_CustomAction = new System.Windows.Forms.Button();
            this.BTN_SelectNoesisPath = new System.Windows.Forms.Button();
            this.BTN_SelectUViewerPath = new System.Windows.Forms.Button();
            this.BTN_SelectBlederPath = new System.Windows.Forms.Button();

            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();

            this.TAB_Page_UAssetExp = new System.Windows.Forms.TabPage();
            this.TAB_Page_Config = new System.Windows.Forms.TabPage();

            this.TRV_uAssets = new System.Windows.Forms.TreeView();

            this.tabControl1 = new System.Windows.Forms.TabControl();

            this.SPLC_UA_ExpA = new System.Windows.Forms.SplitContainer();
            this.SLC_UA_ExpB = new System.Windows.Forms.SplitContainer();


            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TAB_Page_UAssetExp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SPLC_UA_ExpA)).BeginInit();
            this.SPLC_UA_ExpA.Panel1.SuspendLayout();
            this.SPLC_UA_ExpA.Panel2.SuspendLayout();
            this.SPLC_UA_ExpA.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SLC_UA_ExpB)).BeginInit();
            this.SLC_UA_ExpB.Panel1.SuspendLayout();
            this.SLC_UA_ExpB.Panel2.SuspendLayout();
            this.SLC_UA_ExpB.SuspendLayout();
            this.TAB_Page_Config.SuspendLayout();
            this.SuspendLayout();

            // 
            // BTN_SelectMaterialDB
            // 
            this.BTN_SelectMaterialDB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectMaterialDB.BackgroundImage")));
            this.BTN_SelectMaterialDB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectMaterialDB.Location = new System.Drawing.Point(426, 61);
            this.BTN_SelectMaterialDB.Name = "BTN_SelectMaterialDB";
            this.BTN_SelectMaterialDB.Size = new System.Drawing.Size(26, 23);
            this.BTN_SelectMaterialDB.TabIndex = 0;
            this.BTN_SelectMaterialDB.UseVisualStyleBackColor = true;
            // 
            // TXT_MaterialDB
            // 
            this.TXT_MaterialDB.Location = new System.Drawing.Point(3, 61);
            this.TXT_MaterialDB.Name = "TXT_MaterialDB";
            this.TXT_MaterialDB.Size = new System.Drawing.Size(417, 23);
            this.TXT_MaterialDB.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfBlenderPath);
            this.flowLayoutPanel1.Controls.Add(this.TXT_BlenderPath);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectBlederPath);

            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfNoesisPath);
            this.flowLayoutPanel1.Controls.Add(this.TXT_NoesisPath);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectNoesisPath);

            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfUViewerPath);
            this.flowLayoutPanel1.Controls.Add(this.TXT_UViewerPath);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectUViewerPath);


            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfBlueprintDB);
            this.flowLayoutPanel1.Controls.Add(this.TXT_BlueprintDB);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectBlueprintDB);
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfMaterialDB);
            this.flowLayoutPanel1.Controls.Add(this.TXT_MaterialDB);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectMaterialDB);
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfStaticMesh);
            this.flowLayoutPanel1.Controls.Add(this.TXT_StaticMeshFolder);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectRawMeshFolder);
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfSkeletalMesh);
            this.flowLayoutPanel1.Controls.Add(this.TXT_SkeletalMeshFolder);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectSkeletalMeshFolder);
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfTextureFolder);
            this.flowLayoutPanel1.Controls.Add(this.TXT_TexturesFolder);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectTextureFolder);
            this.flowLayoutPanel1.Controls.Add(this.LBL_CnfExportFolder);
            this.flowLayoutPanel1.Controls.Add(this.TXT_ExportFolder);
            this.flowLayoutPanel1.Controls.Add(this.BTN_SelectExportFolder);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(611, 374);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // TXT_BlenderPath
            // 
            this.TXT_BlenderPath.Location = new System.Drawing.Point(3, 3);
            this.TXT_BlenderPath.Name = "TXT_BlenderPath";
            this.TXT_BlenderPath.Size = new System.Drawing.Size(416, 23);
            this.TXT_BlenderPath.TabIndex = 2;
            // 
            // LBL_CnfBlueprintDB
            // 
            this.LBL_CnfBlueprintDB.AutoSize = true;
            this.LBL_CnfBlueprintDB.Location = new System.Drawing.Point(425, 0);
            this.LBL_CnfBlueprintDB.Name = "LBL_CnfBlueprintDB";
            this.LBL_CnfBlueprintDB.Size = new System.Drawing.Size(103, 15);
            this.LBL_CnfBlueprintDB.TabIndex = 11;
            this.LBL_CnfBlueprintDB.Text = "BlueprintDB path: ";
            // 
            // TXT_BlueprintDB
            // 
            this.TXT_BlueprintDB.Location = new System.Drawing.Point(3, 32);
            this.TXT_BlueprintDB.Name = "TXT_BlueprintDB";
            this.TXT_BlueprintDB.Size = new System.Drawing.Size(418, 23);
            this.TXT_BlueprintDB.TabIndex = 2;
            // 
            // BTN_SelectBlueprintDB
            // 
            this.BTN_SelectBlueprintDB.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectBlueprintDB.BackgroundImage")));
            this.BTN_SelectBlueprintDB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectBlueprintDB.Location = new System.Drawing.Point(427, 32);
            this.BTN_SelectBlueprintDB.Name = "BTN_SelectBlueprintDB";
            this.BTN_SelectBlueprintDB.Size = new System.Drawing.Size(25, 23);
            this.BTN_SelectBlueprintDB.TabIndex = 0;
            this.BTN_SelectBlueprintDB.UseVisualStyleBackColor = true;
            // 
            // LBL_CnfMaterialDB
            // 
            this.LBL_CnfMaterialDB.AutoSize = true;
            this.LBL_CnfMaterialDB.Location = new System.Drawing.Point(458, 29);
            this.LBL_CnfMaterialDB.Name = "LBL_CnfMaterialDB";
            this.LBL_CnfMaterialDB.Size = new System.Drawing.Size(98, 15);
            this.LBL_CnfMaterialDB.TabIndex = 11;
            this.LBL_CnfMaterialDB.Text = "MaterialDB path: ";
            // 
            // LBL_CnfStaticMesh
            // 
            this.LBL_CnfStaticMesh.AutoSize = true;
            this.LBL_CnfStaticMesh.Location = new System.Drawing.Point(458, 58);
            this.LBL_CnfStaticMesh.Name = "LBL_CnfStaticMesh";
            this.LBL_CnfStaticMesh.Size = new System.Drawing.Size(108, 15);
            this.LBL_CnfStaticMesh.TabIndex = 11;
            this.LBL_CnfStaticMesh.Text = "Static mesh folder: ";
            // 
            // TXT_StaticMeshFolder
            // 
            this.TXT_StaticMeshFolder.Location = new System.Drawing.Point(3, 90);
            this.TXT_StaticMeshFolder.Name = "TXT_StaticMeshFolder";
            this.TXT_StaticMeshFolder.Size = new System.Drawing.Size(417, 23);
            this.TXT_StaticMeshFolder.TabIndex = 2;
            // 
            // BTN_SelectRawMeshFolder
            // 
            this.BTN_SelectRawMeshFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectRawMeshFolder.BackgroundImage")));
            this.BTN_SelectRawMeshFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectRawMeshFolder.Location = new System.Drawing.Point(426, 90);
            this.BTN_SelectRawMeshFolder.Name = "BTN_SelectRawMeshFolder";
            this.BTN_SelectRawMeshFolder.Size = new System.Drawing.Size(23, 23);
            this.BTN_SelectRawMeshFolder.TabIndex = 0;
            this.BTN_SelectRawMeshFolder.UseVisualStyleBackColor = true;
            // 
            // LBL_CnfSkeletalMesh
            // 
            this.LBL_CnfSkeletalMesh.AutoSize = true;
            this.LBL_CnfSkeletalMesh.Location = new System.Drawing.Point(455, 87);
            this.LBL_CnfSkeletalMesh.Name = "LBL_CnfSkeletalMesh";
            this.LBL_CnfSkeletalMesh.Size = new System.Drawing.Size(119, 15);
            this.LBL_CnfSkeletalMesh.TabIndex = 11;
            this.LBL_CnfSkeletalMesh.Text = "Skeletal mesh folder: ";
            // 
            // TXT_SkeletalMeshFolder
            // 
            this.TXT_SkeletalMeshFolder.Location = new System.Drawing.Point(3, 119);
            this.TXT_SkeletalMeshFolder.Name = "TXT_SkeletalMeshFolder";
            this.TXT_SkeletalMeshFolder.Size = new System.Drawing.Size(417, 23);
            this.TXT_SkeletalMeshFolder.TabIndex = 2;
            // 
            // BTN_SelectSkeletalMeshFolder
            // 
            this.BTN_SelectSkeletalMeshFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectSkeletalMeshFolder.BackgroundImage")));
            this.BTN_SelectSkeletalMeshFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectSkeletalMeshFolder.Location = new System.Drawing.Point(426, 119);
            this.BTN_SelectSkeletalMeshFolder.Name = "BTN_SelectSkeletalMeshFolder";
            this.BTN_SelectSkeletalMeshFolder.Size = new System.Drawing.Size(25, 23);
            this.BTN_SelectSkeletalMeshFolder.TabIndex = 0;
            this.BTN_SelectSkeletalMeshFolder.UseVisualStyleBackColor = true;
            // 
            // LBL_CnfTextureFolder
            // 
            this.LBL_CnfTextureFolder.AutoSize = true;
            this.LBL_CnfTextureFolder.Location = new System.Drawing.Point(457, 116);
            this.LBL_CnfTextureFolder.Name = "LBL_CnfTextureFolder";
            this.LBL_CnfTextureFolder.Size = new System.Drawing.Size(90, 15);
            this.LBL_CnfTextureFolder.TabIndex = 11;
            this.LBL_CnfTextureFolder.Text = "Textures folder: ";
            // 
            // TXT_TexturesFolder
            // 
            this.TXT_TexturesFolder.Location = new System.Drawing.Point(3, 148);
            this.TXT_TexturesFolder.Name = "TXT_TexturesFolder";
            this.TXT_TexturesFolder.Size = new System.Drawing.Size(417, 23);
            this.TXT_TexturesFolder.TabIndex = 2;
            // 
            // BTN_SelectTextureFolder
            // 
            this.BTN_SelectTextureFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectTextureFolder.BackgroundImage")));
            this.BTN_SelectTextureFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectTextureFolder.Location = new System.Drawing.Point(426, 148);
            this.BTN_SelectTextureFolder.Name = "BTN_SelectTextureFolder";
            this.BTN_SelectTextureFolder.Size = new System.Drawing.Size(25, 23);
            this.BTN_SelectTextureFolder.TabIndex = 0;
            this.BTN_SelectTextureFolder.UseVisualStyleBackColor = true;
            // 
            // LBL_CnfExportFolder
            // 
            this.LBL_CnfExportFolder.AutoSize = true;
            this.LBL_CnfExportFolder.Location = new System.Drawing.Point(457, 145);
            this.LBL_CnfExportFolder.Name = "LBL_CnfExportFolder";
            this.LBL_CnfExportFolder.Size = new System.Drawing.Size(81, 15);
            this.LBL_CnfExportFolder.TabIndex = 11;
            this.LBL_CnfExportFolder.Text = "Export folder: ";
            // 
            // TXT_ExportFolder
            // 
            this.TXT_ExportFolder.Location = new System.Drawing.Point(3, 177);
            this.TXT_ExportFolder.Name = "TXT_ExportFolder";
            this.TXT_ExportFolder.Size = new System.Drawing.Size(416, 23);
            this.TXT_ExportFolder.TabIndex = 2;
            // 
            // BTN_SelectExportFolder
            // 
            this.BTN_SelectExportFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_SelectExportFolder.BackgroundImage")));
            this.BTN_SelectExportFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_SelectExportFolder.Location = new System.Drawing.Point(425, 177);
            this.BTN_SelectExportFolder.Name = "BTN_SelectExportFolder";
            this.BTN_SelectExportFolder.Size = new System.Drawing.Size(25, 23);
            this.BTN_SelectExportFolder.TabIndex = 0;
            this.BTN_SelectExportFolder.UseVisualStyleBackColor = true;
            // 
            // BTN_Start
            // 
            this.BTN_Start.Location = new System.Drawing.Point(490, 3);
            this.BTN_Start.Name = "BTN_Start";
            this.BTN_Start.Size = new System.Drawing.Size(78, 23);
            this.BTN_Start.TabIndex = 4;
            this.BTN_Start.Text = "Start";
            this.BTN_Start.UseVisualStyleBackColor = true;
            this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
            // 
            // TRV_uAssets
            // 
            this.TRV_uAssets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TRV_uAssets.Location = new System.Drawing.Point(0, 0);
            this.TRV_uAssets.Name = "TRV_uAssets";
            this.TRV_uAssets.Size = new System.Drawing.Size(186, 337);
            this.TRV_uAssets.TabIndex = 7;
            this.TRV_uAssets.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_uAssets_AfterSelect);
            this.TRV_uAssets.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TRV_uAssets_NodeMouseClick);
            // 
            // TXT_ItemDetails
            // 
            this.TXT_ItemDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_ItemDetails.Location = new System.Drawing.Point(0, 0);
            this.TXT_ItemDetails.Multiline = true;
            this.TXT_ItemDetails.Name = "TXT_ItemDetails";
            this.TXT_ItemDetails.ReadOnly = true;
            this.TXT_ItemDetails.Size = new System.Drawing.Size(384, 337);
            this.TXT_ItemDetails.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TAB_Page_UAssetExp);
            this.tabControl1.Controls.Add(this.TAB_Page_Config);
            this.tabControl1.Location = new System.Drawing.Point(12, 44);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(588, 405);
            this.tabControl1.TabIndex = 10;
            // 
            // TAB_Page_UAssetExp
            // 
            this.TAB_Page_UAssetExp.Controls.Add(this.SPLC_UA_ExpA);
            this.TAB_Page_UAssetExp.Location = new System.Drawing.Point(4, 24);
            this.TAB_Page_UAssetExp.Name = "TAB_Page_UAssetExp";
            this.TAB_Page_UAssetExp.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Page_UAssetExp.Size = new System.Drawing.Size(580, 377);
            this.TAB_Page_UAssetExp.TabIndex = 0;
            this.TAB_Page_UAssetExp.Text = "UAsset";
            this.TAB_Page_UAssetExp.UseVisualStyleBackColor = true;
            // 
            // SPLC_UA_ExpA
            // 
            this.SPLC_UA_ExpA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SPLC_UA_ExpA.Location = new System.Drawing.Point(3, 3);
            this.SPLC_UA_ExpA.Name = "SPLC_UA_ExpA";
            this.SPLC_UA_ExpA.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SPLC_UA_ExpA.Panel1
            // 
            this.SPLC_UA_ExpA.Panel1.Controls.Add(this.flowLayoutPanel2);
            // 
            // SPLC_UA_ExpA.Panel2
            // 
            this.SPLC_UA_ExpA.Panel2.Controls.Add(this.SLC_UA_ExpB);
            this.SPLC_UA_ExpA.Size = new System.Drawing.Size(574, 371);
            this.SPLC_UA_ExpA.SplitterDistance = 30;
            this.SPLC_UA_ExpA.TabIndex = 11;
            this.SPLC_UA_ExpA.Text = "splitContainer2";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.BTN_ReadUAsset);
            this.flowLayoutPanel2.Controls.Add(this.TXT_UAssetPath);
            this.flowLayoutPanel2.Controls.Add(this.BTN_Start);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(574, 30);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // BTN_ReadUAsset
            // 
            this.BTN_ReadUAsset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BTN_ReadUAsset.BackgroundImage")));
            this.BTN_ReadUAsset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_ReadUAsset.Location = new System.Drawing.Point(3, 3);
            this.BTN_ReadUAsset.Name = "BTN_ReadUAsset";
            this.BTN_ReadUAsset.Size = new System.Drawing.Size(28, 28);
            this.BTN_ReadUAsset.TabIndex = 0;
            this.BTN_ReadUAsset.UseVisualStyleBackColor = true;
            // 
            // TXT_UAssetPath
            // 
            this.TXT_UAssetPath.Location = new System.Drawing.Point(37, 3);
            this.TXT_UAssetPath.Name = "TXT_UAssetPath";
            this.TXT_UAssetPath.Size = new System.Drawing.Size(447, 23);
            this.TXT_UAssetPath.TabIndex = 2;
            // 
            // SLC_UA_ExpB
            // 
            this.SLC_UA_ExpB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SLC_UA_ExpB.Location = new System.Drawing.Point(0, 0);
            this.SLC_UA_ExpB.Name = "SLC_UA_ExpB";
            // 
            // SLC_UA_ExpB.Panel1
            // 
            this.SLC_UA_ExpB.Panel1.Controls.Add(this.TRV_uAssets);
            // 
            // SLC_UA_ExpB.Panel2
            // 
            this.SLC_UA_ExpB.Panel2.Controls.Add(this.TXT_ItemDetails);
            this.SLC_UA_ExpB.Size = new System.Drawing.Size(574, 337);
            this.SLC_UA_ExpB.SplitterDistance = 186;
            this.SLC_UA_ExpB.TabIndex = 0;
            this.SLC_UA_ExpB.Text = "splitContainer1";
            // 
            // TAB_Page_Config
            // 
            this.TAB_Page_Config.Controls.Add(this.flowLayoutPanel1);
            this.TAB_Page_Config.Location = new System.Drawing.Point(4, 24);
            this.TAB_Page_Config.Name = "TAB_Page_Config";
            this.TAB_Page_Config.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Page_Config.Size = new System.Drawing.Size(580, 377);
            this.TAB_Page_Config.TabIndex = 1;
            this.TAB_Page_Config.Text = "Config";
            this.TAB_Page_Config.UseVisualStyleBackColor = true;
            // 
            // BTN_CustomAction
            // 
            this.BTN_CustomAction.Location = new System.Drawing.Point(497, 12);
            this.BTN_CustomAction.Name = "BTN_CustomAction";
            this.BTN_CustomAction.Size = new System.Drawing.Size(99, 23);
            this.BTN_CustomAction.TabIndex = 11;
            this.BTN_CustomAction.Text = "Custom action";
            this.BTN_CustomAction.UseVisualStyleBackColor = true;
            this.BTN_CustomAction.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 458);
            this.Controls.Add(this.BTN_CustomAction);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.TAB_Page_UAssetExp.ResumeLayout(false);
            this.SPLC_UA_ExpA.Panel1.ResumeLayout(false);
            this.SPLC_UA_ExpA.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SPLC_UA_ExpA)).EndInit();
            this.SPLC_UA_ExpA.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.SLC_UA_ExpB.Panel1.ResumeLayout(false);
            this.SLC_UA_ExpB.Panel2.ResumeLayout(false);
            this.SLC_UA_ExpB.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SLC_UA_ExpB)).EndInit();
            this.SLC_UA_ExpB.ResumeLayout(false);
            this.TAB_Page_Config.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion


        private System.Windows.Forms.TreeView TRV_uAssets;

        private System.Windows.Forms.TabControl tabControl1;

        private System.Windows.Forms.TabPage TAB_Page_UAssetExp;
        private System.Windows.Forms.TabPage TAB_Page_Config;

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;

        private System.Windows.Forms.SplitContainer SLC_UA_ExpB;

        private System.Windows.Forms.SplitContainer SPLC_UA_ExpA;

        private System.Windows.Forms.TextBox TXT_UAssetPath;
        private System.Windows.Forms.TextBox TXT_MaterialDB;
        private System.Windows.Forms.TextBox TXT_BlueprintDB;

        private System.Windows.Forms.TextBox TXT_StaticMeshFolder;
        private System.Windows.Forms.TextBox TXT_SkeletalMeshFolder;
        private System.Windows.Forms.TextBox TXT_TexturesFolder;
        private System.Windows.Forms.TextBox TXT_ExportFolder;

        private System.Windows.Forms.TextBox TXT_BlenderPath;
        private System.Windows.Forms.TextBox TXT_NoesisPath;
        private System.Windows.Forms.TextBox TXT_UViewerPath;

        private System.Windows.Forms.TextBox TXT_ItemDetails;


        private System.Windows.Forms.Label LBL_CnfBlueprintDB;
        private System.Windows.Forms.Label LBL_CnfTextureFolder;
        private System.Windows.Forms.Label LBL_CnfStaticMesh;
        private System.Windows.Forms.Label LBL_CnfSkeletalMesh;
        private System.Windows.Forms.Label LBL_CnfMaterialDB;
        private System.Windows.Forms.Label LBL_CnfExportFolder;

        private System.Windows.Forms.Label LBL_CnfBlenderPath;
        private System.Windows.Forms.Label LBL_CnfNoesisPath;
        private System.Windows.Forms.Label LBL_CnfUViewerPath;

        private System.Windows.Forms.Button BTN_SelectSkeletalMeshFolder;
        private System.Windows.Forms.Button BTN_SelectTextureFolder;
        private System.Windows.Forms.Button BTN_CustomAction;
        private System.Windows.Forms.Button BTN_SelectExportFolder;
        private System.Windows.Forms.Button BTN_ReadUAsset;
        private System.Windows.Forms.Button BTN_SelectMaterialDB;
        private System.Windows.Forms.Button BTN_SelectBlueprintDB;
        private System.Windows.Forms.Button BTN_Start;
        private System.Windows.Forms.Button BTN_SelectRawMeshFolder;
        private System.Windows.Forms.Button BTN_SelectNoesisPath;
        private System.Windows.Forms.Button BTN_SelectUViewerPath;
        private System.Windows.Forms.Button BTN_SelectBlederPath;
    }
}