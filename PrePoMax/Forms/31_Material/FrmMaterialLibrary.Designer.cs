namespace PrePoMax.Forms
{
    partial class FrmMaterialLibrary
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Materials");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMaterialLibrary));
            this.gbModelMaterials = new System.Windows.Forms.GroupBox();
            this.btnDeleteFromModel = new System.Windows.Forms.Button();
            this.lvModelMaterials = new UserControls.ListViewWithSelection();
            this.btnRename = new System.Windows.Forms.Button();
            this.tbCategoryName = new System.Windows.Forms.TextBox();
            this.gbLibraryMaterials = new System.Windows.Forms.GroupBox();
            this.btnDeleteFromLibrary = new System.Windows.Forms.Button();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.cltvLibrary = new UserControls.CodersLabTreeView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCopyToLibrary = new System.Windows.Forms.Button();
            this.btnCopyToModel = new System.Windows.Forms.Button();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.cbPreview = new System.Windows.Forms.CheckBox();
            this.gbLibraries = new UserControls.CollapsableGroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lvLibraries = new UserControls.ListViewWithSelection();
            this.gbModelMaterials.SuspendLayout();
            this.gbLibraryMaterials.SuspendLayout();
            this.gbLibraries.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbModelMaterials
            // 
            this.gbModelMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbModelMaterials.Controls.Add(this.btnDeleteFromModel);
            this.gbModelMaterials.Controls.Add(this.lvModelMaterials);
            this.gbModelMaterials.Location = new System.Drawing.Point(324, 157);
            this.gbModelMaterials.Name = "gbModelMaterials";
            this.gbModelMaterials.Size = new System.Drawing.Size(263, 388);
            this.gbModelMaterials.TabIndex = 0;
            this.gbModelMaterials.TabStop = false;
            this.gbModelMaterials.Text = "FE Model Materials";
            // 
            // btnDeleteFromModel
            // 
            this.btnDeleteFromModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteFromModel.Location = new System.Drawing.Point(182, 22);
            this.btnDeleteFromModel.Name = "btnDeleteFromModel";
            this.btnDeleteFromModel.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFromModel.TabIndex = 10;
            this.btnDeleteFromModel.Text = "Delete";
            this.btnDeleteFromModel.UseVisualStyleBackColor = true;
            this.btnDeleteFromModel.Click += new System.EventHandler(this.btnDeleteFromModel_Click);
            // 
            // lvModelMaterials
            // 
            this.lvModelMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvModelMaterials.DisableMouse = false;
            this.lvModelMaterials.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lvModelMaterials.FullRowSelect = true;
            this.lvModelMaterials.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvModelMaterials.HideSelection = false;
            this.lvModelMaterials.Location = new System.Drawing.Point(6, 51);
            this.lvModelMaterials.MultiSelect = false;
            this.lvModelMaterials.Name = "lvModelMaterials";
            this.lvModelMaterials.ShowGroups = false;
            this.lvModelMaterials.Size = new System.Drawing.Size(251, 331);
            this.lvModelMaterials.TabIndex = 1;
            this.lvModelMaterials.UseCompatibleStateImageBehavior = false;
            this.lvModelMaterials.View = System.Windows.Forms.View.List;
            this.lvModelMaterials.Enter += new System.EventHandler(this.lvModelMaterials_Enter);
            this.lvModelMaterials.Leave += new System.EventHandler(this.lvModelMaterials_Leave);
            this.lvModelMaterials.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvModelMaterials_MouseDown);
            this.lvModelMaterials.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvModelMaterials_MouseUp);
            // 
            // btnRename
            // 
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRename.Location = new System.Drawing.Point(199, 360);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 0;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // tbCategoryName
            // 
            this.tbCategoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCategoryName.Location = new System.Drawing.Point(6, 360);
            this.tbCategoryName.Name = "tbCategoryName";
            this.tbCategoryName.Size = new System.Drawing.Size(187, 23);
            this.tbCategoryName.TabIndex = 2;
            this.tbCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbCategoryName_KeyDown);
            this.tbCategoryName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbCategoryName_KeyUp);
            // 
            // gbLibraryMaterials
            // 
            this.gbLibraryMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbLibraryMaterials.Controls.Add(this.btnDeleteFromLibrary);
            this.gbLibraryMaterials.Controls.Add(this.btnAddCategory);
            this.gbLibraryMaterials.Controls.Add(this.cltvLibrary);
            this.gbLibraryMaterials.Controls.Add(this.tbCategoryName);
            this.gbLibraryMaterials.Controls.Add(this.btnRename);
            this.gbLibraryMaterials.Location = new System.Drawing.Point(12, 157);
            this.gbLibraryMaterials.Name = "gbLibraryMaterials";
            this.gbLibraryMaterials.Size = new System.Drawing.Size(280, 388);
            this.gbLibraryMaterials.TabIndex = 3;
            this.gbLibraryMaterials.TabStop = false;
            this.gbLibraryMaterials.Text = "Library Materials";
            // 
            // btnDeleteFromLibrary
            // 
            this.btnDeleteFromLibrary.Location = new System.Drawing.Point(199, 22);
            this.btnDeleteFromLibrary.Name = "btnDeleteFromLibrary";
            this.btnDeleteFromLibrary.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFromLibrary.TabIndex = 9;
            this.btnDeleteFromLibrary.Text = "Delete";
            this.btnDeleteFromLibrary.UseVisualStyleBackColor = true;
            this.btnDeleteFromLibrary.Click += new System.EventHandler(this.btnDeleteFromLibrary_Click);
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Location = new System.Drawing.Point(6, 22);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(87, 23);
            this.btnAddCategory.TabIndex = 5;
            this.btnAddCategory.Text = "Add category";
            this.btnAddCategory.UseVisualStyleBackColor = true;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            // 
            // cltvLibrary
            // 
            this.cltvLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cltvLibrary.ChangeHighlightOnFocusLost = true;
            this.cltvLibrary.DisableMouse = false;
            this.cltvLibrary.HighlightForeErrorColor = System.Drawing.Color.Red;
            this.cltvLibrary.Location = new System.Drawing.Point(6, 51);
            this.cltvLibrary.Name = "cltvLibrary";
            treeNode1.Name = "Materials";
            treeNode1.Text = "Materials";
            this.cltvLibrary.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.cltvLibrary.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.cltvLibrary.SelectionMode = UserControls.TreeViewSelectionMode.SingleSelect;
            this.cltvLibrary.Size = new System.Drawing.Size(268, 302);
            this.cltvLibrary.TabIndex = 4;
            this.cltvLibrary.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.cltvLibrary_AfterSelect);
            this.cltvLibrary.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.cltvLibrary_MouseDoubleClick);
            this.cltvLibrary.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cltvLibrary_MouseDown);
            this.cltvLibrary.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cltvLibrary_MouseUp);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnOK.Location = new System.Drawing.Point(425, 551);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCancel.Location = new System.Drawing.Point(506, 550);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnCopyToLibrary
            // 
            this.btnCopyToLibrary.Image = global::PrePoMax.Properties.Resources.Left_arrow;
            this.btnCopyToLibrary.Location = new System.Drawing.Point(296, 238);
            this.btnCopyToLibrary.Name = "btnCopyToLibrary";
            this.btnCopyToLibrary.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToLibrary.TabIndex = 10;
            this.ttText.SetToolTip(this.btnCopyToLibrary, "Copy the selected material model from the FE model to the selected library catego" +
        "ry");
            this.btnCopyToLibrary.UseVisualStyleBackColor = true;
            this.btnCopyToLibrary.Click += new System.EventHandler(this.btnCopyToLibrary_Click);
            // 
            // btnCopyToModel
            // 
            this.btnCopyToModel.Image = global::PrePoMax.Properties.Resources.Right_arrow;
            this.btnCopyToModel.Location = new System.Drawing.Point(296, 208);
            this.btnCopyToModel.Name = "btnCopyToModel";
            this.btnCopyToModel.Size = new System.Drawing.Size(24, 24);
            this.btnCopyToModel.TabIndex = 8;
            this.ttText.SetToolTip(this.btnCopyToModel, "Copy the selected material model from the library to the FE model");
            this.btnCopyToModel.UseVisualStyleBackColor = true;
            this.btnCopyToModel.Click += new System.EventHandler(this.btnCopyToModel_Click);
            // 
            // cbPreview
            // 
            this.cbPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbPreview.AutoSize = true;
            this.cbPreview.Location = new System.Drawing.Point(20, 554);
            this.cbPreview.Name = "cbPreview";
            this.cbPreview.Size = new System.Drawing.Size(169, 19);
            this.cbPreview.TabIndex = 14;
            this.cbPreview.Text = "Preview material properties";
            this.cbPreview.UseVisualStyleBackColor = true;
            this.cbPreview.CheckedChanged += new System.EventHandler(this.cbPreview_CheckedChanged);
            // 
            // gbLibraries
            // 
            this.gbLibraries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLibraries.Controls.Add(this.btnRemove);
            this.gbLibraries.Controls.Add(this.btnAdd);
            this.gbLibraries.Controls.Add(this.btnNew);
            this.gbLibraries.Controls.Add(this.btnSave);
            this.gbLibraries.Controls.Add(this.lvLibraries);
            this.gbLibraries.IsCollapsed = false;
            this.gbLibraries.Location = new System.Drawing.Point(12, 12);
            this.gbLibraries.Name = "gbLibraries";
            this.gbLibraries.Size = new System.Drawing.Size(575, 139);
            this.gbLibraries.TabIndex = 15;
            this.gbLibraries.TabStop = false;
            this.gbLibraries.Text = "Libraries";
            this.gbLibraries.OnCollapsedChanged += new UserControls.CollapsableGroupBox.CollapseChangeEventHandler(this.gbLibraries_OnCollapsedChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(494, 80);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 14;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(494, 51);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(494, 22);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 12;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSave.Location = new System.Drawing.Point(494, 109);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lvLibraries
            // 
            this.lvLibraries.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLibraries.DisableMouse = false;
            this.lvLibraries.HideSelection = false;
            this.lvLibraries.Location = new System.Drawing.Point(6, 22);
            this.lvLibraries.MultiSelect = false;
            this.lvLibraries.Name = "lvLibraries";
            this.lvLibraries.Size = new System.Drawing.Size(482, 110);
            this.lvLibraries.TabIndex = 11;
            this.lvLibraries.UseCompatibleStateImageBehavior = false;
            this.lvLibraries.View = System.Windows.Forms.View.List;
            this.lvLibraries.SelectedIndexChanged += new System.EventHandler(this.lvLibraries_SelectedIndexChanged);
            // 
            // FrmMaterialLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(599, 586);
            this.Controls.Add(this.gbLibraries);
            this.Controls.Add(this.cbPreview);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCopyToLibrary);
            this.Controls.Add(this.btnCopyToModel);
            this.Controls.Add(this.gbLibraryMaterials);
            this.Controls.Add(this.gbModelMaterials);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(540, 550);
            this.Name = "FrmMaterialLibrary";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Material Library Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMaterialLibrary_FormClosing);
            this.Load += new System.EventHandler(this.FrmMaterialLibrary_Load);
            this.gbModelMaterials.ResumeLayout(false);
            this.gbLibraryMaterials.ResumeLayout(false);
            this.gbLibraryMaterials.PerformLayout();
            this.gbLibraries.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbModelMaterials;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.TextBox tbCategoryName;
        private System.Windows.Forms.GroupBox gbLibraryMaterials;
        private UserControls.CodersLabTreeView cltvLibrary;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnCopyToLibrary;
        private System.Windows.Forms.Button btnCopyToModel;
        private System.Windows.Forms.Button btnDeleteFromLibrary;
        private UserControls.ListViewWithSelection lvModelMaterials;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDeleteFromModel;
        private System.Windows.Forms.ToolTip ttText;
        private System.Windows.Forms.CheckBox cbPreview;
        private UserControls.CollapsableGroupBox gbLibraries;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnNew;
        private UserControls.ListViewWithSelection lvLibraries;
        private System.Windows.Forms.Button btnRemove;
    }
}