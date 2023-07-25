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
    class FrmBoundaryLayer : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private ViewBoundaryLayer _viewBoundaryLayer;
        private Button btnPreview;
        private Controller _controller;


        // Constructors                                                                                                             
        public FrmBoundaryLayer(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewBoundaryLayer = null;
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.btnPreview = new System.Windows.Forms.Button();
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
            this.btnPreview.TabIndex = 18;
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // FrmBoundaryLayer
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.btnPreview);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "FrmBoundaryLayer";
            this.Text = "Create Boundary Layer";
            this.Controls.SetChildIndex(this.gbProperties, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnOkAddNew, 0);
            this.Controls.SetChildIndex(this.btnPreview, 0);
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Event handlers                                                                                                           
        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                _viewBoundaryLayer = (ViewBoundaryLayer)propertyGrid.SelectedObject;
                //
                if (_viewBoundaryLayer.GeometryIds != null && _viewBoundaryLayer.GeometryIds.Length > 0)
                {
                    HighlightSurface();
                    _controller.PreviewBoundaryLayer(_viewBoundaryLayer.GeometryIds, _viewBoundaryLayer.Thickness);
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex);
            }
        }

        // Overrides                                                                                                                
        protected override void OnApply(bool onOkAddNew)
        {
            _viewBoundaryLayer = (ViewBoundaryLayer)propertyGrid.SelectedObject;
            //
            if (_viewBoundaryLayer.GeometryIds == null || _viewBoundaryLayer.GeometryIds.Length == 0)
                throw new CaeException("The boundary layer selection must contain at least one item.");
            // Create
            _controller.CreateBoundaryLayerCommand(_viewBoundaryLayer.GeometryIds, _viewBoundaryLayer.Thickness);
            //
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
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
            if (_controller.Model.Properties.ModelSpace != CaeModel.ModelSpaceEnum.ThreeD)
                throw new CaeException("Boundary layer creation is possible only for 3D solid meshes.");
            //
            _propertyItemChanged = false;
            _viewBoundaryLayer = null;
            propertyGrid.SelectedObject = null;
            //
            _viewBoundaryLayer = new ViewBoundaryLayer();
            _controller.Selection.Clear();
            //
            propertyGrid.SelectedObject = _viewBoundaryLayer;
            propertyGrid.Select();
            //
            SetSelectItem();
            //
            ShowHideSelectionForm();
            //
            return true;
        }


        // Methods                                                                                                                         
        private void HighlightSurface()
        {
            try
            {
                if (_controller != null)
                {
                    _controller.ClearSelectionHistory();
                    _controller.SetSelectItemToGeometry();
                    // Surface.CreationData is set to null when the CreatedFrom is changed
                    if (_viewBoundaryLayer.CreationData != null)
                    {
                        _controller.Selection = _viewBoundaryLayer.CreationData.DeepClone(); // Deep copy to not clear
                        _controller.HighlightSelection();
                    }
                }
            }
            catch { }
        }
        private void ShowHideSelectionForm()
        {
            ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
        }
        private void SetSelectItem()
        {
            _controller.SetSelectItemToGeometry();
        }
        public void SelectionChanged(int[] ids)
        {
            if (_viewBoundaryLayer != null)
            {
                _viewBoundaryLayer.GeometryIds = ids;
                _viewBoundaryLayer.CreationData = _controller.Selection.DeepClone();
                //
                propertyGrid.Refresh();
                //
                _propertyItemChanged = true;
            }
        }

        // IFormHighlight
        public void Highlight()
        {
            HighlightSurface();
        }

        // IFormItemSetDataParent
        public bool IsSelectionGeometryBased()
        {
            // Prepare ItemSetDataEditor - prepare Geometry or Mesh based selection
            // Allways use geometry based selection
            return true;
        }

        
    }


}
