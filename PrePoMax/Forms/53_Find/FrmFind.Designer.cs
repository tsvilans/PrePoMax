namespace PrePoMax.Forms
{
    partial class FrmFind
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
            this.btnClose = new System.Windows.Forms.Button();
            this.gbItemType = new System.Windows.Forms.GroupBox();
            this.rbPart = new System.Windows.Forms.RadioButton();
            this.rbSurface = new System.Windows.Forms.RadioButton();
            this.rbEdge = new System.Windows.Forms.RadioButton();
            this.rbFacetElement = new System.Windows.Forms.RadioButton();
            this.rbVertexNode = new System.Windows.Forms.RadioButton();
            this.gbItemData = new System.Windows.Forms.GroupBox();
            this.labItemId = new System.Windows.Forms.Label();
            this.cbAddAnnotation = new System.Windows.Forms.CheckBox();
            this.tbItemId = new UserControls.NumericTextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.gbItemType.SuspendLayout();
            this.gbItemData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(127, 255);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbItemType
            // 
            this.gbItemType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbItemType.Controls.Add(this.rbPart);
            this.gbItemType.Controls.Add(this.rbSurface);
            this.gbItemType.Controls.Add(this.rbEdge);
            this.gbItemType.Controls.Add(this.rbFacetElement);
            this.gbItemType.Controls.Add(this.rbVertexNode);
            this.gbItemType.Location = new System.Drawing.Point(12, 12);
            this.gbItemType.Name = "gbItemType";
            this.gbItemType.Size = new System.Drawing.Size(190, 148);
            this.gbItemType.TabIndex = 10;
            this.gbItemType.TabStop = false;
            this.gbItemType.Text = "Item type";
            // 
            // rbPart
            // 
            this.rbPart.AutoSize = true;
            this.rbPart.Location = new System.Drawing.Point(6, 122);
            this.rbPart.Name = "rbPart";
            this.rbPart.Size = new System.Drawing.Size(46, 19);
            this.rbPart.TabIndex = 18;
            this.rbPart.TabStop = true;
            this.rbPart.Text = "Part";
            this.rbPart.UseVisualStyleBackColor = true;
            this.rbPart.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbSurface
            // 
            this.rbSurface.AutoSize = true;
            this.rbSurface.Location = new System.Drawing.Point(6, 97);
            this.rbSurface.Name = "rbSurface";
            this.rbSurface.Size = new System.Drawing.Size(64, 19);
            this.rbSurface.TabIndex = 17;
            this.rbSurface.TabStop = true;
            this.rbSurface.Text = "Surface";
            this.rbSurface.UseVisualStyleBackColor = true;
            this.rbSurface.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbEdge
            // 
            this.rbEdge.AutoSize = true;
            this.rbEdge.Location = new System.Drawing.Point(6, 72);
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.Size = new System.Drawing.Size(51, 19);
            this.rbEdge.TabIndex = 16;
            this.rbEdge.TabStop = true;
            this.rbEdge.Text = "Edge";
            this.rbEdge.UseVisualStyleBackColor = true;
            this.rbEdge.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbFacetElement
            // 
            this.rbFacetElement.AutoSize = true;
            this.rbFacetElement.Location = new System.Drawing.Point(6, 47);
            this.rbFacetElement.Name = "rbFacetElement";
            this.rbFacetElement.Size = new System.Drawing.Size(101, 19);
            this.rbFacetElement.TabIndex = 15;
            this.rbFacetElement.TabStop = true;
            this.rbFacetElement.Text = "Facet/Element";
            this.rbFacetElement.UseVisualStyleBackColor = true;
            this.rbFacetElement.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbVertexNode
            // 
            this.rbVertexNode.AutoSize = true;
            this.rbVertexNode.Checked = true;
            this.rbVertexNode.Location = new System.Drawing.Point(6, 22);
            this.rbVertexNode.Name = "rbVertexNode";
            this.rbVertexNode.Size = new System.Drawing.Size(91, 19);
            this.rbVertexNode.TabIndex = 14;
            this.rbVertexNode.TabStop = true;
            this.rbVertexNode.Text = "Vertex/Node";
            this.rbVertexNode.UseVisualStyleBackColor = true;
            this.rbVertexNode.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // gbItemData
            // 
            this.gbItemData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbItemData.Controls.Add(this.labItemId);
            this.gbItemData.Controls.Add(this.cbAddAnnotation);
            this.gbItemData.Controls.Add(this.tbItemId);
            this.gbItemData.Controls.Add(this.btnFind);
            this.gbItemData.Location = new System.Drawing.Point(12, 166);
            this.gbItemData.Name = "gbItemData";
            this.gbItemData.Size = new System.Drawing.Size(190, 83);
            this.gbItemData.TabIndex = 11;
            this.gbItemData.TabStop = false;
            this.gbItemData.Text = "Data";
            // 
            // labItemId
            // 
            this.labItemId.AutoSize = true;
            this.labItemId.Location = new System.Drawing.Point(6, 54);
            this.labItemId.Name = "labItemId";
            this.labItemId.Size = new System.Drawing.Size(44, 15);
            this.labItemId.TabIndex = 17;
            this.labItemId.Text = "Item id";
            // 
            // cbAddAnnotation
            // 
            this.cbAddAnnotation.AutoSize = true;
            this.cbAddAnnotation.Location = new System.Drawing.Point(6, 22);
            this.cbAddAnnotation.Name = "cbAddAnnotation";
            this.cbAddAnnotation.Size = new System.Drawing.Size(109, 19);
            this.cbAddAnnotation.TabIndex = 16;
            this.cbAddAnnotation.Text = "Add annotation";
            this.cbAddAnnotation.UseVisualStyleBackColor = true;
            // 
            // tbItemId
            // 
            this.tbItemId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbItemId.Location = new System.Drawing.Point(56, 50);
            this.tbItemId.Name = "tbItemId";
            this.tbItemId.NumericType = UserControls.NumericTextBoxEnum.Integer;
            this.tbItemId.Size = new System.Drawing.Size(72, 23);
            this.tbItemId.TabIndex = 14;
            this.tbItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbItemId.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbItemId_KeyUp);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Location = new System.Drawing.Point(134, 50);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(50, 23);
            this.btnFind.TabIndex = 15;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClear.Location = new System.Drawing.Point(46, 255);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FrmFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(214, 290);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.gbItemData);
            this.Controls.Add(this.gbItemType);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 329);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(230, 329);
            this.Name = "FrmFind";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmQuery_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.FrmQuery_VisibleChanged);
            this.gbItemType.ResumeLayout(false);
            this.gbItemType.PerformLayout();
            this.gbItemData.ResumeLayout(false);
            this.gbItemData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbItemType;
        private System.Windows.Forms.RadioButton rbPart;
        private System.Windows.Forms.RadioButton rbSurface;
        private System.Windows.Forms.RadioButton rbEdge;
        private System.Windows.Forms.RadioButton rbFacetElement;
        private System.Windows.Forms.RadioButton rbVertexNode;
        private System.Windows.Forms.GroupBox gbItemData;
        private System.Windows.Forms.Label labItemId;
        private System.Windows.Forms.CheckBox cbAddAnnotation;
        private UserControls.NumericTextBox tbItemId;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnClear;
    }
}