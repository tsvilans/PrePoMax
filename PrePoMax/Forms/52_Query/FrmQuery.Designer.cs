namespace PrePoMax.Forms
{
    partial class FrmQuery
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Vertex/Node");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Facet/Element");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Edge");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Surface");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Part");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Assembly");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Bounding box size");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Distance");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Angle");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Circle");
            this.btnClose = new System.Windows.Forms.Button();
            this.gbItem = new System.Windows.Forms.GroupBox();
            this.lvQueries = new UserControls.ListViewWithSelection();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClear = new System.Windows.Forms.Button();
            this.gbItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(107, 261);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbItem
            // 
            this.gbItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbItem.Controls.Add(this.lvQueries);
            this.gbItem.Location = new System.Drawing.Point(12, 12);
            this.gbItem.Name = "gbItem";
            this.gbItem.Size = new System.Drawing.Size(170, 243);
            this.gbItem.TabIndex = 10;
            this.gbItem.TabStop = false;
            this.gbItem.Text = "Item";
            // 
            // lvQueries
            // 
            this.lvQueries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvQueries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvQueries.DisableMouse = false;
            this.lvQueries.FullRowSelect = true;
            this.lvQueries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvQueries.HideSelection = false;
            listViewItem1.ToolTipText = "Vertex/Node";
            listViewItem2.ToolTipText = "Facet/Element";
            listViewItem3.ToolTipText = "Edge";
            listViewItem4.ToolTipText = "Surface";
            listViewItem5.ToolTipText = "Part";
            listViewItem6.ToolTipText = "Assembly";
            listViewItem7.ToolTipText = "Bounding box size";
            listViewItem8.ToolTipText = "Distance";
            listViewItem9.ToolTipText = "Angle";
            listViewItem10.ToolTipText = "Circle";
            this.lvQueries.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10});
            this.lvQueries.Location = new System.Drawing.Point(6, 22);
            this.lvQueries.MultiSelect = false;
            this.lvQueries.Name = "lvQueries";
            this.lvQueries.ShowGroups = false;
            this.lvQueries.Size = new System.Drawing.Size(158, 215);
            this.lvQueries.TabIndex = 11;
            this.lvQueries.UseCompatibleStateImageBehavior = false;
            this.lvQueries.View = System.Windows.Forms.View.Details;
            this.lvQueries.SelectedIndexChanged += new System.EventHandler(this.lvQueries_SelectedIndexChanged);
            this.lvQueries.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lvQueries_MouseDown);
            this.lvQueries.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvQueries_MouseUp);
            // 
            // colName
            // 
            this.colName.Width = 107;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClear.Location = new System.Drawing.Point(26, 261);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FrmQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(194, 296);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.gbItem);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(210, 335);
            this.Name = "FrmQuery";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Query";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmQuery_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmQuery_VisibleChanged);
            this.gbItem.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbItem;
        private UserControls.ListViewWithSelection lvQueries;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.Button btnClear;
    }
}