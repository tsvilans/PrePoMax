namespace PrePoMax.Forms
{
    partial class FrmMaterial
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Density");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Slip Wear");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("General", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Elastic");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Elasticity", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Plastic");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Plasticity", new System.Windows.Forms.TreeNode[] {
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Thermal Expansion");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Thermal Conductivity");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Specific Heat");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Thermal", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10});
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("3");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("4");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("5");
            this.tvProperties = new UserControls.CodersLabTreeView();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.cbTemperatureDependent = new System.Windows.Forms.CheckBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lName = new System.Windows.Forms.Label();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.tcProperties = new System.Windows.Forms.TabControl();
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.tpDataPoints = new System.Windows.Forms.TabPage();
            this.dgvData = new UserControls.DataGridViewCopyPaste();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.labSelected = new System.Windows.Forms.Label();
            this.labAvailable = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lvAddedProperties = new UserControls.ListViewWithSelection();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnOKAddNew = new System.Windows.Forms.Button();
            this.gbData.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.tcProperties.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.tpDataPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // tvProperties
            // 
            this.tvProperties.ChangeHighlightOnFocusLost = true;
            this.tvProperties.DisableMouse = false;
            this.tvProperties.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tvProperties.HideSelection = false;
            this.tvProperties.HighlightForeErrorColor = System.Drawing.Color.Red;
            this.tvProperties.Location = new System.Drawing.Point(6, 37);
            this.tvProperties.Name = "tvProperties";
            treeNode1.Name = "Density";
            treeNode1.Text = "Density";
            treeNode1.ToolTipText = "Density";
            treeNode2.Name = "Slip Wear";
            treeNode2.Text = "Slip Wear";
            treeNode2.ToolTipText = "Slip Wear";
            treeNode3.Name = "General";
            treeNode3.Text = "General";
            treeNode4.Name = "Elastic";
            treeNode4.Text = "Elastic";
            treeNode4.ToolTipText = "Elastic";
            treeNode5.Name = "Elasticity";
            treeNode5.Text = "Elasticity";
            treeNode5.ToolTipText = "Elasticity";
            treeNode6.Name = "Plastic";
            treeNode6.Text = "Plastic";
            treeNode6.ToolTipText = "Plastic";
            treeNode7.Name = "Plasticity";
            treeNode7.Text = "Plasticity";
            treeNode7.ToolTipText = "Plasticity";
            treeNode8.Name = "Thermal Expansion";
            treeNode8.Text = "Thermal Expansion";
            treeNode8.ToolTipText = "Thermal Expansion";
            treeNode9.Name = "Thermal Conductivity";
            treeNode9.Text = "Thermal Conductivity";
            treeNode9.ToolTipText = "Thermal Conductivity";
            treeNode10.Name = "Specific Heat";
            treeNode10.Text = "Specific Heat";
            treeNode10.ToolTipText = "Specific Heat";
            treeNode11.Name = "Thermal";
            treeNode11.Text = "Thermal";
            treeNode11.ToolTipText = "Thermal";
            this.tvProperties.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode5,
            treeNode7,
            treeNode11});
            this.tvProperties.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.tvProperties.SelectionMode = UserControls.TreeViewSelectionMode.MultiSelectSameLevel;
            this.tvProperties.Size = new System.Drawing.Size(192, 185);
            this.tvProperties.TabIndex = 2;
            this.tvProperties.DoubleClick += new System.EventHandler(this.tvProperties_DoubleClick);
            // 
            // propertyGrid
            // 
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ReadOnly = false;
            this.propertyGrid.Size = new System.Drawing.Size(429, 231);
            this.propertyGrid.TabIndex = 6;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(300, 647);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(381, 647);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbData
            // 
            this.gbData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbData.Controls.Add(this.cbTemperatureDependent);
            this.gbData.Controls.Add(this.tbDescription);
            this.gbData.Controls.Add(this.lDescription);
            this.gbData.Controls.Add(this.tbName);
            this.gbData.Controls.Add(this.lName);
            this.gbData.Location = new System.Drawing.Point(12, 12);
            this.gbData.Name = "gbData";
            this.gbData.Size = new System.Drawing.Size(450, 130);
            this.gbData.TabIndex = 10;
            this.gbData.TabStop = false;
            this.gbData.Text = "Data";
            // 
            // cbTemperatureDependent
            // 
            this.cbTemperatureDependent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTemperatureDependent.AutoSize = true;
            this.cbTemperatureDependent.Location = new System.Drawing.Point(95, 105);
            this.cbTemperatureDependent.Name = "cbTemperatureDependent";
            this.cbTemperatureDependent.Size = new System.Drawing.Size(235, 19);
            this.cbTemperatureDependent.TabIndex = 4;
            this.cbTemperatureDependent.Text = "Use temperature dependent data points";
            this.cbTemperatureDependent.UseVisualStyleBackColor = true;
            this.cbTemperatureDependent.CheckedChanged += new System.EventHandler(this.cbTemperatureDependent_CheckedChanged);
            // 
            // tbDescription
            // 
            this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDescription.Location = new System.Drawing.Point(95, 48);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbDescription.Size = new System.Drawing.Size(350, 51);
            this.tbDescription.TabIndex = 3;
            this.tbDescription.Text = "1\r\n2\r\n3";
            // 
            // lDescription
            // 
            this.lDescription.AutoSize = true;
            this.lDescription.Location = new System.Drawing.Point(22, 51);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(67, 15);
            this.lDescription.TabIndex = 2;
            this.lDescription.Text = "Description";
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(95, 19);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(350, 23);
            this.tbName.TabIndex = 1;
            this.tbName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbName_KeyDown);
            // 
            // lName
            // 
            this.lName.AutoSize = true;
            this.lName.Location = new System.Drawing.Point(6, 22);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(83, 15);
            this.lName.TabIndex = 0;
            this.lName.Text = "Material name";
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.tcProperties);
            this.gbProperties.Controls.Add(this.btnMoveDown);
            this.gbProperties.Controls.Add(this.btnMoveUp);
            this.gbProperties.Controls.Add(this.labSelected);
            this.gbProperties.Controls.Add(this.labAvailable);
            this.gbProperties.Controls.Add(this.btnAdd);
            this.gbProperties.Controls.Add(this.btnRemove);
            this.gbProperties.Controls.Add(this.tvProperties);
            this.gbProperties.Controls.Add(this.lvAddedProperties);
            this.gbProperties.Location = new System.Drawing.Point(12, 148);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(450, 493);
            this.gbProperties.TabIndex = 0;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Material Models";
            // 
            // tcProperties
            // 
            this.tcProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcProperties.Controls.Add(this.tpProperties);
            this.tcProperties.Controls.Add(this.tpDataPoints);
            this.tcProperties.Location = new System.Drawing.Point(6, 225);
            this.tcProperties.Margin = new System.Windows.Forms.Padding(0);
            this.tcProperties.Name = "tcProperties";
            this.tcProperties.SelectedIndex = 0;
            this.tcProperties.Size = new System.Drawing.Size(443, 265);
            this.tcProperties.TabIndex = 11;
            // 
            // tpProperties
            // 
            this.tpProperties.BackColor = System.Drawing.SystemColors.Control;
            this.tpProperties.Controls.Add(this.propertyGrid);
            this.tpProperties.Location = new System.Drawing.Point(4, 24);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(435, 237);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            // 
            // tpDataPoints
            // 
            this.tpDataPoints.BackColor = System.Drawing.SystemColors.Control;
            this.tpDataPoints.Controls.Add(this.dgvData);
            this.tpDataPoints.Location = new System.Drawing.Point(4, 24);
            this.tpDataPoints.Name = "tpDataPoints";
            this.tpDataPoints.Padding = new System.Windows.Forms.Padding(3);
            this.tpDataPoints.Size = new System.Drawing.Size(435, 237);
            this.tpDataPoints.TabIndex = 1;
            this.tpDataPoints.Text = "Data points";
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.EnableCutMenu = true;
            this.dgvData.EnablePasteMenu = true;
            this.dgvData.EnablePlotMenu = true;
            this.dgvData.Location = new System.Drawing.Point(3, 3);
            this.dgvData.Name = "dgvData";
            this.dgvData.Size = new System.Drawing.Size(429, 231);
            this.dgvData.StartPlotAtZero = false;
            this.dgvData.TabIndex = 0;
            this.dgvData.XColIndex = 0;
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.Image = global::PrePoMax.Properties.Resources.Down_arrow;
            this.btnMoveDown.Location = new System.Drawing.Point(421, 67);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(24, 24);
            this.btnMoveDown.TabIndex = 13;
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.Image = global::PrePoMax.Properties.Resources.Up_arrow;
            this.btnMoveUp.Location = new System.Drawing.Point(421, 37);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(24, 24);
            this.btnMoveUp.TabIndex = 12;
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // labSelected
            // 
            this.labSelected.AutoSize = true;
            this.labSelected.Location = new System.Drawing.Point(235, 19);
            this.labSelected.Name = "labSelected";
            this.labSelected.Size = new System.Drawing.Size(51, 15);
            this.labSelected.TabIndex = 8;
            this.labSelected.Text = "Selected";
            // 
            // labAvailable
            // 
            this.labAvailable.AutoSize = true;
            this.labAvailable.Location = new System.Drawing.Point(7, 19);
            this.labAvailable.Name = "labAvailable";
            this.labAvailable.Size = new System.Drawing.Size(55, 15);
            this.labAvailable.TabIndex = 7;
            this.labAvailable.Text = "Available";
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::PrePoMax.Properties.Resources.Right_arrow;
            this.btnAdd.Location = new System.Drawing.Point(202, 37);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(24, 24);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Image = global::PrePoMax.Properties.Resources.Remove;
            this.btnRemove.Location = new System.Drawing.Point(421, 97);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(24, 24);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lvAddedProperties
            // 
            this.lvAddedProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAddedProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvAddedProperties.DisableMouse = false;
            this.lvAddedProperties.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvAddedProperties.FullRowSelect = true;
            this.lvAddedProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvAddedProperties.HideSelection = false;
            this.lvAddedProperties.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lvAddedProperties.Location = new System.Drawing.Point(230, 37);
            this.lvAddedProperties.MultiSelect = false;
            this.lvAddedProperties.Name = "lvAddedProperties";
            this.lvAddedProperties.ShowGroups = false;
            this.lvAddedProperties.Size = new System.Drawing.Size(187, 185);
            this.lvAddedProperties.TabIndex = 5;
            this.lvAddedProperties.UseCompatibleStateImageBehavior = false;
            this.lvAddedProperties.View = System.Windows.Forms.View.Details;
            this.lvAddedProperties.SelectedIndexChanged += new System.EventHandler(this.lvAddedProperties_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Width = 27;
            // 
            // btnOKAddNew
            // 
            this.btnOKAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOKAddNew.Location = new System.Drawing.Point(204, 647);
            this.btnOKAddNew.Name = "btnOKAddNew";
            this.btnOKAddNew.Size = new System.Drawing.Size(90, 23);
            this.btnOKAddNew.TabIndex = 17;
            this.btnOKAddNew.Text = "OK - Add new";
            this.btnOKAddNew.UseVisualStyleBackColor = true;
            this.btnOKAddNew.Click += new System.EventHandler(this.btnOKAddNew_Click);
            // 
            // FrmMaterial
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 681);
            this.Controls.Add(this.btnOKAddNew);
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.gbData);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(430, 650);
            this.Name = "FrmMaterial";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Edit Material";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMaterial_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmMaterial_VisibleChanged);
            this.gbData.ResumeLayout(false);
            this.gbData.PerformLayout();
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.tcProperties.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.tpDataPoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.CodersLabTreeView tvProperties;
        private UserControls.TabEnabledPropertyGrid propertyGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbData;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lName;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private UserControls.ListViewWithSelection lvAddedProperties;
        private System.Windows.Forms.Label labSelected;
        private System.Windows.Forms.Label labAvailable;
        private System.Windows.Forms.TabControl tcProperties;
        private System.Windows.Forms.TabPage tpProperties;
        private System.Windows.Forms.TabPage tpDataPoints;
        private UserControls.DataGridViewCopyPaste dgvData;
        private System.Windows.Forms.Button btnOKAddNew;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.CheckBox cbTemperatureDependent;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.ColumnHeader colName;
    }
}