using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using CaeMesh.ContactSearchNamespace;

namespace CaeMesh
{
    public class ContactSearch
    {
        // Variables                                                                                                                
        private FeMesh _mesh;
        private FeMesh _geometry;
        private double[][] _nodes;
        private BoundingBox[][] _cellBoundingBoxes;
        private BoundingBox[][] _cellEdgeBoundingBoxes;
        private int[][] _edgeCellBaseCellIds;
        private bool _includeSelfContact;
        private GroupContactPairsByEnum _groupContactPairsBy;


        // Properties                                                                                                               
        public GroupContactPairsByEnum GroupContactPairsBy
        {
            get { return _groupContactPairsBy; }
            set { _groupContactPairsBy = value; }
        }


        // Constructors                                                                                                             
        public ContactSearch(FeMesh mesh, FeMesh geometry)
        {
            _mesh = mesh;
            _geometry = geometry;
            //
            if (_mesh != null && _mesh.Nodes != null)
            {
                _nodes = new double[_mesh.MaxNodeId + 1][];
                foreach (var nodeEntry in _mesh.Nodes) _nodes[nodeEntry.Key] = nodeEntry.Value.Coor;
            }
            //
            _includeSelfContact = false;
            _groupContactPairsBy = GroupContactPairsByEnum.None;
            // Get all edge cell base cell ids
            GetAllEdgeCellBaseCellIds();
        }


