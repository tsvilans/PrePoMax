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
    class FrmSurface : UserControls.FrmProperties, IFormBase, IFormItemSetDataParent, IFormHighlight
    {
        // Variables                                                                                                                
        private HashSet<string> _surfaceNames;
        private string _surfaceToEditName;
        private ViewFeSurface _viewSurface;
        private List<SelectionNode> _prevSelectionNodes;
        private SelectionNodeIds _selectionNodeIds;
        private Controller _controller;


        // Properties                                                                                                               
        public FeSurface Surface
        {
            get { return _viewSurface.GetBase(); }
            set { _viewSurface = new ViewFeSurface(value.DeepClone()); }
        }


        // Constructors                                                                                                             
        public FrmSurface(Controller controller) 
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewSurface = null;
            _surfaceNames = new HashSet<string>();
            //
            SelectionClear = _controller.Selection.Clear;
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmSurface
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmSurface";
            this.Text = "Edit Surface";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewSurface.CreateSurfaceFrom))
            {
                ShowHideSelectionForm();
                //
                HighlightSurface();
            }
            else if (property == nameof(_viewSurface.NodeSetName))
            {
                HighlightSurface();
            }
            else if (property == nameof(_viewSurface.SurfaceType))
            {
                UpdateSurfaceArea();
            }
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewSurface = (ViewFeSurface)propertyGrid.SelectedObject;
            //
            if (_viewSurface == null) throw new CaeGlobals.CaeException("No surface was selected.");
            // Check if the name exists
            CheckName(_surfaceToEditName, Surface.Name, _surfaceNames, "surface");
            //
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection && (Surface.FaceIds == null || Surface.FaceIds.Length == 0))
                throw new CaeException("The surface must contain at least one item.");
            //
            if (_surfaceToEditName == null)
            {
                // Create
                _controller.AddSurfaceCommand(Surface);
            }
            else
            {
                // Replace
                if (_propertyItemChanged || !Surface.Valid)
                {
                    if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                    {
                        // Replace back the ids by the previous selection
                        if (Surface.CreationData.Nodes[0] is SelectionNodeIds sn && sn.Equals(_selectionNodeIds))
                        {
                            Surface.CreationData.Nodes.RemoveAt(0);
                            Surface.CreationData.Nodes.InsertRange(0, _prevSelectionNodes);
                        }
                    }
                    //
                    Surface.Valid = true;
                    _controller.ReplaceSurfaceCommand(_surfaceToEditName, Surface);
                }
            }
            // If all is successful close the ItemSetSelectionForm - except for OKAddNew
            if (!onOkAddNew) ItemSetDataEditor.SelectionForm.Hide();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            ItemSetDataEditor.SelectionForm.Hide();
            // Deactivate selection limit
            _controller.Selection.LimitSelectionToFirstGeometryType = false;
            _controller.Selection.EnableShellEdgeFaceSelection = false;
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string surfaceToEditName)
        {
            // Selection limits must precede _controller.CreateNewSelection to work properly
            _controller.Selection.LimitSelectionToFirstGeometryType = true;
            _controller.Selection.EnableShellEdgeFaceSelection = true;
            //
            this.btnOkAddNew.Visible = surfaceToEditName == null;
            //
            _propertyItemChanged = false;
            _surfaceNames.Clear();
            _surfaceToEditName = null;
            _viewSurface = null;
            propertyGrid.SelectedObject = null;
            _prevSelectionNodes = null;
            _selectionNodeIds = null;
            //
            _surfaceNames.UnionWith(_controller.GetAllMeshEntityNames());
            _surfaceToEditName = surfaceToEditName;
            //
            string[] nodeSetNames = _controller.GetUserNodeSetNames();
            // Create new surface
            if (_surfaceToEditName == null)
            {
                Surface = new FeSurface(GetSurfaceName());
                _controller.Selection.Clear();                
            }
            // Edit existing surface
            else
            {
                Surface = _controller.GetSurface(_surfaceToEditName);   // to clone
                //
                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                {
                    int[] ids = Surface.FaceIds;
                    if (Surface.CreationData == null && ids != null)    // from .inp
                    {
                        // Add creation data
                        Surface.CreationData = new Selection();
                        Surface.CreationData.SelectItem = vtkSelectItem.Surface;
                        Surface.CreationData.Add(new SelectionNodeIds(vtkSelectOperation.Add, false, ids));
                    }
                    // Change node selection history to ids to speed up
                    _selectionNodeIds = new SelectionNodeIds(vtkSelectOperation.None, false, ids);
                    _prevSelectionNodes = Surface.CreationData.Nodes;
                    _controller.CreateNewSelection(Surface.CreationData.CurrentView, vtkSelectItem.Surface, _selectionNodeIds, true);
                    Surface.CreationData = _controller.Selection.DeepClone();
                }
                else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
                    CheckMissingValueRef(ref nodeSetNames, _viewSurface.NodeSetName, s => { _viewSurface.NodeSetName = s; });
                else throw new NotSupportedException();
            }
            //
            _viewSurface.PopulateDropDownList(nodeSetNames);
            //
            propertyGrid.SelectedObject = _viewSurface;
            propertyGrid.Select();
            // Show ItemSetDataForm
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                ItemSetDataEditor.SelectionForm.ItemSetData = new ItemSetData(Surface.FaceIds);
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            }
            //
            SetSelectItem();
            //
            HighlightSurface();
            //
            return true;
        }


        // Methods                                                                                                                  
        private string GetSurfaceName()
        {
            return _surfaceNames.GetNextNumberedKey("Surface");
        }
        private void HighlightSurface()
        {
            try
            {
                UpdateSurfaceArea();
                //
                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && Surface.CreatedFromNodeSetName != null &&
                    _controller.GetAllNodeSetNames().Contains(Surface.CreatedFromNodeSetName))
                {
                    _controller.Highlight3DObjects(new object[] { Surface.CreatedFromNodeSetName });
                }
                else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                {
                    SetSelectItem();
                    //
                    if (Surface.CreationData != null)
                    {
                        _controller.Selection = Surface.CreationData.DeepClone(); // deep copy to not clear
                        _controller.HighlightSelection();
                    }
                    // Clear possible highlight from part/element set
                    else _controller.ClearSelectionHistory();
                }
                else throw new NotSupportedException();
            }
            catch { }
        }
        private void UpdateSurfaceArea()
        {
            try
            {
                // update area
                if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection && Surface.CreationData != null)
                    _controller.UpdateSurfaceArea(Surface);
                else if (Surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && Surface.CreatedFromNodeSetName != null
                         && _controller.GetUserNodeSetNames().Contains(Surface.CreatedFromNodeSetName))
                    _controller.UpdateSurfaceArea(Surface);
                else Surface.Area = 0;
                //
                propertyGrid.Refresh();
            }
            catch { }
        }
        //
        private void ShowHideSelectionForm()
        {
            if (Surface != null && Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                ItemSetDataEditor.SelectionForm.ShowIfHidden(this.Owner);
            else
                ItemSetDataEditor.SelectionForm.Hide();
            //
            SetSelectItem();
        }
        private void SetSelectItem()
        {
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection) _controller.SetSelectItemToSurface();
            else _controller.SetSelectByToOff();
        }
        public void SelectionChanged(int[] ids)
        {
            if (Surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                Surface.FaceIds = ids;
                Surface.CreationData = _controller.Selection.DeepClone();
                UpdateSurfaceArea();
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
            if (_surfaceToEditName == null) return true;
            //
            FeSurface surface = _controller.GetSurface(_surfaceToEditName);     // surface was modified for speed up
            if (surface == null || surface.CreationData == null) return true;   // node based surface
            return surface.CreationData.IsGeometryBased();
        }
    }


}
