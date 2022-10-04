using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmMeshingParameters : UserControls.FrmProperties, IFormBase
    {
        // Variables                                                                                                                
        private Controller _controller;
        private string[] _partNames;
        private MeshingParameters _defaultMeshingParameters;
        private System.Windows.Forms.ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAll;
        private System.Windows.Forms.Button btnPreview;
        private ViewMeshingParameters _viewMeshingParameters;
        private System.Windows.Forms.ToolTip ttText;


        // Callbacks                                                                                                                
        public Action UpdateHighlightFromTree;
        public Func<string[], MeshingParameters, FeMeshRefinement, Task> PreviewEdgeMeshesAsync;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters
        {
            get { return _viewMeshingParameters.GetBase(); }
            set
            {
                _viewMeshingParameters = new ViewMeshingParameters(value.DeepClone());
                propertyGrid.SelectedObject = _viewMeshingParameters;
                propertyGrid.Select();
            }
        }
        public MeshingParameters DefaultMeshingParameters { set { _defaultMeshingParameters = value; } }
        public string[] PartNames
        {
            get { return _partNames; }
            set
            {
                _partNames = value;
                Text = "Edit Meshing Parameters: " + _partNames.ToShortString();
            }
        }


        // Constructors                                                                                                             
        public FrmMeshingParameters(Controller controller) 
            : base(1.5)
        {
            InitializeComponent();
            //
            _controller = controller;
            _partNames = null;
            _defaultMeshingParameters = null;
            _viewMeshingParameters = null;
            //
            btnOkAddNew.Visible = false;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsPropertyGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiResetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPreview = new System.Windows.Forms.Button();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.gbProperties.SuspendLayout();
            this.cmsPropertyGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProperties
            // 
            this.gbProperties.Size = new System.Drawing.Size(310, 414);
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            this.propertyGrid.Size = new System.Drawing.Size(298, 386);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 426);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 426);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(79, 426);
            // 
            // cmsPropertyGrid
            // 
            this.cmsPropertyGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiResetAll});
            this.cmsPropertyGrid.Name = "cmsPropertyGrid";
            this.cmsPropertyGrid.Size = new System.Drawing.Size(118, 26);
            // 
            // tsmiResetAll
            // 
            this.tsmiResetAll.Name = "tsmiResetAll";
            this.tsmiResetAll.Size = new System.Drawing.Size(117, 22);
            this.tsmiResetAll.Text = "Reset all";
            this.tsmiResetAll.Click += new System.EventHandler(this.tsmiResetAll_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Image = global::PrePoMax.Properties.Resources.Show;
            this.btnPreview.Location = new System.Drawing.Point(126, 426);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(28, 23);
            this.btnPreview.TabIndex = 16;
            this.ttText.SetToolTip(this.btnPreview, "Preview");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(334, 461);
            this.Controls.Add(this.btnPreview);
            this.MinimumSize = new System.Drawing.Size(350, 500);
            this.Name = "FrmMeshingParameters";
            this.Text = "Edit Meshing Parameters";
            this.VisibleChanged += new System.EventHandler(this.FrmMeshingParameters_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.cmsPropertyGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmMeshingParameters_VisibleChanged(object sender, EventArgs e)
        {
            UpdateHighlightFromTree?.Invoke();
            // Hide preview button during regeneartion 
            btnPreview.Visible = Visible && !Modal;
            // Disable selection
            if (Visible) _controller.SetSelectByToOff();
        }


        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            // OK
            _viewMeshingParameters = (ViewMeshingParameters)propertyGrid.SelectedObject;
            //
            if (_viewMeshingParameters.GetBase() != null && !Modal)
            {
                _controller.SetMeshingParametersCommand(_partNames, _viewMeshingParameters.GetBase());
            }
            // Regenerate for remeshing (form shown as Modal) must return OK
            else DialogResult = System.Windows.Forms.DialogResult.OK;
        }


        // Methods                                                                                                                         
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            MeshingParameters = _defaultMeshingParameters;
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                MeshingParameters parameters = ((ViewMeshingParameters)(propertyGrid.SelectedObject)).GetBase();
                //
                if (_partNames != null && _partNames.Length > 0)
                {
                    UpdateHighlightFromTree?.Invoke();
                    await PreviewEdgeMeshesAsync?.Invoke(_partNames, parameters, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                Enabled = true;
            }
        }
    }
}
