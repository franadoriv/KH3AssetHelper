﻿namespace KH3MapsExporter {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TRV_Maps = new System.Windows.Forms.TreeView();
            this.CMB_MapsIDsList = new System.Windows.Forms.ComboBox();
            this.LBL_TreeViewItemDetails = new System.Windows.Forms.Label();
            this.TABC_Main = new System.Windows.Forms.TabControl();
            this.TAB_Explorer = new System.Windows.Forms.TabPage();
            this.TAB_Config = new System.Windows.Forms.TabPage();
            this.TBL_CnfOptions = new System.Windows.Forms.TableLayoutPanel();
            this.BTN_UViewerPath = new System.Windows.Forms.Button();
            this.BTN_NoesisPath = new System.Windows.Forms.Button();
            this.BTN_TempFolderPath = new System.Windows.Forms.Button();
            this.TXT_UVIewerPath = new System.Windows.Forms.TextBox();
            this.TXT_NoesisPath = new System.Windows.Forms.TextBox();
            this.TXT_TempFolderPath = new System.Windows.Forms.TextBox();
            this.LBL_UModelViewPath = new System.Windows.Forms.Label();
            this.LBL_NoesisPath = new System.Windows.Forms.Label();
            this.LBL_TempFolderPath = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.TABC_Main.SuspendLayout();
            this.TAB_Explorer.SuspendLayout();
            this.TAB_Config.SuspendLayout();
            this.TBL_CnfOptions.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.LBL_TreeViewItemDetails);
            this.splitContainer1.Size = new System.Drawing.Size(766, 422);
            this.splitContainer1.SplitterDistance = 255;
            this.splitContainer1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TRV_Maps, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CMB_MapsIDsList, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(255, 422);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // TRV_Maps
            // 
            this.TRV_Maps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TRV_Maps.Location = new System.Drawing.Point(3, 23);
            this.TRV_Maps.Name = "TRV_Maps";
            this.TRV_Maps.Size = new System.Drawing.Size(249, 396);
            this.TRV_Maps.TabIndex = 3;
            // 
            // CMB_MapsIDsList
            // 
            this.CMB_MapsIDsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.CMB_MapsIDsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_MapsIDsList.FormattingEnabled = true;
            this.CMB_MapsIDsList.Location = new System.Drawing.Point(3, 3);
            this.CMB_MapsIDsList.Name = "CMB_MapsIDsList";
            this.CMB_MapsIDsList.Size = new System.Drawing.Size(249, 21);
            this.CMB_MapsIDsList.TabIndex = 2;
            this.CMB_MapsIDsList.SelectedValueChanged += new System.EventHandler(this.CMB_MapsIDsList_SelectedValueChanged);
            // 
            // LBL_TreeViewItemDetails
            // 
            this.LBL_TreeViewItemDetails.AutoSize = true;
            this.LBL_TreeViewItemDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBL_TreeViewItemDetails.Location = new System.Drawing.Point(0, 0);
            this.LBL_TreeViewItemDetails.Name = "LBL_TreeViewItemDetails";
            this.LBL_TreeViewItemDetails.Size = new System.Drawing.Size(0, 15);
            this.LBL_TreeViewItemDetails.TabIndex = 0;
            // 
            // TABC_Main
            // 
            this.TABC_Main.Controls.Add(this.TAB_Explorer);
            this.TABC_Main.Controls.Add(this.TAB_Config);
            this.TABC_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TABC_Main.Location = new System.Drawing.Point(0, 28);
            this.TABC_Main.Name = "TABC_Main";
            this.TABC_Main.SelectedIndex = 0;
            this.TABC_Main.Size = new System.Drawing.Size(780, 454);
            this.TABC_Main.TabIndex = 2;
            // 
            // TAB_Explorer
            // 
            this.TAB_Explorer.Controls.Add(this.splitContainer1);
            this.TAB_Explorer.Location = new System.Drawing.Point(4, 22);
            this.TAB_Explorer.Name = "TAB_Explorer";
            this.TAB_Explorer.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Explorer.Size = new System.Drawing.Size(772, 428);
            this.TAB_Explorer.TabIndex = 0;
            this.TAB_Explorer.Text = "Explorer";
            this.TAB_Explorer.UseVisualStyleBackColor = true;
            // 
            // TAB_Config
            // 
            this.TAB_Config.Controls.Add(this.TBL_CnfOptions);
            this.TAB_Config.Location = new System.Drawing.Point(4, 22);
            this.TAB_Config.Name = "TAB_Config";
            this.TAB_Config.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Config.Size = new System.Drawing.Size(772, 428);
            this.TAB_Config.TabIndex = 1;
            this.TAB_Config.Text = "Config";
            this.TAB_Config.UseVisualStyleBackColor = true;
            // 
            // TBL_CnfOptions
            // 
            this.TBL_CnfOptions.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TBL_CnfOptions.ColumnCount = 3;
            this.TBL_CnfOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TBL_CnfOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TBL_CnfOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.TBL_CnfOptions.Controls.Add(this.BTN_UViewerPath, 2, 0);
            this.TBL_CnfOptions.Controls.Add(this.BTN_NoesisPath, 2, 1);
            this.TBL_CnfOptions.Controls.Add(this.BTN_TempFolderPath, 2, 2);
            this.TBL_CnfOptions.Controls.Add(this.TXT_UVIewerPath, 1, 0);
            this.TBL_CnfOptions.Controls.Add(this.TXT_NoesisPath, 1, 1);
            this.TBL_CnfOptions.Controls.Add(this.TXT_TempFolderPath, 1, 2);
            this.TBL_CnfOptions.Controls.Add(this.LBL_UModelViewPath, 0, 0);
            this.TBL_CnfOptions.Controls.Add(this.LBL_NoesisPath, 0, 1);
            this.TBL_CnfOptions.Controls.Add(this.LBL_TempFolderPath, 0, 2);
            this.TBL_CnfOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.TBL_CnfOptions.Location = new System.Drawing.Point(3, 3);
            this.TBL_CnfOptions.Name = "TBL_CnfOptions";
            this.TBL_CnfOptions.RowCount = 3;
            this.TBL_CnfOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TBL_CnfOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TBL_CnfOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.TBL_CnfOptions.Size = new System.Drawing.Size(766, 151);
            this.TBL_CnfOptions.TabIndex = 0;
            // 
            // BTN_UViewerPath
            // 
            this.BTN_UViewerPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_UViewerPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BTN_UViewerPath.Location = new System.Drawing.Point(712, 4);
            this.BTN_UViewerPath.Name = "BTN_UViewerPath";
            this.BTN_UViewerPath.Size = new System.Drawing.Size(50, 43);
            this.BTN_UViewerPath.TabIndex = 0;
            this.BTN_UViewerPath.UseVisualStyleBackColor = true;
            this.BTN_UViewerPath.Click += new System.EventHandler(this.BTN_UViewerPath_Click);
            // 
            // BTN_NoesisPath
            // 
            this.BTN_NoesisPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_NoesisPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BTN_NoesisPath.Location = new System.Drawing.Point(712, 54);
            this.BTN_NoesisPath.Name = "BTN_NoesisPath";
            this.BTN_NoesisPath.Size = new System.Drawing.Size(50, 43);
            this.BTN_NoesisPath.TabIndex = 1;
            this.BTN_NoesisPath.UseVisualStyleBackColor = true;
            this.BTN_NoesisPath.Click += new System.EventHandler(this.BTN_NoesisPath_Click);
            // 
            // BTN_TempFolderPath
            // 
            this.BTN_TempFolderPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTN_TempFolderPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BTN_TempFolderPath.Location = new System.Drawing.Point(712, 104);
            this.BTN_TempFolderPath.Name = "BTN_TempFolderPath";
            this.BTN_TempFolderPath.Size = new System.Drawing.Size(50, 43);
            this.BTN_TempFolderPath.TabIndex = 2;
            this.BTN_TempFolderPath.UseVisualStyleBackColor = true;
            this.BTN_TempFolderPath.Click += new System.EventHandler(this.BTN_TempFolderPath_Click);
            // 
            // TXT_UVIewerPath
            // 
            this.TXT_UVIewerPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_UVIewerPath.Location = new System.Drawing.Point(105, 21);
            this.TXT_UVIewerPath.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.TXT_UVIewerPath.Name = "TXT_UVIewerPath";
            this.TXT_UVIewerPath.Size = new System.Drawing.Size(600, 20);
            this.TXT_UVIewerPath.TabIndex = 3;
            // 
            // TXT_NoesisPath
            // 
            this.TXT_NoesisPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_NoesisPath.Location = new System.Drawing.Point(105, 71);
            this.TXT_NoesisPath.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.TXT_NoesisPath.Name = "TXT_NoesisPath";
            this.TXT_NoesisPath.Size = new System.Drawing.Size(600, 20);
            this.TXT_NoesisPath.TabIndex = 4;
            // 
            // TXT_TempFolderPath
            // 
            this.TXT_TempFolderPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_TempFolderPath.Location = new System.Drawing.Point(105, 121);
            this.TXT_TempFolderPath.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.TXT_TempFolderPath.Name = "TXT_TempFolderPath";
            this.TXT_TempFolderPath.Size = new System.Drawing.Size(600, 20);
            this.TXT_TempFolderPath.TabIndex = 5;
            // 
            // LBL_UModelViewPath
            // 
            this.LBL_UModelViewPath.AutoSize = true;
            this.LBL_UModelViewPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBL_UModelViewPath.Location = new System.Drawing.Point(4, 1);
            this.LBL_UModelViewPath.Name = "LBL_UModelViewPath";
            this.LBL_UModelViewPath.Size = new System.Drawing.Size(94, 49);
            this.LBL_UModelViewPath.TabIndex = 6;
            this.LBL_UModelViewPath.Text = "UViewer path:";
            this.LBL_UModelViewPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LBL_NoesisPath
            // 
            this.LBL_NoesisPath.AutoSize = true;
            this.LBL_NoesisPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBL_NoesisPath.Location = new System.Drawing.Point(4, 51);
            this.LBL_NoesisPath.Name = "LBL_NoesisPath";
            this.LBL_NoesisPath.Size = new System.Drawing.Size(94, 49);
            this.LBL_NoesisPath.TabIndex = 7;
            this.LBL_NoesisPath.Text = "Noesis path:";
            this.LBL_NoesisPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LBL_TempFolderPath
            // 
            this.LBL_TempFolderPath.AutoSize = true;
            this.LBL_TempFolderPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBL_TempFolderPath.Location = new System.Drawing.Point(4, 101);
            this.LBL_TempFolderPath.Name = "LBL_TempFolderPath";
            this.LBL_TempFolderPath.Size = new System.Drawing.Size(94, 49);
            this.LBL_TempFolderPath.TabIndex = 8;
            this.LBL_TempFolderPath.Text = "Temp folder:";
            this.LBL_TempFolderPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(780, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanFolderToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // scanFolderToolStripMenuItem
            // 
            this.scanFolderToolStripMenuItem.Name = "scanFolderToolStripMenuItem";
            this.scanFolderToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.scanFolderToolStripMenuItem.Text = "Scan folder";
            this.scanFolderToolStripMenuItem.Click += new System.EventHandler(this.scanFolderToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(167, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(780, 482);
            this.Controls.Add(this.TABC_Main);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Kingdom Hearts 3 - Map exporter";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.TABC_Main.ResumeLayout(false);
            this.TAB_Explorer.ResumeLayout(false);
            this.TAB_Config.ResumeLayout(false);
            this.TBL_CnfOptions.ResumeLayout(false);
            this.TBL_CnfOptions.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label LBL_TreeViewItemDetails;
        private System.Windows.Forms.TabControl TABC_Main;
        private System.Windows.Forms.TabPage TAB_Explorer;
        private System.Windows.Forms.TabPage TAB_Config;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel TBL_CnfOptions;
        private System.Windows.Forms.Button BTN_UViewerPath;
        private System.Windows.Forms.Button BTN_NoesisPath;
        private System.Windows.Forms.Button BTN_TempFolderPath;
        private System.Windows.Forms.TextBox TXT_UVIewerPath;
        private System.Windows.Forms.TextBox TXT_NoesisPath;
        private System.Windows.Forms.TextBox TXT_TempFolderPath;
        private System.Windows.Forms.Label LBL_UModelViewPath;
        private System.Windows.Forms.Label LBL_NoesisPath;
        private System.Windows.Forms.Label LBL_TempFolderPath;
        private System.Windows.Forms.ToolStripMenuItem scanFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView TRV_Maps;
        private System.Windows.Forms.ComboBox CMB_MapsIDsList;
    }
}
