using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaeGlobals;
using CaeMesh;

namespace PrePoMax.Forms
{
    public partial class FrmQuery : UserControls.PrePoMaxChildForm
    {
        // Variables                                                                                                                
        private int _numOfNodesToSelect;
        private double[][] _coorNodesToDraw;
        private double[][] _coorLinesToDraw;
        private Controller _controller;


        // Callbacks                                                                                                               
        public Action<string> Form_WriteDataToOutput;
        public Action<object, EventArgs> Form_RemoveAnnotations;

        // Constructors                                                                                                             
        public FrmQuery()
        {
            InitializeComponent();
            _numOfNodesToSelect = -1;
        }


        // Event hadlers                                                                                                            
        private void lvQueries_MouseDown(object sender, MouseEventArgs e)
        {
            lvQueries.SelectedItems.Clear();
        }
        private void lvQueries_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void lvQueries_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (lvQueries.SelectedItems.Count > 0)
            {
                switch (lvQueries.SelectedItems[0].Text)
                {
                    case ("Vertex/Node"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _numOfNodesToSelect = 1;
                        break;
                    case ("Facet/Element"):
                        _controller.SelectBy = vtkSelectBy.QueryElement;
                        _controller.Selection.SelectItem = vtkSelectItem.Element;
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Edge"):
                        _controller.SelectBy = vtkSelectBy.QueryEdge;
                        _controller.Selection.SelectItem = vtkSelectItem.Edge;
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Surface"):
                        _controller.SelectBy = vtkSelectBy.QuerySurface;
                        _controller.Selection.SelectItem = vtkSelectItem.Surface;
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Part"):
                        _controller.SelectBy = vtkSelectBy.QueryPart;
                        _controller.Selection.SelectItem = vtkSelectItem.Part;
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Assembly"):
                        _controller.SelectBy = vtkSelectBy.Default;
                        _controller.Selection.SelectItem = vtkSelectItem.None;
                        OutputAssemblyData();
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Bounding box size"):
                        _controller.SelectBy = vtkSelectBy.Default;
                        _controller.Selection.SelectItem = vtkSelectItem.None;
                        OutputBoundingBox();
                        _numOfNodesToSelect = -1;
                        break;
                    case ("Distance"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _numOfNodesToSelect = 2;
                        break;
                    case ("Angle"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _numOfNodesToSelect = 3;
                        break;
                    case ("Circle"):
                        _controller.SelectBy = vtkSelectBy.QueryNode;
                        _controller.Selection.SelectItem = vtkSelectItem.Node;
                        _numOfNodesToSelect = 3;
                        break;
                    default:
                        break;
                }
                // Clear
                RemoveMeasureAnnotation();
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Annotations
            Form_RemoveAnnotations?.Invoke(null, null);
            // Selection
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void FrmQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
        private void FrmQuery_VisibleChanged(object sender, EventArgs e)
        {
            // This is called if some other form is shown to close all other forms
            // This is called after the form visibility changes
            if (this.Visible)
            {
                lvQueries.Items[0].Selected = true;
            }
            // The form was hidden 
            else
            {
                _controller.SelectBy = vtkSelectBy.Default;
                // Clear
                RemoveMeasureAnnotation();
                _controller.ClearSelectionHistoryAndCallSelectionChanged();
            }                
        }


        // Methods                                                                                                                  
        public void PrepareForm(Controller controller)
        {
            _controller = controller;
            lvQueries.HideSelection = false;
            lvQueries.SelectedIndices.Clear();
            //
            _controller.SetSelectByToOff();
        }
        //
        public void PickedIds(int[] ids)
        {
            try
            {
                // Clear annotations
                RemoveMeasureAnnotation();
                //
                if (ids == null || ids.Length == 0) return;
                //
                if (_controller.SelectBy == vtkSelectBy.QueryElement && ids.Length == 1) OneElementPicked(ids[0]);
                else if (_controller.SelectBy == vtkSelectBy.QueryEdge && ids.Length == 1) OneEdgePicked(ids[0]);
                else if (_controller.SelectBy == vtkSelectBy.QuerySurface)
                {
                    SelectionNodeMouse selectionNodeMouse = _controller.Selection.Nodes[0] as SelectionNodeMouse;
                    if (selectionNodeMouse != null)
                    {
                        // Clear - to remove this mouseSelectionNode from the history which is used for speed optimization
                        _controller.Selection.Clear();  
                        ids = _controller.GetIdsFromSelectionNodeMouse(selectionNodeMouse, true);
                        OneSurfacePicked(ids[0]);
                    }
                }
                else if (_controller.SelectBy == vtkSelectBy.QueryPart && ids.Length == 1) OnePartPicked(ids[0]);
                else if (ids.Length == _numOfNodesToSelect)
                {
                    // One node
                    if (ids.Length == 1) OneNodePicked(ids[0]);
                    // Two nodes
                    else if (ids.Length == 2) TwoNodesPicked(ids[0], ids[1]);
                    // Three nodes
                    else if (ids.Length == 3) ThreeNodesPicked(ids[0], ids[1], ids[2]);
                    //
                    _controller.ClearSelectionHistoryAndCallSelectionChanged();
                    //
                    HighlightNodes();
                }
            }
            catch
            { }
        }
        //
        public void OneNodePicked(int nodeId)
        {
            if (Form_WriteDataToOutput != null)
            {
                string data;
                string lenUnit = _controller.GetLengthUnit();
                string lenUnitInBrackets = string.Format("[{0}]", lenUnit);
                _coorNodesToDraw = new double[_numOfNodesToSelect][];
                //
                Vec3D baseV = new Vec3D(_controller.GetNode(nodeId).Coor);
                // Item name
                string itemName = "Node";
                if (_controller.CurrentView == ViewGeometryModelResults.Geometry) itemName = "Vertex";
                //
                Form_WriteDataToOutput("");
                data = string.Format("{0,16}{1,8}{2,16}{3,16}", itemName.PadRight(16), "[/]", "id:", nodeId);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}",
                                     "Base".PadRight(16), lenUnitInBrackets, "x, y, z:", baseV.X, baseV.Y, baseV.Z);
                Form_WriteDataToOutput(data);
                //
                if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    float fieldValue = _controller.GetNodalValue(nodeId);
                    string fieldUnit = "[" + _controller.GetCurrentResultsUnitAbbreviation() + "]";
                    //
                    Vec3D trueScaledV = new Vec3D(_controller.GetScaledNode(1, nodeId).Coor);
                    Vec3D disp = trueScaledV - baseV;
                    //
                    data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}",
                                         "Deformed".PadRight(16), lenUnitInBrackets, "x, y, z:",
                                         trueScaledV.X, trueScaledV.Y, trueScaledV.Z);
                    Form_WriteDataToOutput(data);
                    data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}",
                                         "Displacement".PadRight(16), lenUnitInBrackets, "x, y, z:", disp.X, disp.Y, disp.Z);
                    Form_WriteDataToOutput(data);
                    data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Field value".PadRight(16), fieldUnit, ":", fieldValue);
                    Form_WriteDataToOutput(data);
                    //
                    float scale = _controller.GetScale();
                    baseV = new Vec3D(_controller.GetScaledNode(scale, nodeId).Coor); // for the _coorNodesToDraw
                }
                //
                Form_WriteDataToOutput("");
                //
                _coorNodesToDraw[0] = baseV.Coor;
                _coorLinesToDraw = null;
                //
                _controller.Annotations.AddNodeAnnotation(nodeId);
            }
        }
        public void OneElementPicked(int elementId)
        {
            // Item name
            string itemName = "Element id:";
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry) itemName = "Facet id:";
            //
            Form_WriteDataToOutput("");
            string data = string.Format("{0,16}{1,8}", itemName.PadRight(16), elementId);
            Form_WriteDataToOutput(data);
            if (_controller.CurrentView == ViewGeometryModelResults.Model)
            {
                string elementType = _controller.GetElementType(elementId);
                data = string.Format("{0,16}{1,8}", "Element type:".PadRight(16), elementType);
                Form_WriteDataToOutput(data);
            }
            //
            Form_WriteDataToOutput("");
            //
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            //
            _controller.HighlightElement(elementId);
            //
            _controller.Annotations.AddElementAnnotation(elementId);
        }
        public void OneEdgePicked(int geometryId)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
            BasePart part = _controller.DisplayedMesh.GetPartById(itemTypePartIds[2]);
            double length1 = _controller.DisplayedMesh.GetEdgeLength(geometryId);
            string lenUnit = _controller.GetLengthUnit();
            string lenUnitInBrackets = string.Format("[{0}]", lenUnit);
            //
            Form_WriteDataToOutput("");
            string data = string.Format("Edge on part: {0}", part.Name);            
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16}", "Edge".PadRight(16), "[/]", "id:", itemTypePartIds[0] + 1);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Base".PadRight(16), lenUnitInBrackets, "L:", length1);
            Form_WriteDataToOutput(data);
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                int[] nodeIds;
                _controller.DisplayedMesh.GetEdgeNodeCoor(geometryId, out nodeIds, out double[][] nodeCoor);
                FeNode[] nodes = _controller.GetScaledNodes(1, nodeIds);
                double length2 = 0;
                Vec3D n1;
                Vec3D n2;
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    n1 = new Vec3D(nodes[i].Coor);
                    n2 = new Vec3D(nodes[i + 1].Coor);
                    length2 += (n2 - n1).Len;
                }
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Deformed".PadRight(16), lenUnitInBrackets, "L:", length2);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Delta".PadRight(16), lenUnitInBrackets, "L:", length2 - length1);
                Form_WriteDataToOutput(data);
            }
            Form_WriteDataToOutput("");
            //
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            //
            _controller.HighlightItemsByGeometryEdgeIds(new int[] { geometryId }, false);
            //
            _controller.Annotations.AddEdgeAnnotation(geometryId);
        }
        public void OneSurfacePicked(int geometryId)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
            BasePart part = _controller.DisplayedMesh.GetPartById(itemTypePartIds[2]);
            int surfaceId = itemTypePartIds[0];
            double area1 = _controller.DisplayedMesh.GetSurfaceArea(geometryId);
            string areaUnit = "[" + _controller.GetAreaUnit() + "]";
            //
            Form_WriteDataToOutput("");
            string data = string.Format("Surface on part: {0}", part.Name);
            if (part.Visualization.FaceTypes != null)
            {
                data += string.Format("   Surface type: {0}", part.Visualization.FaceTypes[surfaceId]);
            }
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16}", "Surface".PadRight(16), "[/]", "id:", surfaceId + 1);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Base".PadRight(16), areaUnit, "A:", area1);
            Form_WriteDataToOutput(data);
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                int[] nodeIds;
                _controller.DisplayedMesh.GetFaceNodes(geometryId, out nodeIds);
                FeNode[] nodes = _controller.GetScaledNodes(1, nodeIds);
                Dictionary<int, FeNode> nodesDic = new Dictionary<int, FeNode>();
                for (int i = 0; i < nodes.Length; i++) nodesDic.Add(nodes[i].Id, nodes[i]);
                double area2 = _controller.DisplayedMesh.ComputeFaceArea(part.Visualization, surfaceId, nodesDic);
                //
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Deformed".PadRight(16), areaUnit, "A:", area2);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Delta".PadRight(16), areaUnit, "A:", area2 - area1);
                Form_WriteDataToOutput(data);
            }
            Form_WriteDataToOutput("");
            //
            _controller.ClearSelectionHistoryAndCallSelectionChanged();    // in order to prevent SHIFT ADD
            //
            _controller.HighlightItemsBySurfaceIds(new int[] { geometryId }, false);
            //
            _controller.Annotations.AddSurfaceAnnotation(geometryId);
        }
        public void OnePartPicked(int partId)
        {
            FeMesh mesh = _controller.DisplayedMesh;
            //
            BasePart part = mesh.GetPartById(partId);
            if (part == null) throw new NotSupportedException();
            //
            Form_WriteDataToOutput("");
            string data = string.Format("Part name: {0}", part.Name);
            Form_WriteDataToOutput(data);
            data = string.Format("Part id: {0}", part.PartId);
            Form_WriteDataToOutput(data);
            data = string.Format("Part type: {0}", part.PartType);
            Form_WriteDataToOutput(data);
            // Item name
            string elementsName = "Number of elements:";
            string nodesName = "Number of nodes:";
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                elementsName = "Number of facets:";
                nodesName = "Number of vertices:";
            }
            data = string.Format("{0} {1}", elementsName, part.Labels.Length);
            Form_WriteDataToOutput(data);
            data = string.Format("{0} {1}", nodesName, part.NodeLabels.Length);
            Form_WriteDataToOutput(data);
            Form_WriteDataToOutput("");
            //
            _controller.ClearSelectionHistoryAndCallSelectionChanged();
            //
            _controller.Highlight3DObjects(new object[] { part });
            //
            _controller.Annotations.AddPartAnnotation(part.Name);
        }
        private void OutputAssemblyData()
        {
            FeMesh mesh = _controller.DisplayedMesh;
            if (mesh == null) return;
            double[] bb = _controller.GetBoundingBox();
            double[] size = new double[] { bb[1] - bb[0], bb[3] - bb[2], bb[5] - bb[4] };
            //
            Form_WriteDataToOutput("");
            string data = string.Format("Assembly");
            Form_WriteDataToOutput(data);
            data = string.Format("Number of parts: {0}", mesh.Parts.Count);
            Form_WriteDataToOutput(data);
            // Item name
            string elementsName = "Number of elements:";
            string nodesName = "Number of nodes:";
            if (_controller.CurrentView == ViewGeometryModelResults.Geometry)
            {
                elementsName = "Number of facets:";
                nodesName = "Number of vertices:";
            }
            data = string.Format("{0} {1}", elementsName, mesh.Elements.Count);
            Form_WriteDataToOutput(data);
            data = string.Format("{0} {1}", nodesName, mesh.Nodes.Count);
            Form_WriteDataToOutput(data);
            Form_WriteDataToOutput("");
        }
        private void OutputBoundingBox()
        {
            double[] bb = _controller.GetBoundingBox();
            double[] size = new double[] { bb[1] - bb[0], bb[3] - bb[2], bb[5] - bb[4] };
            string lenUnit = _controller.GetLengthUnit();
            string lenUnitInBrackets = string.Format("[{0}]", lenUnit);
            //
            Form_WriteDataToOutput("");
            string data = string.Format("Bounding box");
            Form_WriteDataToOutput(data);
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                double scale = _controller.GetScale();
                data = string.Format("{0,17}{1,7}{2,16}{3,16:E}", "Def. scale factor".PadRight(17), "[/]", "sf:", scale);
                Form_WriteDataToOutput(data);
            }
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}", "Min".PadRight(16), lenUnitInBrackets,
                                 "x, y, z:", bb[0], bb[2], bb[4]);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}", "Max".PadRight(16), lenUnitInBrackets,
                                 "x, y, z:", bb[1], bb[3], bb[5]);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}", "Size".PadRight(16), lenUnitInBrackets,
                                 "x, y, z:", size[0], size[1], size[2]);
            Form_WriteDataToOutput(data);
        }
        public void TwoNodesPicked(int nodeId1, int nodeId2)
        {
            if (Form_WriteDataToOutput != null)
            {
                string data;
                _coorNodesToDraw = new double[_numOfNodesToSelect][];
                //
                Vec3D trueScaledD = null;
                Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
                Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
                Vec3D baseD = baseV2 - baseV1;
                string lenUnit = _controller.GetLengthUnit();
                string lenUnitInBrackets = string.Format("[{0}]", lenUnit);
                //
                Form_WriteDataToOutput("");
                data = string.Format(
                    "{0,16}{1,8}{2,16}{3,16}, {4,16}", "Distance".PadRight(16), "[/]", "id1, id2:", nodeId1, nodeId2);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}, {6,16:E}",
                                     "Base".PadRight(16), lenUnitInBrackets, "dx, dy, dz, D:", baseD.X, baseD.Y, baseD.Z, baseD.Len);
                Form_WriteDataToOutput(data);
                //
                if (_controller.CurrentView == ViewGeometryModelResults.Results)
                {
                    Vec3D trueScaledV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                    Vec3D trueScaledV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                    trueScaledD = trueScaledV2 - trueScaledV1;
                    Vec3D delta = trueScaledD - baseD;
                    //
                    data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}, {6,16:E}",
                                         "Deformed".PadRight(16), lenUnitInBrackets, "dx, dy, dz, D:",
                                         trueScaledD.X, trueScaledD.Y, trueScaledD.Z, trueScaledD.Len);
                    Form_WriteDataToOutput(data);
                    data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}, {6,16:E}",
                                         "Delta".PadRight(16), lenUnitInBrackets, "dx, dy, dz, D:",
                                         delta.X, delta.Y, delta.Z, trueScaledD.Len - baseD.Len);
                    Form_WriteDataToOutput(data);
                    //
                    float scale = _controller.GetScale();
                    baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                    baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                }
                Form_WriteDataToOutput("");
                //
                _coorNodesToDraw[0] = baseV1.Coor;
                _coorNodesToDraw[1] = baseV2.Coor;
                _coorLinesToDraw = _coorNodesToDraw;
                // Annotation
                string text;
                string numberFormat = _controller.Settings.Annotations.GetNumberFormat();
                //
                if (trueScaledD != null) baseD = trueScaledD;
                //
                text = string.Format("Distance: {0} {1}{2}", baseD.Len.ToString(numberFormat), lenUnit, Environment.NewLine);
                text += string.Format("dx: {0} {1}{2}", baseD.X.ToString(numberFormat), lenUnit, Environment.NewLine);
                text += string.Format("dy: {0} {1}{2}", baseD.Y.ToString(numberFormat), lenUnit, Environment.NewLine);
                text += string.Format("dz: {0} {1}", baseD.Z.ToString(numberFormat), lenUnit);
                //
                Vec3D anchor = (baseV1 + baseV2) * 0.5;
                _controller.Annotations.AddMeasureAnnotation(text, anchor.Coor);
            }
        }
        public void ThreeNodesPicked(int nodeId1, int nodeId2, int nodeId3)
        {
            if (Form_WriteDataToOutput != null)
            {
                if (lvQueries.SelectedItems[0].Text == "Angle") ComputeAngle(nodeId1, nodeId2, nodeId3);
                else if (lvQueries.SelectedItems[0].Text == "Circle") ComputeCircle(nodeId1, nodeId2, nodeId3);
            }
        }
        //
        private void ComputeAngle(int nodeId1, int nodeId2, int nodeId3)
        {
            System.Diagnostics.Debug.Write(nodeId1 + " " + nodeId2 + " " + nodeId3);
            string data;
            double angle;
            double angle2draw;
            Vec3D p;
            Vec3D axis;
            Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
            Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
            Vec3D baseV3 = new Vec3D(_controller.GetNode(nodeId3).Coor);
            //
            angle = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis);
            angle2draw = angle;
            string angleUnit = "°";
            string angleUnitInBrackets = "[°]";
            //
            data = string.Format("{0,16}{1,8}{2,16}{3,16}, {4,16}, {5,16}",
                                 "Angle".PadRight(16), "[/]", "id1, id2, id3:", nodeId1, nodeId2, nodeId3);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Base".PadRight(16), angleUnitInBrackets, "ϕ:", angle);
            Form_WriteDataToOutput(data);
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                baseV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                baseV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                baseV3 = new Vec3D(_controller.GetScaledNode(1, nodeId3).Coor);
                //
                double angle2 = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis);
                double delta = angle2 - angle;
                //
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Deformed".PadRight(16), angleUnitInBrackets, "ϕ:", angle2);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}", "Delta".PadRight(16), angleUnitInBrackets, "ϕ:", delta);
                Form_WriteDataToOutput(data);
                //
                float scale = _controller.GetScale();
                baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                baseV3 = new Vec3D(_controller.GetScaledNode(scale, nodeId3).Coor);    // for the _coorNodesToDraw
                //
                angle = angle2;                                                 // for the annotation
                angle2draw = ComputeAngle(baseV1, baseV2, baseV3, out p, out axis); // for drawing
            }
            Form_WriteDataToOutput("");
            //
            _coorNodesToDraw = new double[_numOfNodesToSelect][];
            _coorNodesToDraw[0] = baseV1.Coor;
            _coorNodesToDraw[1] = baseV2.Coor;
            _coorNodesToDraw[2] = baseV3.Coor;
            //
            List<double[]> coorLines = new List<double[]>() { baseV1.Coor, baseV2.Coor, baseV3.Coor, baseV2.Coor };
            coorLines.AddRange(ComputeCirclePoints(baseV2, axis, p, angle2draw * Math.PI / 180));
            _coorLinesToDraw = coorLines.ToArray();
            // Annotation
            string text;
            string numberFormat = _controller.Settings.Annotations.GetNumberFormat();
            //
            text = string.Format("Angle: {0}{1}", angle.ToString(numberFormat), angleUnit);
            //
            double[] anchor = _coorLinesToDraw[_coorLinesToDraw.Length / 2];
            _controller.Annotations.AddMeasureAnnotation(text, anchor);
        }
        private void ComputeCircle(int nodeId1, int nodeId2, int nodeId3)
        {
            // https://en.wikipedia.org/wiki/Circumscribed_circle
            string data;
            double r;
            Vec3D center;
            Vec3D axis;
            double rDraw;
            Vec3D centerDraw;
            Vec3D axisDraw;
            Vec3D baseV1 = new Vec3D(_controller.GetNode(nodeId1).Coor);
            Vec3D baseV2 = new Vec3D(_controller.GetNode(nodeId2).Coor);
            Vec3D baseV3 = new Vec3D(_controller.GetNode(nodeId3).Coor);
            //
            Vec3D.GetCircle(baseV1, baseV2, baseV3, out r, out center, out axis);
            rDraw = r;
            centerDraw = center;
            axisDraw = axis;
            string lenUnit = _controller.GetLengthUnit();
            string lenUnitInBrackets = string.Format("[{0}]", lenUnit);
            //
            Form_WriteDataToOutput("");
            data = string.Format("{0,16}{1,8}{2,16}{3,16}, {4,16}, {5,16}",
                                 "Circle".PadRight(16), "[/]", "id1, id2, id3:", nodeId1, nodeId2, nodeId3);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}, {6,16:E}",
                                 "Base".PadRight(16), lenUnitInBrackets, "x, y, z, R:", center.X, center.Y, center.Z, r);
            Form_WriteDataToOutput(data);
            data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}",
                                 "Base axis".PadRight(16), lenUnitInBrackets, "x, y, z:", axis.X, axis.Y, axis.Z);
            Form_WriteDataToOutput(data);
            //
            if (_controller.CurrentView == ViewGeometryModelResults.Results)
            {
                baseV1 = new Vec3D(_controller.GetScaledNode(1, nodeId1).Coor);
                baseV2 = new Vec3D(_controller.GetScaledNode(1, nodeId2).Coor);
                baseV3 = new Vec3D(_controller.GetScaledNode(1, nodeId3).Coor);
                //
                Vec3D.GetCircle(baseV1, baseV2, baseV3, out r, out center, out axis);
                //
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}, {6,16:E}",
                                     "Deformed".PadRight(16), lenUnitInBrackets, "x, y, z, R:", center.X, center.Y, center.Z, r);
                Form_WriteDataToOutput(data);
                data = string.Format("{0,16}{1,8}{2,16}{3,16:E}, {4,16:E}, {5,16:E}",
                                     "Deformed axis".PadRight(16), lenUnitInBrackets, "x, y, z:", axis.X, axis.Y, axis.Z);
                Form_WriteDataToOutput(data);
                //
                float scale = _controller.GetScale();
                baseV1 = new Vec3D(_controller.GetScaledNode(scale, nodeId1).Coor);    // for the _coorNodesToDraw
                baseV2 = new Vec3D(_controller.GetScaledNode(scale, nodeId2).Coor);    // for the _coorNodesToDraw
                baseV3 = new Vec3D(_controller.GetScaledNode(scale, nodeId3).Coor);    // for the _coorNodesToDraw
                //
                Vec3D.GetCircle(baseV1, baseV2, baseV3, out rDraw, out centerDraw, out axisDraw);
            }
            Form_WriteDataToOutput("");
            //
            _coorNodesToDraw = new double[_numOfNodesToSelect + 1][];
            _coorNodesToDraw[0] = baseV1.Coor;
            _coorNodesToDraw[1] = baseV2.Coor;
            _coorNodesToDraw[2] = baseV3.Coor;
            _coorNodesToDraw[3] = centerDraw.Coor;
            //
            _coorLinesToDraw = ComputeCirclePoints(centerDraw, axisDraw, baseV1, 2 * Math.PI);
            // Annotation
            string text;
            string numberFormat = _controller.Settings.Annotations.GetNumberFormat();
            //
            text = string.Format("Radius: {0} {1}{2}", r.ToString(numberFormat), lenUnit, Environment.NewLine);
            text += string.Format("X: {0} {1}{2}", center.X.ToString(numberFormat), lenUnit, Environment.NewLine);
            text += string.Format("Y: {0} {1}{2}", center.Y.ToString(numberFormat), lenUnit, Environment.NewLine);
            text += string.Format("Z: {0} {1}", center.Z.ToString(numberFormat), lenUnit);
            //
            double[] anchor = _coorLinesToDraw[2];
            _controller.Annotations.AddMeasureAnnotation(text, anchor);
        }
        private double ComputeAngle(Vec3D baseV1, Vec3D baseV2, Vec3D baseV3, out Vec3D p, out Vec3D axis)
        {
            Vec3D line1 = baseV1 - baseV2;
            Vec3D line2 = baseV3 - baseV2;
            //
            if (line1.Len2 > line2.Len2)  // find shorter line 
            {
                p = baseV2 + line2 * 0.5;
                axis = Vec3D.CrossProduct(line2, line1);
            }
            else
            {
                p = baseV2 + line1 * 0.5;
                axis = Vec3D.CrossProduct(line1, line2);
            }
            axis.Normalize();
            line1.Normalize();
            line2.Normalize();
            //
            double angle = Math.Acos(Vec3D.DotProduct(line1, line2)) * 180 / Math.PI;
            return angle;
        }        
        private double[][] ComputeCirclePoints(Vec3D c, Vec3D axis, Vec3D p, double angle)
        {
            // The circe is constructed by moving the r vector around the axis
            // d vector is the change in normal n and perpendicular r direction
            int segments = 40;
            double [][] coorLines = new double[segments + 1][];
            double dAngle = angle / segments;
            Vec3D r = p - c;
            double rLen = r.Len;
            Vec3D n;
            Vec3D d;
            //
            coorLines[0] = p.Coor;
            for (int i = 0; i < segments; i++)
            {
                n = Vec3D.CrossProduct(axis, r);
                n.Normalize();
                //
                d = rLen * Math.Sin(dAngle) * n - (1 - Math.Cos(dAngle)) * r;
                p = p + d;
                r = r + d;
                //
                coorLines[i + 1] = p.Coor;
            }
            return coorLines;
        }
        //
        private void RemoveMeasureAnnotation()
        {
            _controller.Annotations.RemoveCurrentMeasureAnnotation();
        }
        private void HighlightNodes()
        {
            if (_coorNodesToDraw != null)
            {
                if (_coorNodesToDraw.GetLength(0) == 1)
                {
                    _controller.HighlightNodes(_coorNodesToDraw);
                }
                else if (_coorNodesToDraw.GetLength(0) >= 2)
                {
                    _controller.HighlightNodes(_coorNodesToDraw);
                    _controller.HighlightConnectedLines(_coorLinesToDraw);
                }
            }
        }
    }
}