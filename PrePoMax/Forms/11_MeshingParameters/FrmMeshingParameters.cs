using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using UserControls;

namespace PrePoMax.Forms
{
    class FrmMeshingParameters : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private Controller _controller;
        private HashSet<string> _meshingParametersNames;
        private string _meshingParametersToEditName;
        private System.Windows.Forms.ContextMenuStrip cmsPropertyGrid;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem tsmiResetAll;
        private System.Windows.Forms.Button btnPreview;
        private int _previewBtnDx;
        private ViewMeshingParameters _viewMeshingParameters;
        private System.Windows.Forms.ToolTip ttText;
        private bool _meshingParametersChanged;


        // Callbacks                                                                                                                
        public Func<string[], MeshingParameters, FeMeshRefinement, Task> PreviewEdgeMeshesAsync;


        // Properties                                                                                                               
        public MeshingParameters MeshingParameters
        {
            get { return _viewMeshingParameters.GetBase(); }
            set
            {
                _viewMeshingParameters = new ViewMeshingParameters(value.DeepClone());
                propertyGrid.SelectedObject = _viewMeshingParameters;
            }
        }


        // Constructors                                                                                                             
        public FrmMeshingParameters(Controller controller) 
            : base(1.7)
        {
            InitializeComponent();
            //
            _controller = controller;
            _meshingParametersNames = new HashSet<string>();
            _viewMeshingParameters = null;
            //
            _previewBtnDx = btnCancel.Left - btnOK.Right;
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
            this.gbProperties.Size = new System.Drawing.Size(350, 444);
            // 
            // propertyGrid
            // 
            this.propertyGrid.ContextMenuStrip = this.cmsPropertyGrid;
            this.propertyGrid.Size = new System.Drawing.Size(338, 416);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(200, 456);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(281, 456);
            // 
            // btnOkAddNew
            // 
            this.btnOkAddNew.Location = new System.Drawing.Point(119, 456);
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
            this.btnPreview.Location = new System.Drawing.Point(166, 456);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(28, 23);
            this.btnPreview.TabIndex = 16;
            this.ttText.SetToolTip(this.btnPreview, "Preview");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshingParameters
            // 
            this.ClientSize = new System.Drawing.Size(374, 491);
            this.Controls.Add(this.btnPreview);
            this.MinimumSize = new System.Drawing.Size(390, 530);
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
            _controller.Selection.LimitSelectionToFirstMesherType = Visible;
        }
        private void tsmiResetAll_Click(object sender, EventArgs e)
        {
            MeshingParameters = GetDefaultMeshingParameters(MeshingParameters.CreationIds);
            _meshingParametersChanged = _meshingParametersToEditName != null;
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                ItemSetDataEditor.SelectionForm.Enabled = false;
                //
                MeshingParameters parameters = ((ViewMeshingParameters)(propertyGrid.SelectedObject)).GetBase();
                //
                string[] partNames = _controller.DisplayedMesh.GetPartNamesByIds(MeshingParameters.CreationIds);
                //
                if (partNames != null && partNames.Length > 0)
                {
                    HighlightMeshingParameters();
                    //
                    await PreviewEdgeMeshesAsync?.Invoke(partNames, parameters, null);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
            finally
            {
                Enabled = true;
                ItemSetDataEditor.SelectionForm.Enabled = true;
            }
        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property != nameof(_viewMeshingParameters.Name) &&
                property != nameof(_viewMeshingParameters.Relative))
            {
                _meshingParametersChanged = true;
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewMeshingParameters = (ViewMeshingParameters)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_meshingParametersToEditName, MeshingParameters.Name, _meshingParametersNames, "meshing parameters");
            //
            if (MeshingParameters.CreationIds == null || MeshingParameters.CreationIds.Length == 0)
                throw new CaeException("The meshing parameters selection must contain at least one item.");
            //
            if (_meshingParametersToEditName == null)
            {
                // Create
                _controller.AddMeshingParametersCommand(MeshingParameters);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !MeshingParameters.Valid)
                {
                    MeshingParameters.Valid = true;
                    _controller.ReplaceMeshingParametersCommand(_meshingParametersToEditName, MeshingParameters);
                }
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                ItemSetDataEditor.SelectionForm.Hide();
            }
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string meshingParametersToEditName)
        {
            DialogResult = System.Windows.Forms.DialogResult.None;
            //
            if (meshingParametersToEditName == null)
            {
                btnOkAddNew.Visible = true;
                btnPreview.Location = new System.Drawing.Point(btnOkAddNew.Left - _previewBtnDx - btnPreview.Width, btnOkAddNew.Top);
            }
            else
            {
                btnOkAddNew.Visible = false;
                btnPreview.Location = new System.Drawing.Point(btnOK.Left - _previewBtnDx - btnPreview.Width, btnOkAddNew.Top);
            }
            //
            _propertyItemChanged = false;
            _meshingParametersNames.Clear();
            _meshingParametersToEditName = null;
            _viewMeshingParameters = null;
            propertyGrid.SelectedObject = null;
            _meshingParametersChanged = false;
            //
            _meshingParametersNames.UnionWith(_controller.GetMeshingParametersNames());
            _meshingParametersToEditName = meshingParametersToEditName;
            _meshingParametersChanged = _meshingParametersToEditName != null;
            // Create new meshing parameters
            if (_meshingParametersToEditName == null)
            {
                MeshingParameters = _controller.GetDefaultMeshingParameters(GetMeshingParametersName());
                _controller.Selection.Clear();
            }
            // Edit existing meshing parameters
            else
            {
                MeshingParameters = _controller.GetMeshingParameters(_meshingParametersToEditName);   // to clone
            }
            //
            propertyGrid.SelectedObject = _viewMeshingParameters;
            propertyGrid.Select();
            // Show ItemSetDataForm
            ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(MeshingParameters.CreationIds);
            ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            //
            SetSelectItem();
            //
            HighlightMeshingParameters();
            //
            return true;
        }

        // Methods                                                                                                                         
        private string GetMeshingParametersName()
        {
            return _meshingParametersNames.GetNextNumberedKey("Meshing_Parameters");
        }
        private MeshingParameters GetDefaultMeshingParameters(int[] ids)
        {
            //MeshingParameters meshingParameters = new MeshingParameters(MeshingParameters.Name);
            MeshingParameters meshingParameters = _controller.Settings.Meshing.MeshingParameters;
            meshingParameters.Name = MeshingParameters.Name;
            //
            if (ids != null && ids.Length > 0)
            {
                string[] partNames = _controller.Model.Geometry.GetPartNamesByIds(ids);
                MeshingParameters defaultMeshingParameters = _controller.GetPartDefaultMeshingParameters(partNames);
                //
                if (defaultMeshingParameters != null)
                {
                    defaultMeshingParameters.Name = MeshingParameters.Name;
                    meshingParameters = defaultMeshingParameters;
                }
            }
            // Copy relative
            meshingParameters.RelativeSize = MeshingParameters.RelativeSize;
            // Copy creation data
            meshingParameters.CreationIds = MeshingParameters.CreationIds;
            meshingParameters.CreationData = MeshingParameters.CreationData;
            //
            return meshingParameters;
        }
        private void HighlightMeshingParameters()
        {
            try
            {
                _controller.ClearSelectionHistory();
                if (_viewMeshingParameters == null) { }
                else
                {
                    SetSelectItem();
                    //
                    if (MeshingParameters.CreationData != null)
                    {
                        _controller.Selection = MeshingParameters.CreationData.DeepClone();
                        _controller.HighlightSelection(true);
                    }
                }
            }
            catch { }
        }
        private void SetSelectItem()
        {
            _controller.SetSelectItemToPart();
        }
        public void SelectionChanged(int[] ids)
        {
            if (Enabled)
            {
                //if (!_meshingParametersChanged)
                {
                    MeshingParameters = GetDefaultMeshingParameters(ids);
                }
                //
                MeshingParameters.CreationIds = ids;
                MeshingParameters.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
                //
                if (ids.Length != 0) HighlightMeshingParameters();
            }
        }
        // IFormHighlight
        public void Highlight()
        {
            HighlightMeshingParameters();
        }
        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            // All meshing parameters are geometry based
            return true;
        }
    }
}
