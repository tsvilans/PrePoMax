using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class ContactSurface
    {
        // Variables                                                                                                                
        private FeMesh _mesh;
        private BasePart _part;
        private int _id;
        private int _geometryId;
        private GeometryType _geometryType;
        private HashSet<int> _nodeIds;
        private BoundingBox _boundingBox;
        private bool _internal;


        // Properties                                                                                                               
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }
        public BasePart Part { get { return _part; } set { _part = value; } }
        public int Id { get { return _id; } set { _id = value; } }
        public int GeometryId { get { return _geometryId; } }
        public GeometryType GeometryType { get { return _geometryType; } set { _geometryType = value; } }
        public HashSet<int> NodeIds { get { return _nodeIds; } set { _nodeIds = value; } }
        public BoundingBox BoundingBox { get { return _boundingBox; } set { _boundingBox = value; } }
        public bool Internal { get { return _internal; } set { _internal = value; } }


        // Constructors                                                                                                             
        public ContactSurface(FeMesh mesh, double[][] _nodes, BasePart part, int id, GeometryType geometryType,
                              double boundingBoxOffset)
        {
            _mesh = mesh;
            _part = part;
            _id = id;
            _geometryType = geometryType;
            _geometryId = FeMesh.GetGeometryId(_id, (int)_geometryType, _part.PartId);
            //
            if (_geometryType == GeometryType.ShellEdgeSurface) _nodeIds = part.Visualization.GetNodeIdsByEdge(_id);
            else _nodeIds = part.Visualization.GetNodeIdsBySurface(_id);
            //
            _boundingBox = new BoundingBox();
            _boundingBox.IncludeFirstCoor(_nodes[_nodeIds.First()]);
            foreach (var nodeId in _nodeIds) _boundingBox.IncludeCoorFast(_nodes[nodeId]);
            _boundingBox.Inflate(boundingBoxOffset);
            //
            _internal = false;
        }
        public ContactSurface(ContactSurface contactSurface)
        {
            _mesh = contactSurface._mesh;
            _part = contactSurface._part;
            _id = contactSurface._id;
            _geometryId = contactSurface._geometryId;
            _geometryType = contactSurface._geometryType;
            _nodeIds = new HashSet<int>(contactSurface._nodeIds);
            _boundingBox = new BoundingBox(contactSurface._boundingBox);
            _internal = contactSurface._internal;
        }


        // Methods                                                                                                                  
        public void ConvertToShellBackSurface()
        {
            if (_geometryType == GeometryType.ShellFrontSurface)
            { 
                int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(_geometryId);
                int backId = FeMesh.GetGeometryId(itemTypePartIds[0], (int)GeometryType.ShellBackSurface, itemTypePartIds[2]);
                //
                _geometryType = GeometryType.ShellBackSurface;
                _geometryId= backId;
            }
            else throw new NotSupportedException();
        }
        public int[] GetCell(int cellId)
        {
            if (_geometryType == GeometryType.ShellEdgeSurface)
            {
                return _part.Visualization.EdgeCells[cellId];
            }
            else
            {
                int[] cell = _part.Visualization.Cells[cellId];
                //
                if (_geometryType == GeometryType.ShellBackSurface)
                {
                    if (cell.Length == 3) return new int[] { cell[0], cell[2], cell[1] };
                    else if (cell.Length == 4) return new int[] { cell[0], cell[3], cell[2], cell[1] };
                    else if (cell.Length == 6) return new int[] { cell[0], cell[2], cell[1], cell[5], cell[4], cell[3] };
                    else if (cell.Length == 8) return new int[] { cell[0], cell[3], cell[2], cell[1],
                                                                  cell[7], cell[6], cell[5], cell[4] };
                    else throw new NotSupportedException();
                }
                else return cell;
            }
        }        
        public double[] GetCellNormal(int cellId)
        {
            int[] cell = _part.Visualization.Cells[cellId];
            double[] n1 = _mesh.Nodes[cell[0]].Coor;
            double[] n2 = _mesh.Nodes[cell[1]].Coor;
            double[] n3 = _mesh.Nodes[cell[2]].Coor;
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n = new double[3];
            Geometry.VmV(ref a, n2, n1);
            Geometry.VmV(ref b, n3, n1);
            Geometry.VcrossV(ref n, a, b);
            Geometry.Vnorm(ref n, n);
            return n;
        }
        public void GetEdgeCellNormal(int edgeCellId, int baseCellId, ref double[] cellNormal, ref double[] edgeCellNormal)
        {
            //int baseCellId = _part.Visualization.GetEdgeCellBaseCellIdSlow(edgeCellId);
            if (baseCellId == -1)
                baseCellId = _part.Visualization.GetEdgeCellBaseCellIdSlow(edgeCellId);
            //
            int[] cell = _part.Visualization.Cells[baseCellId];
            int[] edgeCell = _part.Visualization.EdgeCells[edgeCellId];
            int n1Id;
            int numNodes;
            if (cell.Length == 3 || cell.Length == 6) numNodes = 3;
            else numNodes = 4;
            // Find the first edge node
            if (edgeCell[0] == cell[0]) n1Id = 0;
            else if (edgeCell[0] == cell[1]) n1Id = 1;
            else if (edgeCell[0] == cell[2]) n1Id = 2;
            else if (numNodes == 4 && edgeCell[0] == cell[3]) n1Id = 3;
            else throw new NotSupportedException();
            // Find the second node
            int delta;
            if (edgeCell[1] == cell[(n1Id + 1) % numNodes]) delta = 1;
            else delta = -1;
            //
            double[] n1 = _mesh.Nodes[cell[n1Id]].Coor;
            double[] n2 = _mesh.Nodes[cell[(numNodes + n1Id + 1 * delta) % numNodes]].Coor;
            double[] n3 = _mesh.Nodes[cell[(numNodes + n1Id + 2 * delta) % numNodes]].Coor;
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n = new double[3];
            Geometry.VmV(ref a, n2, n1);
            Geometry.VmV(ref b, n3, n1);
            Geometry.VcrossV(ref n, a, b);
            Geometry.Vnorm(ref cellNormal, n);
            //
            Geometry.VcrossV(ref edgeCellNormal, a, n);
            Geometry.Vnorm(ref edgeCellNormal, edgeCellNormal);
        }
    }
}
