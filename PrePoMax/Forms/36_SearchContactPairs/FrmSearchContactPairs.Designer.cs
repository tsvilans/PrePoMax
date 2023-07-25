using CaeGlobals;

namespace PrePoMax.Forms
{
    partial class FrmSearchContactPairs
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.cmsData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSwapMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMergeByMasterSlave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.lGroupBy = new System.Windows.Forms.Label();
            this.cbGroupBy = new System.Windows.Forms.ComboBox();
            this.tbAngle = new UserControls.UnitAwareTextBox();
            this.lAngle = new System.Windows.Forms.Label();
            this.tbDistance = new UserControls.UnitAwareTextBox();
            this.lDistance = new System.Windows.Forms.Label();
            this.lMethod = new System.Windows.Forms.Label();
            this.cbContactPairMethod = new System.Windows.Forms.ComboBox();
            this.lSurfaceinteraction = new System.Windows.Forms.Label();
            this.cbSurfaceInteraction = new System.Windows.Forms.ComboBox();
            this.lAdjustMesh = new System.Windows.Forms.Label();
            this.cbAbjustMesh = new System.Windows.Forms.ComboBox();
            this.lType = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.gbPairs = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbIgnoreHiddenParts = new System.Windows.Forms.CheckBox();
            this.cbSolid = new System.Windows.Forms.CheckBox();
            this.gbGeometryFilters = new System.Windows.Forms.GroupBox();
            this.cbShellEdge = new System.Windows.Forms.CheckBox();
            this.cbShell = new System.Windows.Forms.CheckBox();
            this.gbContactPairParameters = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.cmsData.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.gbPairs.SuspendLayout();
            this.gbGeometryFilters.SuspendLayout();
            this.gbContactPairParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.ContextMenuStrip = this.cmsData;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvData.Location = new System.Drawing.Point(7, 22);
            this.dgvData.Name = "dgvData";
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(597, 345);
            this.dgvData.TabIndex = 1;
            this.dgvData.SelectionChanged += new System.EventHandler(this.dgvData_SelectionChanged);
            // 
            // cmsData
            // 
            this.cmsData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSwapMasterSlave,
            this.tsmiMergeByMasterSlave});
            this.cmsData.Name = "cmsData";
            this.cmsData.Size = new System.Drawing.Size(195, 48);
            // 
            // tsmiSwapMasterSlave
            // 
            this.tsmiSwapMasterSlave.Name = "tsmiSwapMasterSlave";
            this.tsmiSwapMasterSlave.Size = new System.Drawing.Size(194, 22);
            this.tsmiSwapMasterSlave.Text = "Swap master/slave";
            this.tsmiSwapMasterSlave.Click += new System.EventHandler(this.tsmiSwapMasterSlave_Click);
            // 
            // tsmiMergeByMasterSlave
            // 
            this.tsmiMergeByMasterSlave.Name = "tsmiMergeByMasterSlave";
            this.tsmiMergeByMasterSlave.Size = new System.Drawing.Size(194, 22);
            this.tsmiMergeByMasterSlave.Text = "Merge by master/slave";
            this.tsmiMergeByMasterSlave.Click += new System.EventHandler(this.tsmiMergeByMasterSlave_Click);
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.lGroupBy);
            this.gbSearch.Controls.Add(this.cbGroupBy);
            this.gbSearch.Controls.Add(this.tbAngle);
            this.gbSearch.Controls.Add(this.lAngle);
            this.gbSearch.Controls.Add(this.tbDistance);
            this.gbSearch.Controls.Add(this.lDistance);
            this.gbSearch.Location = new System.Drawing.Point(12, 12);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(164, 116);
            this.gbSearch.TabIndex = 3;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Search Parameters";
            // 
            // lGroupBy
            // 
            this.lGroupBy.AutoSize = true;
            this.lGroupBy.Location = new System.Drawing.Point(6, 65);
            this.lGroupBy.Name = "lGroupBy";
            this.lGroupBy.Size = new System.Drawing.Size(56, 15);
            this.lGroupBy.TabIndex = 7;
            this.lGroupBy.Text = "Group by";
            // 
            // cbGroupBy
            // 
            this.cbGroupBy.FormattingEnabled = true;
            this.cbGroupBy.Location = new System.Drawing.Point(68, 62);
            this.cbGroupBy.Name = "cbGroupBy";
            this.cbGroupBy.Size = new System.Drawing.Size(87, 23);
            this.cbGroupBy.TabIndex = 5;
            // 
            // tbAngle
            // 
            this.tbAngle.Location = new System.Drawing.Point(68, 40);
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(87, 23);
            this.tbAngle.TabIndex = 3;
            this.tbAngle.UnitConverter = null;
            // 
            // lAngle
            // 
            this.lAngle.AutoSize = true;
            this.lAngle.Location = new System.Drawing.Point(24, 43);
            this.lAngle.Name = "lAngle";
            this.lAngle.Size = new System.Drawing.Size(38, 15);
            this.lAngle.TabIndex = 3;
            this.lAngle.Text = "Angle";
            // 
            // tbDistance
            // 
            this.tbDistance.Location = new System.Drawing.Point(68, 18);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(87, 23);
            this.tbDistance.TabIndex = 2;
            this.tbDistance.UnitConverter = null;
            // 
            // lDistance
            // 
            this.lDistance.AutoSize = true;
            this.lDistance.Location = new System.Drawing.Point(10, 21);
            this.lDistance.Name = "lDistance";
            this.lDistance.Size = new System.Drawing.Size(52, 15);
            this.lDistance.TabIndex = 0;
            this.lDistance.Text = "Distance";
            // 
            // lMethod
            // 
            this.lMethod.AutoSize = true;
            this.lMethod.Location = new System.Drawing.Point(63, 65);
            this.lMethod.Name = "lMethod";
            this.lMethod.Size = new System.Drawing.Size(49, 15);
            this.lMethod.TabIndex = 15;
            this.lMethod.Text = "Method";
            // 
            // cbContactPairMethod
            // 
            this.cbContactPairMethod.FormattingEnabled = true;
            this.cbContactPairMethod.Location = new System.Drawing.Point(118, 62);
            this.cbContactPairMethod.Name = "cbContactPairMethod";
            this.cbContactPairMethod.Size = new System.Drawing.Size(155, 23);
            this.cbContactPairMethod.TabIndex = 14;
            // 
            // lSurfaceinteraction
            // 
            this.lSurfaceinteraction.AutoSize = true;
            this.lSurfaceinteraction.Location = new System.Drawing.Point(6, 43);
            this.lSurfaceinteraction.Name = "lSurfaceinteraction";
            this.lSurfaceinteraction.Size = new System.Drawing.Size(106, 15);
            this.lSurfaceinteraction.TabIndex = 13;
            this.lSurfaceinteraction.Text = "Surface interaction";
            // 
            // cbSurfaceInteraction
            // 
            this.cbSurfaceInteraction.FormattingEnabled = true;
            this.cbSurfaceInteraction.Location = new System.Drawing.Point(118, 40);
            this.cbSurfaceInteraction.Name = "cbSurfaceInteraction";
            this.cbSurfaceInteraction.Size = new System.Drawing.Size(155, 23);
            this.cbSurfaceInteraction.TabIndex = 12;
            // 
            // lAdjustMesh
            // 
            this.lAdjustMesh.AutoSize = true;
            this.lAdjustMesh.Location = new System.Drawing.Point(39, 87);
            this.lAdjustMesh.Name = "lAdjustMesh";
            this.lAdjustMesh.Size = new System.Drawing.Size(73, 15);
            this.lAdjustMesh.TabIndex = 11;
            this.lAdjustMesh.Text = "Adjust mesh";
            // 
            // cbAbjustMesh
            // 
            this.cbAbjustMesh.FormattingEnabled = true;
            this.cbAbjustMesh.Location = new System.Drawing.Point(118, 84);
            this.cbAbjustMesh.Name = "cbAbjustMesh";
            this.cbAbjustMesh.Size = new System.Drawing.Size(155, 23);
            this.cbAbjustMesh.TabIndex = 10;
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(81, 21);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(31, 15);
            this.lType.TabIndex = 9;
            this.lType.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(118, 18);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(155, 23);
            this.cbType.TabIndex = 8;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(622, 101);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(87, 27);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gbPairs
            // 
            this.gbPairs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPairs.Controls.Add(this.propertyGrid);
            this.gbPairs.Controls.Add(this.dgvData);
            this.gbPairs.Location = new System.Drawing.Point(12, 134);
            this.gbPairs.Name = "gbPairs";
            this.gbPairs.Size = new System.Drawing.Size(916, 382);
            this.gbPairs.TabIndex = 8;
            this.gbPairs.TabStop = false;
            this.gbPairs.Text = "Contact Pairs";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(610, 22);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ReadOnly = false;
            this.propertyGrid.Size = new System.Drawing.Size(300, 345);
            this.propertyGrid.TabIndex = 7;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(835, 522);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 27);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(742, 522);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 27);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbIgnoreHiddenParts
            // 
            this.cbIgnoreHiddenParts.AutoSize = true;
            this.cbIgnoreHiddenParts.Checked = true;
            this.cbIgnoreHiddenParts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIgnoreHiddenParts.Location = new System.Drawing.Point(6, 86);
            this.cbIgnoreHiddenParts.Name = "cbIgnoreHiddenParts";
            this.cbIgnoreHiddenParts.Size = new System.Drawing.Size(129, 19);
            this.cbIgnoreHiddenParts.TabIndex = 16;
            this.cbIgnoreHiddenParts.Text = "Ignore hidden parts";
            this.cbIgnoreHiddenParts.UseVisualStyleBackColor = true;
            // 
            // cbSolid
            // 
            this.cbSolid.AutoSize = true;
            this.cbSolid.Checked = true;
            this.cbSolid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSolid.Location = new System.Drawing.Point(6, 20);
            this.cbSolid.Name = "cbSolid";
            this.cbSolid.Size = new System.Drawing.Size(52, 19);
            this.cbSolid.TabIndex = 17;
            this.cbSolid.Text = "Solid";
            this.cbSolid.UseVisualStyleBackColor = true;
            // 
            // gbGeometryFilters
            // 
            this.gbGeometryFilters.Controls.Add(this.cbIgnoreHiddenParts);
            this.gbGeometryFilters.Controls.Add(this.cbShellEdge);
            this.gbGeometryFilters.Controls.Add(this.cbShell);
            this.gbGeometryFilters.Controls.Add(this.cbSolid);
            this.gbGeometryFilters.Location = new System.Drawing.Point(182, 12);
            this.gbGeometryFilters.Name = "gbGeometryFilters";
            this.gbGeometryFilters.Size = new System.Drawing.Size(146, 116);
            this.gbGeometryFilters.TabIndex = 11;
            this.gbGeometryFilters.TabStop = false;
            this.gbGeometryFilters.Text = "Geometry Filters";
            // 
            // cbShellEdge
            // 
            this.cbShellEdge.AutoSize = true;
            this.cbShellEdge.Checked = true;
            this.cbShellEdge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShellEdge.Location = new System.Drawing.Point(6, 64);
            this.cbShellEdge.Name = "cbShellEdge";
            this.cbShellEdge.Size = new System.Drawing.Size(80, 19);
            this.cbShellEdge.TabIndex = 19;
            this.cbShellEdge.Text = "Shell edge";
            this.cbShellEdge.UseVisualStyleBackColor = true;
            // 
            // cbShell
            // 
            this.cbShell.AutoSize = true;
            this.cbShell.Checked = true;
            this.cbShell.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShell.Location = new System.Drawing.Point(6, 42);
            this.cbShell.Name = "cbShell";
            this.cbShell.Size = new System.Drawing.Size(51, 19);
            this.cbShell.TabIndex = 18;
            this.cbShell.Text = "Shell";
            this.cbShell.UseVisualStyleBackColor = true;
            // 
            // gbContactPairParameters
            // 
            this.gbContactPairParameters.Controls.Add(this.lMethod);
            this.gbContactPairParameters.Controls.Add(this.cbType);
            this.gbContactPairParameters.Controls.Add(this.cbContactPairMethod);
            this.gbContactPairParameters.Controls.Add(this.lType);
            this.gbContactPairParameters.Controls.Add(this.lSurfaceinteraction);
            this.gbContactPairParameters.Controls.Add(this.cbAbjustMesh);
            this.gbContactPairParameters.Controls.Add(this.cbSurfaceInteraction);
            this.gbContactPairParameters.Controls.Add(this.lAdjustMesh);
            this.gbContactPairParameters.Location = new System.Drawing.Point(334, 12);
            this.gbContactPairParameters.Name = "gbContactPairParameters";
            this.gbContactPairParameters.Size = new System.Drawing.Size(282, 116);
            this.gbContactPairParameters.TabIndex = 12;
            this.gbContactPairParameters.TabStop = false;
            this.gbContactPairParameters.Text = "Contact Pair Parameters";
            // 
            // FrmSearchContactPairs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 561);
            this.Controls.Add(this.gbContactPairParameters);
            this.Controls.Add(this.gbGeometryFilters);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbPairs);
            this.Controls.Add(this.gbSearch);
            this.Controls.Add(this.btnSearch);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(960, 600);
            this.Name = "FrmSearchContactPairs";
            this.Text = "Search Contact Pairs";
            this.Activated += new System.EventHandler(this.FrmSearchContactPairs_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSearchContactPairs_FormClosing);
            this.Load += new System.EventHandler(this.FrmSearchContactPairs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.cmsData.ResumeLayout(false);
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gbPairs.ResumeLayout(false);
            this.gbGeometryFilters.ResumeLayout(false);
            this.gbGeometryFilters.PerformLayout();
            this.gbContactPairParameters.ResumeLayout(false);
            this.gbContactPairParameters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.GroupBox gbSearch;
        private UserControls.TabEnabledPropertyGrid propertyGrid;
        private UserControls.UnitAwareTextBox tbAngle;
        private System.Windows.Forms.Label lAngle;
        private UserControls.UnitAwareTextBox tbDistance;
        private System.Windows.Forms.Label lDistance;
        private System.Windows.Forms.GroupBox gbPairs;
        private System.Windows.Forms.Label lGroupBy;
        private System.Windows.Forms.ComboBox cbGroupBy;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lAdjustMesh;
        private System.Windows.Forms.ComboBox cbAbjustMesh;
        private System.Windows.Forms.Label lSurfaceinteraction;
        private System.Windows.Forms.ComboBox cbSurfaceInteraction;
        private System.Windows.Forms.ContextMenuStrip cmsData;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwapMasterSlave;
        private System.Windows.Forms.ToolStripMenuItem tsmiMergeByMasterSlave;
        private System.Windows.Forms.Label lMethod;
        private System.Windows.Forms.ComboBox cbContactPairMethod;
        private System.Windows.Forms.CheckBox cbIgnoreHiddenParts;
        private System.Windows.Forms.CheckBox cbSolid;
        private System.Windows.Forms.GroupBox gbGeometryFilters;
        private System.Windows.Forms.CheckBox cbShellEdge;
        private System.Windows.Forms.CheckBox cbShell;
        private System.Windows.Forms.GroupBox gbContactPairParameters;
    }
}