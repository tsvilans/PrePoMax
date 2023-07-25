using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeMesh;
using CaeGlobals;

namespace PrePoMax.Forms
{
    class FrmMeshRefinement : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private HashSet<string> _meshRefinementNames;
        private string _meshRefinementToEditName;
        private ViewFeMeshRefinement _viewFeMeshRefinement;
        private Button btnPreview;
        private Controller _controller;
        private int _previewBtnDx;
        private ToolTip ttText;
        private System.ComponentModel.IContainer components;
        private string _defaultName;


        // Callbacks                                                                                                                
        public Func<string[], MeshingParameters, FeMeshRefinement, Task> PreviewEdgeMeshesAsync;


        // Properties                                                                                                               
        public FeMeshRefinement MeshRefinement
        {
            get { return _viewFeMeshRefinement.GetBase(); }
            set 
            {
                _viewFeMeshRefinement = new ViewFeMeshRefinement(value.DeepClone());
            }
        }


        // Constructors                                                                                                             
        public FrmMeshRefinement(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewFeMeshRefinement = null;
            _meshRefinementNames = new HashSet<string>();
            _previewBtnDx = btnCancel.Left - btnOK.Right;
            _defaultName = null;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnPreview = new System.Windows.Forms.Button();
            this.ttText = new System.Windows.Forms.ToolTip(this.components);
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.Image = global::PrePoMax.Properties.Resources.Show;
            this.btnPreview.Location = new System.Drawing.Point(45, 376);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(28, 23);
            this.btnPreview.TabIndex = 17;
            this.ttText.SetToolTip(this.btnPreview, "Preview");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmMeshRefinement
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnPreview);
            this.Name = "FrmMeshRefinement";
            this.Text = "Edit Mesh Refinement";
            this.VisibleChanged += new System.EventHandler(this.FrmMeshRefinement_VisibleChanged);
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void FrmMeshRefinement_VisibleChanged(object sender, EventArgs e)
        {
            // Limit selection to the first selected part
            _controller.Selection.LimitSelectionToFirstPart = false;
        }
        async private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;
                ItemSetDataEditor.SelectionForm.Enabled = false;
                //
                FeMeshRefinement meshRefinement = MeshRefinement.DeepClone();
                //
                string[] partNames = _controller.Model.Geometry.GetPartNamesFromGeometryIds(meshRefinement.GeometryIds);
                //
                if (partNames != null && partNames.Length > 0)
                {
                    HighlightMeshRefinement();
                    //
                    await PreviewEdgeMeshesAsync?.Invoke(partNames, null, meshRefinement);
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
        protected override void OnApply(bool onOkAddNew)
        {
            _viewFeMeshRefinement = (ViewFeMeshRefinement)propertyGrid.SelectedObject;
            // Check if the name exists
            CheckName(_meshRefinementToEditName, MeshRefinement.Name, _meshRefinementNames, "mesh refinement");
            //
            if (MeshRefinement.GeometryIds == null || MeshRefinement.GeometryIds.Length == 0)
                throw new CaeException("The mesh refinement selection must contain at least one item.");
            //
            if (_meshRefinementToEditName == null)
            {
                // Create
                _controller.AddMeshRefinementCommand(MeshRefinement);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !MeshRefinement.Valid)
                {
                    MeshRefinement.Valid = true;
                    _controller.ReplaceMeshRefinementCommand(_meshRefinementToEditName, MeshRefinement);
                }
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string meshRefinementToEditName)
        {
            if (meshRefinementToEditName == null)
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
            _meshRefinementNames.Clear();
            _meshRefinementToEditName = null;
            _viewFeMeshRefinement = null;
            propertyGrid.SelectedObject = null;
            //
            _meshRefinementNames.UnionWith(_controller.GetMeshRefinementNames());
            _meshRefinementToEditName = meshRefinementToEditName;
            // Create new mesh refinement
            if (_meshRefinementToEditName == null)
            {
                _defaultName = GetMeshRefinementName();
                MeshRefinement = new FeMeshRefinement(_defaultName);
                _controller.Selection.Clear();
            }
            // Edit existing mesh refinement
            else
            {
                MeshRefinement = _controller.GetMeshRefinement(_meshRefinementToEditName);   // to clone
                // Check validity
                if (!MeshRefinement.Valid)
                {
                    MeshRefinement.CreationData = null;
                    MeshRefinement.GeometryIds = null;
                    MeshRefinement.Valid = true;
                }
            }
            //
            propertyGrid.SelectedObject = _viewFeMeshRefinement;
            propertyGrid.Select();
            // Show ItemSetDataForm
            ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(MeshRefinement.GeometryIds);
            ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            //
            SetSelectItem();
            //
            HighlightMeshRefinement();
            //
            return true;
        }


        // Methods                                                                                                                  
        private string GetMeshRefinementName()
        {
            return _meshRefinementNames.GetNextNumberedKey("Mesh_Refinement");
        }
        private void HighlightMeshRefinement()
        {
            try
            {
                if (_controller != null)
                {
                    _controller.SetSelectItemToGeometry();
                    // Surface.CreationData is set to null when the CreatedFrom is changed
                    if (MeshRefinement.CreationData != null && MeshRefinement.GeometryIds != null &&
                        MeshRefinement.GeometryIds.Length > 0)
                    {
                        // The selection is limited to one part
                        int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(MeshRefinement.GeometryIds[0]);
                        BasePart part = _controller.Model.Geometry.GetPartById(itemTypePartIds[2]);
                        if (part == null) return;
                        //
                        bool backfaceCulling = part.PartType != PartType.Shell;
                        //
                        _controller.Selection = MeshRefinement.CreationData;  // deep copy to not clear?
                        _controller.HighlightSelection(true, backfaceCulling);
                    }
                }
            }
            catch { }
        }
        private void SetSelectItem()
        {
            _controller.SetSelectItemToGeometry();
        }
        //
        public void SelectionChanged(int[] ids)
        {
            if (Enabled)
            {
                MeshRefinement.GeometryIds = ids;
                MeshRefinement.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
                //
                if (ids.Length != 0) HighlightMeshRefinement(); // this will redraw the selection with correct backfaceCulling
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightMeshRefinement();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            // All mesh refinements are geometry based
            return true;
        }
        

       
    }


}
