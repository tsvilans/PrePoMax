namespace PrePoMax.Forms
{
    partial class FrmTransformation
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("X");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Y");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Z");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Symmetry", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Linear");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Circular");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Pattern", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("1");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("2");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("3");
            this.tvTransformations = new System.Windows.Forms.TreeView();
            this.propertyGrid = new UserControls.TabEnabledPropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbTypes = new System.Windows.Forms.GroupBox();
            this.labSelected = new System.Windows.Forms.Label();
            this.labAvailable = new System.Windows.Forms.Label();
            this.lvActiveTransformations = new UserControls.ListViewWithSelection();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.bntApply = new System.Windows.Forms.Button();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.gbTypes.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvTransformations
            // 
            this.tvTransformations.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tvTransformations.Location = new System.Drawing.Point(5, 38);
            this.tvTransformations.Name = "tvTransformations";
            treeNode1.Name = "X";
            treeNode1.Text = "X";
            treeNode2.Name = "Y";
            treeNode2.Text = "Y";
            treeNode3.Name = "Z";
            treeNode3.Text = "Z";
            treeNode4.Name = "Symmetry";
            treeNode4.Text = "Symmetry";
            treeNode5.Name = "Linear";
            treeNode5.Text = "Linear";
            treeNode6.Name = "Circular";
            treeNode6.Text = "Circular";
            treeNode7.Name = "Pattern";
            treeNode7.Text = "Pattern";
            this.tvTransformations.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7});
            this.tvTransformations.Size = new System.Drawing.Size(137, 133);
            this.tvTransformations.TabIndex = 2;
            this.tvTransformations.DoubleClick += new System.EventHandler(this.tvTransformations_DoubleClick);
            // 
            // propertyGrid
            // 
            this.propertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.propertyGrid.LineColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Location = new System.Drawing.Point(3, 19);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.ReadOnly = false;
            this.propertyGrid.Size = new System.Drawing.Size(354, 254);
            this.propertyGrid.TabIndex = 6;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(54, 476);
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
            this.btnCancel.Location = new System.Drawing.Point(297, 476);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbTypes
            // 
            this.gbTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTypes.Controls.Add(this.labSelected);
            this.gbTypes.Controls.Add(this.labAvailable);
            this.gbTypes.Controls.Add(this.lvActiveTransformations);
            this.gbTypes.Controls.Add(this.btnAdd);
            this.gbTypes.Controls.Add(this.btnRemove);
            this.gbTypes.Controls.Add(this.tvTransformations);
            this.gbTypes.Location = new System.Drawing.Point(12, 12);
            this.gbTypes.Name = "gbTypes";
            this.gbTypes.Size = new System.Drawing.Size(360, 176);
            this.gbTypes.TabIndex = 0;
            this.gbTypes.TabStop = false;
            this.gbTypes.Text = "Transformation Types";
            // 
            // labSelected
            // 
            this.labSelected.AutoSize = true;
            this.labSelected.Location = new System.Drawing.Point(177, 19);
            this.labSelected.Name = "labSelected";
            this.labSelected.Size = new System.Drawing.Size(40, 15);
            this.labSelected.TabIndex = 8;
            this.labSelected.Text = "Active";
            // 
            // labAvailable
            // 
            this.labAvailable.AutoSize = true;
            this.labAvailable.Location = new System.Drawing.Point(6, 19);
            this.labAvailable.Name = "labAvailable";
            this.labAvailable.Size = new System.Drawing.Size(55, 15);
            this.labAvailable.TabIndex = 7;
            this.labAvailable.Text = "Available";
            // 
            // lvActiveTransformations
            // 
            this.lvActiveTransformations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvActiveTransformations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvActiveTransformations.DisableMouse = false;
            this.lvActiveTransformations.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvActiveTransformations.FullRowSelect = true;
            this.lvActiveTransformations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvActiveTransformations.HideSelection = false;
            this.lvActiveTransformations.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
            this.lvActiveTransformations.Location = new System.Drawing.Point(176, 38);
            this.lvActiveTransformations.MultiSelect = false;
            this.lvActiveTransformations.Name = "lvActiveTransformations";
            this.lvActiveTransformations.ShowGroups = false;
            this.lvActiveTransformations.Size = new System.Drawing.Size(179, 133);
            this.lvActiveTransformations.TabIndex = 5;
            this.lvActiveTransformations.UseCompatibleStateImageBehavior = false;
            this.lvActiveTransformations.View = System.Windows.Forms.View.Details;
            this.lvActiveTransformations.SelectedIndexChanged += new System.EventHandler(this.lvActiveTransformations_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Width = 27;
            // 
            // btnAdd
            // 
            this.btnAdd.Image = global::PrePoMax.Properties.Resources.Right_arrow;
            this.btnAdd.Location = new System.Drawing.Point(148, 38);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(22, 22);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Image = global::PrePoMax.Properties.Resources.Remove;
            this.btnRemove.Location = new System.Drawing.Point(148, 66);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(22, 22);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(216, 476);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // bntApply
            // 
            this.bntApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bntApply.Location = new System.Drawing.Point(135, 476);
            this.bntApply.Name = "bntApply";
            this.bntApply.Size = new System.Drawing.Size(75, 23);
            this.bntApply.TabIndex = 18;
            this.bntApply.Text = "Apply";
            this.bntApply.UseVisualStyleBackColor = true;
            this.bntApply.Click += new System.EventHandler(this.bntApply_Click);
            // 
            // gbProperties
            // 
            this.gbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProperties.Controls.Add(this.propertyGrid);
            this.gbProperties.Location = new System.Drawing.Point(12, 194);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(360, 276);
            this.gbProperties.TabIndex = 19;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Properties";
            // 
            // FrmTransformation
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 511);
            this.Controls.Add(this.gbProperties);
            this.Controls.Add(this.bntApply);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.gbTypes);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(375, 550);
            this.Name = "FrmTransformation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Create Transformation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTrasformations_FormClosing);
            this.gbTypes.ResumeLayout(false);
            this.gbTypes.PerformLayout();
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvTransformations;
        private UserControls.TabEnabledPropertyGrid propertyGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbTypes;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private UserControls.ListViewWithSelection lvActiveTransformations;
        private System.Windows.Forms.Label labSelected;
        private System.Windows.Forms.Label labAvailable;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button bntApply;
        private System.Windows.Forms.GroupBox gbProperties;
        private System.Windows.Forms.ColumnHeader colName;
    }
}