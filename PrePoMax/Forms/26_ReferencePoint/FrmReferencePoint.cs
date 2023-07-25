using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using CaeModel;
using System.Windows.Forms;
using System.Drawing;

namespace PrePoMax.Forms
{
    class FrmReferencePoint : UserControls.FrmProperties, IFormBase, IFormHighlight
    {
        // Variables                                                                                                                
        private HashSet<string> _referencePointNames;
        private string _referencePointToEditName;
        private ViewFeReferencePoint _viewReferencePoint;
        private Controller _controller;
        private double[][] _coorNodesToDraw;
        private string[] _nodeSetNames;
        private string[] _surfaceNames;


        // Properties                                                                                                               
        public FeReferencePoint ReferencePoint
        {
            get { return _viewReferencePoint.GetBase(); }
            set { _viewReferencePoint = new ViewFeReferencePoint(value.DeepClone()); }
        }
       

        // Constructors                                                                                                             
        public FrmReferencePoint(Controller controller)
        {
            InitializeComponent();
            //
            _controller = controller;
            _viewReferencePoint = null;
            _referencePointNames = new HashSet<string>();
            //
            _coorNodesToDraw = new double[1][];
            _coorNodesToDraw[0] = new double[3];
        }
        private void InitializeComponent()
        {
            this.gbProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // FrmReferencePoint
            // 
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Name = "FrmReferencePoint";
            this.Text = "Edit Reference Point";
            this.gbProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        // Overrides                                                                                                                
        protected override void OnPropertyGridPropertyValueChanged()
        {
            string property = propertyGrid.SelectedGridItem.PropertyDescriptor.Name;
            //
            if (property == nameof(_viewReferencePoint.CreatedFrom))
            {
                SetSelectItemAndSelection();
                //
                _controller.UpdateReferencePoint(ReferencePoint);
            }
            else if (property == nameof(_viewReferencePoint.RegionType) || property == nameof(_viewReferencePoint.NodeSetName) ||
                     property == nameof(_viewReferencePoint.SurfaceName))
            {
                _controller.UpdateReferencePoint(ReferencePoint);
            }
            //
            HighlightReferencePoint();
            //
            base.OnPropertyGridPropertyValueChanged();
        }
        protected override void OnPropertyGridSelectedGridItemChanged()
        {
            object value = propertyGrid.SelectedGridItem.Value;
            if (value != null)
            {
                string valueString = value.ToString();
                object[] objects = null;
                //
                if (propertyGrid.SelectedObject == null)
                { }
                else if (propertyGrid.SelectedObject is ViewFeReferencePoint)
                {
                    ViewFeReferencePoint vrp = propertyGrid.SelectedObject as ViewFeReferencePoint;
                    if (valueString == vrp.NodeSetName) objects = new object[] { vrp.NodeSetName };
                    else if (valueString == vrp.SurfaceName) objects = new object[] { vrp.SurfaceName };
                    else objects = null;
                }
                else throw new NotImplementedException();
                //
                _controller.Highlight3DObjects(objects);
            }
            HighlightReferencePoint();
        }
        protected override void OnApply(bool onOkAddNew)
        {
            _viewReferencePoint = (ViewFeReferencePoint)propertyGrid.SelectedObject;
            //
            CheckName(_referencePointToEditName, _viewReferencePoint.Name, _referencePointNames, "reference point");
            // Create
            if (_referencePointToEditName == null)
            {
                _controller.AddReferencePointCommand(ReferencePoint);
            }
            // Replace
            else if (_propertyItemChanged)
            {
                 _controller.ReplaceReferencePointCommand(_referencePointToEditName, ReferencePoint);
                _referencePointToEditName = null; // prevents the execution of toInternal in OnHideOrClose
            }
            // Convert the reference point from internal to show it
            else
            {
                ReferencePointInternal(false);
            }
            // If all is successful turn off the selection
            TurnOffSelection();
        }
        protected override void OnHideOrClose()
        {
            // Close the ItemSetSelectionForm
            TurnOffSelection();
            // Convert the reference point from internal to show it
            ReferencePointInternal(false);
            //
            base.OnHideOrClose();
        }
        protected override bool OnPrepareForm(string stepName, string referencePointToEditName)
        {
            this.btnOkAddNew.Visible = referencePointToEditName == null;
            //
            _propertyItemChanged = false;
            _referencePointNames.Clear();
            _referencePointToEditName = null;
            _viewReferencePoint = null;
            //
            _referencePointNames.UnionWith(_controller.GetAllMeshEntityNames());
            _referencePointToEditName = referencePointToEditName;
            //
            _nodeSetNames = _controller.GetUserNodeSetNames();
            _surfaceNames = _controller.GetUserSurfaceNames();
            // Create new reference point
            if (_referencePointToEditName == null)
            {
                if (_controller.Model.Properties.ModelSpace.IsTwoD())
                    ReferencePoint = new FeReferencePoint(GetReferencePointName(), 0, 0);
                else  ReferencePoint = new FeReferencePoint(GetReferencePointName(), 0, 0, 0);
                //
                ReferencePoint.Color = _controller.Settings.Pre.ConstraintSymbolColor;
            }
            // Edit existing reference point
            else
            {
                ReferencePoint = _controller.GetReferencePoint(_referencePointToEditName); // to clone
                // Convert the reference point to internal to hide it
                ReferencePointInternal(true);
                // Check for deleted regions
                if (ReferencePoint.CreatedFrom == FeReferencePointCreatedFrom.BoundingBoxCenter ||
                    ReferencePoint.CreatedFrom == FeReferencePointCreatedFrom.CenterOfGravity)
                {
                    ViewFeReferencePoint vrp = _viewReferencePoint; // shorten
                    if (vrp.RegionType == RegionTypeEnum.NodeSetName.ToFriendlyString())
                        CheckMissingValueRef(ref _nodeSetNames, vrp.NodeSetName, s => { vrp.NodeSetName = s; });
                    else if (vrp.RegionType == RegionTypeEnum.SurfaceName.ToFriendlyString())
                        CheckMissingValueRef(ref _surfaceNames, vrp.SurfaceName, s => { vrp.SurfaceName = s; });
                    else throw new NotSupportedException();
                }
                // CheckMissingValue changes _propertyItemChanged -> update coordinates
                if (_propertyItemChanged) _controller.UpdateReferencePoint(ReferencePoint);
            }
            //
            _viewReferencePoint.PopulateDropDownLists(_nodeSetNames, _surfaceNames);
            //
            propertyGrid.SelectedObject = _viewReferencePoint;
            propertyGrid.Select();
            //
            SetSelectItemAndSelection();
            //
            HighlightReferencePoint();
            //
            return true;
        }


        // Methods                                                                                                                  
        public void PickedIds(int[] ids)
        {
            if (ids != null)
            {
                FeReferencePoint rp = ReferencePoint;
                //
                if (ids.Length == 0)
                {
                    _viewReferencePoint.X = 0;
                    _viewReferencePoint.Y = 0;
                    _viewReferencePoint.Z = 0;
                }
                else if (ids.Length == 1 && rp.CreatedFrom == FeReferencePointCreatedFrom.Selection)
                {
                    FeNode node = _controller.Model.Mesh.Nodes[ids[0]];
                    _viewReferencePoint.X = node.X;
                    _viewReferencePoint.Y = node.Y;
                    _viewReferencePoint.Z = node.Z;
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                    //
                    _controller.ClearSelectionHistory();    // must be here to reset the number of picked ids
                    //
                    HighlightReferencePoint();
                }
                else if (ids.Length == 2 && rp.CreatedFrom == FeReferencePointCreatedFrom.BetweenTwoPoints)
                {
                    FeNode node1 = _controller.Model.Mesh.Nodes[ids[0]];
                    FeNode node2 = _controller.Model.Mesh.Nodes[ids[1]];
                    _viewReferencePoint.X = (node1.X + node2.X) / 2;
                    _viewReferencePoint.Y = (node1.Y + node2.Y) / 2;
                    _viewReferencePoint.Z = (node1.Z + node2.Z) / 2;
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                    //
                    _controller.ClearSelectionHistory();    // must be here to reset the number of picked ids
                    //
                    HighlightReferencePoint();
                }
                else if (ids.Length == 3 && rp.CreatedFrom == FeReferencePointCreatedFrom.CircleCenter)
                {
                    Vec3D v1 = new Vec3D(_controller.Model.Mesh.Nodes[ids[0]].Coor);
                    Vec3D v2 = new Vec3D(_controller.Model.Mesh.Nodes[ids[1]].Coor);
                    Vec3D v3 = new Vec3D(_controller.Model.Mesh.Nodes[ids[2]].Coor);
                    Vec3D.GetCircle(v1, v2, v3, out double r, out Vec3D center, out Vec3D axis);
                    _viewReferencePoint.X = center.X;
                    _viewReferencePoint.Y = center.Y;
                    _viewReferencePoint.Z = center.Z;
                    //
                    propertyGrid.Refresh();
                    //
                    _propertyItemChanged = true;
                    //
                    _controller.ClearSelectionHistory();    // must be here to reset the number of picked ids
                    //
                    HighlightReferencePoint();
                }
            }
        }
        private string GetReferencePointName()
        {
            return _referencePointNames.GetNextNumberedKey("Reference_Point");
        }
        private void HighlightReferencePoint()
        {
            _coorNodesToDraw[0][0] = _viewReferencePoint.X;
            _coorNodesToDraw[0][1] = _viewReferencePoint.Y;
            _coorNodesToDraw[0][2] = _viewReferencePoint.Z;
            //
            _controller.HighlightNodes(_coorNodesToDraw);
        }
        private void TurnOffSelection()
        {
            _controller.SetSelectByToOff();
            _controller.Selection.SelectItem = vtkSelectItem.None;
        }
        private void SetSelectItemAndSelection()
        {
            if (ReferencePoint is null) { }
            else if (ReferencePoint.CreatedFrom == FeReferencePointCreatedFrom.Selection ||
                     ReferencePoint.CreatedFrom == FeReferencePointCreatedFrom.BetweenTwoPoints ||
                     ReferencePoint.CreatedFrom == FeReferencePointCreatedFrom.CircleCenter)
            {
                _controller.SelectBy = vtkSelectBy.QueryNode;
                _controller.SetSelectItemToNode();
            }
            else
            {
                TurnOffSelection();
            }
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        private void ReferencePointInternal(bool toInternal)
        {
            if (_referencePointToEditName != null)
            {
                // Convert the constraint from/to internal to hide/show it
                _controller.GetReferencePoint(_referencePointToEditName).Internal = toInternal;
                _controller.FeModelUpdate(UpdateType.RedrawSymbols);
            }
        }
        // IFormHighlight
        public void Highlight()
        {
            HighlightReferencePoint();
        }

    }
}