        // Methods                                                                                                                  
        public List<MasterSlaveItem> FindContactPairs(double distance, double angleDeg, GeometryFilterEnum filter,
                                                      bool tryToResolve)
        {
            if (_mesh == null) return null;
            // Bounding boxes for each cell
            ComputeCellBoundingBoxes(distance, filter.HasFlag(GeometryFilterEnum.IgnoreHidden));
            // Find all surfaces of the assembly
            ContactSurface[] contactSurfaces = GetAllContactSurfaces(distance, filter);
            // Find all surface pairs in contact
            double angleRad = angleDeg * Math.PI / 180;
            List<ContactSurface[]> contactSurfacePairs = new List<ContactSurface[]>();
            //
            for (int i = 0; i < contactSurfaces.Length; i++)
            {
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (CheckSurfaceToSurfaceDistance(contactSurfaces[i], contactSurfaces[j], distance, angleRad))
                    {
                        contactSurfacePairs.Add(new ContactSurface[] { contactSurfaces[i], contactSurfaces[j] });
                    }
                }
            }
            // Group surface pairs
            if (_groupContactPairsBy == GroupContactPairsByEnum.None)
                return GroupContactPairsByNone(contactSurfacePairs, tryToResolve);
            else if (_groupContactPairsBy == GroupContactPairsByEnum.BySurfaceAngle)
                throw new NotSupportedException();
            else if (_groupContactPairsBy == GroupContactPairsByEnum.ByParts)
                return GroupContactPairsByParts(contactSurfacePairs, tryToResolve);
            else throw new NotSupportedException();
        }
        private void ComputeCellBoundingBoxes(double distance, bool ignoreHidden)
        {
            // Bounding boxes for each cell
            int[] cell;
            BoundingBox bb;
            _cellBoundingBoxes = new BoundingBox[_mesh.GetMaxPartId() + 1][];
            _cellEdgeBoundingBoxes = new BoundingBox[_mesh.GetMaxPartId() + 1][];
            //
            foreach (var partEntry in _mesh.Parts)
            {
                if (ignoreHidden && !partEntry.Value.Visible) continue;
                //
                _cellBoundingBoxes[partEntry.Value.PartId] = new BoundingBox[partEntry.Value.Visualization.Cells.Length];
                for (int i = 0; i < partEntry.Value.Visualization.Cells.Length; i++)
                {
                    cell = partEntry.Value.Visualization.Cells[i];
                    bb = new BoundingBox();
                    bb.IncludeFirstCoor(_nodes[cell[0]]);
                    bb.IncludeCoorFast(_nodes[cell[1]]);
                    bb.IncludeCoorFast(_nodes[cell[2]]);
                    if (cell.Length == 4 || cell.Length == 8) bb.IncludeCoorFast(_nodes[cell[3]]);
                    bb.Inflate(distance * 0.5);
                    //
                    _cellBoundingBoxes[partEntry.Value.PartId][i] = bb;
                }
                // Shell edge faces
                if (partEntry.Value.PartType == PartType.Shell)
                {
                    _cellEdgeBoundingBoxes[partEntry.Value.PartId]
                        = new BoundingBox[partEntry.Value.Visualization.EdgeCells.Length];
                    for (int i = 0; i < partEntry.Value.Visualization.EdgeCells.Length; i++)
                    {
                        cell = partEntry.Value.Visualization.EdgeCells[i];
                        bb = new BoundingBox();
                        bb.IncludeFirstCoor(_nodes[cell[0]]);
                        bb.IncludeCoorFast(_nodes[cell[1]]);
                        if (cell.Length == 3) bb.IncludeCoorFast(_nodes[cell[2]]);
                        bb.Inflate(distance * 0.5);
                        //
                        _cellEdgeBoundingBoxes[partEntry.Value.PartId][i] = bb;
                    }
                }
            }
        }
        private void GetAllEdgeCellBaseCellIds()
        {
            _edgeCellBaseCellIds = new int[_mesh.GetMaxPartId() + 1][];
            foreach (var partEntry in _mesh.Parts)
            {
                _edgeCellBaseCellIds[partEntry.Value.PartId] = partEntry.Value.Visualization.GetAllEdgeCellBaseCellIds();
            }
        }
        private ContactSurface[] GetAllContactSurfaces(double distance, GeometryFilterEnum filter)
        {
            HashSet<int> freeEdgeIds;
            ContactSurface contactSurface;
            List<ContactSurface> contactSurfacesList = new List<ContactSurface>();
            //
            foreach (var partEntry in _mesh.Parts)
            {
                if (filter.HasFlag(GeometryFilterEnum.IgnoreHidden) && !partEntry.Value.Visible) continue;
                //
                if (filter.HasFlag(GeometryFilterEnum.Solid) && partEntry.Value.PartType == PartType.Solid)
                {
                    // Solid faces
                    for (int i = 0; i < partEntry.Value.Visualization.FaceCount; i++)
                    {
                        contactSurfacesList.Add(new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                   GeometryType.SolidSurface, distance * 0.5));
                    }
                }
                else if (partEntry.Value.PartType == PartType.Shell)
                {
                    // Shell faces
                    if (filter.HasFlag(GeometryFilterEnum.Shell))
                    {
                        for (int i = 0; i < partEntry.Value.Visualization.FaceCount; i++)
                        {
                            // Front face
                            contactSurface = new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                GeometryType.ShellFrontSurface, distance * 0.5);
                            contactSurfacesList.Add(contactSurface);
                            // Back face
                            contactSurface = new ContactSurface(contactSurface);    // copy
                            contactSurface.ConvertToShellBackSurface();
                            contactSurfacesList.Add(contactSurface);
                        }
                    }
                    // Shell edge faces
                    if (filter.HasFlag(GeometryFilterEnum.ShellEdge))
                    {
                        freeEdgeIds = partEntry.Value.Visualization.GetFreeEdgeIds();
                        for (int i = 0; i < partEntry.Value.Visualization.EdgeCount; i++)
                        {
                            contactSurface = new ContactSurface(_mesh, _nodes, partEntry.Value, i,
                                                                GeometryType.ShellEdgeSurface, distance * 0.5);
                            // Fnd internal edges on shells
                            if (!freeEdgeIds.Contains(i)) contactSurface.Internal = true;
                            contactSurfacesList.Add(contactSurface);
                        }
                    }
                }
            }
            //
            ContactSurface[] contactSurfaces = contactSurfacesList.ToArray();
            // Find internal surfaces on compound parts
            for (int i = 0; i < contactSurfaces.Length; i++)
            {
                for (int j = i + 1; j < contactSurfaces.Length; j++)
                {
                    if (contactSurfaces[i].GeometryType == contactSurfaces[j].GeometryType && // skip front and back shell
                        contactSurfaces[i].NodeIds.Count() == contactSurfaces[j].NodeIds.Count() &&
                        contactSurfaces[i].BoundingBox.Intersects(contactSurfaces[j].BoundingBox) &&
                        contactSurfaces[i].NodeIds.Union(contactSurfaces[j].NodeIds).Count() == contactSurfaces[i].NodeIds.Count)
                    {
                        contactSurfaces[i].Internal = true;
                        contactSurfaces[j].Internal = true;
                    }
                }
            }
            return contactSurfaces;
        }
        private bool CheckSurfaceToSurfaceDistance(ContactSurface cs1, ContactSurface cs2, double distance, double angleRad)
        {
            if (cs1.Internal || cs2.Internal) return false;
            if (!_includeSelfContact && cs1.Part == cs2.Part) return false;
            if (!cs1.BoundingBox.Intersects(cs2.BoundingBox)) return false;
            //
            BoundingBox bb1;
            BoundingBox bb2;
            BoundingBox[] bbs1;
            BoundingBox[] bbs2;
            BoundingBox intersection = cs1.BoundingBox.GetIntersection(cs2.BoundingBox);
            //
            int[] cell1;
            int[] cell2;
            int[] cell1Ids;
            int[] cell2Ids;
            double[] cellNorm = new double[3];
            double[] edgeCellNorm = new double[3];
            double[][] t1 = new double[3][];
            double[][] t2 = new double[3][];
            t1[2] = new double[3];
            t2[2] = new double[3];
            //
            bool edgeSurface1 = cs1.GeometryType == GeometryType.ShellEdgeSurface;
            bool edgeSurface2 = cs2.GeometryType == GeometryType.ShellEdgeSurface;
            bool allowPenetration = (edgeSurface1 && edgeSurface2) ||
                                    (cs1.GeometryType == GeometryType.SolidSurface &&
                                     cs1.GeometryType == GeometryType.SolidSurface);
            bool onlyInternal = (cs1.GeometryType == GeometryType.SolidSurface ||
                                 cs1.GeometryType == GeometryType.ShellFrontSurface ||
                                 cs1.GeometryType == GeometryType.ShellBackSurface) &&
                                (cs2.GeometryType == GeometryType.SolidSurface ||
                                 cs2.GeometryType == GeometryType.ShellFrontSurface ||
                                 cs2.GeometryType == GeometryType.ShellBackSurface);

            onlyInternal = false;

            // Use face cell ids or edge cell ids
            if (edgeSurface1)
            {
                bbs1 = _cellEdgeBoundingBoxes[cs1.Part.PartId];
                cell1Ids = cs1.Part.Visualization.EdgeCellIdsByEdge[cs1.Id];
            }
            else
            {
                bbs1 = _cellBoundingBoxes[cs1.Part.PartId];
                cell1Ids = cs1.Part.Visualization.CellIdsByFace[cs1.Id];
            }
            //
            foreach (int cell1Id in cell1Ids)
            {
                bb1 = bbs1[cell1Id];
                //
                if (bb1.Intersects(intersection))
                //if (bb1.MaxX < intersection.MinX) continue;
                //else if (bb1.MinX > intersection.MaxX) continue;
                //else if (bb1.MaxY < intersection.MinY) continue;
                //else if (bb1.MinY > intersection.MaxY) continue;
                //else if (bb1.MaxZ < intersection.MinZ) continue;
                //else if (bb1.MinZ > intersection.MaxZ) continue;
                //else
                {
                    // Use face cell ids or edge cell ids
                    if (edgeSurface2)
                    {
                        bbs2 = _cellEdgeBoundingBoxes[cs2.Part.PartId];
                        cell2Ids = cs2.Part.Visualization.EdgeCellIdsByEdge[cs2.Id];
                    }
                    else
                    {
                        bbs2 = _cellBoundingBoxes[cs2.Part.PartId];
                        cell2Ids = cs2.Part.Visualization.CellIdsByFace[cs2.Id];
                    }
                    //
                    foreach (int cell2Id in cell2Ids)
                    {
                        bb2 = bbs2[cell2Id];
                        //
                        if (bb1.Intersects(bb2))
                        //if (bb1.MaxX < bb2.MinX) continue;
                        //else if (bb1.MinX > bb2.MaxX) continue;
                        //else if (bb1.MaxY < bb2.MinY) continue;
                        //else if (bb1.MinY > bb2.MaxY) continue;
                        //else if (bb1.MaxZ < bb2.MinZ) continue;
                        //else if (bb1.MinZ > bb2.MaxZ) continue;
                        //else
                        {
                            cell1 = cs1.GetCell(cell1Id);   // reverse cell ids for shell back faces or get shell edge cell
                            cell2 = cs2.GetCell(cell2Id);   // reverse cell ids for shell back faces or get shell edge cell
                            // Cell 1 triangle 1
                            t1[0] = _nodes[cell1[0]];
                            t1[1] = _nodes[cell1[1]];
                            if (!edgeSurface1) t1[2] = _nodes[cell1[2]];
                            // Cell 2 triangle 1
                            t2[0] = _nodes[cell2[0]];
                            t2[1] = _nodes[cell2[1]];
                            if (!edgeSurface2) t2[2] = _nodes[cell2[2]];
                            // 1. triangle is edge segment - create a triangle out of it
                            if (edgeSurface1)
                            {
                                cs1.GetEdgeCellNormal(cell1Id, _edgeCellBaseCellIds[cs1.Part.PartId][cell1Id],
                                                      ref cellNorm, ref edgeCellNorm);
                                Geometry.VpVxS(ref t1[2], t1[1], cellNorm, 0.001 * distance);
                            }
                            // 2. triangle is edge segment - create a triangle out of it
                            if (edgeSurface2)
                            {
                                cs2.GetEdgeCellNormal(cell2Id, _edgeCellBaseCellIds[cs2.Part.PartId][cell2Id],
                                                      ref cellNorm, ref edgeCellNorm);
                                Geometry.VpVxS(ref t2[2], t2[1], cellNorm, 0.001 * distance);
                            }
                            if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad, allowPenetration, onlyInternal))
                                return true;
                            // Cell 2 is a rectangle
                            if (cell2.Length == 4 || cell2.Length == 8)
                            {
                                // Cell 2 triangle 2
                                t2[0] = _nodes[cell2[0]];
                                t2[1] = _nodes[cell2[2]];
                                t2[2] = _nodes[cell2[3]];
                                //
                                if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad, allowPenetration, onlyInternal))
                                    return true;
                            }
                            // Cell 1 is a rectangle
                            if (cell1.Length == 4 || cell1.Length == 8)
                            {
                                // Cell 1 triangle 2
                                t1[0] = _nodes[cell1[0]];
                                t1[1] = _nodes[cell1[2]];
                                t1[2] = _nodes[cell1[3]];
                                //
                                if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad, allowPenetration, onlyInternal))
                                    return true;
                                // Cell 2 triangle 1 again
                                if (cell2.Length == 4 || cell2.Length == 8)
                                {
                                    t2[0] = _nodes[cell2[0]];
                                    t2[1] = _nodes[cell2[1]];
                                    t2[2] = _nodes[cell2[2]];
                                    //
                                    if (CheckTriangleToTriangleDistance(t1, t2, distance, angleRad, allowPenetration, onlyInternal))
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool CheckTriangleToTriangleDistance(double[][] t1, double[][] t2, double distance, double angleRad,
                                                     bool penetrationPossible, bool onlyInternal)
        {
            double dist;
            double ang;
            //
            double[] a = new double[3];
            double[] b = new double[3];
            double[] n1 = new double[3];
            double[] n2 = new double[3];
            // Triangle 1
            Geometry.VmV(ref a, t1[1], t1[0]);
            Geometry.VmV(ref b, t1[2], t1[0]);
            Geometry.VcrossV(ref n1, a, b);
            Geometry.VxS(ref n1, n1, 1 / Math.Sqrt(Geometry.VdotV(n1, n1)));
            // Triangle 2
            Geometry.VmV(ref a, t2[1], t2[0]);
            Geometry.VmV(ref b, t2[2], t2[0]);
            Geometry.VcrossV(ref n2, a, b);
            Geometry.VxS(ref n2, n2, 1 / Math.Sqrt(Geometry.VdotV(n2, n2)));
            //
            // 180° - the normals point in the opposite directions
            ang = Math.PI - Math.Acos(Geometry.VdotV(n1, n2));
            if (ang < angleRad)
            {
                // Closest points on triangles t1 and t2 are p adn q, respectively
                double[] p = new double[3];
                double[] q = new double[3];
                double[] pq = new double[3];
                //
                dist = Geometry.TriDist(ref p, ref q, t1, t2, onlyInternal);
                //
                if (dist < distance)
                {
                    // Check the orientraion of the triangle normals
                    if (dist > 0)
                    {
                        double ang1;
                        double ang2;
                        // pq is a vector from p to q
                        Geometry.VmV(ref pq, q, p);
                        Geometry.Vnorm(ref pq, pq);
                        ang1 = Geometry.VdotV(pq, n1);
                        if (!penetrationPossible && ang1 < 0) return false;     // negative value means n1 poits away from q
                        ang2 = Geometry.VdotV(pq, n2);
                        if (!penetrationPossible && ang2 > 0) return false;     // positive value means n2 poits away from p
                        // Check that the closest triangle points are one above the other
                        // The angle between the closest direction vector and normals must be small < 5° = 0.995
                        if (Math.Abs(ang1) < 0.995 && Math.Abs(ang2) < 0.995) return false;
                    }

                    // Shrink the triangles and try again
                    double[][] t1s = Geometry.ShrinkTriangle(t1, 0.5);
                    double[][] t2s = Geometry.ShrinkTriangle(t2, 0.5);
                    dist = Geometry.TriDist(ref p, ref q, t1s, t2s, false);
                    if (dist > distance)
                        return false;

                    return true;
                }
            }
            return false;
        }
        //
        private List<MasterSlaveItem> GroupContactPairsByNone(List<ContactSurface[]> contactSurfacePairs, bool tryToResolve)
        {
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            //
            foreach (var csp in contactSurfacePairs)
            {
                masterSlaveItems.Add(new MasterSlaveItem(csp[0].Part.Name, csp[1].Part.Name,
                                                         new HashSet<int>() { csp[0].GeometryId },
                                                         new HashSet<int>() { csp[1].GeometryId }));
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(masterSlaveItems, _mesh);
            masterSlaveItems = contactGraph.GetMasterSlaveItems(tryToResolve);
            //
            return masterSlaveItems;
        }
        private List<MasterSlaveItem> GroupContactPairsByParts(List<ContactSurface[]> contactSurfacePairs, bool tryToResolve)
        {
            int i;
            int j;
            int iType;
            int jType;
            int[] key;
            MasterSlaveItem masterSlaveItem;
            CompareIntArray comparer = new CompareIntArray();
            Dictionary<int[], MasterSlaveItem> partKeyMasterSlaveItems = new Dictionary<int[], MasterSlaveItem>(comparer);
            //
            Dictionary<int, int> partIds = GetPartIdsMergedByCompounds();
            // Mege by part Id
            foreach (var csp in contactSurfacePairs)
            {
                if (partIds[csp[0].Part.PartId] < partIds[csp[1].Part.PartId])
                {
                    i = 0;
                    j = 1;
                }
                else
                {
                    i = 1;
                    j = 0;
                }
                //
                if (csp[i].GeometryType == GeometryType.ShellEdgeSurface) iType = 1;
                else iType = 0;
                if (csp[j].GeometryType == GeometryType.ShellEdgeSurface) jType = 1;
                else jType = 0;
                //
                key = new int[] { partIds[csp[i].Part.PartId], iType, partIds[csp[j].Part.PartId], jType };
                if (partKeyMasterSlaveItems.TryGetValue(key, out masterSlaveItem))
                {
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GeometryId);
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GeometryId);
                }
                else
                {
                    masterSlaveItem = new MasterSlaveItem(csp[i].Part.Name, csp[j].Part.Name,
                                                          new HashSet<int>(), new HashSet<int>());
                    masterSlaveItem.MasterGeometryIds.Add(csp[i].GeometryId);
                    masterSlaveItem.SlaveGeometryIds.Add(csp[j].GeometryId);
                    partKeyMasterSlaveItems.Add(key, masterSlaveItem);
                }
            }
            //
            ContactGraph contactGraph = new ContactGraph();
            contactGraph.AddMasterSlaveItems(partKeyMasterSlaveItems.Values, _mesh);
            List<MasterSlaveItem> masterSlaveItems = contactGraph.GetMasterSlaveItems(tryToResolve);
            //
            return masterSlaveItems;
          }
        private Dictionary<int,int> GetPartIdsMergedByCompounds()
        {
            int maxPartId = 0;
            Dictionary<int, int> partIds = new Dictionary<int, int>();
            foreach (var partEntry in _mesh.Parts)
            {
                partIds.Add(partEntry.Value.PartId, partEntry.Value.PartId);
                if (partEntry.Value.PartId > maxPartId) maxPartId = partEntry.Value.PartId;
            }
            //
            if (_geometry != null && _geometry.Parts != null)
            {
                BasePart part;
                foreach (var geometryPartEntry in _geometry.Parts)
                {
                    if (geometryPartEntry.Value is CompoundGeometryPart cgp)
                    {
                        foreach (string subPartName in cgp.SubPartNames)
                        {
                            if (_mesh.Parts.TryGetValue(subPartName, out part)) partIds[part.PartId] = maxPartId;
                        }
                        maxPartId++;
                    }
                }
            }
            //
            return partIds;
        }
    }
}
