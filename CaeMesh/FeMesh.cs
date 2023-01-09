using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

namespace CaeMesh
{
    [Serializable]
    public class FeMesh : ISerializable
    {
        // Variables                                                                                                                
        [NonSerialized] private Dictionary<int, FeNode> _nodes;
        [NonSerialized] private Dictionary<int, FeElement> _elements;
        [NonSerialized] private Octree.PointOctree<int> _octree;
        [NonSerialized] private Dictionary<string, double[]> _partOffsets;

        private MeshRepresentation _meshRepresentation;                         //ISerializable
        private OrderedDictionary<string, FeMeshRefinement> _meshRefinements;   //ISerializable
        private OrderedDictionary<string, BasePart> _parts;                     //ISerializable
        private OrderedDictionary<string, FeNodeSet> _nodeSets;                 //ISerializable
        private OrderedDictionary<string, FeElementSet> _elementSets;           //ISerializable
        private OrderedDictionary<string, FeSurface> _surfaces;                 //ISerializable
        private OrderedDictionary<string, FeReferencePoint> _referencePoints;   //ISerializable
        private int _maxNodeId;                                                 //ISerializable
        private int _maxElementId;                                              //ISerializable
        private BoundingBox _boundingBox;                                       //ISerializable


        // Properties                                                                                                               
        public Dictionary<int, FeNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }
        public Dictionary<int, FeElement> Elements
        {
            get { return _elements; }
            set { _elements = value; }
        }
        public OrderedDictionary<string, FeMeshRefinement> MeshRefinements
        {
            get { return _meshRefinements; }
        }
        public OrderedDictionary<string, BasePart> Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }
        public OrderedDictionary<string, FeNodeSet> NodeSets
        {
            get { return _nodeSets; }
        }
        public OrderedDictionary<string, FeElementSet> ElementSets
        {
            get { return _elementSets; }
        }
        public OrderedDictionary<string, FeSurface> Surfaces
        {
            get { return _surfaces; }
        }
        public OrderedDictionary<string, FeReferencePoint> ReferencePoints
        {
            get { return _referencePoints; }
        }
        public int MaxNodeId
        {
            get { return _maxNodeId; }
        }
        public int MaxElementId
        {
            get { return _maxElementId; }
        }
        public BoundingBox BoundingBox { get { return _boundingBox.DeepCopy(); } }        


        // Constructors                                                                                                             
        public FeMesh(MeshRepresentation representation)
            : this(new Dictionary<int, FeNode>(), new Dictionary<int, FeElement>(), representation)
        {
        }
        public FeMesh(Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements, MeshRepresentation representation,
                      ImportOptions importOptions = ImportOptions.DetectEdges)
            : this(nodes, elements, representation, null, null, false, importOptions)
        {
        }
        public FeMesh(Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements, MeshRepresentation representation,
                      List<InpElementSet> inpElementTypeSets)
            : this(nodes, elements, representation, inpElementTypeSets, null, false, ImportOptions.DetectEdges)
        {
        }
        public FeMesh(Dictionary<int, FeNode> nodes, Dictionary<int, FeElement> elements, MeshRepresentation representation,
                      List<InpElementSet> inpElementTypeSets, string partNamePrefix, bool convertToSecondOrder,
                      ImportOptions importOptions)
        {
            if (convertToSecondOrder) LinearToParabolic(ref nodes, ref elements);
            //
            int id;
            int[] ids;
            _nodes = new Dictionary<int, FeNode>(nodes.Count);
            _elements = new Dictionary<int, FeElement>(elements.Count);
            // Sort nodes is here becouse of the sort elements
            ids = nodes.Keys.ToArray();
            Array.Sort(ids);
            for (int i = 0; i < ids.Length; i++)
            {
                id = ids[i];
                _nodes.Add(id, nodes[id]);
            }
            // Sort elements - the extract part starts at the first element; if this is not the element with
            // the lowest id then the elements may get renombered if the part with the lowest element id is
            // not hte first part
            ids = elements.Keys.ToArray();
            Array.Sort(ids);
            for (int i = 0; i < ids.Length; i++)
            {
                id = ids[i];
                _elements.Add(id, elements[id]);
            }
            //
            _meshRepresentation = representation;
            //
            _nodeSets = new OrderedDictionary<string, FeNodeSet>("Node sets");
            _elementSets = new OrderedDictionary<string, FeElementSet>("Element sets");
            //
            _surfaces = new OrderedDictionary<string, FeSurface>("Surfaces");
            _referencePoints = new OrderedDictionary<string, FeReferencePoint>("Reference points");
            //
            _parts = new OrderedDictionary<string, BasePart>("Base parts");
            ExtractPartsFast(inpElementTypeSets, partNamePrefix, importOptions);
            //
            _meshRefinements = new OrderedDictionary<string, FeMeshRefinement>("Mesh refinements");
            //
            UpdateMaxNodeAndElementIds();
        }
        public FeMesh(FeMesh mesh, string[] partsToKeep)
        {
            _meshRepresentation = mesh._meshRepresentation;
            _meshRefinements = new OrderedDictionary<string, FeMeshRefinement>("Mesh refinements");
            //
            _parts = new OrderedDictionary<string, BasePart>("Base parts");
            foreach (var partName in partsToKeep)
            {
                _parts.Add(partName, mesh.Parts[partName].DeepCopy());
            }
            //
            HashSet<int> nodeIds = new HashSet<int>();
            HashSet<int> elementIds = new HashSet<int>();
            foreach (var entry in _parts)
            {
                nodeIds.UnionWith(entry.Value.NodeLabels);
                elementIds.UnionWith(entry.Value.Labels);
            }
            //
            _nodes = new Dictionary<int, FeNode>();
            foreach (var nodeId in nodeIds)
            {
                _nodes.Add(nodeId, mesh.Nodes[nodeId].DeepCopy());
            }
            //
            _elements = new Dictionary<int, FeElement>();
            foreach (var elementId in elementIds)
            {
                _elements.Add(elementId, mesh.Elements[elementId].DeepCopy());
            }
            //
            _nodeSets = new OrderedDictionary<string, FeNodeSet>("Node sets");
            _elementSets = new OrderedDictionary<string, FeElementSet>("Element sets");
            _surfaces = new OrderedDictionary<string, FeSurface>("Surfaces");
            _referencePoints = new OrderedDictionary<string, FeReferencePoint>("Referenece points");
            //
            _maxNodeId = mesh._maxNodeId;
            _maxElementId = mesh._maxElementId;
            //
            ComputeBoundingBox();
        }
        public FeMesh(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_meshRepresentation":
                        _meshRepresentation = (MeshRepresentation)entry.Value; break;
                    case "_meshRefinements":
                        _meshRefinements = (OrderedDictionary<string, FeMeshRefinement>)entry.Value; break;
                    case "_parts":
                        if (entry.Value is Dictionary<string, BasePart> bpd)
                        {
                            // Compatibility for version v0.5.1
                            bpd.OnDeserialization(null);
                            _parts = new OrderedDictionary<string, BasePart>("", bpd);
                        }
                        else if (entry.Value is OrderedDictionary<string, BasePart> bpod) _parts = bpod;
                        else if (entry.Value == null) _parts = null;
                        else throw new NotSupportedException();
                        break;
                    case "_nodeSets":
                        if (entry.Value is Dictionary<string, FeNodeSet> nsd)
                        {
                            // Compatibility for version v0.5.1
                            nsd.OnDeserialization(null);
                            _nodeSets = new OrderedDictionary<string, FeNodeSet>("", nsd);
                        }
                        else if (entry.Value is OrderedDictionary<string, FeNodeSet> nsod) _nodeSets = nsod;
                        else if (entry.Value == null) _nodeSets = null;
                        else throw new NotSupportedException();
                        break;
                    case "_elementSets":
                        if (entry.Value is Dictionary<string, FeElementSet> esd)
                        {
                            // Compatibility for version v0.5.1
                            esd.OnDeserialization(null);
                            _elementSets = new OrderedDictionary<string, FeElementSet>("", esd);
                        }
                        else if (entry.Value is OrderedDictionary<string, FeElementSet> esod) _elementSets = esod;
                        else if (entry.Value == null) _elementSets = null;
                        else throw new NotSupportedException();
                        break;
                    case "_surfaces":
                        if (entry.Value is Dictionary<string, FeSurface> sd)
                        {
                            // Compatibility for version v0.5.1
                            sd.OnDeserialization(null);
                            _surfaces = new OrderedDictionary<string, FeSurface>("", sd);
                        }
                        else if (entry.Value is OrderedDictionary<string, FeSurface> sod) _surfaces = sod;
                        else if (entry.Value == null) _surfaces = null;
                        else throw new NotSupportedException();
                        break;
                    case "_referencePoints":
                        if (entry.Value is Dictionary<string, FeReferencePoint> rpd)
                        {
                            // Compatibility for version v0.5.1
                            rpd.OnDeserialization(null);
                            _referencePoints = new OrderedDictionary<string, FeReferencePoint>("", rpd);
                        }
                        else if (entry.Value is OrderedDictionary<string, FeReferencePoint> rpod) _referencePoints = rpod;
                        else if (entry.Value == null) _referencePoints = null;
                        else throw new NotSupportedException();
                        break;
                    case "_maxNodeId":
                        _maxNodeId = (int)entry.Value; break;
                    case "_maxElementId":
                        _maxElementId = (int)entry.Value; break;
                    case "_boundingBox":
                        _boundingBox = (BoundingBox)entry.Value; break;
                    // Compatibility for version v0.9.0
                    case "_manifoldGeometry":
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            // Compatibility for version v0.5.2
            if (_meshRefinements == null) _meshRefinements = new OrderedDictionary<string, FeMeshRefinement>("Mesh refinements");
        }


        // Static methods                                                                                                           
        public static void PrepareForSavig(FeMesh mesh)
        {
            if (mesh != null && mesh._parts != null)
            {
                foreach (var entry in mesh._parts)
                {
                    entry.Value.VisualizationCopy = entry.Value.Visualization;
                    entry.Value.Visualization = null;
                }
            }
        }
        public static void ResetAfterSavig(FeMesh mesh)
        {
            if (mesh != null && mesh._parts != null)
            {
                foreach (var entry in mesh._parts)
                {
                    if (entry.Value.VisualizationCopy != null)
                    {
                        entry.Value.Visualization = entry.Value.VisualizationCopy;
                        entry.Value.VisualizationCopy = null;
                    }
                }
            }
        }
        public static void WriteToBinaryFile(FeMesh mesh, System.IO.BinaryWriter bw)
        {
            if (mesh == null)
            {
                bw.Write(0);   // 0 nodes
                bw.Write(0);   // 0 elements
                bw.Write(0);   // 0 parts
            }
            else
            {
                // Nodes
                Dictionary<int, FeNode> nodes = mesh.Nodes;
                if (nodes == null) bw.Write(0);
                else
                {
                    bw.Write(nodes.Count);
                    foreach (var entry in nodes)
                    {
                        bw.Write(entry.Key);
                        bw.Write(entry.Value.X);
                        bw.Write(entry.Value.Y);
                        bw.Write(entry.Value.Z);
                    }
                }
                // Elements
                Dictionary<int, FeElement> elements = mesh.Elements;
                if (elements == null) bw.Write(0);
                else
                {
                    bw.Write(elements.Count);
                    foreach (var entry in elements)
                    {
                        bw.Write(entry.Value.PartId);
                        bw.Write(entry.Key);
                        bw.Write(entry.Value.GetVtkCellType());

                        bw.Write(entry.Value.NodeIds.Length);
                        for (int i = 0; i < entry.Value.NodeIds.Length; i++)
                        {
                            bw.Write(entry.Value.NodeIds[i]);
                        }
                    }
                }
                // Parts
                OrderedDictionary<string, BasePart> parts = mesh._parts;
                if (parts == null) bw.Write(-1);
                else
                {
                    bw.Write(parts.Count);
                    foreach (var entry in parts)
                    {
                        VisualizationData.WriteToBinaryStream(entry.Value.VisualizationCopy, bw);
                    }
                }
            }
        }
        public static void ReadFromBinaryFile(FeMesh mesh, System.IO.BinaryReader br, int version)
        {
            // Nodes
            if (mesh.Nodes == null) mesh.Nodes = new Dictionary<int, FeNode>();
            else mesh.Nodes.Clear();
            //
            int numOfNodes = br.ReadInt32();
            int id;
            double x, y, z;
            for (int i = 0; i < numOfNodes; i++)
            {
                id = br.ReadInt32();
                x = br.ReadDouble();
                y = br.ReadDouble();
                z = br.ReadDouble();
                mesh.Nodes.Add(id, new FeNode(id, x, y, z));
            }
            // Elements
            if (mesh.Elements == null) mesh.Elements = new Dictionary<int, FeElement>();
            else mesh.Elements.Clear();
            //
            int partId;
            int[] nodeIds;
            vtkCellType cellType;
            int numOfElements = br.ReadInt32();
            for (int i = 0; i < numOfElements; i++)
            {
                partId = br.ReadInt32();
                id = br.ReadInt32();
                cellType = (vtkCellType)br.ReadInt32();

                numOfNodes = br.ReadInt32();
                nodeIds = new int[numOfNodes];
                for (int j = 0; j < numOfNodes; j++)
                {
                    nodeIds[j] = br.ReadInt32();
                }
                //
                switch (cellType)
                {
                    case vtkCellType.VTK_LINE:
                        mesh.Elements.Add(id, new LinearBeamElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_EDGE:
                        mesh.Elements.Add(id, new ParabolicBeamElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_TRIANGLE:
                        mesh.Elements.Add(id, new LinearTriangleElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUAD:
                        mesh.Elements.Add(id, new LinearQuadrilateralElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_TRIANGLE:
                        mesh.Elements.Add(id, new ParabolicTriangleElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_QUAD:
                        mesh.Elements.Add(id, new ParabolicQuadrilateralElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_TETRA:
                        mesh.Elements.Add(id, new LinearTetraElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_WEDGE:
                        mesh.Elements.Add(id, new LinearWedgeElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_HEXAHEDRON:
                        mesh.Elements.Add(id, new LinearHexaElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_TETRA:
                        mesh.Elements.Add(id, new ParabolicTetraElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_WEDGE:
                        mesh.Elements.Add(id, new ParabolicWedgeElement(id, nodeIds) { PartId = partId });
                        break;
                    case vtkCellType.VTK_QUADRATIC_HEXAHEDRON:
                        mesh.Elements.Add(id, new ParabolicHexaElement(id, nodeIds) { PartId = partId });
                        break;
                    default:
                        break;
                }
            }
            // Parts
            if (version >= 1_003_001)
            {
                //if (mesh.Parts == null) mesh.Parts = new OrderedDictionary<string, BasePart>();
                //else mesh.Parts.Clear();
                //
                int numOfParts = br.ReadInt32();
                VisualizationData visualization;
                foreach (var entry in mesh._parts)
                {
                    VisualizationData.ReadFromBinaryStream(out visualization, br);
                    entry.Value.Visualization = visualization;
                }
            }
        }


        // Methods                                                                                                                  
        public string[] CheckValidity(List<Tuple<NamedClass, string>> items)
        {
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>
            List<string> invalidItems = new List<string>();
            bool valid;
            // Node set
            FeNodeSet nodeSet;
            foreach (var entry in _nodeSets)
            {
                nodeSet = entry.Value;
                if (!nodeSet.Valid)
                {
                    items.Add(new Tuple<NamedClass, string>(nodeSet, null));
                    invalidItems.Add("Node set: " + nodeSet.Name);
                }
            }
            // Element set
            FeElementSet elementSet;
            foreach (var entry in _elementSets)
            {
                elementSet = entry.Value;
                // Element set from parts
                if (elementSet.CreatedFromParts)
                    elementSet.Valid = GetPartNamesByIds(elementSet.Labels).Length == elementSet.Labels.Length;
                //
                if (!elementSet.Valid)
                {
                    items.Add(new Tuple<NamedClass, string>(elementSet, null));
                    invalidItems.Add("Element set: " + elementSet.Name);
                }
            }
            // Surfaces
            FeSurface surface = null;
            foreach (var entry in _surfaces)
            {
                surface = entry.Value;
                //
                valid = surface.Valid;              // this is set to invalid after deleting a part
                if (!valid) surface.Valid = true;   // set this to true to detect a change in validity
                // Type node and created from selection - surface creates a node set with name: surface.NodeSetName
                valid &= !(surface.Type == FeSurfaceType.Node && surface.CreatedFrom == FeSurfaceCreatedFrom.Selection
                          && !_nodeSets.ContainsValidKey(surface.NodeSetName));
                // From node set
                valid &= !(surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet && !_nodeSets.ContainsValidKey(surface.CreatedFromNodeSetName));
                // Has element faces
                if (surface.ElementFaces != null)
                {
                    foreach (var faceEntry in surface.ElementFaces) valid &= _elementSets.ContainsValidKey(faceEntry.Value);
                }
                valid &= !(surface.Type == FeSurfaceType.Element && surface.Area <= 0);
                SetItemValidity(surface, valid, items);
                if (!valid && surface.Active) invalidItems.Add("Surface: " + surface.Name);
            }
            // Reference points
            FeReferencePoint referencePoint;
            bool validFromCoordinates;
            bool validFromBB;
            bool validFromCG;
            foreach (var entry in _referencePoints)
            {
                referencePoint = entry.Value;
                validFromCoordinates = referencePoint.CreatedFrom == FeReferencePointCreatedFrom.Selection ||
                                       referencePoint.CreatedFrom == FeReferencePointCreatedFrom.BetweenTwoPoints ||
                                       referencePoint.CreatedFrom == FeReferencePointCreatedFrom.CircleCenter;
                validFromBB = (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.BoundingBoxCenter &&
                               (_nodeSets.ContainsValidKey(referencePoint.RegionName) ||
                               _surfaces.ContainsValidKey(referencePoint.RegionName)));
                validFromCG = (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.CenterOfGravity &&
                               (_nodeSets.ContainsValidKey(referencePoint.RegionName) ||
                               _surfaces.ContainsValidKey(referencePoint.RegionName)));
                valid = validFromCoordinates || validFromBB || validFromCG;
                SetItemValidity(referencePoint, valid, items);
                if (!valid && referencePoint.Active) invalidItems.Add("Reference point: " + referencePoint.Name);
            }
            //
            return invalidItems.ToArray();
        }
        private void SetItemValidity(NamedClass item, bool validity, List<Tuple<NamedClass, string>> items)
        {
            // only changed items are added for the update
            if (item.Valid != validity)
            {
                item.Valid = validity;
                items.Add(new Tuple<NamedClass, string>(item, null));
            }
        }
        public string[] GetAllMeshEntityNames()
        {
            List<string> names = new List<string>();
            names.AddRange(_parts.Keys);
            names.AddRange(_nodeSets.Keys);
            names.AddRange(_elementSets.Keys);
            names.AddRange(_surfaces.Keys);
            names.AddRange(_referencePoints.Keys);
            return names.ToArray();
        }

        // Compare 
        public double IsEqual(FeMesh mesh)
        {
            double similarity = 1;
            //
            if (_elements.Count != mesh.Elements.Count) return 0;
            //
            if (_nodes.Count == mesh.Nodes.Count)
            {
                int numToCheck = 10000;
                int n = Math.Min(numToCheck, _nodes.Count);
                n = Math.Max(_nodes.Count / numToCheck, n);
                //
                int nodeId;
                int[] keys = _nodes.Keys.ToArray();
                //
                int count = 0;
                Random rand = new Random();
                FeNode node1;
                FeNode node2;
                for (int i = 0; i < n; i++)
                {
                    nodeId = (int)(rand.NextDouble() * (keys.Length - 1));
                    if (_nodes.TryGetValue(keys[nodeId], out node1) && mesh.Nodes.TryGetValue(keys[nodeId], out node2))
                    {
                        if (node1.IsEqual(node2))
                            count++;
                    }
                    else return 0;
                }
                //
                if (count != n) similarity *= (double)count / n;
            }
            else
            {
                if (_maxElementId == mesh.MaxElementId) return 2;
                else return 0;
            }
            return similarity;
        }
        public override int GetHashCode()
        {
            int hash = 0;
            if (_nodes != null)
            {
                foreach (var entry in _nodes)
                {
                    hash ^= entry.Value.X.GetHashCode() ^ entry.Value.Y.GetHashCode() << 2 ^ entry.Value.Z.GetHashCode() >> 2;
                }
            }
            return hash;
        }

        // Bounding box
        public void UpdateMaxNodeAndElementIds()
        {
            // determine max node id
            _maxNodeId = 0;
            foreach (var entry in _nodes)
            {
                if (entry.Value.Id > _maxNodeId) _maxNodeId = entry.Value.Id;
            }

            // determine max element id
            _maxElementId = 0;
            foreach (var entry in _elements)
            {
                if (entry.Value.Id > _maxElementId) _maxElementId = entry.Value.Id;
            }
        }
        private void ComputeBoundingBox()
        {
            if (_boundingBox == null) _boundingBox = new BoundingBox();
            _boundingBox.Reset();
            //
            if (_nodes != null && _nodes.Count > 0)
            {
                FeNode node;
                foreach (var entry in _parts)
                {
                    entry.Value.BoundingBox.Reset();
                    for (int i = 0; i < entry.Value.NodeLabels.Length; i++)
                    {
                        node = _nodes[entry.Value.NodeLabels[i]];
                        entry.Value.BoundingBox.IncludeNode(node);
                    }
                    _boundingBox.IncludeBox(entry.Value.BoundingBox);
                }
                // Compound parts
                foreach (var entry in _parts)
                {
                    if (entry.Value is CompoundGeometryPart cgp)
                    {
                        cgp.BoundingBox.Reset();
                        foreach (var partName in cgp.SubPartNames)
                        {
                            cgp.BoundingBox.IncludeBox(_parts[partName].BoundingBox);
                        }
                    }
                }
            }
        }
        public double GetBoundingBoxVolumeAsCubeSide()
        {
            return Math.Pow((_boundingBox.MaxX - _boundingBox.MinX) *
                            (_boundingBox.MaxY - _boundingBox.MinY) *
                            (_boundingBox.MaxZ - _boundingBox.MinZ), 1.0 / 3.0);


        }
        public double GetBoundingBoxAreaAsSquareSide()
        {
            return Math.Pow((_boundingBox.MaxX - _boundingBox.MinX) *
                            (_boundingBox.MaxY - _boundingBox.MinY),
                            1.0 / 2.0);
        }

        // Convert to parabolic
        public static void LinearToParabolic(ref Dictionary<int, FeNode> nodes, ref Dictionary<int, FeElement> elements,
                                             Dictionary<int[], FeNode> midNodes = null)
        {
            CompareIntArray comparer = new CompareIntArray();
            if (midNodes == null) midNodes = new Dictionary<int[], FeNode>(comparer);
            //
            Dictionary<int, FeElement> elementsOut = new Dictionary<int, FeElement>();
            //
            int maxNodeId = int.MinValue;
            foreach (var entry in nodes) if (entry.Key > maxNodeId) maxNodeId = entry.Key;
            //
            int[] nodeIds;
            FeNode[] elNodes;
            FeElement linElement;
            FeElement parElement;
            foreach (var entry in elements)
            {
                linElement = entry.Value;
                if (linElement is LinearBeamElement)
                {
                    elNodes = new FeNode[3];
                    elNodes[0] = nodes[linElement.NodeIds[0]];
                    elNodes[1] = nodes[linElement.NodeIds[1]];
                    elNodes[2] = GetOrCreateMidNode(elNodes[0], elNodes[1], ref midNodes, ref maxNodeId);
                    //
                    nodeIds = new int[] { elNodes[0].Id, elNodes[1].Id, elNodes[2].Id };
                    parElement = new ParabolicBeamElement(entry.Key, nodeIds);
                }
                else if (linElement is LinearTriangleElement)
                {
                    elNodes = new FeNode[6];
                    elNodes[0] = nodes[linElement.NodeIds[0]];
                    elNodes[1] = nodes[linElement.NodeIds[1]];
                    elNodes[2] = nodes[linElement.NodeIds[2]];
                    elNodes[3] = GetOrCreateMidNode(elNodes[0], elNodes[1], ref midNodes, ref maxNodeId);
                    elNodes[4] = GetOrCreateMidNode(elNodes[1], elNodes[2], ref midNodes, ref maxNodeId);
                    elNodes[5] = GetOrCreateMidNode(elNodes[2], elNodes[0], ref midNodes, ref maxNodeId);
                    //
                    nodeIds = new int[] { elNodes[0].Id, elNodes[1].Id, elNodes[2].Id,
                                          elNodes[3].Id, elNodes[4].Id, elNodes[5].Id };
                    parElement = new ParabolicTriangleElement(entry.Key, nodeIds);
                }
                else if (linElement is LinearQuadrilateralElement)
                {
                    elNodes = new FeNode[8];
                    elNodes[0] = nodes[linElement.NodeIds[0]];
                    elNodes[1] = nodes[linElement.NodeIds[1]];
                    elNodes[2] = nodes[linElement.NodeIds[2]];
                    elNodes[3] = nodes[linElement.NodeIds[3]];
                    elNodes[4] = GetOrCreateMidNode(elNodes[0], elNodes[1], ref midNodes, ref maxNodeId);
                    elNodes[5] = GetOrCreateMidNode(elNodes[1], elNodes[2], ref midNodes, ref maxNodeId);
                    elNodes[6] = GetOrCreateMidNode(elNodes[2], elNodes[3], ref midNodes, ref maxNodeId);
                    elNodes[7] = GetOrCreateMidNode(elNodes[3], elNodes[0], ref midNodes, ref maxNodeId);
                    //
                    nodeIds = new int[] { elNodes[0].Id, elNodes[1].Id, elNodes[2].Id, elNodes[3].Id,
                                          elNodes[4].Id, elNodes[5].Id, elNodes[6].Id, elNodes[7].Id };
                    parElement = new ParabolicQuadrilateralElement(entry.Key, nodeIds);
                }
                else if (linElement is LinearTetraElement linTetEl)
                {
                    elNodes = new FeNode[10];
                    elNodes[0] = nodes[linElement.NodeIds[0]];
                    elNodes[1] = nodes[linElement.NodeIds[1]];
                    elNodes[2] = nodes[linElement.NodeIds[2]];
                    elNodes[3] = nodes[linElement.NodeIds[3]];
                    elNodes[4] = GetOrCreateMidNode(elNodes[0], elNodes[1], ref midNodes, ref maxNodeId);
                    elNodes[5] = GetOrCreateMidNode(elNodes[1], elNodes[2], ref midNodes, ref maxNodeId);
                    elNodes[6] = GetOrCreateMidNode(elNodes[2], elNodes[0], ref midNodes, ref maxNodeId);
                    elNodes[7] = GetOrCreateMidNode(elNodes[0], elNodes[3], ref midNodes, ref maxNodeId);
                    elNodes[8] = GetOrCreateMidNode(elNodes[1], elNodes[3], ref midNodes, ref maxNodeId);
                    elNodes[9] = GetOrCreateMidNode(elNodes[2], elNodes[3], ref midNodes, ref maxNodeId);
                    //
                    nodeIds = new int[] { elNodes[0].Id, elNodes[1].Id, elNodes[2].Id, elNodes[3].Id,
                                          elNodes[4].Id, elNodes[5].Id, elNodes[6].Id,
                                          elNodes[7].Id, elNodes[8].Id, elNodes[9].Id };
                    parElement = new ParabolicTetraElement(entry.Key, nodeIds);
                }
                else throw new NotSupportedException();
                //
                parElement.PartId = linElement.PartId;
                elementsOut.Add(parElement.Id, parElement);
            }
            // Add nodes
            Dictionary<int, FeNode> nodesOut = new Dictionary<int, FeNode>(nodes);
            foreach (var entry in midNodes) nodesOut.Add(entry.Value.Id, entry.Value);
            //
            nodes = nodesOut;
            elements = elementsOut;
        }
        private static FeNode GetOrCreateMidNode(FeNode n1, FeNode n2, ref Dictionary<int[], FeNode> midNodes, ref int maxNodeId)
        {
            int[] key;
            if (n1.Id < n2.Id) key = new int[] { n1.Id, n2.Id };
            else key = new int[] { n2.Id, n1.Id };
            //
            FeNode newNode;
            if (!midNodes.TryGetValue(key, out newNode))
            {
                maxNodeId++;
                newNode = new FeNode(maxNodeId, GetMidNodeCoor(n1, n2));
                midNodes.Add(key, newNode);
            }
            return newNode;
        }
        public static double[] GetMidNodeCoor(FeNode n1, FeNode n2)
        {
            double[] coor = new double[3];
            coor[0] = 0.5 * (n1.X + n2.X);
            coor[1] = 0.5 * (n1.Y + n2.Y);
            coor[2] = 0.5 * (n1.Z + n2.Z);
            return coor;
        }

        #region Parts  #############################################################################################################
        private void ExtractPartsFast(List<InpElementSet> inpElementTypeSets, string namePrefix, ImportOptions importOptions)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            // Find connected elements for each node
            FeElement element;
            HashSet<int> partNodeIds = new HashSet<int>();
            List<int> partElementIds = new List<int>();
            HashSet<Type> partElementTypes = new HashSet<Type>();
            //
            HashSet<int> existingPartIds = new HashSet<int>();
            Dictionary<int, HashSet<int>> existingPartNodeIds = new Dictionary<int, HashSet<int>>();
            Dictionary<int, List<int>> existingPartElementIds = new Dictionary<int, List<int>>();
            Dictionary<int, HashSet<Type>> existingPartElementTypes = new Dictionary<int, HashSet<Type>>();
            Dictionary<int, List<FeElement>> nodeElements = new Dictionary<int, List<FeElement>>();
            //
            foreach (var entry in _elements)
            {
                element = entry.Value;
                //
                foreach (var nodeId in element.NodeIds)
                {
                    if (nodeElements.ContainsKey(nodeId)) nodeElements[nodeId].Add(element);
                    else nodeElements.Add(nodeId, new List<FeElement>() { element });
                }
                // Get existing part data
                if (element.PartId != -1)
                {
                    existingPartIds.Add(element.PartId);
                    //
                    if (existingPartElementIds.TryGetValue(element.PartId, out partElementIds)) partElementIds.Add(element.Id);
                    else existingPartElementIds.Add(element.PartId, new List<int>() { element.Id });
                    //
                    if (existingPartNodeIds.TryGetValue(element.PartId, out partNodeIds)) partNodeIds.UnionWith(element.NodeIds);
                    else existingPartNodeIds.Add(element.PartId, new HashSet<int>(element.NodeIds));
                    //
                    if (existingPartElementTypes.TryGetValue(element.PartId, out partElementTypes))
                        partElementTypes.Add(element.GetType());
                    else existingPartElementTypes.Add(element.PartId, new HashSet<Type>() { element.GetType() });
                }
            }
            // Create new 
            partNodeIds = new HashSet<int>();
            partElementIds = new List<int>();
            partElementTypes = new HashSet<Type>();
            //
            int partId = 0;
            HashSet<int> addedPartIds = new HashSet<int>();
            string name;
            BasePart part;
            List<int> sortedPartNodeIds;
            HashSet<string> inpElementTypeNames = null;
            HashSet<int> inpElementTypeSetLabels = null;

            // Extract parts
            foreach (var entry in _elements)
            {
                element = entry.Value;
                if (element.PartId == -1)
                {
                    // Find new free partId
                    partId = 0;
                    do { partId++; } while (addedPartIds.Contains(partId) || existingPartIds.Contains(partId));
                    element.PartId = partId;    // set Part Id to the seed element of the Flood Fill
                    //
                    partNodeIds.Clear();
                    partNodeIds.UnionWith(element.NodeIds);
                    //
                    partElementIds.Clear();
                    partElementIds.Add(entry.Key);
                    //
                    partElementTypes.Clear();
                    partElementTypes.Add(element.GetType());
                    // find inp element type set
                    if (inpElementTypeSets != null)
                    {
                        foreach (var elementTypeEntry in inpElementTypeSets)
                        {
                            if (elementTypeEntry.ElementLabels.Contains(element.Id))
                            {
                                if (inpElementTypeNames == null) inpElementTypeNames = new HashSet<string>();
                                inpElementTypeNames.UnionWith(elementTypeEntry.InpElementTypeNames);
                                //
                                inpElementTypeSetLabels = elementTypeEntry.ElementLabels;
                                break;
                            }
                        }
                    }
                    // Find connected elements of the same type
                    if (element is FeElement1D)
                    {
                        FloodFillFast<FeElement1D>(element, partId, nodeElements, ref partNodeIds, ref partElementIds,
                                                   ref partElementTypes, inpElementTypeSetLabels);
                    }
                    else if (element is FeElement2D)
                    {
                        FloodFillFast<FeElement2D>(element, partId, nodeElements, ref partNodeIds, ref partElementIds,
                                                   ref partElementTypes, inpElementTypeSetLabels);
                    }
                    else if (element is FeElement3D)
                    {
                        FloodFillFast<FeElement3D>(element, partId, nodeElements, ref partNodeIds, ref partElementIds,
                                                   ref partElementTypes, inpElementTypeSetLabels);
                    }
                    else throw new NotSupportedException();
                }
                else if (existingPartIds.Contains(element.PartId) && !addedPartIds.Contains(element.PartId))
                {
                    partId = element.PartId;
                    partNodeIds = existingPartNodeIds[element.PartId];
                    partElementIds = existingPartElementIds[element.PartId];
                    partElementTypes = existingPartElementTypes[element.PartId];
                }
                else if (addedPartIds.Contains(element.PartId)) continue;
                else throw new NotSupportedException();
                // Use name prefix
                if (namePrefix != null && namePrefix != "") name = namePrefix + "-";
                else name = "";
                // Get new name
                if (element is FeElement1D) name += "Wire_part";
                else if (element is FeElement2D) name += "Shell_part";
                else if (element is FeElement3D) name += "Solid_part";
                else throw new NotSupportedException();
                name = _parts.GetNextNumberedKey(name);
                // Sort node ids
                sortedPartNodeIds = new List<int>(partNodeIds);
                sortedPartNodeIds.Sort();
                // Sort element ids
                partElementIds.Sort();
                // Create part
                if (_meshRepresentation == MeshRepresentation.Geometry)
                    part = new GeometryPart(name, partId, sortedPartNodeIds.ToArray(), partElementIds.ToArray(),
                                            partElementTypes.ToArray());
                else if (_meshRepresentation == MeshRepresentation.Mesh)
                {
                    part = new MeshPart(name, partId, sortedPartNodeIds.ToArray(), partElementIds.ToArray(),
                                        partElementTypes.ToArray());
                    if (inpElementTypeNames != null)
                        (part as MeshPart).SetPropertiesFromInpElementTypeName(inpElementTypeNames.ToArray());
                }
                else if (_meshRepresentation == MeshRepresentation.Results)
                    part = new ResultPart(name, partId, sortedPartNodeIds.ToArray(), partElementIds.ToArray(),
                                          partElementTypes.ToArray());
                else throw new NotSupportedException();
                // Add part
                _parts.Add(name, part);
                addedPartIds.Add(part.PartId);
            }
            watch.Stop();
            // Bounding box of parts and mesh
            _boundingBox = new BoundingBox();
            ComputeBoundingBox();
            // Merge geometry parts   ???
            if (importOptions == ImportOptions.ImportOneCADSolidPart &&
                _meshRepresentation == MeshRepresentation.Geometry && _parts.Count > 1) MergeGeometryParts();
            //Extract visualization
            double edgeAngle;
            bool isCADPart;
            if (importOptions == ImportOptions.ImportCADShellParts || importOptions == ImportOptions.ImportOneCADSolidPart)
            {
                edgeAngle = -1;
                isCADPart = true;
            }
            else if (importOptions == ImportOptions.DetectEdges || importOptions == ImportOptions.ImportStlParts)
            {
                edgeAngle = Globals.EdgeAngle;
                isCADPart = false;
                //
                if (importOptions == ImportOptions.ImportStlParts && _elements.Count > 1E5) edgeAngle = -1;
            }
            else throw new NotSupportedException();
            //
            GeometryPart geometryPart;
            List<string> partsToRename = new List<string>();
            //
            foreach (var entry in _parts)
            {
                part = entry.Value;
                if (part.PartType == PartType.Solid)
                {
                    ExtractSolidPartVisualization(part, edgeAngle);
                }
                else if (part.PartType == PartType.Shell)
                {
                    ExtractShellPartVisualization(part, isCADPart, edgeAngle);
                    //
                    if (part is GeometryPart gp)
                    {
                        // Collect shell parts to be converted to solidAsShell
                        if (importOptions == ImportOptions.ImportOneCADSolidPart || 
                            (importOptions == ImportOptions.ImportStlParts && !gp.HasFreeEdges)) partsToRename.Add(gp.Name);
                    }
                }
                else if (part.PartType == PartType.Wire)
                {
                    ExtractWirePartVisualization(part, edgeAngle);
                }
            }
            // Rename shell geometry parts to solidAsShell parts
            foreach (var partName in partsToRename)
            {
                geometryPart = _parts[partName] as GeometryPart;
                _parts.Remove(partName);
                geometryPart.Name = geometryPart.Name.Replace("Shell", "Solid");
                geometryPart.SetPartType(PartType.SolidAsShell);
                _parts.Add(geometryPart.Name, geometryPart);
            }
            // Sort parts by part id - for compound parts the ids are important since names are renamed
            string[] partNames = new string[_parts.Count];
            int[] partIds = new int[_parts.Count];
            int count = 0;
            foreach (var entry in _parts)
            {
                partNames[count] = entry.Key;
                partIds[count] = entry.Value.PartId;
                count++;
            }
            Array.Sort(partIds, partNames);
            _parts.SortKeysAs(partNames);
            //
            //ResetPartsColor();
        }
        private void FloodFillFast<T>(FeElement element, int partId, Dictionary<int, List<FeElement>> nodeElements,
                                      ref HashSet<int> partNodeIds, ref List<int> partElementIds,
                                      ref HashSet<Type> partElementTypes, HashSet<int> elementTypeSet)
        {
            UniqueQueue<FeElement> neighbours = new UniqueQueue<FeElement>();
            neighbours.Enqueue(element);
            //
            FeElement el;
            while (neighbours.Count > 0)
            {
                el = neighbours.Dequeue();
                //
                foreach (var nodeId in el.NodeIds)
                {
                    foreach (var currEl in nodeElements[nodeId])
                    {
                        if (currEl.PartId == -1 && currEl is T && !(elementTypeSet != null && !elementTypeSet.Contains(currEl.Id)))
                        {
                            currEl.PartId = partId;
                            neighbours.Enqueue(currEl);
                            partNodeIds.UnionWith(currEl.NodeIds);
                            partElementIds.Add(currEl.Id);
                            partElementTypes.Add(currEl.GetType());
                        }
                    }
                }
            }
        }
        // Visualization
        public void ExtractSolidPartVisualization(BasePart part, double edgeAngle)
        {
            part.Visualization.ExtractVisualizationCellsFromElements3D(_elements, part.Labels);
            //
            ExtractEdgesFromShellByAngle(part, edgeAngle);  // extracts free and error elements
            //
            HashSet<int> vertexNodeIds = ExtractVerticesFromEdgesByAngle(part, edgeAngle);
            //
            SplitVisualizationEdgesAndFaces(part, vertexNodeIds);
        }
        public void ExtractShellPartVisualization(BasePart part, bool isCADPart, double edgeAngle)
        {
            part.Visualization.ExtractVisualizationCellsFromElements2D(_elements, part.Labels);
            //
            ExtractEdgesFromShellByAngle(part, edgeAngle);  // extracts free and error elements
            //
            HashSet<int> vertexNodeIds = ExtractVerticesFromEdgesByAngle(part, edgeAngle);            
            //
            SplitVisualizationEdgesAndFaces(part, vertexNodeIds);
            //
            if (part is GeometryPart gp)
            {
                if (isCADPart) CheckForFreeAndErrorElementsInCADPart(part);
                // Stl geometry part
                else
                {
                    // For shell geometry parts clear error elements
                    if (gp.FreeEdgeCellIds != null)
                    {
                        gp.ErrorEdgeCellIds = null;
                        gp.ErrorNodeIds = null;
                    }
                }
            }
        }
        private void ExtractWirePartVisualization(BasePart part, double edgeAngle)
        {
            int n = part.Labels.Length;
            int[] elementIds = part.Labels;
            //
            int[] visualizationCellsIds = part.Labels;
            int[][] visualizationCells = new int[n][];
            //
            int count = 0;
            foreach (var id in elementIds) visualizationCells[count++] = _elements[id].GetVtkNodeIds();
            //
            part.Visualization.CellIds = visualizationCellsIds;
            part.Visualization.Cells = visualizationCells;
            // Model edges
            part.Visualization.EdgeCells = visualizationCells;
            HashSet<int> vertexIds = ExtractVerticesFromEdgesByAngle(part, edgeAngle, true);
            int[] nodeIds = vertexIds.ToArray();
            int[][] modelPoints = new int[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++) modelPoints[i] = new int[] { nodeIds[i] };
            // Overwrite edges
            part.Visualization.EdgeCells = modelPoints;
        }
        private void ExtractEdgesFromShellByAngle(BasePart part, double angle)
        {
            int[][] cells = part.Visualization.Cells;
            // Get all edges
            Dictionary<int[], CellEdgeData> allEdges;
            allEdges = part.Visualization.GetCellEdgeData(GetVisualizationEdgeCells);
            // Get only edges where cells meet at an angle > input angle
            // Get free surface edges
            double phi;
            int[] cellsIds;
            CellEdgeData data;
            int visualizationCell1i;
            int visualizationCell2i;
            List<int[]> edgeCells = new List<int[]>();
            GeometryPart geometryPart;
            List<int> freeEdgeCellIds = new List<int>();
            HashSet<int> freeNodeIds = new HashSet<int>();
            List<int> errorEdgeCellIds = new List<int>();
            HashSet<int> errorNodeIds = new HashSet<int>();
            //
            angle *= Math.PI / 180;
            if (part is GeometryPart gp) geometryPart = gp;
            else geometryPart = null;
            //
            foreach (var entry in allEdges)
            {
                data = entry.Value;
                cellsIds = data.CellIds.ToArray();      // for faster loops
                // Free and error edges
                if (cellsIds.Length != 2)
                {
                    edgeCells.Add(data.NodeIds);
                    //
                    if (geometryPart != null)
                    {
                        if (cellsIds.Length == 1)
                        {
                            // Free edges
                            freeEdgeCellIds.Add(edgeCells.Count() - 1);
                            freeNodeIds.UnionWith(data.NodeIds);
                        }
                        else
                        {
                            // Error edges
                            errorEdgeCellIds.Add(edgeCells.Count() - 1);
                            errorNodeIds.UnionWith(data.NodeIds);
                        }
                    }
                    //
                    continue;
                }
                // Angle edges
                if (angle >= 0)
                {
                    for (int i = 0; i < cellsIds.Length - 1; i++)
                    {
                        for (int j = i + 1; j < cellsIds.Length; j++)
                        {
                            visualizationCell1i = cellsIds[i];
                            visualizationCell2i = cellsIds[j];
                            // phi [0 ... Math.PI]
                            phi = ComputeAngleInRadFromCellIndices(cells[visualizationCell1i], cells[visualizationCell2i]);
                            // Move phi to [0 ... Math.PI / 2]
                            if (phi > Math.PI / 2) phi = Math.PI - phi;
                            if (phi > angle) edgeCells.Add(data.NodeIds);
                        }
                    }
                }                
            }
            part.Visualization.EdgeCells = edgeCells.ToArray();
            if (geometryPart != null)
            {
                if (freeEdgeCellIds.Count > 0) geometryPart.FreeEdgeCellIds = freeEdgeCellIds.ToArray();
                else geometryPart.FreeEdgeCellIds = null;
                if (freeNodeIds.Count > 0) geometryPart.FreeNodeIds = freeNodeIds.ToArray();
                else geometryPart.ErrorNodeIds = null;
                //
                if (errorEdgeCellIds.Count > 0) geometryPart.ErrorEdgeCellIds = errorEdgeCellIds.ToArray();
                else geometryPart.ErrorEdgeCellIds = null;
                if (errorNodeIds.Count > 0) geometryPart.ErrorNodeIds = errorNodeIds.ToArray();
                else geometryPart.ErrorNodeIds = null;
            }
            // Get cell neighbours
            HashSet<int>[] cellNeighbours = new HashSet<int>[cells.Length];
            foreach (var entry in allEdges)
            {
                cellsIds = entry.Value.CellIds.ToArray();       // for faster loops
                // Free edges
                if (cellsIds.Length == 1)
                {
                    visualizationCell1i = cellsIds[0];
                    //
                    if (cellNeighbours[visualizationCell1i] == null) cellNeighbours[visualizationCell1i] = new HashSet<int>();
                    //
                    cellNeighbours[visualizationCell1i].Add(-1); // -1 for the free edge
                }
                else
                {
                    //
                    for (int i = 0; i < cellsIds.Length - 1; i++)
                    {
                        for (int j = i + 1; j < cellsIds.Length; j++)
                        {
                            visualizationCell1i = cellsIds[i];
                            visualizationCell2i = cellsIds[j];
                            //
                            if (cellNeighbours[visualizationCell1i] == null)
                                cellNeighbours[visualizationCell1i] = new HashSet<int>();
                            if (cellNeighbours[visualizationCell2i] == null)
                                cellNeighbours[visualizationCell2i] = new HashSet<int>();
                            //
                            cellNeighbours[visualizationCell1i].Add(visualizationCell2i);
                            cellNeighbours[visualizationCell2i].Add(visualizationCell1i);
                        }
                    }
                }
            }
            // Collect cell neighbours in an array
            int[][] cellNeighboursArray = new int[cellNeighbours.Length][];
            for (int i = 0; i < cellNeighbours.Length; i++)
            {
                if (cellNeighbours[i] != null) cellNeighboursArray[i] = cellNeighbours[i].ToArray();
            }
            part.Visualization.CellNeighboursOverCellEdge = cellNeighboursArray;
        }
        private HashSet<int> ExtractVerticesFromEdgesByAngle(BasePart part, double angle, bool includeEndPoints = false)
        {
            int[] cell1;
            int[] cell2;
            int commonNode;
            double phi;
            HashSet<int> vertexNodeIds = new HashSet<int>();
            //
            if (angle >= 0)
            {
                angle *= Math.PI / 180;
                //
                List<int> edgeIdCellIds;
                Dictionary<int, List<int>> nodeIdEdgeCellIds = new Dictionary<int, List<int>>();
                // Speed optimization for dealnig with lastiteration spring elements
                int[] sorted;
                HashSet<int[]> existingCells;
                CompareIntArray comparer = new CompareIntArray();
                Dictionary<int, HashSet<int[]>> nodeIdExistingCells = new Dictionary<int, HashSet<int[]>>();
                //
                for (int i = 0; i < part.Visualization.EdgeCells.Length; i++)
                {
                    cell1 = part.Visualization.EdgeCells[i];
                    sorted = cell1.ToArray();
                    Array.Sort(sorted);
                    //
                    for (int j = 0; j < 2; j++)     // only consider the outter 2 nodes
                    {
                        if (nodeIdEdgeCellIds.TryGetValue(cell1[j], out edgeIdCellIds) &&
                            nodeIdExistingCells.TryGetValue(cell1[j], out existingCells))
                        {
                            if (!existingCells.Contains(sorted))
                            {
                                edgeIdCellIds.Add(i);
                                existingCells.Add(sorted);
                            }
                        }
                        else
                        {
                            nodeIdEdgeCellIds.Add(cell1[j], new List<int> { i });
                            nodeIdExistingCells.Add(cell1[j], new HashSet<int[]>(comparer) { sorted });
                        }
                    }
                }
                //
                bool nodeAdded;
                foreach (var entry in nodeIdEdgeCellIds)
                {
                    nodeAdded = false;
                    edgeIdCellIds = entry.Value;
                    //
                    if (edgeIdCellIds.Count == 1)
                    {
                        if (includeEndPoints) vertexNodeIds.Add(entry.Key);
                    }
                    else
                    {
                        for (int i = 0; i < edgeIdCellIds.Count; i++)
                        {
                            cell1 = part.Visualization.EdgeCells[edgeIdCellIds[i]];
                            for (int j = i + 1; j < edgeIdCellIds.Count; j++)
                            {
                                cell2 = part.Visualization.EdgeCells[edgeIdCellIds[j]];
                                commonNode = entry.Key;
                                //
                                phi = ComputeAngleInRadFromEdgeCellIndices(cell1, cell2, commonNode);
                                //
                                phi = Math.PI - phi;
                                if (phi > angle)
                                {
                                    vertexNodeIds.Add(commonNode);
                                    nodeAdded = true;
                                    break;
                                }
                            }
                            if (nodeAdded) break;
                        }
                    }
                }
            }
            return vertexNodeIds;
        }
        private void CheckForFreeAndErrorElementsInCADPart(BasePart part)
        {
            if (part.PartType == PartType.Wire) return;
            else if (part is GeometryPart gp)
            {
                // Clear
                gp.FreeEdgeCellIds = null;
                gp.FreeNodeIds = null;
                //
                VisualizationData vis = gp.Visualization;
                int[] freeEdgeCellIds = vis.GetFreeEdgeCellIds(GetVisualizationEdgeCells);
                int[] freeVetexNodeIds = vis.GetFreeVertexNodeIds(freeEdgeCellIds);
                // Save
                if (freeEdgeCellIds.Length > 0) gp.FreeEdgeCellIds = freeEdgeCellIds;
                else gp.FreeEdgeCellIds = null;
                if (freeVetexNodeIds.Length > 0) gp.FreeNodeIds = freeVetexNodeIds;
                else gp.FreeNodeIds = null;
                // Error
                CheckForErrorElementsInCADPart(gp);
            }
        }
        private void CheckForErrorElementsInCADPart(GeometryPart gp)
        {
            // Clear
            gp.ErrorEdgeCellIds = null;
            gp.ErrorNodeIds = null;
            //
            int[] errorEdgeCellIds = null;
            int[] errorNodeIds = null;
            // Shell parts
            if (gp.PartType == PartType.Shell)
            {
                gp.Visualization.CheckForErrorElementsInShellCADPart(out errorEdgeCellIds, out errorNodeIds);
            }
            // Save
            gp.ErrorEdgeCellIds = errorEdgeCellIds;
            gp.ErrorNodeIds = errorNodeIds;
        }
        private void SplitVisualizationEdgesAndFaces(BasePart part, HashSet<int> vertexNodeIds = null)
        {
            SplitEdgesByVertices(part, vertexNodeIds);
            ComputeEdgeLengths(part);
            SplitVisualizationFacesByEdges(part);
            ComputeFaceAreas(part);
        }
        private void SplitEdgesByVertices(BasePart part, HashSet<int> vertexNodeIds = null)
        {
            part.Visualization.EdgeCellIdsByEdge = null;
            // Build part edges map
            CompareIntArray comparer = new CompareIntArray();
            // This are edge cells connected to the node
            Dictionary<int, List<int>> nodeEdgeCellIds = new Dictionary<int, List<int>>();  
            List<int> edgeCellIds;
            int nodeId;
            int[][] allEdgeCells = part.Visualization.EdgeCells;
            for (int i = 0; i < allEdgeCells.Length; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    nodeId = allEdgeCells[i][j];
                    if (nodeEdgeCellIds.TryGetValue(nodeId, out edgeCellIds)) edgeCellIds.Add(i);
                    else nodeEdgeCellIds.Add(nodeId, new List<int>() { i });
                }
            }
            //
            if (vertexNodeIds == null)
            {
                vertexNodeIds = new HashSet<int>();
                // Vertex connects one or more than two edge cells
                foreach (var entry in nodeEdgeCellIds)
                {
                    if (entry.Value.Count != 2) vertexNodeIds.Add(entry.Key);
                }
            }
            // Find edge neighbours
            HashSet<int>[] edgeNeighboursHash = new HashSet<int>[allEdgeCells.Length];  
            foreach (var entry in nodeEdgeCellIds)
            {
                foreach (var edge1Id in entry.Value)
                {
                    if (edgeNeighboursHash[edge1Id] == null) edgeNeighboursHash[edge1Id] = new HashSet<int>();
                    //
                    foreach (var edge2Id in entry.Value)
                    {
                        if (edge1Id == edge2Id) continue;
                        else edgeNeighboursHash[edge1Id].Add(edge2Id);
                    }
                }
            }
            // Speed up
            int[][] edgeNeighbours = new int[allEdgeCells.Length][];
            for (int i = 0; i < edgeNeighbours.Length; i++) edgeNeighbours[i] = edgeNeighboursHash[i].ToArray();
            // Spread
            int[] oneEdgeCells;
            List<int[]> edges = new List<int[]>();
            HashSet<int> visitedEdgeCellIds = new HashSet<int>();
            //
            for (int i = 0; i < allEdgeCells.Length; i++)
            {
                if (!visitedEdgeCellIds.Contains(i))
                {
                    oneEdgeCells = GetSplitEdgeByEdgeCellId(part, i, edgeNeighbours, vertexNodeIds);
                    edges.Add(oneEdgeCells);
                    visitedEdgeCellIds.UnionWith(oneEdgeCells);
                }
            }
            //
            part.Visualization.VertexNodeIds = vertexNodeIds.ToArray();
            part.Visualization.EdgeCellIdsByEdge = edges.ToArray();
        }
        private void ComputeEdgeLengths(BasePart part)
        {
            int edgeCellId;
            int[] edgeCell;
            Vec3D v1;
            Vec3D v2;
            VisualizationData visualization = part.Visualization;
            double[] edgeLengths = new double[visualization.EdgeCellIdsByEdge.Length];

            // for each edge
            for (int i = 0; i < visualization.EdgeCellIdsByEdge.Length; i++)
            {
                edgeLengths[i] = 0;
                // for each edge cell
                for (int j = 0; j < visualization.EdgeCellIdsByEdge[i].Length; j++)
                {
                    edgeCellId = visualization.EdgeCellIdsByEdge[i][j];
                    edgeCell = visualization.EdgeCells[edgeCellId];

                    if (edgeCell.Length == 2)
                    {
                        v1 = new Vec3D(_nodes[edgeCell[0]].Coor);
                        v2 = new Vec3D(_nodes[edgeCell[1]].Coor);
                        edgeLengths[i] += (v2 - v1).Len;
                    }
                    else if (edgeCell.Length == 3)
                    {
                        v1 = new Vec3D(_nodes[edgeCell[0]].Coor);
                        v2 = new Vec3D(_nodes[edgeCell[2]].Coor);
                        edgeLengths[i] += (v2 - v1).Len;

                        v1 = new Vec3D(_nodes[edgeCell[1]].Coor);
                        edgeLengths[i] += (v2 - v1).Len;
                    }
                    else throw new NotSupportedException();
                }
            }

            visualization.EdgeLengths = edgeLengths;
        }
        private void SplitVisualizationFacesByEdges(BasePart part)
        {
            part.Visualization.CellIdsByFace = null;
            //
            if (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell || part.PartType == PartType.Shell)
            {
                // Build part edges map
                CompareIntArray comparer = new CompareIntArray();
                Dictionary<int[], int> modelEdges = new Dictionary<int[], int>(comparer);
                int[] sortedNodes;
                for (int i = 0; i < part.Visualization.EdgeCellIdsByEdge.Length; i++)
                {
                    foreach (var edgeCellId in part.Visualization.EdgeCellIdsByEdge[i])
                    {
                        sortedNodes = part.Visualization.EdgeCells[edgeCellId].ToArray();
                        Array.Sort(sortedNodes);
                        if (!modelEdges.ContainsKey(sortedNodes)) modelEdges.Add(sortedNodes, i);
                        else
                        {
                            // This gets hit on element selection on parabolic meshes
                            int error = 1;
                        }
                    }
                }
                //
                int[] faceCellIds;
                int[] faceEdgeIds;
                List<int[]> visualizationFaces = new List<int[]>();
                List<int[]> faceEdges = new List<int[]>();
                HashSet<int> visitedVisualizationCellIds = new HashSet<int>();
                //
                for (int cellId = 0; cellId < part.Visualization.CellIds.Length; cellId++)
                {
                    if (!visitedVisualizationCellIds.Contains(cellId))
                    {
                        GetSplitVisualizationFaceByCellId(part, cellId, modelEdges, out faceCellIds, out faceEdgeIds);
                        visualizationFaces.Add(faceCellIds);
                        faceEdges.Add(faceEdgeIds);
                        visitedVisualizationCellIds.UnionWith(faceCellIds);
                    }
                }
                //
                part.Visualization.CellIdsByFace = visualizationFaces.ToArray();
                part.Visualization.FaceEdgeIds = faceEdges.ToArray();
            }
        }
        private void ComputeFaceAreas(BasePart part)
        {
            VisualizationData visualization = part.Visualization;
            visualization.FaceAreas = new double[visualization.CellIdsByFace.Length];
            // For each face
            for (int i = 0; i < visualization.CellIdsByFace.Length; i++)
            {
                visualization.FaceAreas[i] = ComputeFaceArea(visualization, i, _nodes);
            }
        }
        public double ComputeFaceArea(VisualizationData visualization, int faceId, Dictionary<int, FeNode> nodes)
        {
            int faceCellId;
            int[] cell;
            double faceArea = 0;
            // For each face cell
            for (int i = 0; i < visualization.CellIdsByFace[faceId].Length; i++)
            {
                faceCellId = visualization.CellIdsByFace[faceId][i];
                cell = visualization.Cells[faceCellId];
                //
                if (cell.Length == 3)
                    faceArea += GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
                else if (cell.Length == 4)
                    faceArea += GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                            nodes[cell[3]]);
                else if (cell.Length == 6)
                    faceArea += GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                           nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
                else if (cell.Length == 8)
                    faceArea += GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                            nodes[cell[3]], nodes[cell[4]], nodes[cell[5]],
                                                            nodes[cell[6]], nodes[cell[7]]);
                else throw new NotSupportedException();
            }
            //
            return faceArea;
        }
        public double ComputeAngleInRadFromCellIndices(int[] cell1, int[] cell2)
        {
            FeNode n1 = ComputeNormalFromFaceCellIndices(cell1);
            FeNode n2 = ComputeNormalFromFaceCellIndices(cell2);
            //
            double scalar = n1.X * n2.X + n1.Y * n2.Y + n1.Z * n2.Z;
            if (scalar > 1) return 0;
            else if (scalar < -1) return Math.PI;
            else return Math.Acos(scalar);
        }
        private double[] ComputeDirectionFromEdgeCellNodes(FeNode n1, FeNode n2)
        {
            double[] d = new double[3];
            d[0] = n2.X - n1.X;
            d[1] = n2.Y - n1.Y;
            d[2] = n2.Z - n1.Z;
            return d;
        }
        public FeNode ComputeNormalFromFaceCellIndices(int[] cell)
        {
            FeNode node1 = _nodes[cell[0]];
            FeNode node2 = _nodes[cell[1]];
            FeNode node3 = _nodes[cell[2]];
            //
            if (cell.Length == 3 || cell.Length == 6)
            {
                return ComputeNormalFromFaceCellNodes(node1, node2, node3);
            }
            else if (cell.Length == 4 || cell.Length == 8)
            {
                FeNode node4 = _nodes[cell[3]];
                return ComputeNormalFromFaceCellNodes(node1, node2, node3, node4);
            }
            else throw new NotSupportedException();
        }
        private FeNode ComputeNormalFromFaceCellNodes(FeNode n1, FeNode n2, FeNode n3)
        {
            FeNode v = new FeNode(0, n2.X - n1.X, n2.Y - n1.Y, n2.Z - n1.Z);
            FeNode w = new FeNode(0, n3.X - n1.X, n3.Y - n1.Y, n3.Z - n1.Z);
            //
            FeNode n = new FeNode();
            n.X = v.Y * w.Z - v.Z * w.Y;
            n.Y = v.Z * w.X - v.X * w.Z;
            n.Z = v.X * w.Y - v.Y * w.X;
            //
            double d = Math.Sqrt(Math.Pow(n.X, 2) + Math.Pow(n.Y, 2) + Math.Pow(n.Z, 2));
            if (d != 0)
            {
                n.X /= d;
                n.Y /= d;
                n.Z /= d;
            }
            return n;
        }
        private FeNode ComputeNormalFromFaceCellNodes(FeNode n1, FeNode n2, FeNode n3, FeNode n4)
        {
            FeNode v1 = ComputeNormalFromFaceCellNodes(n1, n2, n3);
            FeNode v2 = ComputeNormalFromFaceCellNodes(n2, n3, n4);
            FeNode v3 = ComputeNormalFromFaceCellNodes(n3, n4, n1);
            FeNode v4 = ComputeNormalFromFaceCellNodes(n4, n1, n2);
            return new FeNode(-1, (v1.X + v2.X + v3.X + v4.X) / 4,
                                  (v1.Y + v2.Y + v3.Y + v4.Y) / 4,
                                  (v1.Z + v2.Z + v3.Z + v4.Z) / 4);
        }
        public double ComputeAngleInRadFromEdgeCellIndices(int[] cell1, int[] cell2, int nodeIdAtAngle)
        {
            Vec3D t1 = ComputeDirectionFromEdgeCellIndices(cell1, nodeIdAtAngle);
            Vec3D t2 = ComputeDirectionFromEdgeCellIndices(cell2, nodeIdAtAngle);
            //
            double scalar = Vec3D.DotProduct(t1, t2);
            if (scalar > 1) return 0;
            else if (scalar < -1) return Math.PI;
            else return Math.Acos(scalar);
        }
        private Vec3D ComputeDirectionFromEdgeCellIndices(int[] cell, int nodeIdAtAngle)
        {
            Vec3D n1 = new Vec3D(_nodes[cell[0]].Coor);
            Vec3D n2 = new Vec3D(_nodes[cell[1]].Coor);
            Vec3D t;
            if (cell[0] == nodeIdAtAngle) t = n2 - n1;
            else if (cell[1] == nodeIdAtAngle) t = n1 - n2;
            else throw new NotSupportedException();
            //
            t.Normalize();
            //
            return t;
        }
        public int[] GetSplitEdgeByEdgeCellId(BasePart part, int edgeCellId, int[][] edgeNeighbours, HashSet<int> verticeNodeIds)
        {
            int[][] edgeCells = part.Visualization.EdgeCells;
            if (edgeCells == null) return null;
            //
            int nodeId;
            int[] edgeCell1;
            int[] edgeCell2;
            HashSet<int> newEdgeCellIds = new HashSet<int>();
            HashSet<int> edgeCellIds = new HashSet<int>();
            HashSet<int> notVisitedEdgeCellIds = new HashSet<int>();
            edgeCellIds.Add(edgeCellId);
            notVisitedEdgeCellIds.Add(edgeCellId);
            //
            do
            {
                // Find new edge candidates
                newEdgeCellIds.Clear();
                // Search each not visited edge cell
                foreach (var notVisitedCellId in notVisitedEdgeCellIds)
                {
                    // Continue if edge has at least one neighbour
                    if (edgeNeighbours[notVisitedCellId] != null)
                    {
                        edgeCell1 = edgeCells[notVisitedCellId];
                        // Continue for each edge neighbour
                        foreach (var edgeNeighbourId in edgeNeighbours[notVisitedCellId])
                        {
                            // Is it a new edge cell
                            if (!edgeCellIds.Contains(edgeNeighbourId) && !newEdgeCellIds.Contains(edgeNeighbourId))
                            {
                                edgeCell2 = edgeCells[edgeNeighbourId];
                                //
                                if (edgeCell1[0] == edgeCell2[0] || edgeCell1[0] == edgeCell2[1]) nodeId = edgeCell1[0];
                                else if (edgeCell1[1] == edgeCell2[0] || edgeCell1[1] == edgeCell2[1]) nodeId = edgeCell1[1];
                                else throw new NotSupportedException();
                                //
                                if (!verticeNodeIds.Contains(nodeId))   // stop the edge at the node that connects more than two edge cells
                                {
                                    newEdgeCellIds.Add(edgeNeighbourId);
                                }
                            }
                        }
                    }
                    else
                    {
                        int error = 1;
                    }
                }
                // Add new edge candidates to edge and to cells to visit
                notVisitedEdgeCellIds.Clear();
                edgeCellIds.UnionWith(newEdgeCellIds);
                notVisitedEdgeCellIds.UnionWith(newEdgeCellIds);
            }
            while (notVisitedEdgeCellIds.Count > 0);
            //
            return SortEdgeCellIds(part, edgeCellIds);
        }
        //
        private int[] SortEdgeCellIds(BasePart part, HashSet<int> edgeCellIds)
        {
            int[][] edgeCells = part.Visualization.EdgeCells;
            //
            int nodeId;
            List<int> ids;
            Dictionary<int, List<int>> nodeEdgeCellIds = new Dictionary<int, List<int>>();
            foreach (int edgeCellId in edgeCellIds)
            {
                for (int i = 0; i < 2; i++)
                {
                    nodeId = edgeCells[edgeCellId][i];
                    if (nodeEdgeCellIds.TryGetValue(nodeId, out ids)) ids.Add(edgeCellId);
                    else nodeEdgeCellIds.Add(nodeId, new List<int>() { edgeCellId });
                }
            }
            //
            int firstNodeId = -1;
            int lastNodeId = -1;
            foreach (var entry in nodeEdgeCellIds)
            {
                // Find first or last node id
                if (entry.Value.Count == 1)
                {
                    if (firstNodeId == -1) firstNodeId = entry.Key;
                    else if (lastNodeId == -1)
                    {
                        // use frst id beforre overwriting it in the next line
                        lastNodeId = Math.Max(firstNodeId, entry.Key);
                        firstNodeId = Math.Min(firstNodeId, entry.Key);
                    }
                    else
                        return edgeCellIds.ToArray();
                        //throw new Exception("More than two nodes should not be external nodes!");
                }
            }
            // Check for loop
            if (firstNodeId == -1)
            {
                firstNodeId = nodeEdgeCellIds.Keys.First();
                lastNodeId = firstNodeId;
            }
            //
            int[] edgeCell;
            HashSet<int> visitedEdgeCellIds = new HashSet<int>();
            List<int> sortedEdgeCellIds = new List<int>();
            int count = 0;
            do
            {
                ids = nodeEdgeCellIds[firstNodeId];
                foreach (var cellId in ids)
                {
                    if (visitedEdgeCellIds.Add(cellId)) // if add possible 
                    {
                        edgeCell = edgeCells[cellId];
                        if (edgeCell[0] != firstNodeId) // swap
                        {
                            edgeCell[1] = edgeCell[0];
                            edgeCell[0] = firstNodeId;
                        }
                        sortedEdgeCellIds.Add(cellId);
                        firstNodeId = edgeCell[1];      // for the next edge cell the last node is the first
                        count = 0;
                        // If the first one is added break:
                        // by loops this prevents going in the opposite direction
                        break;
                    }
                }
            }
            while (firstNodeId != lastNodeId && count++ < 1000);
            //
            return sortedEdgeCellIds.ToArray();
        }
        public void GetSplitVisualizationFaceByCellId(BasePart part, int cellId, Dictionary<int[], int> modelEdges,
                                                      out int[] surfaceCellIds, out int[] surfaceEdgeIds)
        {
            surfaceCellIds = null;
            surfaceEdgeIds = null;
            //
            int[][] visualizationCells = part.Visualization.Cells;
            int[][] allCellNeighbours = part.Visualization.CellNeighboursOverCellEdge;
            if (visualizationCells == null) return;
            // Spread
            int edgeId;
            int[] edgeNodes;
            HashSet<int> surfaceEdgeIdsHash = new HashSet<int>();
            HashSet<int> cellNodes = new HashSet<int>();
            HashSet<int> newSurfaceCellIds = new HashSet<int>();        // cells added per one layer
            HashSet<int> surfaceCellIdsHash = new HashSet<int>();       // all added cells
            HashSet<int> notVisitedCellIds = new HashSet<int>();        // new border cells to be examined
            HashSet<int> freeEdgeCellIds = new HashSet<int>();
            //
            surfaceCellIdsHash.Add(cellId);
            notVisitedCellIds.Add(cellId);
            //
            do
            {
                // Find new surface candidates
                newSurfaceCellIds.Clear();
                // Try to add cells from the border layer of allready added cells
                foreach (var notVisitedCellId in notVisitedCellIds)
                {
                    // Check if the border cell exists
                    if (allCellNeighbours[notVisitedCellId] != null)
                    {
                        // Check all neighbours of the selected border cell
                        foreach (var neighbourId in allCellNeighbours[notVisitedCellId])
                        {
                            // -1 stands for a cell with no neighbour
                            if (neighbourId == -1) freeEdgeCellIds.Add(notVisitedCellId);
                            else
                            {
                                cellNodes.Clear();
                                cellNodes.UnionWith(visualizationCells[notVisitedCellId]);
                                cellNodes.IntersectWith(visualizationCells[neighbourId]);
                                edgeNodes = cellNodes.ToArray();
                                Array.Sort(edgeNodes);
                                // Check if the neighbour was allready added in the previous or in this border layer
                                if (!surfaceCellIdsHash.Contains(neighbourId) && !newSurfaceCellIds.Contains(neighbourId))
                                {
                                    if (modelEdges.TryGetValue(edgeNodes, out edgeId)) surfaceEdgeIdsHash.Add(edgeId);
                                    else newSurfaceCellIds.Add(neighbourId);
                                }
                                else
                                {
                                    if (modelEdges.TryGetValue(edgeNodes, out edgeId)) surfaceEdgeIdsHash.Add(edgeId);
                                }
                            }
                        }
                    }
                    else
                    {
                        int error = 1;
                    }
                }
                // Add new surface candidates to surface and to cells to visit
                notVisitedCellIds.Clear();
                surfaceCellIdsHash.UnionWith(newSurfaceCellIds);
                notVisitedCellIds.UnionWith(newSurfaceCellIds);
            }
            while (notVisitedCellIds.Count > 0);
            // Cells
            surfaceCellIds = surfaceCellIdsHash.ToArray();
            // Add Free edges
            HashSet<int> freeEdgeCellsNodeIds = new HashSet<int>();
            if (freeEdgeCellIds.Count > 0)
            {
                foreach (var freeEdgeCellId in freeEdgeCellIds)
                {
                    freeEdgeCellsNodeIds.UnionWith(visualizationCells[freeEdgeCellId]);
                }
                foreach (var entry in modelEdges)
                {
                    if (!surfaceEdgeIdsHash.Contains(entry.Value) && freeEdgeCellsNodeIds.Intersect(entry.Key).Count() == entry.Key.Length)
                        surfaceEdgeIdsHash.Add(entry.Value);
                }
                //
                //for (int i = 0; i < surfaceCellIds.Length; i++)
                //    freeEdgeCellsNodeIds.UnionWith(visualizationCells[surfaceCellIds[i]]);
                //foreach (var entry in modelEdges)
                //{
                //    if (freeEdgeCellsNodeIds.Intersect(entry.Key).Count() == entry.Key.Length) surfaceEdgeIdsHash.Add(entry.Value);
                //}
            }
            surfaceEdgeIds = surfaceEdgeIdsHash.ToArray();
        }
        public void GetSplitVisualizationFaceByCellId_old(BasePart part, int cellId, Dictionary<int[], int> modelEdges,
                                                          out int[] surfaceCellIds, out int[] surfaceEdgeIds)
        {
            surfaceCellIds = null;
            surfaceEdgeIds = null;
            //
            int[][] visualizationCells = part.Visualization.Cells;
            int[][] allCellNeighbours = part.Visualization.CellNeighboursOverCellEdge;
            if (visualizationCells == null) return;
            // Spread
            int edgeId;
            int[] edgeNodes;
            HashSet<int> surfaceEdgeIdsHash = new HashSet<int>();
            HashSet<int> cellNodes = new HashSet<int>();
            HashSet<int> newSurfaceCellIds = new HashSet<int>();        // cells added per one layer
            HashSet<int> surfaceCellIdsHash = new HashSet<int>();       // all added cells
            HashSet<int> notVisitedCellIds = new HashSet<int>();        // new border cells to be examined
            HashSet<int> freeEdgeCellIds = new HashSet<int>();
            //
            surfaceCellIdsHash.Add(cellId);
            notVisitedCellIds.Add(cellId);
            //
            do
            {
                // Find new surface candidates
                newSurfaceCellIds.Clear();
                // Try to add cells from the border layer of allready added cells
                foreach (var notVisitedCellId in notVisitedCellIds)
                {
                    // Check if the border cell exists
                    if (allCellNeighbours[notVisitedCellId] != null)
                    {
                        // Check all neighbours of the selected border cell
                        foreach (var neighbourId in allCellNeighbours[notVisitedCellId])
                        {
                            // Check if the neighbour was allready added in the previous or in this border layer
                            if (!surfaceCellIdsHash.Contains(neighbourId) && !newSurfaceCellIds.Contains(neighbourId))
                            {
                                // -1 stands for a cell with no neighbour
                                if (neighbourId == -1) freeEdgeCellIds.Add(notVisitedCellId);
                                // If neighbour exists
                                else
                                {
                                    cellNodes.Clear();
                                    cellNodes.UnionWith(visualizationCells[notVisitedCellId]);
                                    cellNodes.IntersectWith(visualizationCells[neighbourId]);
                                    edgeNodes = cellNodes.ToArray();
                                    Array.Sort(edgeNodes);
                                    //
                                    if (modelEdges.TryGetValue(edgeNodes, out edgeId)) surfaceEdgeIdsHash.Add(edgeId);
                                    else newSurfaceCellIds.Add(neighbourId);
                                }
                            }
                        }
                    }
                    else
                    {
                        int error = 1;
                    }
                }
                // Add new surface candidates to surface and to cells to visit
                notVisitedCellIds.Clear();
                surfaceCellIdsHash.UnionWith(newSurfaceCellIds);
                notVisitedCellIds.UnionWith(newSurfaceCellIds);
            }
            while (notVisitedCellIds.Count > 0);
            // Cells
            surfaceCellIds = surfaceCellIdsHash.ToArray();
            // Add Free edges
            HashSet<int> freeEdgeCellsNodeIds = new HashSet<int>();
            if (freeEdgeCellIds.Count > 0)
            {
                foreach (var freeEdgeCellId in freeEdgeCellIds)
                {
                    freeEdgeCellsNodeIds.UnionWith(visualizationCells[freeEdgeCellId]);
                }
                foreach (var entry in modelEdges)
                {
                    if (!surfaceEdgeIdsHash.Contains(entry.Value) && freeEdgeCellsNodeIds.Intersect(entry.Key).Count() == entry.Key.Length)
                        surfaceEdgeIdsHash.Add(entry.Value);
                }
                //
                //for (int i = 0; i < surfaceCellIds.Length; i++)
                //    freeEdgeCellsNodeIds.UnionWith(visualizationCells[surfaceCellIds[i]]);
                //foreach (var entry in modelEdges)
                //{
                //    if (freeEdgeCellsNodeIds.Intersect(entry.Key).Count() == entry.Key.Length) surfaceEdgeIdsHash.Add(entry.Value);
                //}
            }
            surfaceEdgeIds = surfaceEdgeIdsHash.ToArray();
        }
        //
        public void RenumberVisualizationSurfaces(Dictionary<string, Dictionary<int, HashSet<int>>> partSurfaceIdNodeIds,
                                                  SortedDictionary<int, GeomFaceType> faceTypes = null, // Plane, Cylinder, Cone...
                                                  Dictionary<string, Dictionary<int, int>> partIdNewSurfIdOldSurfId = null)
        {
            Dictionary<int, int> newSurfIdOldSurfId;
            // For each part
            foreach (var entry in _parts)
            {
                newSurfIdOldSurfId = RenumberVisualizationSurfaces(entry.Value, partSurfaceIdNodeIds[entry.Key], faceTypes);
                if (partIdNewSurfIdOldSurfId != null && newSurfIdOldSurfId != null)
                    partIdNewSurfIdOldSurfId.Add(entry.Key, newSurfIdOldSurfId);
                if (faceTypes != null) CheckForFreeAndErrorElementsInCADPart(entry.Value);
            }
        }
        public void RenumberVisualizationSurfaces(Dictionary<int, HashSet<int>> surfaceIdNodeIds,
                                                  SortedDictionary<int, GeomFaceType> faceTypes = null, // Plane, Cylinder, Cone...
                                                  Dictionary<string, Dictionary<int, int>> partIdNewSurfIdOldSurfId = null)
        {
            Dictionary<int, int> newSurfIdOldSurfId;
            // For each part
            foreach (var entry in _parts)
            {
                newSurfIdOldSurfId = RenumberVisualizationSurfaces(entry.Value, surfaceIdNodeIds, faceTypes);
                if (partIdNewSurfIdOldSurfId != null && newSurfIdOldSurfId != null)
                    partIdNewSurfIdOldSurfId.Add(entry.Key, newSurfIdOldSurfId);
                if (faceTypes != null) CheckForFreeAndErrorElementsInCADPart(entry.Value);
            }
        }
        public Dictionary<int, int> RenumberVisualizationSurfaces(BasePart part, Dictionary<int, HashSet<int>> surfaceIdNodeIds,
                                                                  SortedDictionary<int, GeomFaceType> faceTypes = null)
        {
            VisualizationData vis;
            HashSet<int> surfaceNodeIds;        // node ids of the part surface
            Dictionary<int, int> oldIdNewId = new Dictionary<int, int>();
            int[] newSurfaceIds;
            int[] oldSurfaceIds;
            GeomFaceType[] partFaceTypes;
            int surfaceCount;
            int oneSurfCount;
            //
            vis = part.Visualization;
            // Skip wire parts
            if (vis.CellIdsByFace == null) return null;
            //
            surfaceCount = 0;
            newSurfaceIds = new int[vis.CellIdsByFace.Length];
            oldSurfaceIds = new int[vis.CellIdsByFace.Length];
            partFaceTypes = new GeomFaceType[vis.CellIdsByFace.Length];
            Dictionary<int, HashSet<int>> nodeIdsBySurfaces = vis.GetNodeIdsBySurfaces();
            // For each part surface
            int foundKey;
            int foundFirstKey;
            int foundSecondKey;
            for (int i = 0; i < vis.CellIdsByFace.Length; i++)
            {
                surfaceNodeIds = nodeIdsBySurfaces[i];
                // Find the surface with the same node ids
                foundFirstKey = -1;
                foundSecondKey = -1;
                oneSurfCount = 0;
                //
                foreach (var surfaceNodeIdsEntry in surfaceIdNodeIds)
                {
                    if (surfaceNodeIdsEntry.Value.Count > 0)
                    {
                        if (surfaceNodeIdsEntry.Value.Count == surfaceNodeIds.Count &&
                            surfaceNodeIdsEntry.Value.Except(surfaceNodeIds).Count() == 0)
                        {
                            foundFirstKey = surfaceNodeIdsEntry.Key;
                            break;
                        }
                        else if (surfaceNodeIdsEntry.Value.Intersect(surfaceNodeIds).Count() == surfaceNodeIdsEntry.Value.Count())
                        {
                            foundSecondKey = surfaceNodeIdsEntry.Key;
                            // Do not break, search all faces for possible firstKey
                        }
                    }
                }
                if (foundFirstKey != -1 || foundSecondKey != -1)
                {
                    foundKey = foundFirstKey != -1 ? foundFirstKey : foundSecondKey;
                    //
                    newSurfaceIds[surfaceCount] = foundKey;
                    oldSurfaceIds[surfaceCount] = surfaceCount;
                    //
                    oldIdNewId.Add(surfaceCount, foundKey);
                    //
                    surfaceCount++;
                    oneSurfCount++;
                }
                //
                if (oneSurfCount == 0)
                {
                    oneSurfCount = 0;
                    //throw new Exception("RenumberVisualizationSurfaces: the surface to renumber was not found.");
                }

            }
            Array.Sort(newSurfaceIds, oldSurfaceIds);
            // Renumber surfaces
            part.RenumberVisualizationSurfaces(oldSurfaceIds);
            // Invert map
            int[] invertMap = new int[oldSurfaceIds.Length];
            for (int i = 0; i < oldSurfaceIds.Length; i++) invertMap[oldSurfaceIds[i]] = i;
            Dictionary<int, int> tmp = new Dictionary<int, int>();
            foreach (var entry in oldIdNewId) tmp.Add(invertMap[entry.Key], entry.Value);
            oldIdNewId = tmp;
            //
            int faceId;
            if (faceTypes != null)
            {
                for (int i = 0; i < newSurfaceIds.Length; i++)
                {
                    faceId = newSurfaceIds[i];
                    if (faceTypes.ContainsKey(faceId)) partFaceTypes[i] = faceTypes[faceId];
                    else partFaceTypes[i] = GeomFaceType.Unknown;
                }
                part.Visualization.FaceTypes = partFaceTypes;
            }
            //
            return oldIdNewId;
        }
        public void RenumberVisualizationEdges(Dictionary<string, Dictionary<int, HashSet<int>>> partEdgeIdNodeIds,
                                               Dictionary<string, Dictionary<int, int>> partIdNewEdgeIdOldEdgeId = null)
        {
            Dictionary<int, int> newEdgeIdOldEdgeId;
            // For each part
            foreach (var entry in _parts)
            {
                newEdgeIdOldEdgeId = RenumberVisualizationEdges(entry.Value, partEdgeIdNodeIds[entry.Key]);
                if (partIdNewEdgeIdOldEdgeId != null && newEdgeIdOldEdgeId != null)
                    partIdNewEdgeIdOldEdgeId.Add(entry.Key, newEdgeIdOldEdgeId);
            }
        }
        public void RenumberVisualizationEdges(Dictionary<int, HashSet<int>> edgeIdNodeIds,
                                               Dictionary<string, Dictionary<int, int>> partIdNewEdgeIdOldEdgeId = null)
        {
            Dictionary<int, int> newEdgeIdOldEdgeId;
            // For each part
            foreach (var entry in _parts)
            {
                newEdgeIdOldEdgeId = RenumberVisualizationEdges(entry.Value, edgeIdNodeIds);
                if (partIdNewEdgeIdOldEdgeId != null && newEdgeIdOldEdgeId != null)
                    partIdNewEdgeIdOldEdgeId.Add(entry.Key, newEdgeIdOldEdgeId);
            }
        }
        public Dictionary<int, int> RenumberVisualizationEdges(BasePart part, Dictionary<int, HashSet<int>> edgeIdNodeIds)
        {
            VisualizationData vis;
            HashSet<int> edgeNodeIds;       // node ids of the part surface
            Dictionary<int, int> oldIdNewId = new Dictionary<int, int>();
            int[] newEdgeIds;
            int[] oldEdgeIds;
            int edgeCount;
            bool edgeFound;
            //
            vis = part.Visualization;
            // Skip wire parts
            if (vis.EdgeCellIdsByEdge == null) return null;
            // Split edges with midpoints
            if (SplitVizualizationEdges(vis, edgeIdNodeIds)) ComputeEdgeLengths(part);
            //
            edgeCount = 0;
            newEdgeIds = new int[vis.EdgeCellIdsByEdge.Length];
            oldEdgeIds = new int[vis.EdgeCellIdsByEdge.Length];
            Dictionary<int, HashSet<int>> nodeIdsByEdges = vis.GetNodeIdsByEdges();
            // For each part edge
            for (int i = 0; i < vis.EdgeCellIdsByEdge.Length; i++)
            {
                edgeNodeIds = nodeIdsByEdges[i];
                // Find the edge with the same node ids
                edgeFound = false;
                foreach (var edgeEntry in edgeIdNodeIds)
                {
                    if (edgeEntry.Value.Count > 0)  // for remeshing it can be 0
                    {
                        if (edgeEntry.Value.Count == edgeNodeIds.Count && edgeEntry.Value.Except(edgeNodeIds).ToArray().Length == 0)
                        {
                            newEdgeIds[edgeCount] = edgeEntry.Key;
                            oldEdgeIds[edgeCount] = edgeCount;
                            oldIdNewId.Add(edgeCount, edgeEntry.Key);
                            //
                            edgeCount++;
                            edgeFound = true;
                            break;
                        }
                    }
                }
                // Next line is for when the mesh is converted to parabolic outside netgen
                if (!edgeFound)
                {
                    foreach (var edgeEntry in edgeIdNodeIds)
                    {
                        if (edgeEntry.Value.Count > 0)  // for remeshing it can be 0
                        {
                            if (edgeEntry.Value.Intersect(edgeNodeIds).Count() == edgeEntry.Value.Count())
                            {
                                newEdgeIds[edgeCount] = edgeEntry.Key;
                                oldEdgeIds[edgeCount] = edgeCount;
                                oldIdNewId.Add(edgeCount, edgeEntry.Key);
                                //
                                edgeCount++;
                                edgeFound = true;
                                break;
                            }
                        }
                    }
                }
                //
                if (!edgeFound)
                {
                    int error = 1;
                }
            }
            //
            Array.Sort(newEdgeIds, oldEdgeIds);
            // Renumber edges
            part.RenumberVisualizationEdges(oldEdgeIds);
            // Invert map
            int[] invertMap = new int[oldEdgeIds.Length];
            for (int i = 0; i < oldEdgeIds.Length; i++) invertMap[oldEdgeIds[i]] = i;
            Dictionary<int, int> tmp = new Dictionary<int, int>();
            foreach (var entry in oldIdNewId) tmp.Add(invertMap[entry.Key], entry.Value);
            oldIdNewId = tmp;
            //
            return oldIdNewId;
        }
        private bool SplitVizualizationEdges(VisualizationData visualization, Dictionary<int, HashSet<int>> edgeIdNodeIds)
        {
            bool edgeFound;
            bool parabolic = false;
            int[] edgeCellIdsByEdge;
            int[] edgeCell;
            List<int> newEdge = new List<int>();
            List<int> oldEdge = new List<int>();
            List<int[]> newEdges = new List<int[]>();
            HashSet<int> edgeNodeIds = new HashSet<int>();
            HashSet<int> edgeCellHash = new HashSet<int>();
            Dictionary<int, List<int>> edgeSplitTo = new Dictionary<int, List<int>>();
            //
            parabolic = visualization.EdgeCells.Length > 0 && visualization.EdgeCells[0].Length == 3;
            // For each edge
            for (int i = 0; i < visualization.EdgeCellIdsByEdge.Length; i++)
            {
                // Create a list of nodes on the edge
                edgeNodeIds.Clear();
                edgeCellIdsByEdge = visualization.EdgeCellIdsByEdge[i];
                for (int j = 0; j < edgeCellIdsByEdge.Length; j++) edgeNodeIds.UnionWith(visualization.EdgeCells[edgeCellIdsByEdge[j]]);
                // Find the edge with the same node ids
                edgeFound = false;
                foreach (var entry in edgeIdNodeIds)
                {
                    // All nodes are the same
                    if (entry.Value.Count == edgeNodeIds.Count && entry.Value.Except(edgeNodeIds).Count() == 0)
                    {
                        edgeFound = true;
                        break;
                    }
                    // Next line is for when the mesh is converted to parabolic outside netgen
                    if (parabolic & entry.Value.Intersect(edgeNodeIds).Count() == entry.Value.Count())
                    {
                        edgeFound = true;
                        break;
                    }
                }
                // Try to find all nodes of the new edge inside the existing edge
                if (!edgeFound)
                {
                    int newEdgeId;
                    edgeFound = false;
                    List<int> newEdgeIds = new List<int>();
                    foreach (var entry in edgeIdNodeIds)
                    {
                        // Check if it is a part of the edge
                        if (entry.Value.IsProperSubsetOf(edgeNodeIds))
                        {
                            newEdge.Clear();
                            oldEdge.Clear();
                            for (int j = 0; j < edgeCellIdsByEdge.Length; j++)
                            {
                                edgeCell = visualization.EdgeCells[edgeCellIdsByEdge[j]];
                                edgeCellHash.Clear();
                                edgeCellHash.UnionWith(edgeCell);
                                // Split edge to oldEdge and newEdge
                                if (edgeCellHash.IsSubsetOf(entry.Value)) oldEdge.Add(edgeCellIdsByEdge[j]);
                                else newEdge.Add(edgeCellIdsByEdge[j]);
                            }
                            // Overwrite the old edge
                            visualization.EdgeCellIdsByEdge[i] = oldEdge.ToArray();
                            // Add the new edge to the new edge list
                            newEdges.Add(newEdge.ToArray());
                            // Get the id for the new edge
                            newEdgeId = visualization.EdgeCellIdsByEdge.Length + newEdges.Count - 1;
                            // Save the new id the edge was split into
                            if (edgeSplitTo.TryGetValue(i, out newEdgeIds)) newEdgeIds.Add(newEdgeId);
                            else edgeSplitTo.Add(i, new List<int>() { newEdgeId });
                            //
                            edgeFound = true;
                            break;
                        }
                    }
                    if (!edgeFound)
                    {
                        int error = 1;
                    }
                }
            }
            // Add new vertices - end points of edges
            int n1;
            int n2;
            HashSet<int> vertices = new HashSet<int>(visualization.VertexNodeIds);
            foreach (var newEdgeEntry in newEdges)
            {
                n1 = visualization.EdgeCells[newEdgeEntry[0]][0];
                n2 = visualization.EdgeCells[newEdgeEntry.Last()][1];
                vertices.Add(n1);
                vertices.Add(n2);
            }
            visualization.VertexNodeIds = vertices.ToArray();
            Array.Sort(visualization.VertexNodeIds);
            // Add new edges to the visualization
            List<int[]> oldEdges = visualization.EdgeCellIdsByEdge.ToList();
            oldEdges.AddRange(newEdges);
            visualization.EdgeCellIdsByEdge = oldEdges.ToArray();
            // Add new edges to the surfaces
            HashSet<int> edgeIdsHash = new HashSet<int>();
            Dictionary<int, HashSet<int>> surfaceIdNewEdgeIds = new Dictionary<int, HashSet<int>>();
            int count = 0;
            foreach (int[] edgeIds in visualization.FaceEdgeIds)
            {
                foreach (int edgeId in edgeIds)
                {
                    if (edgeSplitTo.ContainsKey(edgeId))
                    {
                        if (surfaceIdNewEdgeIds.TryGetValue(count, out edgeIdsHash))
                        {
                            edgeIdsHash.UnionWith(edgeSplitTo[edgeId]);
                        }
                        else
                        {
                            edgeIdsHash = new HashSet<int>(edgeIds);
                            edgeIdsHash.UnionWith(edgeSplitTo[edgeId]);
                            surfaceIdNewEdgeIds.Add(count, edgeIdsHash);
                        }
                    }
                }
                count++;
            }
            foreach (var entry in surfaceIdNewEdgeIds)
            {
                visualization.FaceEdgeIds[entry.Key] = entry.Value.ToArray();
            }
            // If changes were made return true in order to recompute edge lengths
            return true;
        }
        
        // Merge and split parts
        public bool ArePartsMergable(string[] partNames)
        {
            HashSet<PartType> partTypes = new HashSet<PartType>();
            foreach (var partName in partNames) partTypes.Add(_parts[partName].PartType);
            return partTypes.Count == 1;
        }
        private void MergeGeometryParts()
        {
            string name = null;
            int partId = -1;
            BasePart part;
            List<string> mergedPartNames = new List<string>();
            List<int> allNodeIds = new List<int>();
            List<int> allElementIds = new List<int>();
            HashSet<Type> allElementTypes = new HashSet<Type>();
            List<PartType> partTypes = new List<PartType>() { PartType.Shell, PartType.Wire };
            //
            foreach (var partType in partTypes)
            {
                // Clear
                mergedPartNames.Clear();
                allNodeIds.Clear();
                allElementIds.Clear();
                allElementTypes.Clear();
                // Gather all data
                foreach (var entry in _parts)
                {
                    if (mergedPartNames.Count == 0)
                    {
                        name = entry.Key;
                        partId = entry.Value.PartId;
                    }
                    if (entry.Value.PartType == partType)
                    {
                        mergedPartNames.Add(entry.Key);
                        allNodeIds.AddRange(entry.Value.NodeLabels);
                        allElementIds.AddRange(entry.Value.Labels);
                        allElementTypes.UnionWith(entry.Value.ElementTypes);
                    }
                }
                // Remove merged parts
                foreach (var mergedPartName in mergedPartNames) _parts.Remove(mergedPartName);
                // Add merged part
                allNodeIds.Sort();
                allElementIds.Sort();
                //
                part = new GeometryPart(name, partId, allNodeIds.ToArray(), allElementIds.ToArray(), allElementTypes.ToArray());
                _parts.Add(name, part);
                // Renumber element PartIds
                foreach (var elementId in allElementIds) _elements[elementId].PartId = partId;
            }
        }
        public void MergeGeometryParts(string[] partNamesToMerge, out GeometryPart newGeometryPart, out string[] mergedPartNames)
        {
            newGeometryPart = null;
            mergedPartNames = null;
            if (partNamesToMerge == null || partNamesToMerge.Length < 2) return;
            // Find parts to merge
            HashSet<int> allElementIds = new HashSet<int>();
            List<string> mergedPartsList = new List<string>();
            int minId = int.MaxValue;
            BasePart part;
            foreach (string partName in partNamesToMerge)
            {
                if (_parts.TryGetValue(partName, out part) && part is GeometryPart meshPart)
                {
                    mergedPartsList.Add(partName);
                    allElementIds.UnionWith(meshPart.Labels);
                    if (meshPart.PartId < minId) minId = meshPart.PartId;
                }
            }
            if (mergedPartsList.Count == 1) return;
            //
            mergedPartNames = mergedPartsList.ToArray();
            // Hide parts
            HashSet<PartType> geometryPartTypes = new HashSet<PartType>();
            foreach (var partName in mergedPartNames)
            {
                part = _parts[partName];
                part.Visible = false;
                geometryPartTypes.Add(part.PartType);
            }
            if (geometryPartTypes.Count != 1) throw new NotSupportedException();
            // Create new part
            part = CreateBasePartFromElementIds(allElementIds.ToArray());
            //
            newGeometryPart = new GeometryPart(part);
            newGeometryPart.Name = _parts.GetNextNumberedKey("Compound_part");
            newGeometryPart.PartId = minId;
            newGeometryPart.SetPartType(geometryPartTypes.First());
            SetPartColorFromColorTable(newGeometryPart);
            // Renumber elements
            foreach (var elementId in newGeometryPart.Labels) _elements[elementId].PartId = minId;
            // Add new part
            _parts.Add(newGeometryPart.Name, newGeometryPart);
            // Update bounding boxes
            ComputeBoundingBox();
        }
        public void MergeMeshParts(string[] partNamesToMerge, out MeshPart newMeshPart, out string[] mergedPartNames)
        {
            newMeshPart = null;
            mergedPartNames = null;
            if (partNamesToMerge == null || partNamesToMerge.Length < 2) return;
            // Find parts to merge
            HashSet<int> allElementIds = new HashSet<int>();
            List<string> mergedPartsList = new List<string>();
            int minId = int.MaxValue;
            BasePart part;
            foreach (string partName in partNamesToMerge)
            {
                if (_parts.TryGetValue(partName, out part) && part is MeshPart meshPart)
                {
                    mergedPartsList.Add(partName);
                    allElementIds.UnionWith(meshPart.Labels);
                    if (meshPart.PartId < minId) minId = meshPart.PartId;
                }
            }
            if (mergedPartsList.Count == 1) return;
            //
            mergedPartNames = mergedPartsList.ToArray();
            // Remove parts
            foreach (var partName in mergedPartNames) _parts.Remove(partName);
            // Create new part
            part = CreateBasePartFromElementIds(allElementIds.ToArray());
            //
            newMeshPart = new MeshPart(part);
            newMeshPart.Name = _parts.GetNextNumberedKey("Merged_part");
            newMeshPart.PartId = minId;
            SetPartColorFromColorTable(newMeshPart);
            // Renumber elements
            foreach (var elementId in newMeshPart.Labels) _elements[elementId].PartId = minId;
            // Add new part
            _parts.Add(newMeshPart.Name, newMeshPart);
            // Update bounding boxes
            ComputeBoundingBox();
        }
        public void MergeResultParts(string[] partNamesToMerge, out ResultPart newResultPart, out string[] mergedParts)
        {
            newResultPart = null;
            mergedParts = null;
            if (partNamesToMerge == null || partNamesToMerge.Length < 2) return;
            // Find parts to merge
            HashSet<int> allElementIds = new HashSet<int>();
            List<string> mergedPartsList = new List<string>();
            int minId = int.MaxValue;
            BasePart part;
            foreach (string partName in partNamesToMerge)
            {
                if (_parts.TryGetValue(partName, out part) && part is ResultPart resultPart)
                {
                    mergedPartsList.Add(partName);
                    allElementIds.UnionWith(resultPart.Labels);
                    if (resultPart.PartId < minId) minId = resultPart.PartId;
                }
            }
            if (mergedPartsList.Count == 1) return;
            //
            mergedParts = mergedPartsList.ToArray();
            // Remove parts
            foreach (var partName in mergedParts) _parts.Remove(partName);
            // Create new part
            part = CreateBasePartFromElementIds(allElementIds.ToArray());
            //
            newResultPart = new ResultPart(part);
            newResultPart.Name = _parts.GetNextNumberedKey("Merged_part");
            newResultPart.PartId = minId;
            SetPartColorFromColorTable(newResultPart);
            // Renumber elements
            foreach (var elementId in newResultPart.Labels) _elements[elementId].PartId = minId;
            // Add new part
            _parts.Add(newResultPart.Name, newResultPart);
            // Update bounding boxes
            ComputeBoundingBox();
        }
        public void MergePartsBasedOnMesh(FeMesh mesh, Type resultingPartType)
        {
            _parts.Clear();
            //
            BasePart part;
            foreach (var entry in mesh.Parts)
            {
                // Create a base part
                part = CreateBasePartFromElementIds(entry.Value.Labels);
                // Convert a base part
                if (resultingPartType == typeof(GeometryPart)) part = new GeometryPart(part);
                else if (resultingPartType == typeof(MeshPart)) part = new MeshPart(part);
                else if (resultingPartType == typeof(ResultPart)) part = new ResultPart(part);
                else throw new NotSupportedException();
                //
                part.Name = entry.Value.Name;
                part.PartId = entry.Value.PartId;
                part.Color = entry.Value.Color;
                // Renumber elements
                foreach (var elementId in part.Labels) _elements[elementId].PartId = part.PartId;
                // Add new part
                _parts.Add(part.Name, part);
            }
            // Update bounding boxes
            ComputeBoundingBox();
        }
        public void SplitCompoundMesh()
        {
            bool found;
            foreach (var entry1 in _parts)
            {
                found = false;
                foreach (var entry2 in _parts)
                {
                    // Find the same part
                    if (entry1.Key == entry2.Key) found = true;
                    // Skip the same part
                    if (!found || entry1.Key == entry2.Key) continue;
                    // Split the parts which share the same nodes
                    SplitCompoundParts(entry1.Value, entry2.Value);
                }
            }
        }
        public void SplitCompoundParts(BasePart part1, BasePart part2)
        {
            HashSet<int> nodeIds1 = new HashSet<int>(part1.NodeLabels);
            HashSet<int> nodeIds2 = new HashSet<int>(part2.NodeLabels);
            nodeIds1.IntersectWith(nodeIds2);
            // Split the parts if they share the same nodes
            if (nodeIds1.Count > 0)
            {
                FeNode oldNode;
                FeNode newNode;
                Dictionary<int, int> newIds = new Dictionary<int, int>();
                // Duplicate shared nodes
                foreach (var nodeId in nodeIds1)
                {
                    oldNode = _nodes[nodeId];
                    newNode = AddNodeByCoor(oldNode.X, oldNode.Y, oldNode.Z);
                    newIds.Add(oldNode.Id, newNode.Id);
                }
                // Renumber element nodes
                int id;
                FeElement element;
                foreach (var elementId in part2.Labels)
                {
                    element = _elements[elementId];
                    for (int i = 0; i < element.NodeIds.Length; i++)
                    {
                        if (newIds.TryGetValue(element.NodeIds[i], out id)) element.NodeIds[i] = id;
                    }
                }
                // Renumber 3D part's node labels
                for (int i = 0; i < part2.NodeLabels.Length; i++)
                {
                    if (newIds.TryGetValue(part2.NodeLabels[i], out id)) part2.NodeLabels[i] = id;
                }
                // Rename visualization cells and edges
                //part2.Visualization.ResetCellNeighboursOverCell();  // internal cells now become external cells: Reset
                part2.RenumberVisualizationNodes(newIds);
            }
        }
        //
        public void CreatePartsFromElementSets(string[] elementSetNames, out BasePart[] modifiedParts, out BasePart[] newParts)
        {
            // Get parts from ids
            int maxPartId = -int.MaxValue;
            Dictionary<int, BasePart> partIdNamePairs = new Dictionary<int, BasePart>();
            foreach (var entry in _parts)
            {
                partIdNamePairs.Add(entry.Value.PartId, entry.Value);
                if (entry.Value.PartId > maxPartId) maxPartId = entry.Value.PartId;
            }
            maxPartId++;
            // Get element ids to remove from parts by partIds
            int partId;
            FeElementSet elementSet;
            FeElement element;
            List<string> newPartNames = new List<string>();
            Dictionary<int, List<int>> elementIdsToRemove = new Dictionary<int, List<int>>();
            List<int> elementIds;
            for (int i = 0; i < elementSetNames.Length; i++)
            {
                if (_elementSets.TryGetValue(elementSetNames[i], out elementSet))
                {
                    newPartNames.Add(elementSetNames[i]);
                    //
                    foreach (var elementId in elementSet.Labels)
                    {
                        element = _elements[elementId];
                        partId = element.PartId;
                        //
                        if (partId != -1)   // for contact elements in .cel files
                        {
                            if (elementIdsToRemove.TryGetValue(partId, out elementIds)) elementIds.Add(elementId);
                            else elementIdsToRemove.Add(partId, new List<int>() { elementId });
                        }
                        //
                        element.PartId = maxPartId + i;
                    }
                }
            }
            // Modify existing parts
            int count = 0;
            BasePart part;
            BasePart newBasePart;
            MeshPart newMeshPart;
            ResultPart newResultPart;
            List<FeElement1D> edgeElements = new List<FeElement1D>();
            modifiedParts = new BasePart[elementIdsToRemove.Count];
            foreach (var entry in elementIdsToRemove)
            {
                part = partIdNamePairs[entry.Key];
                part.Labels = part.Labels.Except(entry.Value).ToArray();
                //
                newBasePart = CreateBasePartFromElementIds(part.Labels);
                edgeElements.AddRange(GetLineElementsFromEdges(newBasePart)); // get new free edges
                if (part is MeshPart mp)
                {
                    newMeshPart = new MeshPart(newBasePart);
                    newMeshPart.CopyActiveElementTypesFrom(mp);
                    newBasePart = newMeshPart;
                }
                else if (part is ResultPart rp)
                {
                    newResultPart = new ResultPart(newBasePart);
                    newBasePart = newResultPart;
                }
                //
                newBasePart.Name = part.Name;
                newBasePart.PartId = part.PartId;
                SetPartColorFromColorTable(newBasePart);
                // Replace part
                _parts[newBasePart.Name] = newBasePart;
                // Keep existing edges
                edgeElements.AddRange(GetLineElementsFromEdges(part));
                ConvertLineFeElementsToEdges(edgeElements, new string[] { newBasePart.Name });
                //
                modifiedParts[count++] = newBasePart;
            }
            // Create new parts
            count = 0;
            newParts = new BasePart[newPartNames.Count];
            string[] allMeshNames;
            foreach (var newPartName in newPartNames)
            {
                newBasePart = CreateBasePartFromElementIds(_elementSets[newPartName].Labels);
                if (_meshRepresentation == MeshRepresentation.Mesh)
                {
                    newMeshPart = new MeshPart(newBasePart);
                    newBasePart = newMeshPart;
                }
                else if (_meshRepresentation == MeshRepresentation.Results)
                {
                    newResultPart = new ResultPart(newBasePart);
                    newBasePart = newResultPart;
                }
                // Check name
                newBasePart.Name = "From_" + newPartName;
                allMeshNames = GetAllMeshEntityNames();
                if (allMeshNames.Contains(newBasePart.Name))
                    newBasePart.Name = allMeshNames.GetNextNumberedKey(newBasePart.Name);
                //
                newBasePart.PartId = maxPartId + count;
                SetPartColorFromColorTable(newBasePart);
                //
                _parts.Add(newBasePart.Name, newBasePart);
                newParts[count++] = newBasePart;
                // Keep existing edges
                ConvertLineFeElementsToEdges(edgeElements, new string[] { newBasePart.Name });
            }
            // Remove element sets
            for (int i = 0; i < elementSetNames.Length; i++) _elementSets.Remove(elementSetNames[i]);
            // Update bounding boxes
            ComputeBoundingBox();
        }
        public BasePart CreateBasePartFromElementIds(int[] elementIds)
        {
            HashSet<Type> partElementTypes = new HashSet<Type>();
            HashSet<int> partNodeIds = new HashSet<int>();
            HashSet<int> partElementIds = new HashSet<int>();
            FeElement element;
            for (int i = 0; i < elementIds.Length; i++)
            {
                element = _elements[elementIds[i]];
                partElementIds.Add(element.Id);
                partElementTypes.Add(element.GetType());
                partNodeIds.UnionWith(element.NodeIds);
            }
            BasePart part = new BasePart("Part-from-element-Ids", -1, partNodeIds.ToArray(),
                                         partElementIds.ToArray(), partElementTypes.ToArray());
            //
            if (part.PartType == PartType.Solid)
                ExtractSolidPartVisualization(part, Globals.EdgeAngle);
            else if (part.PartType == PartType.Shell || part.PartType == PartType.SolidAsShell)
                ExtractShellPartVisualization(part, isCADPart: false, Globals.EdgeAngle);
            else if (part.PartType == PartType.Unknown)
                throw new NotSupportedException();
            //
            return part;
        }
        public BasePart[] CreateBasePartsByTypeFromElementIds(int[] elementIds)
        {
            HashSet<Type>[] partElementTypes = new HashSet<Type>[3];
            HashSet<int>[] partNodeIds = new HashSet<int>[3];
            HashSet<int>[] partElementIds = new HashSet<int>[3];
            for (int i = 0; i < 3; i++)
            {
                partElementTypes[i] = new HashSet<Type>();
                partNodeIds[i] = new HashSet<int>();
                partElementIds[i] = new HashSet<int>();
            }
            //
            int index;
            FeElement element;
            //
            for (int i = 0; i < elementIds.Length; i++)
            {
                element = _elements[elementIds[i]];
                if (element is FeElement3D) index = 0;
                else if (element is FeElement2D) index = 1;
                else if (element is FeElement1D) index = 2;
                else throw new NotSupportedException();
                //
                partElementIds[index].Add(element.Id);
                partElementTypes[index].Add(element.GetType());
                partNodeIds[index].UnionWith(element.NodeIds);
            }
            BasePart part;
            List<BasePart> parts = new List<BasePart>();
            if (partElementIds[0].Count > 0)
            {
                part = new BasePart("Part-3D-from-element-Ids", -1, partNodeIds[0].ToArray(),
                                    partElementIds[0].ToArray(), partElementTypes[0].ToArray());
                parts.Add(part);
            }
            if (partElementIds[1].Count > 0)
            {
                part = new BasePart("Part-2D-from-element-Ids", -1, partNodeIds[1].ToArray(),
                                    partElementIds[1].ToArray(), partElementTypes[1].ToArray());
                parts.Add(part);
            }
            if (partElementIds[2].Count > 0)
            {
                part = new BasePart("Part-1D-from-element-Ids", -1, partNodeIds[2].ToArray(),
                                    partElementIds[2].ToArray(), partElementTypes[2].ToArray());
                parts.Add(part);
            }
            //
            foreach (var newPart in parts)
            {
                if (newPart.PartType == PartType.Solid)
                    ExtractSolidPartVisualization(newPart, Globals.EdgeAngle);
                else if (newPart.PartType == PartType.Shell || newPart.PartType == PartType.SolidAsShell)
                    ExtractShellPartVisualization(newPart, isCADPart: false, Globals.EdgeAngle);
                else if (newPart.PartType == PartType.Unknown)
                    throw new NotSupportedException();
            }
            //
            return parts.ToArray();
        }
        public GeometryPart CreateGeometryPartFromElementIds(int[] elementIds)
        {
            HashSet<Type> partElementTypes = new HashSet<Type>();
            HashSet<int> partNodeIds = new HashSet<int>();
            HashSet<int> partElementIds = new HashSet<int>();
            FeElement element;
            for (int i = 0; i < elementIds.Length; i++)
            {
                element = _elements[elementIds[i]];
                partElementIds.Add(element.Id);
                partElementTypes.Add(element.GetType());
                partNodeIds.UnionWith(element.NodeIds);
            }
            GeometryPart part = new GeometryPart("Part-from-element-Ids", -1, partNodeIds.ToArray(),
                                                 partElementIds.ToArray(), partElementTypes.ToArray());
            //
            if (part.PartType == PartType.Solid)
                ExtractSolidPartVisualization(part, Globals.EdgeAngle);
            else if (part.PartType == PartType.Shell || part.PartType == PartType.SolidAsShell)
                ExtractShellPartVisualization(part, isCADPart: false, Globals.EdgeAngle);
            //
            return part;
        }
        private FeElement1D[] GetLineElementsFromEdges(BasePart part)
        {
            VisualizationData vis = part.Visualization;
            FeElement1D[] edgeElements = new FeElement1D[vis.EdgeCells.Length];
            for (int i = 0; i < vis.EdgeCells.Length; i++)
            {
                edgeElements[i] = new LinearBeamElement(i, vis.EdgeCells[i]);
            }
            return edgeElements;
        }
        //
        public void ConvertLineFeElementsToEdges(HashSet<int> vertexNodeIds = null, bool checkForErrors = false,
                                                 string partName = null)
        {
            // Create a list of all 1D elements
            List<FeElement1D> edgeElements = new List<FeElement1D>();
            foreach (var entry in _elements)
            {
                if (entry.Value is FeElement1D element1D) edgeElements.Add(element1D);
            }
            //
            string[] partNames;
            if (partName != null) partNames = new string[] { partName };
            else partNames = _parts.Keys.ToArray();
            //
            ConvertLineFeElementsToEdges(edgeElements, partNames, vertexNodeIds, checkForErrors);
        }
        public void ConvertLineFeElementsToEdges(List<FeElement1D> edgeElements, string[] partNames,
                                                 HashSet<int> vertexNodeIds = null, bool checkForErrors = false)
        {
            bool add;
            int n1;
            int[] key;
            int[][] cells;
            int[][] cellEdges;
            BasePart part;
            List<int> cellIds;
            List<int[]> edgeCells = new List<int[]>();
            CompareIntArray comparer = new CompareIntArray();
            HashSet<int[]> sortedEdgeCells = new HashSet<int[]>(comparer);    // to be unique
            Dictionary<int, List<int>> nodeCellIds = new Dictionary<int, List<int>>();
            // For each part
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                //
                if (part.PartType == PartType.Solid
                    || part.PartType == PartType.SolidAsShell
                    || part.PartType == PartType.Shell)
                {
                    // Build a map of cellIds connected to each node
                    cells = part.Visualization.Cells;
                    nodeCellIds.Clear();
                    for (int i = 0; i < cells.Length; i++)
                    {
                        foreach (var nodeId in cells[i])
                        {
                            if (nodeCellIds.TryGetValue(nodeId, out cellIds)) cellIds.Add(i);
                            else nodeCellIds.Add(nodeId, new List<int>() { i });
                        }
                    }
                    //
                    edgeCells.Clear();
                    sortedEdgeCells.Clear();
                    // For each LinearFeElement find a cell edge
                    foreach (var element in edgeElements)
                    {
                        n1 = element.NodeIds[0];
                        // Get neighbouring cells
                        if (nodeCellIds.TryGetValue(n1, out cellIds))
                        {
                            foreach (var cellId in cellIds)
                            {
                                // Get linear or parabolic cell edge and compare it to LinearFeElement
                                cellEdges = GetVisualizationEdgeCells(cells[cellId], ElementFaceType.Face);
                                // Check all cell edges
                                foreach (var cellEdge in cellEdges)
                                {
                                    add = true;
                                    // Check all LinearFeElement nodeIds
                                    foreach (var nodeId in element.NodeIds)
                                    {
                                        if (!cellEdge.Contains(nodeId))
                                        {
                                            add = false;
                                            break;
                                        }
                                    }
                                    if (add)
                                    {
                                        key = cellEdge.ToArray();
                                        Array.Sort(key);
                                        if (sortedEdgeCells.Add(key)) edgeCells.Add(cellEdge);
                                    }
                                }
                            }
                        }
                    }
                    //
                    part.Visualization.EdgeCells = edgeCells.ToArray();
                    //
                    SplitVisualizationEdgesAndFaces(part, vertexNodeIds);
                    //
                    if (checkForErrors) CheckForFreeAndErrorElementsInCADPart(part);
                }
            }
        }
        public void ConvertEdgeNodesToEdges(BasePart part, Dictionary<int, HashSet<int>> edgeIdNodeIds)
        {
            int[] key;
            int[][] cells;
            int[][] cellEdges;
            List<int> cellIds;
            List<int[]> allEdgeCells = new List<int[]>();
            CompareIntArray comparer = new CompareIntArray();
            HashSet<int[]> sortedEdgeCells = new HashSet<int[]>(comparer);    // to be unique
            Dictionary<int, List<int>> nodeCellIds = new Dictionary<int, List<int>>();
            //
            if (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell || part.PartType == PartType.Shell)
            {
                // Build a map of cellIds connected to each node
                cells = part.Visualization.Cells;
                nodeCellIds.Clear();
                for (int i = 0; i < cells.Length; i++)
                {
                    foreach (var nodeId in cells[i])
                    {
                        if (nodeCellIds.TryGetValue(nodeId, out cellIds)) cellIds.Add(i);
                        else nodeCellIds.Add(nodeId, new List<int>() { i });
                    }
                }
                //
                allEdgeCells.Clear();
                sortedEdgeCells.Clear();
                // For each edge find a cell edges
                foreach (var edgeEntry in edgeIdNodeIds)
                {
                    foreach (var nodeId in edgeEntry.Value)
                    {
                        // Get neighbouring cells
                        if (nodeCellIds.TryGetValue(nodeId, out cellIds))
                        {
                            foreach (var cellId in cellIds)
                            {
                                // Get linear or parabolic cell edge and compare it to edge nodes
                                cellEdges = GetVisualizationEdgeCells(cells[cellId], ElementFaceType.Face);
                                // Check all cell edges
                                foreach (var cellEdge in cellEdges)
                                {
                                    if (edgeEntry.Value.Intersect(cellEdge).Count() == cellEdge.Length)
                                    {
                                        key = cellEdge.ToArray();
                                        Array.Sort(key);
                                        if (sortedEdgeCells.Add(key)) allEdgeCells.Add(cellEdge);
                                    }
                                }
                            }
                        }
                    }
                }
                //
                part.Visualization.EdgeCells = allEdgeCells.ToArray();
                //
                SplitVisualizationEdgesAndFaces(part);
            }
        }
        //
        public int GetMaxPartId()
        {
            // Find the max part id
            int maxId = -1;
            foreach (var entry in _parts)
            {
                if (entry.Value.PartId > maxId)
                {
                    maxId = entry.Value.PartId;
                }
            }
            return maxId;
        }
        public BasePart GetPartById(int id)
        {
            // Find the part
            BasePart part = null;
            foreach (var entry in _parts)
            {
                if (entry.Value.PartId == id)
                {
                    part = entry.Value;
                    break;
                }
            }
            return part;
        }
        public BasePart GetPartContainingElementId(int elementId)
        {
            if (elementId < 0) return null;
            //
            FeElement element = _elements[elementId];
            foreach (var entry in _parts)
            {
                if (entry.Value.PartId == element.PartId) return entry.Value;
            }
            return null;
        }
        public string[] GetPartNamesByIds(int[] ids)
        {
            if (ids == null) return null;
            //
            HashSet<int> idsHash = new HashSet<int>(ids);
            List<string> partNames = new List<string>();
            foreach (var entry in _parts)
            {
                if (idsHash.Contains(entry.Value.PartId)) partNames.Add(entry.Key); 
            }
            return partNames.ToArray();
        }
        public int[] GetPartIdsByNames(string[] partNames)
        {
            int[] partIds = new int[partNames.Length];
            for (int i = 0; i < partNames.Length; i++) partIds[i] = _parts[partNames[i]].PartId;
            return partIds;
        }
        public double[][] GetPartOffsetsByNames(string[] partNames)
        {
            double[][] partOffsets = new double[partNames.Length][];
            for (int i = 0; i < partNames.Length; i++) partOffsets[i] = _parts[partNames[i]].Offset.ToArray();
            return partOffsets;
        }
        public int[] GetPartIdsByElementIds(int[] elementIds)
        {
            HashSet<int> partIds = new HashSet<int>();
            foreach (var elementId in elementIds) partIds.Add(_elements[elementId].PartId);
            return partIds.ToArray();
        }
        public string[] GetPartNamesByElementIds(int[] elementIds)
        {
            int[] ids = GetPartIdsByElementIds(elementIds);
            return GetPartNamesByIds(ids);
        }
        public int[] GetPartIdsByNodeIds(int[] nodeIds)
        {
            List<int> partIds = new List<int>();
            HashSet<int> nodeIdsHash = new HashSet<int>();
            foreach (var entry in _parts)
            {
                if (entry.Value.NodeLabels.Intersect(nodeIds).Count() > 0) partIds.Add(entry.Value.PartId);
            }
            return partIds.ToArray();
        }
        public BasePart GetPartByGeometryId(int geometryId)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryId);
            return GetPartById(itemTypePartIds[2]);
        }
        public HashSet<BasePart> GetPartsFromSelectionIds(int[] prevIds, vtkSelectItem selectItem)
        {
            HashSet<BasePart> parts = new HashSet<BasePart>();
            if (selectItem == vtkSelectItem.Node)
            {
                foreach (var entry in _parts)
                {
                    if (entry.Value.NodeLabels.Intersect(prevIds).Count() > 0) parts.Add(entry.Value);
                }
            }
            else if (selectItem == vtkSelectItem.Element)
            {
                foreach (var entry in _parts)
                {
                    if (entry.Value.Labels.Intersect(prevIds).Count() > 0) parts.Add(entry.Value);
                }
            }
            else if (selectItem == vtkSelectItem.Edge)
            {
                int[] itemTypePartIds;
                foreach (var edgeId in prevIds)
                {
                    itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(edgeId);
                    parts.Add(GetPartById(itemTypePartIds[2]));
                }
            }
            else if (selectItem == vtkSelectItem.Surface)
            {
                int elementId;
                int vtkCellId;
                HashSet<int> partIds = new HashSet<int>();
                foreach (var faceId in prevIds)
                {
                    GetElementIdVtkCellIdFromFaceId(faceId, out elementId, out vtkCellId);
                    partIds.Add(_elements[elementId].PartId);
                }
                foreach (var partId in partIds) parts.Add(GetPartById(partId));
            }
            else if (selectItem == vtkSelectItem.Geometry)
            {
                foreach (var geometryId in prevIds) parts.Add(GetPartByGeometryId(geometryId));
            }
            else if (selectItem == vtkSelectItem.Part)
            {
                foreach (var partId in prevIds) parts.Add(GetPartById(partId));
            }
            else throw new NotSupportedException();
            //
            return parts;
        }
        // 3D - 2D
        public void UpdatePartsElementTypes(Dictionary<Type, HashSet<Enum>> elementTypeEnums)
        {
            foreach (var entry in _parts)
            {
                if (entry.Value is MeshPart mp) mp.UpdateElementTypeEnums(elementTypeEnums);
            }
        }
        // Section view - OLD
        public void ApplySectionView(Octree.Plane sectionPlane)
        {
            // Create octree if this is the first time the section cut was made
            if (_octree == null)
            {
                double size = _boundingBox.GetDiagonal();
                Octree.Point min = new Octree.Point(_boundingBox.MinX, _boundingBox.MinY, _boundingBox.MinZ);
                Octree.Point max = new Octree.Point(_boundingBox.MaxX, _boundingBox.MaxY, _boundingBox.MaxZ);
                _octree = new Octree.PointOctree<int>(size, (min + max) * 0.5, size / 100);

                foreach (var entry in _nodes)
                {
                    _octree.Add(entry.Key, new Octree.Point(entry.Value.Coor));
                }

                foreach (var entry in _parts)
                {
                    entry.Value.VisualizationCopy = entry.Value.Visualization.DeepCopy();
                }
            }

            // Split nodes in octree by plane
            List<int> front = new List<int>();
            List<int> onPlane = new List<int>();
            List<int> back = new List<int>();
            _octree.GetObjectsSplitByPlane(ref sectionPlane, front, onPlane, back);

            HashSet<int> frontNodes = new HashSet<int>(front);
            HashSet<int> backNodes = new HashSet<int>(back);
            frontNodes.UnionWith(onPlane);

            // Section cut
            BasePart part;
            VisualizationData visCut;
            foreach (var entry in _parts)
            {
                part = entry.Value;
                visCut = part.VisualizationCopy.DeepCopy();
                //visCut = part.Visualization;
                visCut.ApplySectionView(_elements, part.Labels, frontNodes, backNodes);
                part.Visualization = visCut;

                // Recompute the areas and lengths
                ComputeFaceAreas(part);
                ComputeEdgeLengths(part);
            }
        }
        public void RemoveSectionView()
        {
            _octree = null;
            foreach (var entry in _parts)
            {
                entry.Value.Visualization = entry.Value.VisualizationCopy;
                entry.Value.VisualizationCopy = null;
            }
        }
        // Exploded view
        public Dictionary<string, double[]> GetExplodedViewOffsets_(int explodedType, double scaleFactor, string[] partNames = null)
        {
            //
            // https://stackoverflow.com/questions/3265986/an-algorithm-to-space-out-overlapping-rectangles
            //
            if (partNames == null) partNames = _parts.Keys.ToArray();
            // Get connected part names
            List<List<BasePart>> allConnectedParts = GetAllConnectedParts(partNames);
            // Get bounding boxes of the selected parts
            int count = 0;
            BoundingBox box;
            List<BoundingBox> nonIntersectingBBs = new List<BoundingBox>();
            BoundingBox[] boxes = new BoundingBox[allConnectedParts.Count];
            foreach (var connectedParts in allConnectedParts)
            {
                boxes[count] = new BoundingBox();
                foreach (var connectedPart in connectedParts) boxes[count].IncludeBox(connectedPart.BoundingBox);
                //
                boxes[count].Scale(1.2);
                boxes[count].Tag = connectedParts;
                count++;
            }
            Array.Sort(boxes, new BoundingBoxVolmeComparer());
            // Add the largest box
            BoundingBox globalBox = new BoundingBox();
            nonIntersectingBBs.Add(boxes[0]);
            globalBox.IncludeBox(boxes[0]);
            //
            Vec3D center;
            Vec3D offset;
            Vec3D direction;
            List<BasePart> parts;
            Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
            for (int i = 1; i < boxes.Length; i++)
            {
                box = boxes[i];
                center = new Vec3D(globalBox.GetCenter());
                offset = new Vec3D();
                direction = new Vec3D(box.GetCenter()) - center;
                // Set the offset type
                if (explodedType == 1) direction.Y = direction.Z = 0;
                else if (explodedType == 2) direction.X = direction.Z = 0;
                else if (explodedType == 3) direction.X = direction.Y = 0;
                else if (explodedType == 4) direction.Z = 0;
                else if (explodedType == 5) direction.Y = 0;
                else if (explodedType == 6) direction.X = 0;
                // Fix the 0 length direction
                if (direction.Len2 < 1E-6 * globalBox.GetDiagonal()) direction.Coor = new double[] { 1, 1, 1 };
                // Set the offset type
                if (explodedType == 1) direction.Y = direction.Z = 0;
                else if (explodedType == 2) direction.X = direction.Z = 0;
                else if (explodedType == 3) direction.X = direction.Y = 0;
                else if (explodedType == 4) direction.Z = 0;
                else if (explodedType == 5) direction.Y = 0;
                else if (explodedType == 6) direction.X = 0;
                //
                direction.Normalize();
                direction *= (0.01 * globalBox.GetDiagonal());
                //
                count = 0;
                while (box.Intersects(nonIntersectingBBs) && count++ < 10000)
                {
                    box.AddOffset(direction.Coor);
                    offset += direction;
                }
                nonIntersectingBBs.Add(box);
                globalBox.IncludeBox(box);
                // Compound parts
                parts = (List<BasePart>)box.Tag;
                foreach (var part in parts) partOffsets.Add(part.Name, (offset * scaleFactor).Coor);
            }
            //
            return partOffsets;
        }
        public Dictionary<string, double[]> GetExplodedViewOffsets(int explodedType, double scaleFactor, string[] partNames = null)
        {
            //
            // https://stackoverflow.com/questions/3265986/an-algorithm-to-space-out-overlapping-rectangles
            //
            if (partNames == null) partNames = _parts.Keys.ToArray();
            // Get connected part names
            List<List<BasePart>> allConnectedParts = GetAllConnectedParts(partNames);
            // Get bounding boxes of the selected parts
            int count = 0;
            BoundingBox[] boxes = new BoundingBox[allConnectedParts.Count];
            foreach (var connectedParts in allConnectedParts)
            {
                boxes[count] = new BoundingBox();
                foreach (var connectedPart in connectedParts) boxes[count].IncludeBox(connectedPart.BoundingBox);
                count++;
            }
            // Compute BB offsets
            double[][] offsets = BoundingBox.GetExplodedBBOffsets(explodedType, scaleFactor, boxes);
            //
            count = 0;
            Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
            foreach (var connectedParts in allConnectedParts)
            {
                foreach (var connectedPart in connectedParts) partOffsets.Add(connectedPart.Name, offsets[count]);
                count++;
            }
            //
            return partOffsets;
        }
        public void ApplyExplodedView(Dictionary<string, double[]> partOffsets)
        {
            double[] offset;
            BasePart part;
            FeNode node;
            HashSet<int> movedNodes = new HashSet<int>();
            //
            foreach (var entry in partOffsets)
            {
                offset = entry.Value;
                if (_parts.TryGetValue(entry.Key, out part))
                {
                    part.Offset = offset;
                    if (offset != null && (offset[0] != 0 || offset[1] != 0 || offset[2] != 0))
                    {
                        part.BoundingBox.AddOffset(offset);
                        //
                        foreach (var nodeId in part.NodeLabels.Except(movedNodes))
                        {
                            node = _nodes[nodeId];
                            node.X += offset[0];
                            node.Y += offset[1];
                            node.Z += offset[2];
                            _nodes[nodeId] = node;
                        }
                        movedNodes.UnionWith(part.NodeLabels);
                    }
                }
            }
            if (movedNodes.Count > 0)
            {
                foreach (var entry in _nodeSets) UpdateNodeSetCenterOfGravity(entry.Value);
                foreach (var entry in _referencePoints) UpdateReferencePoint(entry.Value);
            }
        }
        public void SuppressExploededView(string[] partNames = null)
        {
            partNames = ConnectedPartNames(partNames);
            // Remove already suppresed parts
            if (_partOffsets != null) partNames = partNames.Except(_partOffsets.Keys).ToArray();
            // Suppress exploded view
            Dictionary<string, double[]>  partOffsets = RemoveExplodedView(partNames);
            //
            if (_partOffsets == null) _partOffsets = partOffsets;
            else
            {
                foreach (var entry in partOffsets) _partOffsets.Add(entry.Key, entry.Value);
            }
        }
        public bool ResumeExploededView()
        {
            bool update = false;
            if (_partOffsets != null && _partOffsets.Count > 0)
            {
                ApplyExplodedView(_partOffsets);
                update = true;
            }
            _partOffsets = null;
            //
            return update;
        }
        public Dictionary<string, double[]> RemoveExplodedView(string[] partNames = null)
        {
            double[] offset;
            BasePart part;
            FeNode node;
            HashSet<int> movedNodes = new HashSet<int>();
            Dictionary<string, double[]> partOffsets = new Dictionary<string, double[]>();
            //
            if (partNames == null) partNames = _parts.Keys.ToArray();
            //
            foreach (var partName in partNames)
            {
                if (_parts.TryGetValue(partName, out part))
                {
                    part = _parts[partName];
                    offset = part.Offset;
                    //
                    if (offset != null && (offset[0] != 0 || offset[1] != 0 || offset[2] != 0))
                    {
                        foreach (var nodeId in part.NodeLabels.Except(movedNodes))
                        {
                            node = _nodes[nodeId];
                            node.X -= offset[0];
                            node.Y -= offset[1];
                            node.Z -= offset[2];
                            _nodes[nodeId] = node;
                        }
                        movedNodes.UnionWith(part.NodeLabels);
                        //
                        _parts[partName].Offset = new double[3];
                        part.BoundingBox.RemoveOffset(offset);
                        //
                        partOffsets.Add(partName, offset); // only add non-zero offsets
                    }
                }
            }
            if (movedNodes.Count > 0)
            {
                foreach (var entry in _nodeSets) UpdateNodeSetCenterOfGravity(entry.Value);
                foreach (var entry in _referencePoints) UpdateReferencePoint(entry.Value);
            }
            return partOffsets;
        }
        private List<List<BasePart>> GetAllConnectedParts(string[] partNames)
        {
            BasePart part;
            HashSet<BasePart> partsToInclude = new HashSet<BasePart>();
            foreach (var partName in partNames)
            {
                if (_parts.TryGetValue(partName, out part)) partsToInclude.Add(part);
            }
            //
            BasePart[] parts = _parts.Values.ToArray();
            HashSet<string> includedParts = new HashSet<string>();
            List<BasePart> connectedParts;
            List<List<BasePart>> allConnectedParts = new List<List<BasePart>>();
            //
            if (_meshRepresentation == MeshRepresentation.Geometry)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] is CompoundGeometryPart cgp)
                    {
                        connectedParts = new List<BasePart>();
                        for (int j = 0; j < cgp.SubPartNames.Length; j++)
                        {
                            part = _parts[cgp.SubPartNames[j]];
                            if (partsToInclude.Contains(part))
                            {
                                connectedParts.Add(part);
                                includedParts.Add(cgp.SubPartNames[j]);
                            }
                        }
                        if (connectedParts.Count() > 0) allConnectedParts.Add(connectedParts);
                        //
                        includedParts.Add(parts[i].Name);   // add compound part
                    }
                }
                for (int i = 0; i < parts.Length; i++)
                {
                    if (partsToInclude.Contains(parts[i]) && !includedParts.Contains(parts[i].Name))
                    {
                        connectedParts = new List<BasePart>() { parts[i] };
                        allConnectedParts.Add(connectedParts);
                    }
                }
            }
            else if (_meshRepresentation == MeshRepresentation.Mesh || _meshRepresentation == MeshRepresentation.Results)
            {
                Node<BasePart> node1;
                Node<BasePart> node2;
                Graph<BasePart> graph = new Graph<BasePart>();
                Dictionary<BasePart, Node<BasePart>> keys = new Dictionary<BasePart, Node<BasePart>>();
                //
                for (int i = 0; i < parts.Length; i++)
                {
                    if (partsToInclude.Contains(parts[i]))
                    {
                        if (!keys.TryGetValue(parts[i], out node1))
                        {
                            node1 = new Node<BasePart>(parts[i]);
                            keys.Add(parts[i], node1);
                            graph.AddNode(node1);
                        }
                        //
                        for (int j = i + 1; j < parts.Length; j++)
                        {
                            if (partsToInclude.Contains(parts[j]) &&
                                parts[i].BoundingBox.Intersects(parts[j].BoundingBox) &&
                                parts[i].NodeLabels.Intersect(parts[j].NodeLabels).Count() > 0)
                            {
                                if (!keys.TryGetValue(parts[j], out node2))
                                {
                                    node2 = new Node<BasePart>(parts[j]);
                                    keys.Add(parts[j], node2);
                                    graph.AddNode(node2);
                                }
                                graph.AddUndirectedEdge(node1, node2);
                            }
                        }
                    }
                }
                List<Graph<BasePart>> subGraphs = graph.GeConnectedSubgraphs();
                foreach (var subGraph in subGraphs)
                {
                    connectedParts = new List<BasePart>();
                    foreach (var node in subGraph.Nodes) connectedParts.Add(node.Value);
                    if (partsToInclude.Intersect(connectedParts).Count() > 0) allConnectedParts.Add(connectedParts);
                }
            }
            //
            return allConnectedParts;
        }
        private string[] ConnectedPartNames(string[] partNames)
        {
            if (partNames == null) return _parts.Keys.ToArray();
            else
            {
                HashSet<string> connectedPartNames = new HashSet<string>(partNames);
                // Get all part names that are connected to the given part names
                if (partNames != null)
                {
                    List<List<BasePart>> allConnectedParts = GetAllConnectedParts(_parts.Keys.ToArray());
                    foreach (var connectedParts in allConnectedParts)
                    {
                        if (connectedParts.Count > 1)
                        {
                            foreach (var connectedPart in connectedParts)
                            {
                                if (connectedPartNames.Contains(connectedPart.Name))
                                {
                                    // Add all connected part names
                                    foreach (var connectedPartForName in connectedParts)
                                        connectedPartNames.Add(connectedPartForName.Name);
                                    break;
                                }
                            }
                        }
                    }
                }
                return connectedPartNames.ToArray();
            }
        }
        // Colors
        public void ResetPartsColor()
        {
            foreach (var entry in _parts)
            {
                if (entry.Value.Color == System.Drawing.Color.Gray) // gray is the default base part color
                    SetPartColorFromColorTable(entry.Value);
            }
        }
        public void SetPartsColor(System.Drawing.Color color)
        {
            foreach (var entry in _parts)
            {
                entry.Value.Color = color;
            }
        }
        public void SetPartColorFromColorTable(BasePart part)
        {
            part.Color = Globals.ColorTable[(part.PartId - 1) % Globals.ColorTable.Length];
        }        

        #endregion #################################################################################################################

        #region Renumber  ##########################################################################################################
        public void RenumberNodes(int startId = 0)
        {
            Dictionary<int, int> newIds = new Dictionary<int, int>();
            Dictionary<int, FeNode> renNodes = new Dictionary<int, FeNode>();
            int oldId;
            int newId = startId;
            FeNode newNode;
            // Renumber nodes and fill the lookup map oldIds
            foreach (FeNode node in _nodes.Values)
            {
                newNode = node;
                oldId = node.Id;

                newIds.Add(oldId, newId);
                newNode.Id = newId;
                renNodes.Add(newId, newNode);

                newId++;
            }
            _nodes = renNodes;
            _maxNodeId = newId - 1;
            // Renumber element nodes
            FeElement newElement;
            foreach (FeElement element in _elements.Values)
            {
                newElement = element;
                for (int i = 0; i < newElement.NodeIds.Length; i++)
                {
                    newElement.NodeIds[i] = newIds[newElement.NodeIds[i]];
                }
            }
            // Renumber node sets
            FeNodeSet nodeSet;
            foreach (var entry in _nodeSets)
            {
                nodeSet = entry.Value;
                for (int i = 0; i < nodeSet.Labels.Length; i++)
                {
                    nodeSet.Labels[i] = newIds[nodeSet.Labels[i]];
                }
                // Renumber selection
                if (nodeSet.CreationData != null)
                {
                    foreach (var node in nodeSet.CreationData.Nodes)
                    {
                        if (node is SelectionNodeIds snids)
                        {
                            if (snids.GeometryIds) { }
                            else
                            {
                                for (int i = 0; i < snids.ItemIds.Length; i++)
                                {
                                    snids.ItemIds[i] = newIds[snids.ItemIds[i]];
                                }
                            }
                        }
                    }
                }
            }
            // Renumber 3D part's nodes, visualization cells and edges
            BasePart part;
            foreach (var entry in _parts)
            {
                part = entry.Value;
                //
                for (int i = 0; i < part.NodeLabels.Length; i++)
                {
                    part.NodeLabels[i] = newIds[part.NodeLabels[i]];
                }
                //
                part.RenumberVisualizationNodes(newIds);
            }
        }
        public void RenumberElements(int startId = 0)
        {
            Dictionary<int, int> oldIdnewId = new Dictionary<int, int>();
            Dictionary<int, FeElement> renumberedElements = new Dictionary<int, FeElement>();
            int oldId;
            int newId = startId;
            FeElement newElement;
            // Renumber elements and fill the lookup map oldIds
            foreach (var entry in _elements)
            {
                newElement = entry.Value;
                oldId = entry.Key;
                //
                oldIdnewId.Add(oldId, newId);
                newElement.Id = newId;
                renumberedElements.Add(newId, newElement);
                //
                newId++;
            }
            _elements = renumberedElements;
            _maxElementId = newId - 1;
            // Renumber element sets
            FeElementSet elementSet;
            foreach (var entry in _elementSets)
            {
                elementSet = entry.Value;
                if (elementSet.CreatedFromParts) continue;
                //
                for (int i = 0; i < elementSet.Labels.Length; i++)
                {
                    elementSet.Labels[i] = oldIdnewId[elementSet.Labels[i]];
                }
                // Renumber selection
                if (elementSet.CreationData != null)
                {                    
                    foreach (var node in elementSet.CreationData.Nodes)
                    {
                        if (node is SelectionNodeIds snids)
                        {
                            for (int i = 0; i < snids.ItemIds.Length; i++)
                            {
                                snids.ItemIds[i] = oldIdnewId[snids.ItemIds[i]];
                            }
                        }
                    }
                }
            }
            // Renumebr surface faceIds
            int faceId;
            FeSurface surface;
            foreach (var entry in _surfaces)
            {
                surface = entry.Value;
                if (surface.FaceIds != null)
                {
                    for (int i = 0; i < surface.FaceIds.Length; i++)
                    {
                        faceId = surface.FaceIds[i] % 10;
                        oldId = surface.FaceIds[i] / 10;
                        newId = oldIdnewId[oldId];
                        surface.FaceIds[i] = newId * 10 + faceId;
                    }
                }
            }
            BasePart part;
            foreach (var entry in _parts)
            {
                // Renumber parts
                part = entry.Value;
                for (int i = 0; i < part.Labels.Length; i++)
                {
                    part.Labels[i] = oldIdnewId[part.Labels[i]];
                }
                // Renumber 3D part's visualization cells
                part.RenumberVisualizationElements(oldIdnewId);
            }
        }
        public void RenumberParts(int startId = 0)
        {
            Dictionary<int, int> newId = new Dictionary<int, int>();
            foreach (var part in _parts)
            {
                newId.Add(part.Value.PartId, startId);
                part.Value.PartId = startId;
                startId++;
            }
            //
            foreach (var entry in _elements) entry.Value.PartId = newId[entry.Value.PartId];
        }
        public void ChangePartId(string partName, int newPartId)
        {
            int oldPartId;
            BasePart part = _parts[partName];
            oldPartId = part.PartId;
            if (newPartId == oldPartId) return;
            // Renumber
            part.PartId = newPartId;
            foreach (var elementId in part.Labels) _elements[elementId].PartId = newPartId;
        }

        #endregion #################################################################################################################

        #region Surfaces  ##########################################################################################################
        public void GetSurfaceGeometry(string name, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes,
                                       bool onlyVisible)
        {
            FeSurface surface = _surfaces[name];
            KeyValuePair<FeFaceName, string>[] elementSets = surface.ElementFaces.ToArray();
            //
            List<int[]> cellList = new List<int[]>();
            FeElement element;
            //
            Dictionary<int, bool> partVisibilities = new Dictionary<int, bool>();
            foreach (var part in _parts) partVisibilities.Add(part.Value.PartId, part.Value.Visible);
            //
            foreach (var entry in elementSets)
            {
                foreach (int elementId in _elementSets[entry.Value].Labels)
                {
                    element = _elements[elementId];
                    if (!(onlyVisible && !partVisibilities[element.PartId])) cellList.Add(element.GetVtkCellFromFaceName(entry.Key));
                }
            }
            cells = cellList.ToArray();
            //
            GetSurfaceGeometry(cells, out nodeCoor, out cellTypes);
        }
        public void GetSurfaceGeometry(int[][] cells, out double[][] nodeCoor, out int[] cellTypes)
        {
            GetSurfaceGeometry(cells, out int[] nodeIds, out nodeCoor, out cellTypes);
        }
        public void GetSurfaceGeometry(int[][] cells, out int[] nodeIds, out double[][] nodeCoor, out int[] cellTypes)
        {
            // Get the node ids of the used nodes
            HashSet<int> nodesNeeded = new HashSet<int>();
            for (int i = 0; i < cells.Length; i++) nodesNeeded.UnionWith(cells[i]);
            // Create node array and a lookup table
            nodeCoor = new double[nodesNeeded.Count][];
            nodeIds = new int[nodesNeeded.Count];
            Dictionary<int, int> oldNew = new Dictionary<int, int>();
            int count = 0;
            foreach (int id in nodesNeeded)
            {
                nodeIds[count] = id;
                nodeCoor[count] = _nodes[id].Coor;
                oldNew.Add(id, count);
                count++;
            }
            // Renumber triangles and add cell type
            cellTypes = new int[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                for (int j = 0; j < cells[i].Length; j++)
                {
                    cells[i][j] = oldNew[cells[i][j]];
                }
                //
                if (cells[i].Length == 2) cellTypes[i] = (int)vtkCellType.VTK_LINE;
                else if (cells[i].Length == 3) cellTypes[i] = (int)vtkCellType.VTK_TRIANGLE;
                else if (cells[i].Length == 4) cellTypes[i] = (int)vtkCellType.VTK_QUAD;
                else if (cells[i].Length == 6) cellTypes[i] = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
                else if (cells[i].Length == 8) cellTypes[i] = (int)vtkCellType.VTK_QUADRATIC_QUAD;
                else throw new NotSupportedException();
            }
        }
        public void GetSurfaceEdgesGeometry(string name, out double[][] nodeCoor, out int[][] cells, out int[] vtkCellTypes,
                                            bool onlyVisible)
        {
            FeSurface surface = _surfaces[name];
            KeyValuePair<FeFaceName, string>[] elementSets = surface.ElementFaces.ToArray();
            //
            List<int[]> cellList = new List<int[]>();
            List<ElementFaceType> elementFaceTypes = new List<ElementFaceType>();
            FeElement element;
            //
            Dictionary<int, bool> partVisibilities = new Dictionary<int, bool>();
            foreach (var part in _parts) partVisibilities.Add(part.Value.PartId, part.Value.Visible);
            //
            foreach (var entry in elementSets)
            {                
                foreach (int elementId in _elementSets[entry.Value].Labels)
                {
                    element = _elements[elementId];
                    // Skip shell edge faces
                    if (element is FeElement2D && !(entry.Key == FeFaceName.S1 || entry.Key == FeFaceName.S2))
                        elementFaceTypes.Add(ElementFaceType.Edge);
                    else elementFaceTypes.Add(ElementFaceType.Face);
                    //
                    if (!(onlyVisible && !partVisibilities[element.PartId])) cellList.Add(element.GetVtkCellFromFaceName(entry.Key));
                }
            }
            // Get edges
            cells = GetFreeEdgesFromVisualizationCells(cellList.ToArray(), elementFaceTypes.ToArray());
            //
            GetSurfaceEdgesGeometry(cells, out int[] nodeIds, out nodeCoor, out vtkCellTypes);
        }
        public void GetSurfaceEdgesGeometry(int[][] cells, out int[] nodeIds, out double[][] nodeCoor, out int[] vtkCellTypes)
        {
            // Get the node ids of the used nodes
            HashSet<int> nodesNeeded = new HashSet<int>();
            for (int i = 0; i < cells.Length; i++) nodesNeeded.UnionWith(cells[i]);
            // Create node array and a lookup table
            nodeCoor = new double[nodesNeeded.Count][];
            nodeIds = new int[nodesNeeded.Count];
            Dictionary<int, int> oldNew = new Dictionary<int, int>();
            int count = 0;
            //
            foreach (int id in nodesNeeded)
            {
                nodeIds[count] = id;
                nodeCoor[count] = _nodes[id].Coor;
                oldNew.Add(id, count);
                count++;
            }
            // Renumber edge cell node ids
            vtkCellTypes = new int[cells.Length];
            //
            for (int i = 0; i < cells.Length; i++)
            {
                for (int j = 0; j < cells[i].Length; j++) cells[i][j] = oldNew[cells[i][j]];
                //
                if (cells[i].Length == 2) vtkCellTypes[i] = (int)vtkCellType.VTK_LINE;
                else if (cells[i].Length == 3) vtkCellTypes[i] = (int)vtkCellType.VTK_QUADRATIC_EDGE;
                else throw new NotSupportedException();
            }
        }
        //
        public void CreateSurfaceItems(FeSurface surface)
        {
            surface.ClearElementFaces();     // area = 0;
            //
            if (surface.Type == FeSurfaceType.Node) CreateSurfaceNodeSet(surface);
            else if (surface.Type == FeSurfaceType.Element) CreateSurfaceFaces(surface);
            else throw new CaeException("Surface type not supported.");
        }
        private void CreateSurfaceNodeSet(FeSurface surface)
        {
            double area;
            int[] nodeIds;
            FeSurfaceFaceTypes surfaceFaceTypes;
            Dictionary<FeFaceName, List<int>> elementSets;
            //
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
            {
                CreateSurfaceFacesFromSelection(surface.FaceIds, out nodeIds, out elementSets, out area, out surfaceFaceTypes);
                // Node set
                string nodeSetName = GetNextFreeInternalName(_nodeSets, surface.Name);
                FeNodeSet nodeSet = new FeNodeSet(nodeSetName, nodeIds);
                nodeSet.Internal = true;
                UpdateNodeSetCenterOfGravity(nodeSet);
                _nodeSets.Add(nodeSetName, nodeSet);
                surface.NodeSetName = nodeSetName;
                surface.SurfaceFaceTypes = surfaceFaceTypes;
            }
            else if (surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                surface.NodeSetName = surface.CreatedFromNodeSetName;
            }
            else throw new NotSupportedException();
            //
            surface.Area = 0;
        }
        private void CreateSurfaceFaces(FeSurface surface)
        {
            double area;
            int[] nodeIds;
            FeSurfaceFaceTypes surfaceFaceTypes = FeSurfaceFaceTypes.Unknown;
            Dictionary<FeFaceName, List<int>> elementSets;
            //
            surface.ClearElementFaces(); // area = 0 
            //
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection || surface.CreatedFrom == FeSurfaceCreatedFrom.Faces)
            {
                CreateSurfaceFacesFromSelection(surface.FaceIds, out nodeIds, out elementSets, out area, out surfaceFaceTypes);
                surface.Area = area;
                surface.SurfaceFaceTypes = surfaceFaceTypes;
            }
            else if (surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                CreateSurfaceFacesFromNodeSet(surface, out nodeIds, out elementSets, out area, out surfaceFaceTypes);
                surface.Area = area;
                surface.SurfaceFaceTypes = surfaceFaceTypes;
            }
            else throw new NotSupportedException();
            //
            if (elementSets.Count == 0)
            {
                surface.NodeSetName = null;
                surface.ClearElementFaces();
                return;
            }
            // Node set
            string nodeSetName = GetNextFreeInternalName(_nodeSets, surface.Name);
            FeNodeSet nodeSet = new FeNodeSet(nodeSetName, nodeIds);
            nodeSet.Internal = true;
            UpdateNodeSetCenterOfGravity(nodeSet);
            _nodeSets.Add(nodeSetName, nodeSet);
            surface.NodeSetName = nodeSetName;
            // Element sets
            FeElementSet elementSet;
            string elementSetName;
            foreach (KeyValuePair<FeFaceName, List<int>> entry in elementSets)
            {
                elementSetName = GetNextFreeInternalName(_elementSets, surface.Name + "_" + entry.Key);
                elementSet = new FeElementSet(elementSetName, entry.Value.ToArray());
                elementSet.Internal = true;
                _elementSets.Add(elementSetName, elementSet);
                surface.AddElementFace(entry.Key, elementSetName);
            }
        }
        private void GetNodeAndElementIdsFromNodeSetSurface(FeSurface surface, out int[] nodeIds, out int[] elementIds,
                                                            out bool edgeBased)
        {
            edgeBased = false;
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                if (_nodeSets.ContainsKey(surface.CreatedFromNodeSetName))
                {
                    HashSet<int> allNodeSetIds = new HashSet<int>(_nodeSets[surface.CreatedFromNodeSetName].Labels);
                    HashSet<int> visualizationNodeIds = new HashSet<int>();
                    HashSet<int> hashElementIds = new HashSet<int>();
                    // For each node get all elements
                    int elementId;
                    HashSet<int> allFreeEdgeNodeIds = new HashSet<int>();
                    foreach (var entry in _parts)
                    {
                        for (int i = 0; i < entry.Value.Visualization.Cells.Length; i++)
                        {
                            elementId = entry.Value.Visualization.CellIds[i];
                            foreach (var nodeId in entry.Value.Visualization.Cells[i])
                            {
                                if (allNodeSetIds.Contains(nodeId))
                                {
                                    visualizationNodeIds.Add(nodeId);
                                    hashElementIds.Add(elementId);
                                }
                            }
                        }
                        //
                        allFreeEdgeNodeIds.UnionWith(entry.Value.Visualization.GetFreeEdgeNodeIds());
                    }
                    nodeIds = visualizationNodeIds.ToArray();
                    elementIds = hashElementIds.ToArray();
                    //
                    edgeBased = allFreeEdgeNodeIds.Intersect(visualizationNodeIds).Count() == visualizationNodeIds.Count;
                }
                else // return empty sets
                {
                    nodeIds = new int[0];
                    elementIds = new int[0];
                }
            }
            else throw new CaeException("The surface is not created from node set.");
        }
        private void CreateSurfaceFacesFromSelection(int[] surfaceFaceIds, out int[] nodeIds,
                                                     out Dictionary<FeFaceName, List<int>> elementSets,
                                                     out double area, out FeSurfaceFaceTypes surfaceFaceTypes)
        {
            nodeIds = null;
            elementSets = new Dictionary<FeFaceName, List<int>>();
            area = 0;
            //
            int vtkCellId;
            int elementId;
            FeFaceName faceName;
            List<int> elementIds;
            HashSet<int> allNodeIds = new HashSet<int>();
            //
            int[] cell;
            foreach (var faceId in surfaceFaceIds)
            {
                cell = GetCellFromFaceId(faceId, out ElementFaceType elementFaceType, out FeElement element);
                allNodeIds.UnionWith(cell);
                //
                GetElementIdVtkCellIdFromFaceId(faceId, out elementId, out vtkCellId);
                faceName = FeElement.FaceNameFromVtkCellId(vtkCellId);
                //
                area += element.GetArea(faceName, _nodes);
                //
                if (elementSets.TryGetValue(faceName, out elementIds)) elementIds.Add(element.Id);
                else elementSets.Add(faceName, new List<int>() { element.Id });
            }
            nodeIds = allNodeIds.ToArray();
            //
            surfaceFaceTypes = GetSurfaceTypeFromFaceIds(surfaceFaceIds);
        }
        private FeSurfaceFaceTypes GetSurfaceTypeFromFaceIds(IEnumerable<int> surfaceFaceIds)
        {
            FeSurfaceFaceTypes surfaceFaceTypes = FeSurfaceFaceTypes.Unknown;
            if (surfaceFaceIds.Count() > 0)
            {
                HashSet<FeSurfaceFaceTypes> allSurfaceFaceTypes = GetSurfaceFaceTypesFromFaceIds(surfaceFaceIds);
                if (allSurfaceFaceTypes.Count == 1) surfaceFaceTypes = allSurfaceFaceTypes.First();
                else if (allSurfaceFaceTypes.Count > 1)
                    surfaceFaceTypes = allSurfaceFaceTypes.First();
                else throw new NotSupportedException();
            }
            return surfaceFaceTypes;
        }
        private void CreateSurfaceFacesFromNodeSet(FeSurface surface, out int[] nodeIds,
                                                   out Dictionary<FeFaceName, List<int>> elementSets,
                                                   out double area, out FeSurfaceFaceTypes surfaceFaceTypes)
        {
            int[] elementIds;
            bool edgeBased;
            GetNodeAndElementIdsFromNodeSetSurface(surface, out nodeIds, out elementIds, out edgeBased);
            // To speed up the search
            HashSet<int> nodeSetLookUp = new HashSet<int>(nodeIds);
            //
            area = 0;
            FeElement element;
            List<int> elementIdList;
            Dictionary<FeFaceName, double> faces;
            HashSet<int> faceIds = new HashSet<int>();
            elementSets = new Dictionary<FeFaceName, List<int>>();
            //
            foreach (int elementId in elementIds)
            {
                element = _elements[elementId];
                faces = element.GetFaceNamesAndAreasFromNodeSet(nodeSetLookUp, _nodes, edgeBased);
                //
                foreach (var entry in faces)
                {
                    area += entry.Value;
                    //
                    if (elementSets.TryGetValue(entry.Key, out elementIdList)) elementIdList.Add(elementId);
                    else elementSets.Add(entry.Key, new List<int>() { elementId });
                    //
                    faceIds.Add(elementId * 10 + FeElement.VtkCellIdFromFaceName(entry.Key));
                }
            }
            //
            surfaceFaceTypes = FeSurfaceFaceTypes.Unknown;
            if (faceIds.Count > 0)
            {
                HashSet<FeSurfaceFaceTypes> allSurfaceFaceTypes = GetSurfaceFaceTypesFromFaceIds(faceIds);
                if (allSurfaceFaceTypes.Count == 1) surfaceFaceTypes = allSurfaceFaceTypes.First();
                else throw new NotSupportedException();
            }
        }
        public HashSet<FeSurfaceFaceTypes> GetSurfaceFaceTypesFromFaceIds(IEnumerable<int> faceIds)
        {
            int elementId;
            int vtkCellId;
            FeElement element;
            HashSet<FeSurfaceFaceTypes> allSurfaceFaceTypes = new HashSet<FeSurfaceFaceTypes>();
            foreach (var faceId in faceIds)
            {
                GetElementIdVtkCellIdFromFaceId(faceId, out elementId, out vtkCellId);
                element = _elements[elementId];
                //
                if (element is FeElement1D) allSurfaceFaceTypes.Add(FeSurfaceFaceTypes.BeamFaces);
                else if (element is FeElement2D)
                {
                    GetElementIdVtkCellIdFromFaceId(faceId, out elementId, out vtkCellId);
                    if (vtkCellId <= 1) allSurfaceFaceTypes.Add(FeSurfaceFaceTypes.ShellFaces);
                    else allSurfaceFaceTypes.Add(FeSurfaceFaceTypes.ShellEdgeFaces);
                }
                else if (element is FeElement3D) allSurfaceFaceTypes.Add(FeSurfaceFaceTypes.SolidFaces);
                else throw new NotSupportedException();
            }
            return allSurfaceFaceTypes;
        }
        public void GetElementIdVtkCellIdFromFaceId(int faceId, out int elementId, out int vtkCellId)
        {
            elementId = faceId / 10;
            vtkCellId = faceId % 10;
        }
        public int[] GetCellFromFaceId(int faceId, out ElementFaceType elementFaceType, out FeElement element)
        {
            int elementId;
            int vtkCellId;
            GetElementIdVtkCellIdFromFaceId(faceId, out elementId, out vtkCellId);
            //
            FeFaceName faceName = FeElement.FaceNameFromVtkCellId(vtkCellId);
            //
            if (_elements.TryGetValue(elementId, out element))
            {
                if (element is FeElement3D element3D)
                {
                    elementFaceType = ElementFaceType.Face;
                    return element3D.GetVtkCellFromFaceName(faceName);
                }
                else if (element is FeElement2D element2D)    // shell and geometry
                {
                    if (vtkCellId <= 1) elementFaceType = ElementFaceType.Face;
                    else elementFaceType = ElementFaceType.Edge;
                    //
                    return element2D.GetVtkCellFromFaceName(faceName);
                }
                else throw new NotSupportedException();
            }
            else throw new CaeException("The selected face id does not exist.");
        }
        //
        public static string GetNextFreeInternalName<T>(IDictionary<string, T> dictionary, string postFix)
        {
            return dictionary.GetNextNumberedKey(Globals.InternalName, "_" + postFix);
        }
        public static string GetNextFreeSelectionName<T>(IDictionary<string, T> dictionary, string postFix)
        {
            return dictionary.GetNextNumberedKey(Globals.InternalSelectionName, "_" + postFix);
        }
        //
        public FeNodeSet GetSurfaceNodeSet(string surfaceName)
        {
            FeSurface surface;

            if (_surfaces.TryGetValue(surfaceName, out surface)) return _nodeSets[surface.NodeSetName];
            else return null;
        }
        public double GetSurfaceArea(int geometrySurfaceId)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometrySurfaceId);
            BasePart part = GetPartById(itemTypePartIds[2]);
            return part.Visualization.FaceAreas[itemTypePartIds[0]];
        }
        public int[][] GetSurfaceCells(int geometrySurfaceId, out ElementFaceType[] elementFaceTypes)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometrySurfaceId);
            BasePart part = GetPartById(itemTypePartIds[2]);
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            int[] cellIds = part.Visualization.CellIdsByFace[itemTypePartIds[0]];
            int[][] cells = new int[cellIds.Length][];
            elementFaceTypes = new ElementFaceType[cellIds.Length];
            //
            ElementFaceType elementFaceType = ElementFaceType.Face;
            if (geomType == GeometryType.ShellEdgeSurface) elementFaceType = ElementFaceType.Edge;
            else if (geomType == GeometryType.Vertex) elementFaceType = ElementFaceType.Vertex;
            //
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = part.Visualization.Cells[cellIds[i]].ToArray();
                elementFaceTypes[i] = elementFaceType;
            }
            return cells;
        }
        //
        public double[] GetSurfaceCG(string surfaceName)
        {
            FeSurface surface;
            if (_surfaces.TryGetValue(surfaceName, out surface))
            {
                if (surface.Type == FeSurfaceType.Node)
                {
                    return _nodeSets[surface.NodeSetName].CenterOfGravity;
                }
                else if (surface.Type == FeSurfaceType.Element)
                {
                    double area = 0;
                    double[] cg = new double[3];
                    double elArea;
                    double[] elCg;
                    foreach (var entry in surface.ElementFaces)
                    {
                        foreach (var elementId in _elementSets[entry.Value].Labels)
                        {
                            elCg = _elements[elementId].GetFaceCG(entry.Key, _nodes, out elArea);
                            cg[0] += elCg[0] * elArea;
                            cg[1] += elCg[1] * elArea;
                            cg[2] += elCg[2] * elArea;
                            area += elArea;
                        }
                    }
                    cg[0] /= area;
                    cg[1] /= area;
                    cg[2] /= area;
                    //
                    return cg;
                }
                else throw new CaeException("The surface type is not supported.");
            }
            //
            return new double[3];
        }
        public double[] GetSurfaceNormal(string surfaceName)
        {
            FeSurface surface;
            if (_surfaces.TryGetValue(surfaceName, out surface))
            {
                if (surface.Type == FeSurfaceType.Element)
                {
                    double elArea = 0;
                    double[] normal = new double[3];
                    double[] elNormal;
                    foreach (var entry in surface.ElementFaces)
                    {
                        foreach (var elementId in _elementSets[entry.Value].Labels)
                        {
                            GetElementFaceNormalAndArea(elementId, entry.Key, out elNormal, out elArea);
                            normal[0] += elNormal[0] * elArea;
                            normal[1] += elNormal[1] * elArea;
                            normal[2] += elNormal[2] * elArea;
                        }
                    }
                    //
                    double len = Math.Sqrt(Math.Pow(normal[0], 2) + Math.Pow(normal[1], 2) + Math.Pow(normal[2], 2));
                    if (len > 0)
                    {
                        normal[0] /= len;
                        normal[1] /= len;
                        normal[2] /= len;
                        return normal;
                    }
                }
                else throw new CaeException("The surface type is not supported.");
            }
            //
            return new double[3];
        }
        #endregion #################################################################################################################

        #region Extraction  ########################################################################################################
        public bool IsShellElementFrontFaceSelected(int elementId, double[] selectionDirection)
        {
            if (selectionDirection != null)
            {
                FeElement element;
                if (_elements.TryGetValue(elementId, out element))
                {
                    if (element is FeElement2D)
                    {
                        // Front face is S2
                        GetElementFaceNormal(elementId, FeFaceName.S2, out double[] elementNormal);
                        //
                        Vec3D n1 = new Vec3D(selectionDirection);
                        Vec3D n2 = new Vec3D(elementNormal);
                        double cross = Vec3D.DotProduct(n1, n2);
                        //
                        if (cross > 0) return false;
                    }
                }
            }
            return true;
        }
        public int[] GetPartNodeIds(int elementId)
        {
            int partId = _elements[elementId].PartId;
            BasePart part = GetPartById(partId);
            return part.NodeLabels;
        }
        public int[] GetPartElementIds(int elementId)
        {
            int partId = _elements[elementId].PartId;
            BasePart part = GetPartById(partId);
            return part.Labels.ToArray();
        }
        public int[] GetSurfaceElementIds(string surfaceName)
        {
            FeSurface surface;
            HashSet<int> elementIds = new HashSet<int>();
            //
            if (_surfaces.TryGetValue(surfaceName, out surface))
            {
                foreach (var entry in surface.ElementFaces)
                {
                    elementIds.UnionWith(_elementSets[entry.Value].Labels);
                }
            }
            return elementIds.ToArray();
        }
        public int GetCellFaceNodeIdClosestToPoint(double[] point, int[] cellFaceNodeIds)
        {
            double distance;
            double minDistance = double.MaxValue;
            int minId = -1;
            double[] coor;
            //
            for (int i = 0; i < cellFaceNodeIds.Length; i++)
            {
                coor = _nodes[cellFaceNodeIds[i]].Coor;
                distance = Math.Pow(coor[0] - point[0], 2) + Math.Pow(coor[1] - point[1], 2) +
                           Math.Pow(coor[2] - point[2], 2);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minId = i;
                }
            }
            return cellFaceNodeIds[minId];
        }
        public int GetCellFaceNodeIdClosestToPoint(double[] point, int[] cellFaceNodeIds, double[][] cellFaceNodeCoor)
        {
            double distance;
            double minDistance = double.MaxValue;
            int minId = -1;
            double[] coor;
            //
            for (int i = 0; i < cellFaceNodeCoor.Length; i++)
            {
                coor = cellFaceNodeCoor[i];
                distance = Math.Pow(coor[0] - point[0], 2) + Math.Pow(coor[1] - point[1], 2) +
                           Math.Pow(coor[2] - point[2], 2);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minId = i;
                }
            }
            return cellFaceNodeIds[minId];
        }
        public int[][] GetEdgeCells(int elementId, int[] edgeNodeIds)
        {
            BasePart part;
            int partEdgeId;
            return GetEdgeCells(elementId, edgeNodeIds, out part, out partEdgeId);
        }
        public int GetEdgeGeometryIdFromNodeIds(int elementId, int[] edgeNodeIds)
        {
            BasePart part;
            int partEdgeId;
            GetEdgeCells(elementId, edgeNodeIds, out part, out partEdgeId);
            int geometryId = partEdgeId * 100000 + (int)GeometryType.Edge * 10000 + part.PartId;
            return geometryId;
        }
        public int[][] GetEdgeCells(int elementId, int[] edgeNodeIds, out BasePart part, out int edgeId)
        {
            // Get all faces containing at least 1 node id
            // Face id = 10 * elementId + vtkCellId
            int[] faceIds = GetVisualizationFaceIds(edgeNodeIds, new int[] { elementId }, false, false, shellFrontFace: true);
            bool add;
            int[] cell = null;
            FeElement element = null;
            HashSet<int> hashCell = new HashSet<int>();
            HashSet<int> allCells = new HashSet<int>();
            // Find the first face containing all edge node ids and save it to allCells
            foreach (int faceId in faceIds)
            {
                // The same element is always returned, but not the same cell
                cell = GetCellFromFaceId(faceId, out ElementFaceType elementFaceType, out element);
                if (cell.Length < edgeNodeIds.Length) continue;
                //
                hashCell.Clear();
                hashCell.UnionWith(cell);
                allCells.UnionWith(cell);
                add = true;
                for (int i = 0; i < edgeNodeIds.Length; i++)
                {
                    if (!hashCell.Contains(edgeNodeIds[i]))
                    {
                        add = false;
                        break;
                    }
                }
                if (add) break;
            }
            // Find "surface" edge cells that contain edgeNodeIds
            CompareIntArray comparer = new CompareIntArray();
            int edgeCellId;
            int[] edgeCell;
            Dictionary<int[], int> edgeCellEdgeId = new Dictionary<int[], int>(comparer);
            //
            edgeId = -1;
            part = GetPartContainingElementId(elementId);
            if (part == null) return null;
            VisualizationData visualization = part.Visualization;
            HashSet<int> intersection = new HashSet<int>();
            // For each edge
            for (int i = 0; i < visualization.EdgeCellIdsByEdge.Length; i++)
            {
                // For each edge cell
                for (int j = 0; j < visualization.EdgeCellIdsByEdge[i].Length; j++)
                {
                    edgeCellId = visualization.EdgeCellIdsByEdge[i][j];
                    edgeCell = visualization.EdgeCells[edgeCellId].ToArray();
                    //
                    intersection.Clear();
                    intersection.UnionWith(edgeCell);
                    intersection.IntersectWith(edgeNodeIds);
                    //
                    if (intersection.Count > 0) edgeCellEdgeId.Add(edgeCell, i);
                }
            }
            // If the face is connected to more than one "surface" edge, find the one with the most equal nodes
            if (edgeCellEdgeId.Count == 1) edgeId = edgeCellEdgeId.Values.First();
            else if (edgeCellEdgeId.Count > 1)
            {
                int maxEqualNodes = 0;
                int maxEqualCellNodes = 0;
                foreach (var entry in edgeCellEdgeId)
                {
                    intersection.Clear();
                    intersection.UnionWith(edgeNodeIds);
                    intersection.IntersectWith(entry.Key);

                    if (intersection.Count > maxEqualNodes)
                    {
                        maxEqualNodes = intersection.Count;
                        edgeId = entry.Value;
                        //
                        if (intersection.Count == 1)
                        {
                            intersection.Clear();
                            intersection.UnionWith(allCells);
                            intersection.IntersectWith(entry.Key);
                            maxEqualCellNodes = intersection.Count;
                        }
                    }
                    // edgeNodeIds contain only one "surface" edge node
                    else if (intersection.Count == maxEqualNodes && intersection.Count == 1 &&
                             intersection.First() == edgeNodeIds[0])    // first edgeNode is the closest
                    {
                        intersection.Clear();
                        intersection.UnionWith(allCells);
                        intersection.IntersectWith(entry.Key);
                        if (intersection.Count > maxEqualCellNodes)
                        {
                            maxEqualCellNodes = intersection.Count;
                            maxEqualNodes = 1;
                            edgeId = entry.Value;
                        }
                    }
                }
            }
            //
            if (edgeId != -1)
            {
                int[][] edgeCells = new int[visualization.EdgeCellIdsByEdge[edgeId].Length][];
                for (int i = 0; i < edgeCells.Length; i++)
                {
                    edgeCells[i] = visualization.EdgeCells[visualization.EdgeCellIdsByEdge[edgeId][i]];
                }
                return edgeCells;
            }
            else return null;
        }
        public int[] GetEdgeNodeIds(int elementId, int[] edgeNodeIds)
        {
            int[][] edgeCells = GetEdgeCells(elementId, edgeNodeIds);
            if (edgeCells != null)
            {
                HashSet<int> nodeIds = new HashSet<int>();
                foreach (var edgeCell in edgeCells)
                {
                    nodeIds.UnionWith(edgeCell);
                }
                return nodeIds.ToArray();
            }
            else return null;
        }
        public bool GetFaceId(int elementId, int[] cellFaceGlobalNodeIds, out BasePart part, out int faceId)
        {
            part = null;
            faceId = -1;
            if (elementId < 0) return false;
            //
            int partId = _elements[elementId].PartId;
            part = GetPartById(partId);
            // Find the picked surface cell
            int[][] cells = part.Visualization.Cells;
            int[] cellIds = part.Visualization.CellIds;
            if (cells == null) return false;
            //
            int faceCellId = -1;
            int numberOfSameNodes = cellFaceGlobalNodeIds.Length;
            HashSet<int> faceNodeIds = new HashSet<int>(cellFaceGlobalNodeIds);
            int count;
            for (int i = 0; i < cells.Length; i++)
            {
                if (part.Visualization.CellIds[i] == elementId)     // is this one of element cells
                {
                    count = 0;
                    for (int j = 0; j < cells[i].Length; j++)
                    {
                        if (faceNodeIds.Contains(cells[i][j]))
                        {
                            count++;
                            if (count == numberOfSameNodes)
                            {
                                faceCellId = i;
                                break;
                            }
                        }
                    }
                }
                if (faceCellId != -1) break;
            }
            if (faceCellId == -1) return false;
            //
            for (int i = 0; i < part.Visualization.CellIdsByFace.Length; i++)
            {
                if (part.Visualization.CellIdsByFace[i].Contains(faceCellId))
                {
                    faceId = i;
                    break;
                }
            }
            //
            return true;
        }
        public int[] GetSurfaceNodeIds(int elementId, int[] cellFaceGlobalNodeIds)
        {
            BasePart part;
            int faceId;
            if (GetFaceId(elementId, cellFaceGlobalNodeIds, out part, out faceId))
            {
                int[][] cells = part.Visualization.Cells;
                HashSet<int> surfaceNodes = new HashSet<int>();
                if (faceId != -1)
                {
                    foreach (var surfaceCellId in part.Visualization.CellIdsByFace[faceId])
                    {
                        surfaceNodes.UnionWith(cells[surfaceCellId]);
                    }
                }
                return surfaceNodes.ToArray();
            }
            else return null;
        }
        public int[] GetEdgeByAngleNodeIds(int elementId, int[] edgeGlobalNodeIds, double angle)
        {
            int partId = _elements[elementId].PartId;
            BasePart part = GetPartById(partId);
            int[][] cells = part.Visualization.Cells;
            // Build node neighbours map
            Dictionary<int, HashSet<int>> nodeNeighbours = new Dictionary<int, HashSet<int>>();
            HashSet<int> neighbours;
            //
            int n1Id, n2Id;
            int[] lookUp = null;
            int[] lookUp3 = new int[] { 0, 1, 2, 0 };
            int[] lookUp4 = new int[] { 0, 1, 2, 3, 0 };
            int[] lookUp6 = new int[] { 0, 3, 1, 4, 2, 5, 0 };
            int[] lookUp8 = new int[] { 0, 4, 1, 5, 2, 6, 3, 7, 0 };
            //
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Length == 3) lookUp = lookUp3;
                else if (cells[i].Length == 4) lookUp = lookUp4;
                else if (cells[i].Length == 6) lookUp = lookUp6;
                else if (cells[i].Length == 8) lookUp = lookUp8;
                else throw new NotSupportedException();
                //
                for (int j = 0; j < cells[i].Length; j++)
                {
                    n1Id = cells[i][lookUp[j]];
                    n2Id = cells[i][lookUp[j + 1]];
                    //
                    if (nodeNeighbours.TryGetValue(n1Id, out neighbours)) neighbours.Add(n2Id);
                    else nodeNeighbours.Add(n1Id, new HashSet<int>() { n2Id });
                    //
                    if (nodeNeighbours.TryGetValue(n2Id, out neighbours)) neighbours.Add(n1Id);
                    else nodeNeighbours.Add(n2Id, new HashSet<int>() { n1Id });
                }
            }
            // Spread
            angle *= Math.PI / 180;
            int n3Id;
            HashSet<int> allNodeIds = new HashSet<int>(edgeGlobalNodeIds);
            // Go forward
            n1Id = edgeGlobalNodeIds[0];
            n2Id = edgeGlobalNodeIds[edgeGlobalNodeIds.Length - 1]; // for quadratic mesh it is node id 2
            while (true)
            {
                n3Id = GetNextEdgeNodeId(n1Id, n2Id, nodeNeighbours[n2Id], angle);
                if (n3Id >= 0 && !allNodeIds.Contains(n3Id))
                {
                    allNodeIds.Add(n3Id);
                    n1Id = n2Id;
                    n2Id = n3Id;
                }
                else break;
            }
            // Go backward
            n1Id = edgeGlobalNodeIds[edgeGlobalNodeIds.Length - 1];
            n2Id = edgeGlobalNodeIds[0];
            while (true)
            {
                n3Id = GetNextEdgeNodeId(n1Id, n2Id, nodeNeighbours[n2Id], angle);
                if (n3Id >= 0 && !allNodeIds.Contains(n3Id))
                {
                    allNodeIds.Add(n3Id);
                    n1Id = n2Id;
                    n2Id = n3Id;
                }
                else break;
            }
            //
            return allNodeIds.ToArray();
        }
        public int[] GetSurfaceByAngleNodeIds(int elementId, int[] cellFaceGlobalNodeIds, double angle)
        {
            int partId = _elements[elementId].PartId;
            BasePart part = GetPartById(partId);
            // Find the picked surface cell
            int[][] cells = part.Visualization.Cells;
            int[] cellIds = part.Visualization.CellIds;
            if (cells == null) return null;
            //
            int surfaceCellId = -1;
            int numberOfSameNodes = cellFaceGlobalNodeIds.Length;
            HashSet<int> faceNodeIds = new HashSet<int>(cellFaceGlobalNodeIds);
            int count;
            for (int i = 0; i < cells.Length; i++)
            {
                if (part.Visualization.CellIds[i] == elementId)     // is this one of element cells
                {
                    count = 0;
                    for (int j = 0; j < cells[i].Length; j++)
                    {
                        if (faceNodeIds.Contains(cells[i][j]))
                        {
                            count++;
                            if (count == numberOfSameNodes)
                            {
                                surfaceCellId = i;
                                break;
                            }
                        }
                    }
                }
                if (surfaceCellId != -1) break;
            }
            if (surfaceCellId == -1) return null;
            // Spread
            int[][] allCellNeighbours = part.Visualization.CellNeighboursOverCellEdge;
            HashSet<int> surfaceCellIds = new HashSet<int>();
            HashSet<int> notVisitedCellIds = new HashSet<int>();
            surfaceCellIds.Add(surfaceCellId);
            notVisitedCellIds.Add(surfaceCellId);
            //
            double alpha;
            angle *= Math.PI / 180;
            HashSet<int> newSurfaceCellIds = new HashSet<int>();
            do
            {
                // Find new surface candidates
                newSurfaceCellIds.Clear();
                foreach (var notVisitedCellId in notVisitedCellIds)
                {
                    foreach (var neighbourId in allCellNeighbours[notVisitedCellId])
                    {
                        if (neighbourId == -1) continue;
                        //
                        if (!surfaceCellIds.Contains(neighbourId) && !newSurfaceCellIds.Contains(neighbourId))
                        {
                            alpha = ComputeAngleInRadFromCellIndices(cells[notVisitedCellId], cells[neighbourId]);
                            if (alpha <= angle)
                            {
                                newSurfaceCellIds.Add(neighbourId);
                            }
                        }
                    }
                }
                // Add new surface candidates to surface and to cells to visit
                notVisitedCellIds.Clear();
                foreach (var newSurfaceCellId in newSurfaceCellIds)
                {
                    surfaceCellIds.Add(newSurfaceCellId);
                    notVisitedCellIds.Add(newSurfaceCellId);
                }
            }
            while (newSurfaceCellIds.Count > 0);
            //
            HashSet<int> surfaceNodes = new HashSet<int>();
            foreach (var cellId in surfaceCellIds) surfaceNodes.UnionWith(cells[cellId]);
            //
            return surfaceNodes.ToArray();
        }
        public int[] GetElementIdsFromNodeIds(int[] nodeIds, bool containsEdge, bool containsFace, bool containsElement,
                                              int partId = -1)
        {
            if (nodeIds == null) return null;
            //
            HashSet<int> allNodeIds = new HashSet<int>(nodeIds);
            HashSet<int> allElementIds = new HashSet<int>();
            //
            bool parabolic;
            int minNumberOfNodesToContain;
            int countNodes;
            FeElement element;
            int vtkType;
            bool added;
            int[][] vtkCells;
            //
            foreach (var entry in _elements)
            {
                element = entry.Value;
                // Filter the elements by part id
                if (partId > -1 && element.PartId != partId) continue;
                //
                vtkType = element.GetVtkCellType();
                parabolic = FeElement.IsParabolic(element);
                //
                minNumberOfNodesToContain = 1;
                //
                if (containsEdge)
                {
                    minNumberOfNodesToContain = 2;
                }
                else if (containsFace)
                {
                    vtkCells = null;
                    if (element is LinearTriangleElement || element is LinearTetraElement)
                        minNumberOfNodesToContain = 3;
                    else if (element is ParabolicTriangleElement || element is ParabolicTetraElement)
                        minNumberOfNodesToContain = 6;
                    else if (element is LinearQuadrilateralElement || element is LinearHexaElement)
                        minNumberOfNodesToContain = 4;
                    else if (element is ParabolicQuadrilateralElement || element is ParabolicHexaElement)
                        minNumberOfNodesToContain = 8;
                    else if (element is LinearWedgeElement lwe) vtkCells = lwe.GetAllVtkCells();
                    else if (element is ParabolicWedgeElement pwe) vtkCells = pwe.GetAllVtkCells();
                    else throw new NotSupportedException();
                    //
                    if (vtkCells != null)
                    {
                        added = false;
                        for (int i = 0; i < vtkCells.Length; i++)
                        {
                            if (allNodeIds.Intersect(vtkCells[i]).Count() == vtkCells[i].Length)
                            {
                                allElementIds.Add(entry.Key);
                                added = true;
                                break;
                            }
                        }
                        if (added) continue;  // for each element
                    }
                }
                else if (containsElement) minNumberOfNodesToContain = element.NodeIds.Length;
                //
                countNodes = 0;
                for (int i = 0; i < element.NodeIds.Length; i++)
                {
                    if (allNodeIds.Contains(element.NodeIds[i]))
                    {
                        countNodes++;
                    }
                    if (countNodes >= minNumberOfNodesToContain) break;
                }
                //
                if (countNodes >= minNumberOfNodesToContain)
                    allElementIds.Add(entry.Key);
            }
            // Return
            return allElementIds.ToArray();
        }
        public int[] GetVisualizationFaceIds(int[] nodeIds, int[] elementIds, bool containsEdge, bool containsFace,
                                             bool shellFrontFace)
        {
            if (containsEdge && containsFace) throw new NotSupportedException();
            //
            HashSet<int> hashElementIds = new HashSet<int>(elementIds);
            HashSet<int> hashNodeIds;
            HashSet<int> globalVisualizationFaceIds = new HashSet<int>();
            // Get all visualization cell ids
            int elementId;
            int vtkCellId;
            int[] vtkCellIds;
            int[] cell;
            int minNumberOfNodes;
            FeElement element;
            HashSet<int> edgeNodes = new HashSet<int>();
            HashSet<int> freeEdgeNodeIds = new HashSet<int>();
            //
            if (nodeIds != null && nodeIds.Length > 0) hashNodeIds = new HashSet<int>(nodeIds);
            else return new int[0];
            //
            foreach (var entry in _parts)
            {
                if (entry.Value is CompoundGeometryPart) continue;
                //
                for (int i = 0; i < entry.Value.Visualization.CellIds.Length; i++)
                {
                    elementId = entry.Value.Visualization.CellIds[i];
                    //
                    if (hashElementIds.Contains(elementId))
                    {
                        cell = entry.Value.Visualization.Cells[i];          // these are surface cells
                        //
                        if (containsEdge)
                        {
                            if (cell.Length <= 4) minNumberOfNodes = 2;     // linear
                            else minNumberOfNodes = 3;                      // parabolic
                            // Get all free edge node ids
                            freeEdgeNodeIds = entry.Value.Visualization.GetFreeEdgeNodeIds();
                        }
                        else if (containsFace)
                        {
                            if (cell.Length <= 4) minNumberOfNodes = 3;     // linear
                            else minNumberOfNodes = 4;                      // parabolic
                        }
                        else
                        {
                            if (hashNodeIds.Count > 0) minNumberOfNodes = 1;
                            else minNumberOfNodes = -1;
                        }
                        //
                        edgeNodes.Clear();
                        foreach (int nodeId in cell)                        // maybe try the Intersect !!!
                        {
                            if (hashNodeIds.Contains(nodeId) && !(containsEdge && !freeEdgeNodeIds.Contains(nodeId)))
                                edgeNodes.Add(nodeId);
                        }
                        //
                        if (edgeNodes.Count >= minNumberOfNodes)
                        {
                            element = _elements[elementId];
                            //
                            if (element is FeElement3D element3D)           // solid
                            {
                                vtkCellId = element3D.GetVtkCellIdFromCell(cell);
                                if (vtkCellId != -1) globalVisualizationFaceIds.Add(10 * elementId + vtkCellId);
                                else throw new Exception();
                            }
                            else if (element is FeElement2D element2D)      // shell and geometry
                            {
                                if (containsEdge) // for shell edge surface
                                {
                                    vtkCellIds = element2D.GetAllVtkCellIdsFromNodeIds(edgeNodes);
                                    //
                                    if (vtkCellIds.Length > 0)
                                    {
                                        foreach (var edgeVtkCellId in vtkCellIds)
                                        {
                                            globalVisualizationFaceIds.Add(10 * elementId + edgeVtkCellId);
                                        }
                                    }
                                    else throw new Exception();
                                }
                                else
                                {
                                    if (shellFrontFace) vtkCellId = 1;
                                    else vtkCellId = 0;
                                    globalVisualizationFaceIds.Add(10 * elementId + vtkCellId);
                                }
                            }
                            else throw new NotSupportedException();
                        }
                    }
                }
            }
            return globalVisualizationFaceIds.ToArray();
        }
        public int[] GetVisualizationFaceIds(int geometrySurfaceId, int partId, bool shellFrontFace)
        {
            BasePart part = GetPartById(partId);
            if (part == null) return new int[0];
            int[][] cells = part.Visualization.Cells;
            int[] cellIds = part.Visualization.CellIds;
            HashSet<int> globalVisualizationFaceIds = new HashSet<int>();
            //
            if (geometrySurfaceId < part.Visualization.CellIdsByFace.Length)
            {
                int[] cellFaceIds = part.Visualization.CellIdsByFace[geometrySurfaceId];
                //
                int vtkCellId;
                int elementId;
                FeElement element;
                //
                for (int i = 0; i < cellFaceIds.Length; i++)
                {
                    elementId = cellIds[cellFaceIds[i]];
                    element = _elements[elementId];
                    if (element is FeElement3D element3D)
                    {
                        vtkCellId = element3D.GetVtkCellIdFromCell(cells[cellFaceIds[i]]);
                        if (vtkCellId != -1) globalVisualizationFaceIds.Add(10 * elementId + vtkCellId);
                        else throw new Exception();
                    }
                    else if (element is FeElement2D) // shell nad geometry
                    {
                        vtkCellId = shellFrontFace ? 1 : 0;
                        globalVisualizationFaceIds.Add(10 * elementId + vtkCellId);
                    }
                    else throw new NotSupportedException();
                }
            }
            return globalVisualizationFaceIds.ToArray();
        }
        public int[] GetVisualizationFaceIdsFromShellEdge(int geometryEdgeId, int partId)
        {
            BasePart part = GetPartById(partId);
            if (part == null || part.PartType != PartType.Shell) return new int[0];
            //
            int count = 0;
            int faceId = -1;
            // Find the face id of the geometry edge
            for (int i = 0; i < part.Visualization.FaceEdgeIds.Length; i++)
            {
                foreach (var edgeId in part.Visualization.FaceEdgeIds[i])
                {
                    if (edgeId == geometryEdgeId)
                    {
                        faceId = i;
                        count++;
                    }
                    if (count > 1) return new int[0];
                }
            }
            //
            HashSet<int> hashNodeIds = new HashSet<int>();
            int[][] cells = part.Visualization.Cells;
            int[] cellIds = part.Visualization.CellIds;
            int[][] edgeCells = part.Visualization.EdgeCells;
            int[] edgeCellIds = part.Visualization.EdgeCellIdsByEdge[geometryEdgeId];
            // Collect all edge node ids
            for (int i = 0; i < edgeCellIds.Length; i++) hashNodeIds.UnionWith(edgeCells[edgeCellIds[i]]);
            //            
            int[] cell;
            int elementId;
            int[] vtkCellIds;
            FeElement2D element2D;
            int minNumberOfNodes;
            HashSet<int> globalVisualizationFaceIds = new HashSet<int>();
            HashSet<int> edgeNodes = new HashSet<int>();
            // For each cell on the edge face
            foreach (var visCellId in part.Visualization.CellIdsByFace[faceId])
            {
                cell = cells[visCellId];
                if (cell.Length <= 4) minNumberOfNodes = 2;
                else minNumberOfNodes = 3;
                // Get all cell nodes that are on the edge
                edgeNodes.Clear();
                foreach (int nodeId in cell)
                {
                    if (hashNodeIds.Contains(nodeId)) edgeNodes.Add(nodeId);
                }
                //
                if (edgeNodes.Count >= minNumberOfNodes)
                {
                    elementId = cellIds[visCellId];
                    element2D = (FeElement2D)_elements[elementId];
                    vtkCellIds = element2D.GetAllVtkCellIdsFromNodeIds(edgeNodes);
                    //
                    if (vtkCellIds.Length > 0)
                    {
                        foreach (var vtkCellId in vtkCellIds)
                        {
                            globalVisualizationFaceIds.Add(10 * elementId + vtkCellId);
                        }
                    }
                    else throw new Exception();
                }
            }
            //
            return globalVisualizationFaceIds.ToArray();
        }
        public int[] GetVisualizationFaceIds(string partName, bool shellFrontFace)
        {
            // Get all visualization cell ids
            int elementId;
            int vtkCellId;
            int[] cell;
            FeElement element;
            BasePart part;
            HashSet<int> visualizationFaceIds = new HashSet<int>();
            //
            if (_parts.TryGetValue(partName, out part))
            {
                for (int i = 0; i < part.Visualization.CellIds.Length; i++)
                {
                    cell = part.Visualization.Cells[i];   // these are surface cells
                    elementId = part.Visualization.CellIds[i];
                    element = _elements[elementId];
                    if (element is FeElement3D element3D)
                    {
                        vtkCellId = element3D.GetVtkCellIdFromCell(cell);
                        if (vtkCellId != -1) visualizationFaceIds.Add(10 * elementId + vtkCellId);
                        else throw new Exception();
                    }
                    else if (element is FeElement2D) // shell nad geometry
                    {
                        vtkCellId = shellFrontFace ? 1 : 0;
                        visualizationFaceIds.Add(10 * elementId + vtkCellId);
                    }
                    else throw new NotSupportedException();
                }
            }
            //
            return visualizationFaceIds.ToArray();
        }
        public int[] GetVisibleVisualizationFaceIds(bool shellFrontFace = true)
        {
            // Get all visualization cell ids
            int elementId;
            int vtkCellId;
            int[] cell;
            FeElement element;
            HashSet<int> visualizationFaceIds = new HashSet<int>();
            //
            foreach (var entry in _parts)
            {
                if (entry.Value.Visible)
                {
                    for (int i = 0; i < entry.Value.Visualization.CellIds.Length; i++)
                    {
                        cell = entry.Value.Visualization.Cells[i];   // these are surface cells
                        elementId = entry.Value.Visualization.CellIds[i];
                        element = _elements[elementId];
                        if (element is FeElement3D element3D)
                        {
                            vtkCellId = element3D.GetVtkCellIdFromCell(cell);
                            if (vtkCellId != -1) visualizationFaceIds.Add(10 * elementId + vtkCellId);
                            else throw new Exception();
                        }
                        else if (element is FeElement2D) // shell nad geometry
                        {
                            vtkCellId = shellFrontFace ? 1 : 0;
                            visualizationFaceIds.Add(10 * elementId + vtkCellId);
                        }
                        else throw new NotSupportedException();
                    }
                }
            }
            return visualizationFaceIds.ToArray();
        }

        // Get geometry ids
        public int GetGeometryIdByPrecision(double[] point, int elementId, int[] cellFaceNodeIds, bool shellFrontFace,
                                            double precision)
        {
            if (precision <= 0) throw new NotSupportedException();
            // GeometryId = itemId * 100000 + typeId * 10000 + partId
            int itemId = -1;
            int typeId = (int)GeometryType.Unknown;
            int partId = -1;
            int geometryId;
            //
            BasePart part;
            int faceId;
            if (GetFaceId(elementId, cellFaceNodeIds, out part, out faceId))
            {
                partId = part.PartId;
                VisualizationData visualization = part.Visualization;
                int vertexId = -1;
                int edgeId = -1;
                double vertexDist;
                double edgeDist;
                // Get closest edge distance
                edgeDist = PointToClosestFaceEdgeDistance(point, visualization, faceId, out edgeId);
                // Check if any edge was found (for models without edges)
                if (edgeId != -1) vertexDist = PointToClosestEdgeVertexDistance(point, visualization, edgeId, out vertexId);
                else vertexDist = double.MaxValue;
                //
                double edgeDelta = precision;
                double faceDelta = precision;
                //
                if (vertexDist < edgeDelta)
                {
                    typeId = (int)GeometryType.Vertex;
                    itemId = vertexId;
                }
                else if (edgeDist < faceDelta)
                {
                    // Shell edge can be a shell edge surface
                    if (_elements[elementId] is FeElement2D) typeId = (int)GeometryType.ShellEdgeSurface;
                    else typeId = (int)GeometryType.Edge;
                    //
                    itemId = edgeId;
                }
                else
                {
                    // Surfaces
                    if (part.PartType == PartType.Shell)
                        typeId = shellFrontFace ? (int)GeometryType.ShellFrontSurface : (int)GeometryType.ShellBackSurface;
                    else if (part.PartType == PartType.SolidAsShell)
                        typeId = (int)GeometryType.ShellFrontSurface;
                    else if (part.PartType == PartType.Solid)
                        typeId = (int)GeometryType.SolidSurface;
                    else throw new NotSupportedException();
                    //
                    itemId = faceId;
                }
            }
            //
            geometryId = itemId * 100000 + typeId * 10000 + partId;
            return geometryId;
        }
        public int GetGeometryVertexIdByPrecision(double[] point, int elementId, int[] cellFaceNodeIds, double precision)
        {
            if (precision <= 0) throw new NotSupportedException();
            // GeometryId = itemId * 100000 + typeId * 10000 + partId
            int itemId = -1;
            int typeId = (int)GeometryType.Unknown;
            int partId = -1;
            int geometryId;
            //
            BasePart part;
            int faceId;
            if (GetFaceId(elementId, cellFaceNodeIds, out part, out faceId))
            {
                partId = part.PartId;
                VisualizationData visualization = part.Visualization;
                int vertexId;
                int edgeId;
                // Get closest edge distance
                double edgeDist = PointToClosestFaceEdgeDistance(point, visualization, faceId, out edgeId);
                double vertexDist = PointToClosestEdgeVertexDistance(point, visualization, edgeId, out vertexId);
                //
                if (vertexDist < 2 * precision)
                {
                    typeId = (int)GeometryType.Vertex;
                    itemId = vertexId;
                }
            }
            //
            geometryId = itemId * 100000 + typeId * 10000 + partId;
            return geometryId;
        }
        public int[] GetGeometryIds(int[] nodeIds, int[] elementIds)
        {
            // geometryId = itemId * 100000 + typeId * 10000 + partId       
            int itemId = -1;
            int typeId = (int)GeometryType.Unknown;
            int partId = -1;
            HashSet<int> geometryIds = new HashSet<int>();
            VisualizationData visualization;
            HashSet<int> selectedNodes = new HashSet<int>(nodeIds);
            HashSet<int> nodesToRemove = new HashSet<int>();
            Dictionary<int, HashSet<int>> nodeIdsByItems;
            //
            foreach (var entry in _parts)
            {
                if (entry.Value is CompoundGeometryPart) continue;
                //
                partId = entry.Value.PartId;
                visualization = entry.Value.Visualization;
                // Surfaces
                if (entry.Value.PartType == PartType.Shell) typeId = (int)GeometryType.ShellFrontSurface;
                else if (entry.Value.PartType == PartType.SolidAsShell) typeId = (int)GeometryType.ShellFrontSurface;
                else if (entry.Value.PartType == PartType.Solid) typeId = (int)GeometryType.SolidSurface;
                else throw new NotSupportedException();
                //
                nodesToRemove.Clear();
                nodeIdsByItems = visualization.GetNodeIdsBySurfaces();
                foreach (var surfaceEntry in nodeIdsByItems)
                {
                    // Check for one first - speed?
                    if (selectedNodes.Count >= surfaceEntry.Value.Count && selectedNodes.Contains(surfaceEntry.Value.First()))
                    {
                        if (surfaceEntry.Value.IsSubsetOf(selectedNodes))
                        {
                            itemId = surfaceEntry.Key;
                            geometryIds.Add(itemId * 100000 + typeId * 10000 + partId);
                            nodesToRemove.UnionWith(surfaceEntry.Value);
                            
                        }
                    }
                }
                selectedNodes.ExceptWith(nodesToRemove);
                // Edges
                typeId = (int)GeometryType.Edge;
                nodesToRemove.Clear();
                nodeIdsByItems = visualization.GetNodeIdsByEdges();
                foreach (var edgeEntry in nodeIdsByItems)
                {
                    // Check for one first - speed?
                    if (selectedNodes.Count >= edgeEntry.Value.Count && selectedNodes.Contains(edgeEntry.Value.First()))
                    {
                        if (edgeEntry.Value.IsSubsetOf(selectedNodes))
                        {
                            itemId = edgeEntry.Key;
                            geometryIds.Add(itemId * 100000 + typeId * 10000 + partId);
                            nodesToRemove.UnionWith(edgeEntry.Value);                            
                        }
                    }
                }
                selectedNodes.ExceptWith(nodesToRemove);
                // Vertices
                typeId = (int)GeometryType.Vertex;
                for (int i = 0; i < visualization.VertexNodeIds.Length; i++)
                {
                    if (selectedNodes.Contains(visualization.VertexNodeIds[i]))
                    {
                        itemId = i;
                        geometryIds.Add(itemId * 100000 + typeId * 10000 + partId);
                    }
                }
            }
            return geometryIds.ToArray();
        }
        public int[] GetGeometryIds2(int[] elementIds)
        {
            // geometryId = itemId * 100000 + typeId * 10000 + partId       
            int itemId = -1;
            int typeId = (int)GeometryType.Unknown;
            int partId = -1;
            int prevCount;
            HashSet<int> geometryIds = new HashSet<int>();
            VisualizationData visualization;
            HashSet<int> selectedElements = new HashSet<int>(elementIds);
            Dictionary<int, HashSet<int>> elementIdsBySurfaces;
            //
            foreach (var entry in _parts)
            {
                if (entry.Value is CompoundGeometryPart) continue;
                //
                partId = entry.Value.PartId;
                visualization = entry.Value.Visualization;
                // Surfaces
                if (entry.Value.PartType == PartType.Shell)
                    typeId = (int)GeometryType.ShellFrontSurface;
                else if (entry.Value.PartType == PartType.SolidAsShell)
                    typeId = (int)GeometryType.ShellFrontSurface;
                else if (entry.Value.PartType == PartType.Solid)
                    typeId = (int)GeometryType.SolidSurface;
                else throw new NotSupportedException();
                //
                elementIdsBySurfaces = visualization.GetElementIdsBySurfaces();
                //
                foreach (var elementIdsEntry in elementIdsBySurfaces)
                {
                    prevCount = selectedElements.Count;
                    selectedElements.ExceptWith(elementIdsEntry.Value);
                    if (prevCount != selectedElements.Count())
                    {
                        itemId = elementIdsEntry.Key;
                        geometryIds.Add(itemId * 100000 + typeId * 10000 + partId);
                        // Add faces of the shell back face
                        if (entry.Value.PartType == PartType.Shell && typeId == (int)GeometryType.ShellFrontSurface)
                        {
                            geometryIds.Add(itemId * 100000 + (int)GeometryType.ShellBackSurface * 10000 + partId);
                        }
                    }
                }
            }
            return geometryIds.ToArray();
        }
        private double PointToClosestFaceEdgeDistance(double[] point, VisualizationData visualization, int faceId, out int closestEdgeId)
        {
            int currEdgeId;
            int edgeIdMin = -1;
            double minDist = double.MaxValue;
            double dist;
            // For each face edge
            for (int i = 0; i < visualization.FaceEdgeIds[faceId].Length; i++)
            {
                currEdgeId = visualization.FaceEdgeIds[faceId][i];
                dist = PointToFaceEdgeDistance(point, visualization, currEdgeId);
                if (dist < minDist)
                {
                    minDist = dist;
                    edgeIdMin = currEdgeId;
                }
            }
            closestEdgeId = edgeIdMin;
            return minDist;
        }
        private double PointToFaceEdgeDistance(double[] point, VisualizationData visualization, int edgeId)
        {
            double min = double.MaxValue;
            double dist = -1;
            int edgeCellId;
            int[] nodeIds;
            // For each edge cell
            for (int i = 0; i < visualization.EdgeCellIdsByEdge[edgeId].Length; i++)
            {
                edgeCellId = visualization.EdgeCellIdsByEdge[edgeId][i];
                nodeIds = visualization.EdgeCells[edgeCellId];
                // Ignore the middle node on the parabolic edge
                dist = Geometry.PointToSegmentDistance(point, _nodes[nodeIds[0]].Coor, _nodes[nodeIds[1]].Coor);
                if (dist < min) min = dist;
            }
            return min;
        }
        private double PointToClosestEdgeVertexDistance(double[] point, VisualizationData visualization, int edgeId, out int vertexId)
        {
            int edgeCellId;
            int node1Id;
            int node2Id;
            edgeCellId = visualization.EdgeCellIdsByEdge[edgeId][0];
            node1Id = visualization.EdgeCells[edgeCellId][0];
            edgeCellId = visualization.EdgeCellIdsByEdge[edgeId][visualization.EdgeCellIdsByEdge[edgeId].Length - 1];
            node2Id = visualization.EdgeCells[edgeCellId][1];
            // Compute distances
            Vec3D p = new Vec3D(point);
            Vec3D v1 = new Vec3D(_nodes[node1Id].Coor);
            Vec3D v2 = new Vec3D(_nodes[node2Id].Coor);
            double d1 = (v1 - p).Len;
            double d2 = (v2 - p).Len;
            // Find smallest distance
            double minDist;
            double minNodeId;
            if (d1 < d2)
            {
                minDist = d1;
                minNodeId = node1Id;
            }
            else
            {
                minDist = d2;
                minNodeId = node2Id;
            }
            // Find vertex id
            vertexId = -1;
            for (int i = 0; i < visualization.VertexNodeIds.Length; i++)
            {
                if (visualization.VertexNodeIds[i] == minNodeId)
                {
                    vertexId = i;
                    break;
                }
            }
            return minDist;
        }
        //
        public int[] GetGeometryEdgeIdsByAngle(double[] point, int elementId, int[] edgeNodeIds, int[] cellFaceNodeIds,
                                               double angle, bool shellEdgeFace)
        {
            BasePart part;
            int faceId;
            // First find a face
            if (!GetFaceId(elementId, cellFaceNodeIds, out part, out faceId)) return new int[0];
            //
            int partEdgeId;
            VisualizationData visualization = part.Visualization;
            // Check for models with no model edges
            if (visualization.EdgeCells.Length == 0) return new int[0];
            // Get closest edge
            PointToClosestFaceEdgeDistance(point, visualization, faceId, out partEdgeId);
            // Get edge cell ids from edge id
            HashSet<int> allEdgeIds = new HashSet<int>();
            int[] edgeCellIds = visualization.EdgeCellIdsByEdge[partEdgeId];
            int[][] edgeCells = new int[edgeCellIds.Length][];
            for (int i = 0; i < edgeCells.Length; i++) edgeCells[i] = visualization.EdgeCells[edgeCellIds[i]];
            if (edgeCells == null) return new int[0];
            //
            allEdgeIds.Add(partEdgeId);
            // Get all edge ids connected to the vertex (node)
            int node1Id;
            int node2Id;
            Dictionary<int, HashSet<int>> nodeEdgeIds = new Dictionary<int, HashSet<int>>();
            HashSet<int> edgeIds;
            for (int i = 0; i < visualization.EdgeCellIdsByEdge.Length; i++)
            {
                edgeCellIds = visualization.EdgeCellIdsByEdge[i];
                // Get the first node of the first edge cell
                node1Id = visualization.EdgeCells[edgeCellIds[0]][0];
                // Get the last node of the last edge cell
                node2Id = visualization.EdgeCells[edgeCellIds[edgeCellIds.Length - 1]][1];
                //
                if (nodeEdgeIds.TryGetValue(node1Id, out edgeIds)) edgeIds.Add(i);
                else nodeEdgeIds.Add(node1Id, new HashSet<int>() { i });
                //
                if (nodeEdgeIds.TryGetValue(node2Id, out edgeIds)) edgeIds.Add(i);
                else nodeEdgeIds.Add(node2Id, new HashSet<int>() { i });
            }
            // Spread
            angle *= Math.PI / 180;
            // Go forward
            HashSet<int> nodes = new HashSet<int>();
            node1Id = edgeCells[0][0];
            int edge1Id = partEdgeId;
            int edge2Id;
            while (true)
            {
                // An attempt to prevent multiple loops
                if (nodes.Contains(node1Id)) break;
                nodes.Add(node1Id);
                // This is computed on undeformed mesh
                GetNextEdgeAndNodeId(edge1Id, node1Id, nodeEdgeIds[node1Id], angle, visualization, out edge2Id, out node2Id);
                if (edge2Id >= 0 && !allEdgeIds.Contains(edge2Id))
                {
                    allEdgeIds.Add(edge2Id);
                    edge1Id = edge2Id;
                    node1Id = node2Id;
                }
                else break;
            }
            // Go backward
            node1Id = edgeCells[edgeCells.Length - 1][1];
            edge1Id = partEdgeId;
            while (true)
            {
                // An attempt to prevent multiple loops
                if (nodes.Contains(node1Id)) break;
                nodes.Add(node1Id);
                // This is computed on undeformed mesh
                GetNextEdgeAndNodeId(edge1Id, node1Id, nodeEdgeIds[node1Id], angle, visualization, out edge2Id, out node2Id);
                if (edge2Id >= 0 && !allEdgeIds.Contains(edge2Id))
                {
                    allEdgeIds.Add(edge2Id);
                    edge1Id = edge2Id;
                    node1Id = node2Id;
                }
                else break;
            }
            // Get geometry ids                                             
            int typeId = shellEdgeFace ? (int)GeometryType.ShellEdgeSurface : (int)GeometryType.Edge;
            int partId = part.PartId;
            //
            int count = 0;
            int[] geometryIds = new int[allEdgeIds.Count];
            foreach (int edgeId in allEdgeIds)
            {
                geometryIds[count++] = edgeId * 100000 + typeId * 10000 + partId;
            }
            return geometryIds;
        }
        public int[] GetGeometrySurfaceIdsByAngle(int elementId, int[] cellFaceNodeIds, bool shellFrontFace, double angle)
        {
            BasePart part;
            int faceId;
            if (GetFaceId(elementId, cellFaceNodeIds, out part, out faceId))
            {
                int[][] cells = part.Visualization.Cells;
                int count = 0;
                int[] faceIdOfCells = new int[cells.Length];
                // Create array of face ids for all cells
                foreach (int[] faceCellIds in part.Visualization.CellIdsByFace)
                {
                    foreach (int cellId in faceCellIds)
                    {
                        faceIdOfCells[cellId] = count;
                    }
                    count++;
                }
                //
                int cell1Id;
                int cell2Id;
                int face1Id;
                int face2Id;
                int[] edgeHash;
                double alpha;
                double[] minAngle;  // Array for easyier/faster assignment of new values                
                HashSet<int> faceNeighbours;
                Dictionary<int, HashSet<int>> allFaceNeighbours = new Dictionary<int, HashSet<int>>();
                CompareIntArray comparer = new CompareIntArray();
                Dictionary<int[], double[]> edgeAngles = new Dictionary<int[], double[]>(comparer);
                // Create a face neighbours map and compute edge angles
                for (int i = 0; i < part.Visualization.Cells.Length; i++)
                {
                    cell1Id = i;
                    face1Id = faceIdOfCells[cell1Id];
                    for (int j = 0; j < part.Visualization.CellNeighboursOverCellEdge[i].Length; j++)
                    {
                        cell2Id = part.Visualization.CellNeighboursOverCellEdge[i][j];
                        if (cell2Id == -1) continue;
                        //
                        face2Id = faceIdOfCells[cell2Id];
                        // Compute angle only for edge cells
                        if (face1Id != face2Id)
                        {
                            if (allFaceNeighbours.TryGetValue(face1Id, out faceNeighbours)) faceNeighbours.Add(face2Id);
                            else allFaceNeighbours.Add(face1Id, new HashSet<int>() { face2Id });
                            //
                            if (allFaceNeighbours.TryGetValue(face2Id, out faceNeighbours)) faceNeighbours.Add(face1Id);
                            else allFaceNeighbours.Add(face2Id, new HashSet<int>() { face1Id });
                            //
                            alpha = ComputeAngleInRadFromCellIndices(cells[cell1Id], cells[cell2Id]);
                            edgeHash = new int[] { Math.Min(face1Id, face2Id), Math.Max(face1Id, face2Id) };
                            //
                            if (edgeAngles.TryGetValue(edgeHash, out minAngle))
                            {
                                if (alpha < minAngle[0]) minAngle[0] = alpha;
                            }
                            else edgeAngles.Add(edgeHash, new double[] { alpha });
                        }
                    }
                }
                //
                angle *= Math.PI / 180;
                HashSet<int> surfaceIds = new HashSet<int>();
                HashSet<int> notVisitedSurfaceIds = new HashSet<int>();
                HashSet<int> newSurfaceIds = new HashSet<int>();
                surfaceIds.Add(faceId);
                notVisitedSurfaceIds.Add(faceId);
                // Spread
                do
                {
                    if (allFaceNeighbours.Count == 0) break;    // for shells with 1 surface there is no neighbours
                    // Find new surface candidates
                    newSurfaceIds.Clear();
                    foreach (var notVisitedSurfaceId in notVisitedSurfaceIds)
                    {
                        foreach (var neighbourId in allFaceNeighbours[notVisitedSurfaceId])
                        {
                            if (!surfaceIds.Contains(neighbourId) && !newSurfaceIds.Contains(neighbourId))
                            {
                                alpha = edgeAngles[new int[] { Math.Min(notVisitedSurfaceId, neighbourId),
                                                               Math.Max(notVisitedSurfaceId, neighbourId) }][0];
                                if (alpha <= angle)
                                {
                                    newSurfaceIds.Add(neighbourId);
                                }
                            }
                        }
                    }

                    // Add new surface candidates to surfaces and to surfaces to visit
                    notVisitedSurfaceIds.Clear();
                    foreach (var newSurfaceId in newSurfaceIds)
                    {
                        surfaceIds.Add(newSurfaceId);
                        notVisitedSurfaceIds.Add(newSurfaceId);
                    }
                }
                while (newSurfaceIds.Count > 0);
                // Get geometry ids
                int partId = part.PartId;
                int typeId;
                // Surfaces
                if (part.PartType == PartType.Shell)
                    typeId = shellFrontFace ? (int)GeometryType.ShellFrontSurface : (int)GeometryType.ShellBackSurface;
                else if (part.PartType == PartType.SolidAsShell)
                    typeId = (int)GeometryType.ShellFrontSurface;
                else if (part.PartType == PartType.Solid)
                    typeId = (int)GeometryType.SolidSurface;
                else throw new NotSupportedException();
                //
                count = 0;
                int[] geometryIds = new int[surfaceIds.Count];
                foreach (int surfaceId in surfaceIds)
                {
                    geometryIds[count++] = surfaceId * 100000 + typeId * 10000 + partId;
                }
                return geometryIds;
            }
            //
            return null;
        }

        // Get node, element or face ids from geometry ids
        public int[] GetIdsFromGeometryIds(int[] geometryIds, vtkSelectItem selectItem, bool onlyVisibile = false)
        {
            HashSet<int> ids = new HashSet<int>();
            for (int i = 0; i < geometryIds.Length; i++)
            {
                ids.UnionWith(GetIdsFromGeometryId(geometryIds[i], selectItem, onlyVisibile));
            }
            return ids.ToArray();
        }
        private int[] GetIdsFromGeometryId(int geometryId, vtkSelectItem selectItem, bool onlyVisibile)
        {
            // geometryId = itemId * 100000 + typeId * 10000 + partId;
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryId);
            int[] nodeIds;
            //
            BasePart part = GetPartById(itemTypePartIds[2]);
            if (part == null || (onlyVisibile && !part.Visible)) return new int[0];
            //
            if (selectItem == vtkSelectItem.Node)
            {
                nodeIds = GetNodeIdsFromGeometryId(geometryId);
                return nodeIds;
            }
            else if (selectItem == vtkSelectItem.Element)
            {
                return GetElementIdsFromGeometryId(itemTypePartIds, geometryId);
            }
            else if (selectItem == vtkSelectItem.Edge || selectItem == vtkSelectItem.Geometry)
            {
                return new int[] { geometryId };
            }
            else if (selectItem == vtkSelectItem.Surface)
            {
                GeometryType geomType = (GeometryType)itemTypePartIds[1];
                if (geomType == GeometryType.ShellEdgeSurface)
                {
                    return GetVisualizationFaceIdsFromShellEdge(itemTypePartIds[0], itemTypePartIds[2]);
                }
                else if (geomType == GeometryType.SolidSurface ||
                         geomType == GeometryType.ShellFrontSurface ||
                         geomType == GeometryType.ShellBackSurface)
                {
                    bool shellFrontFace = geomType == GeometryType.ShellFrontSurface;
                    return GetVisualizationFaceIds(itemTypePartIds[0], itemTypePartIds[2], shellFrontFace);
                }
                else if (geomType == GeometryType.Part)
                {
                    List<int> faceIds = new List<int>(GetVisualizationFaceIds(part.Name, true));
                    faceIds.AddRange(GetVisualizationFaceIds(part.Name, false));
                    return faceIds.ToArray();
                }
                else return new int[] { };
            }
            else if (selectItem == vtkSelectItem.Part)
            {
                return new int[] { itemTypePartIds[2] };
            }
            else throw new NotSupportedException();
        }
        public int[] GetNodeIdsFromGeometryId(int geometryId)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryId);
            // Find part by id
            BasePart part = GetPartById(itemTypePartIds[2]);
            if (part == null) return new int[0];
            //
            VisualizationData vis = part.Visualization;
            HashSet<int> nodeIds = new HashSet<int>();
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            //
            if (geomType == GeometryType.Vertex)
            {
                nodeIds.Add(vis.VertexNodeIds[itemTypePartIds[0]]);
            }
            else if (geomType == GeometryType.Edge ||
                     geomType == GeometryType.ShellEdgeSurface)
            {
                foreach (var edgeCellId in vis.EdgeCellIdsByEdge[itemTypePartIds[0]])
                {
                    nodeIds.UnionWith(vis.EdgeCells[edgeCellId]);
                }
            }
            else if (geomType == GeometryType.SolidSurface ||
                     geomType == GeometryType.ShellFrontSurface ||
                     geomType == GeometryType.ShellBackSurface)
            {
                foreach (var cellId in vis.CellIdsByFace[itemTypePartIds[0]])
                {
                    nodeIds.UnionWith(vis.Cells[cellId]);
                }
            }
            else if (geomType == GeometryType.Part)
            {
                nodeIds.UnionWith(part.NodeLabels);
            }
            else throw new NotSupportedException();
            //
            return nodeIds.ToArray();
        }
        public int[] GetElementIdsFromGeometryId(int[] itemTypePartIds, int geometryId)
        {
            bool containsEdge = false;
            bool containsFace = false;
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            if (geomType == GeometryType.Vertex) { }
            else if (geomType == GeometryType.Edge ||
                     geomType == GeometryType.ShellEdgeSurface) containsEdge = true;
            else if (geomType == GeometryType.SolidSurface ||
                     geomType == GeometryType.ShellFrontSurface ||
                     geomType == GeometryType.ShellBackSurface ||
                     geomType == GeometryType.Part) containsFace = true;
            else throw new NotSupportedException();
            //
            int[] nodeIds = GetNodeIdsFromGeometryId(geometryId);
            return GetElementIdsFromNodeIds(nodeIds, containsEdge, containsFace, false, itemTypePartIds[2]);
        }
        // Get node, edge or triangle coordinates for mesh refinement for Netgen
        public void GetVetexAndEdgeCoorFromGeometryIds(int[] ids, double meshSize, bool edgeRepresentation,
                                                             out double[][] points, out double[][][] lines)
        {
            int[][] cells = GetCellsFromGeometryIds(ids, edgeRepresentation);
            List<double[]> pointList = new List<double[]>();
            List<double[][]> lineList = new List<double[][]>();
            //
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Length == 1)
                {
                    pointList.Add(_nodes[cells[i][0]].Coor);
                }
                else if (cells[i].Length == 2)
                {
                    lineList.Add(GetEdgePointCoor(cells[i]));
                }
                else if (cells[i].Length == 3)
                {
                    SplitTriangle(cells[i], meshSize, out double[][] trianglePoints, out double[][][] triangleLines);
                    pointList.AddRange(trianglePoints);
                    lineList.AddRange(triangleLines);
                }
                else throw new NotSupportedException();
            }
            points = pointList.ToArray();
            lines = lineList.ToArray();
        }
        // Get node, edge or triangle coordinates for mesh refinement for Highlight
        public void GetVetexAndEdgeCoorFromGeometryIds(int[] ids, double meshSize, bool edgeRepresentation,
                                                       out double[][] points)
        {
            int[][] cells = GetCellsFromGeometryIds(ids, edgeRepresentation);
            List<double[]> pointList = new List<double[]>();
            //
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Length == 1)
                {
                    pointList.Add(_nodes[cells[i][0]].Coor);
                }
                else if (cells[i].Length == 2)
                {
                    pointList.AddRange(SplitEdge(cells[i], meshSize));
                }
                else if (cells[i].Length == 3)
                {
                    SplitTriangle(cells[i], meshSize, out double[][] trianglePoints, out double[][][] triangleLines);
                    pointList.AddRange(trianglePoints);
                }
                else throw new NotSupportedException();
            }
            points = pointList.ToArray();
        }
        private int[][] GetCellsFromGeometryIds(int[] ids, bool edgeRepresentation)
        {
            int[][] cells;
            List<int[]> cellsList = new List<int[]>();
            foreach (var id in ids)
            {
                cells = GetCellsFromGeometryId(id, edgeRepresentation);
                if (cells != null) cellsList.AddRange(cells);
            }
            return cellsList.ToArray();
        }
        private int[][] GetCellsFromGeometryId(int geometryId, bool edgeRepresentation)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryId);
            // Find part by id
            BasePart part = GetPartById(itemTypePartIds[2]);
            if (part == null) return null;
            //
            VisualizationData vis = part.Visualization;
            List<int[]> cellsList = new List<int[]>();
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            //
            if (geomType == GeometryType.Vertex)
            {
                cellsList.Add(new int[] { vis.VertexNodeIds[itemTypePartIds[0]] });
            }
            else if (geomType == GeometryType.Edge || geomType == GeometryType.ShellEdgeSurface)
            {
                foreach (var edgeCellId in vis.EdgeCellIdsByEdge[itemTypePartIds[0]])
                {
                    cellsList.Add(vis.EdgeCells[edgeCellId]);
                }
            }
            else if (geomType == GeometryType.SolidSurface ||
                     geomType == GeometryType.ShellFrontSurface ||
                     geomType == GeometryType.ShellBackSurface)
            {
                if (edgeRepresentation)
                {
                    foreach (var edgeId in vis.FaceEdgeIds[itemTypePartIds[0]])
                    {
                        foreach (var edgeCellId in vis.EdgeCellIdsByEdge[edgeId])
                        {
                            cellsList.Add(vis.EdgeCells[edgeCellId]);
                        }
                    }
                }
                else
                {
                    foreach (var cellId in vis.CellIdsByFace[itemTypePartIds[0]])
                    {
                        cellsList.Add(vis.Cells[cellId]);
                    }
                }
            }
            else if (geomType == GeometryType.Part)
            {
                if (edgeRepresentation)
                {
                    cellsList.AddRange(vis.EdgeCells);
                }
                else
                {
                    cellsList.AddRange(vis.Cells);
                }
            }
            else throw new NotSupportedException();
            //
            return cellsList.ToArray();
        }
        private double[][] SplitEdge(int[] edgeNodeIds, double meshSize)
        {
            Vec3D n1 = new Vec3D(_nodes[edgeNodeIds[0]].Coor);
            Vec3D n2 = new Vec3D(_nodes[edgeNodeIds[1]].Coor);
            Vec3D e = n2 - n1;
            //
            int numSeg = (int)(e.Len / meshSize) + 1;
            double[][] coor = new double[numSeg + 1][];
            coor[0] = n1.Coor;
            coor[numSeg] = n2.Coor;
            if (numSeg > 1)
            {
                e *= (1.0 / numSeg);
                for (int i = 1; i < numSeg; i++)
                {
                    coor[i] = (n1 + i * e).Coor;
                }
            }
            return coor;
        }
        private double[][] GetEdgePointCoor(int[] edgeNodeIds)
        {
            Vec3D n1 = new Vec3D(_nodes[edgeNodeIds[0]].Coor);
            Vec3D n2 = new Vec3D(_nodes[edgeNodeIds[1]].Coor);
            //
            double[][] coor = new double[2][];
            coor[0] = n1.Coor;
            coor[1] = n2.Coor;
            return coor;
        }
        private Vec3D[] SplitTriangleEdge(Vec3D n1, Vec3D n2, double meshSize)
        {
            Vec3D e = n2 - n1;
            //
            int numSeg = (int)(e.Len / meshSize) + 1;
            Vec3D[] coor = new Vec3D[numSeg + 1];
            coor[0] = n1;
            coor[numSeg] = n2;
            if (numSeg > 1)
            {
                e *= (1.0 / numSeg);
                for (int i = 1; i < numSeg; i++)
                {
                    coor[i] = (n1 + i * e);
                }
            }
            return coor;
        }
        private void SplitTriangle(int[] triangleNodeIds, double meshSize, out double[][] points, out double[][][] lines)
        {
            Vec3D[] triangle = new Vec3D[3];
            triangle[0] = new Vec3D(_nodes[triangleNodeIds[0]].Coor);
            triangle[1] = new Vec3D(_nodes[triangleNodeIds[1]].Coor);
            triangle[2] = new Vec3D(_nodes[triangleNodeIds[2]].Coor);
            //
            HashSet<Vec3D[]> splitTriangles = new HashSet<Vec3D[]>();
            HashSet<Vec3D[]> trianglesToSplit = new HashSet<Vec3D[]>();
            HashSet<Vec3D[]> newTrianglesToSplit = new HashSet<Vec3D[]>();
            trianglesToSplit.Add(triangle);
            //
            while (trianglesToSplit.Count > 0)
            {
                newTrianglesToSplit.Clear();
                foreach (var triangleToSplit in trianglesToSplit)
                {
                    if (TriangleSize(triangleToSplit) > meshSize)
                    {
                        newTrianglesToSplit.UnionWith(SplitTriangle(triangleToSplit));
                    }
                    else
                    {
                        splitTriangles.Add(triangleToSplit);
                    }
                }
                trianglesToSplit.Clear();
                trianglesToSplit.UnionWith(newTrianglesToSplit);
            }
            //
            HashSet<Vec3D> nodes = new HashSet<Vec3D>();
            List<double[][]> lineList = new List<double[][]>();
            double[][] line;
            foreach (var splitTriangle in splitTriangles)
            {
                //nodes.UnionWith(SplitTriangleEdge(splitTriangle[0], splitTriangle[1], meshSize));
                //nodes.UnionWith(SplitTriangleEdge(splitTriangle[1], splitTriangle[2], meshSize));
                //nodes.UnionWith(SplitTriangleEdge(splitTriangle[2], splitTriangle[0], meshSize));
                //
                line = new double[2][];
                line[0] = splitTriangle[0].Coor;
                line[1] = splitTriangle[1].Coor;
                lineList.Add(line);
                //
                line = new double[2][];
                line[0] = splitTriangle[1].Coor;
                line[1] = splitTriangle[2].Coor;
                lineList.Add(line);
                //
                line = new double[2][];
                line[0] = splitTriangle[2].Coor;
                line[1] = splitTriangle[1].Coor;
                lineList.Add(line);
            }
            //
            int count = 0;
            points = new double[nodes.Count][];
            foreach (var node in nodes) points[count++] = node.Coor;
            //
            lines = lineList.ToArray();
        }
        private double TriangleSize(Vec3D[] triangle)
        {
            Vec3D n1 = triangle[0];
            Vec3D n2 = triangle[1];
            Vec3D n3 = triangle[2];
            //
            double l1 = (n2 - n1).Len;
            double l2 = (n3 - n2).Len;
            double l3 = (n1 - n3).Len;
            //
            double s = (l1 + l2 + l3) / 2;
            // Heurons formula for area
            double A = Math.Sqrt(s * (s - l1) * (s - l2) * (s - l3));
            // Return min triangle height
            return 2 * A / Math.Max(Math.Max(l1, l2), l3);
        }
        private Vec3D[][] SplitTriangle(Vec3D[] triangle)
        {
            Vec3D n1 = triangle[0];
            Vec3D n2 = triangle[1];
            Vec3D n3 = triangle[2];
            //
            Vec3D n4 = (n1 + n2) * 0.5;
            Vec3D n5 = (n2 + n3) * 0.5;
            Vec3D n6 = (n3 + n1) * 0.5;
            //
            Vec3D[][] coor = new Vec3D[4][];
            coor[0] = new Vec3D[3];
            coor[0][0] = n1;
            coor[0][1] = n4;
            coor[0][2] = n6;
            //
            coor[1] = new Vec3D[3];
            coor[1][0] = n4;
            coor[1][1] = n5;
            coor[1][2] = n6;
            //
            coor[2] = new Vec3D[3];
            coor[2][0] = n4;
            coor[2][1] = n2;
            coor[2][2] = n5;
            //
            coor[3] = new Vec3D[3];
            coor[3][0] = n6;
            coor[3][1] = n5;
            coor[3][2] = n3;
            //
            return coor;
        }
        //
        public bool IsThisIdGeometryId(int id)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(id);
            //
            if (itemTypePartIds[0] >= 0 &&
                Enum.IsDefined(typeof(GeometryType), itemTypePartIds[1]) &&
                GetPartById(itemTypePartIds[2]) != null)
                return true;
            else
                return false;
        }
        public string[] GetPartNamesFromGeometryIds(int[] ids)
        {
            int[] partIds = GetPartIdsFromGeometryIds(ids);
            if (partIds == null) return null;
            //
            int count = 0;
            string[] partNames = new string[partIds.Length];
            foreach (var partId in partIds) partNames[count++] = GetPartById(partId).Name;
            //
            return partNames.ToArray();
        }
        public static int[] GetPartIdsFromGeometryIds(int[] ids)
        {
            if (ids == null) return null;
            //
            HashSet<int> partIds = new HashSet<int>();
            int[] itemTypePartIds;
            for (int i = 0; i < ids.Length; i++)
            {
                itemTypePartIds = GetItemTypePartIdsFromGeometryId(ids[i]);
                partIds.Add(itemTypePartIds[2]);
            }
            //
            return partIds.ToArray();
        }
        public static int GetPartIdFromGeometryId(int id)
        {
            return GetItemTypePartIdsFromGeometryId(id)[2];
        }
        public static int[] GetItemTypePartIdsFromGeometryId(int geometryId)
        {
            // geometryId = itemId * 100000 + typeId * 10000 + partId;
            int partId = geometryId % 10000;
            int typeId = (geometryId / 10000) % 10;     // GeometryType
            int itemId = geometryId / 100000;
            return new int[] { itemId, typeId, partId };
        }
        public static int GetGeometryPartIdFromGeometryId(int geometryId)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryId);
            return itemTypePartIds[0] * 100000 + (int)GeometryType.Part * 10000 + itemTypePartIds[2];
        }
        //
        private int GetNextEdgeNodeId(int n1Id, int n2Id, HashSet<int> n2Neighbours, double angle)
        {
            double minAngle = double.MaxValue;
            int minNodeId = -1;
            double alpha;

            foreach (int nId3 in n2Neighbours)
            {
                if (nId3 != n1Id)   // skip the first node
                {
                    alpha = GetEdgeAngle(n1Id, n2Id, nId3);
                    if (alpha <= angle && alpha < minAngle)
                    {
                        minAngle = alpha;
                        minNodeId = nId3;
                    }
                }
            }

            return minNodeId;
        }
        private double GetEdgeAngle(int n1Id, int n2Id, int n3Id)
        {
            double[] n1 = _nodes[n1Id].Coor;
            double[] n2 = _nodes[n2Id].Coor;
            double[] n3 = _nodes[n3Id].Coor;

            double[] a = new double[] { n2[0] - n1[0], n2[1] - n1[1], n2[2] - n1[2] };
            double[] b = new double[] { n3[0] - n2[0], n3[1] - n2[1], n3[2] - n2[2] };

            double d = Math.Sqrt(Math.Pow(a[0], 2) + Math.Pow(a[1], 2) + Math.Pow(a[2], 2));
            a[0] /= d;
            a[1] /= d;
            a[2] /= d;

            d = Math.Sqrt(Math.Pow(b[0], 2) + Math.Pow(b[1], 2) + Math.Pow(b[2], 2));
            b[0] /= d;
            b[1] /= d;
            b[2] /= d;

            double cosAngle = Math.Min(1, a[0] * b[0] + a[1] * b[1] + a[2] * b[2]);

            return Math.Acos(cosAngle);
        }
        private void GetNextEdgeAndNodeId(int edge1Id, int node1Id, HashSet<int> nodeEdges, double angle,
                                         VisualizationData visualization, out int edge2Id, out int node2Id)
        {
            edge2Id = -1;
            node2Id = -1;
            double minAngle = double.MaxValue;
            int minEdgeId = -1;
            double alpha;

            foreach (int edgeId in nodeEdges)
            {
                if (edgeId != edge1Id)   // skip self
                {
                    alpha = GetEdgeAngle(edge1Id, edgeId, node1Id, visualization);
                    if (alpha <= angle && alpha < minAngle)
                    {
                        minAngle = alpha;
                        minEdgeId = edgeId;
                    }
                }
            }
            if (minEdgeId < 0) return;

            edge2Id = minEdgeId;
            int[] edgeCellIds = visualization.EdgeCellIdsByEdge[edge2Id];
            if (visualization.EdgeCells[edgeCellIds[0]][0] == node1Id)
                node2Id = visualization.EdgeCells[edgeCellIds.Last()][1];
            else node2Id = visualization.EdgeCells[edgeCellIds[0]][0];
        }
        private double GetEdgeAngle(int edge1Id, int edge2Id, int nodeId, VisualizationData visualization)
        {
            int[] edge1CellIds = visualization.EdgeCellIdsByEdge[edge1Id];
            int[] edge2CellIds = visualization.EdgeCellIdsByEdge[edge2Id];

            int n1;
            int n2;
            if (visualization.EdgeCells[edge1CellIds[0]][0] == nodeId)
                n1 = visualization.EdgeCells[edge1CellIds[0]][1];
            else n1 = visualization.EdgeCells[edge1CellIds.Last()][0];

            if (visualization.EdgeCells[edge2CellIds[0]][0] == nodeId)
                n2 = visualization.EdgeCells[edge2CellIds[0]][1];
            else n2 = visualization.EdgeCells[edge2CellIds.Last()][0];

            return GetEdgeAngle(n1, nodeId, n2);
        }

        #endregion #################################################################################################################

        #region Add entities #######################################################################################################
        public FeNode AddNodeByCoor(double x, double y, double z)
        {
            _maxNodeId++;
            FeNode node = new FeNode(_maxNodeId, x, y, z);
            _nodes.Add(node.Id, node);
            _boundingBox.IncludeNode(node);
            return node;
        }
        public void AddNodeSet(FeNodeSet nodeSet)
        {
            FeNodeSet existingNodeSet;
            // Sort labels
            Array.Sort(nodeSet.Labels);
            //
            if (_nodeSets.TryGetValue(nodeSet.Name, out existingNodeSet))   // in Calculix the sets with the same name are merged
            {
                existingNodeSet.Labels = existingNodeSet.Labels.Concat(nodeSet.Labels).Distinct().ToArray();
            }
            else
            {
                List<int> nodeIds = new List<int>();
                // Add only node ids of existing elements
                foreach (int nodeId in nodeSet.Labels)
                {
                    if (_nodes.ContainsKey(nodeId)) nodeIds.Add(nodeId);
                }
                //
                if (nodeIds.Count > 0)
                {
                    FeNodeSet newNodeSet = new FeNodeSet(nodeSet.Name, nodeIds.ToArray());
                    //
                    UpdateNodeSetCenterOfGravity(newNodeSet);
                    //
                    if (nodeSet.Labels.Length != newNodeSet.Labels.Length) newNodeSet.Valid = false;
                    //
                    _nodeSets.Add(newNodeSet.Name, newNodeSet);
                }
            }
        }
        public void AddNodeSetFromElementSet(string elementSetName)
        {
            FeNodeSet nodeSet = GetNodeSetFromPartOrElementSetName(elementSetName, false);
            _nodeSets.Add(nodeSet.Name, nodeSet);
        }
        public FeNodeSet GetNodeSetFromPartOrElementSetName(string regionName, bool onlyVisible)
        {
            FeGroup group;
            if (_elementSets.ContainsKey(regionName)) group = _elementSets[regionName];
            else if (_parts.ContainsKey(regionName)) group = _parts[regionName];
            else throw new CaeException("The element set name or part name does not exist.");
            //
            Dictionary<int, bool> partVisibilities = new Dictionary<int, bool>();
            foreach (var part in _parts) partVisibilities.Add(part.Value.PartId, part.Value.Visible);
            // Create a node set from the element set
            HashSet<int> nodeIds = new HashSet<int>();
            FeElement element;
            for (int i = 0; i < group.Labels.Length; i++)
            {
                element = _elements[group.Labels[i]];
                if (!(onlyVisible && !partVisibilities[element.PartId])) nodeIds.UnionWith(element.NodeIds);
            }
            //
            string nodeSetName = _nodeSets.GetNextNumberedKey(regionName + "_el");
            FeNodeSet nodeSet = new FeNodeSet(nodeSetName, nodeIds.ToArray());
            UpdateNodeSetCenterOfGravity(nodeSet);
            return nodeSet;
        }
        public FeNodeSet GetNodeSetFromPartNames(string[] partNames, bool onlyVisible)
        {
            // Create a node set from parts
            string nodeSetName = "_";
            HashSet<int> nodeIds = new HashSet<int>();
            BasePart part;
            if (partNames.Length == 0) nodeSetName += "None_";
            else
            {
                foreach (var partName in partNames)
                {
                    part = _parts[partName];
                    if (!(onlyVisible && !part.Visible)) nodeIds.UnionWith(part.NodeLabels);
                    nodeSetName += partName + "_";
                }
            }
            nodeSetName += DateTime.Now.Ticks;
            FeNodeSet nodeSet = new FeNodeSet(nodeSetName, nodeIds.ToArray());
            UpdateNodeSetCenterOfGravity(nodeSet);
            return nodeSet;
        }
        public void AddElementSet(FeElementSet elementSet)
        {
            FeElementSet existingElementSet;
            BasePart part;
            // Sort labels
            Array.Sort(elementSet.Labels);
            //
            if (_elementSets.TryGetValue(elementSet.Name, out existingElementSet))  // in Calculix the sets with the same name are merged
            {
                existingElementSet.Labels = existingElementSet.Labels.Concat(elementSet.Labels).Distinct().ToArray();
                return;
            }
            else if (_parts.TryGetValue(elementSet.Name, out part))             // does a part exists
            {
                CompareIntArray comparer = new CompareIntArray();
                if (comparer.Equals(part.Labels, elementSet.Labels)) return;    // skip element sets with the same name and ids as parts
                else
                {
                    // Rename part;
                    HashSet<string> allNames = new HashSet<string>(_elementSets.Keys);
                    allNames.UnionWith(_parts.Keys);
                    _parts.Remove(part.Name);
                    part.Name = allNames.GetNextNumberedKey(part.Name.Split('-')[0]);
                    _parts.Add(part.Name, part);
                }
            }
            //
            List<int> elementIds = new List<int>();
            // Add only element ids of existing elements
            foreach (int elementId in elementSet.Labels)
            {
                if (_elements.ContainsKey(elementId)) elementIds.Add(elementId);
            }
            //
            if (elementIds.Count > 0)
            {
                FeElementSet newElementSet = new FeElementSet(elementSet.Name, elementIds.ToArray());
                //
                if (elementSet.Labels.Length != newElementSet.Labels.Length) newElementSet.Valid = false;
                //
                _elementSets.Add(newElementSet.Name, newElementSet);
            }
        }
        public void AddSurface(FeSurface surface)
        {
            if (surface.CreatedFrom == FeSurfaceCreatedFrom.Faces)
            {
                AddSurfaceFromFaces(ref surface);
            }
            else if (surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
            {
                AddSurfaceFromNodeSet(surface);
            }
            else throw new CaeException("Function 'Add surface' only usable for surfaces created from faces.");
            //
            _surfaces.Add(surface.Name, surface);
        }
        private void AddSurfaceFromFaces(ref FeSurface surface)
        {
            FeElementSet elementSet;
            List<FeFaceName> tmpFaces;
            Dictionary<int, List<FeFaceName>> allElementIdsFaces = new Dictionary<int, List<FeFaceName>>();
            //
            foreach (var entry in surface.ElementFaces)
            {
                if (_elementSets.TryGetValue(entry.Value, out elementSet))
                {
                    foreach (int elementId in elementSet.Labels)
                    {
                        if (allElementIdsFaces.TryGetValue(elementId, out tmpFaces)) tmpFaces.Add(entry.Key);
                        else allElementIdsFaces.Add(elementId, new List<FeFaceName>() { entry.Key });
                    }
                    elementSet.Internal = true;     // hide element set
                }
                else surface.Valid = false;
            }
            //
            FeElement element;
            List<int> faceIds = new List<int>();
            HashSet<int> allNodeIds = new HashSet<int>();
            CompareIntArray comparer = new CompareIntArray();
            //
            int vtkCellId;
            int[] cell;
            foreach (var entry in allElementIdsFaces)
            {
                element = _elements[entry.Key];
                if (element is FeElement3D element3D)
                {
                    foreach (var faceName in entry.Value)
                    {
                        cell = element3D.GetVtkCellFromFaceName(faceName);
                        vtkCellId = element3D.GetVtkCellIdFromCell(cell);
                        if (vtkCellId != -1)
                        {
                            allNodeIds.UnionWith(cell);
                            faceIds.Add(10 * element.Id + vtkCellId);
                        }
                    }
                }
                else if (element is FeElement2D element2D)
                {
                    foreach (var faceName in entry.Value)
                    {
                        cell = element2D.GetVtkCellFromFaceName(faceName);
                        vtkCellId = element2D.GetVtkCellIdFromCell(cell);
                        if (vtkCellId != -1)
                        {
                            allNodeIds.UnionWith(cell);
                            faceIds.Add(10 * element.Id + vtkCellId);
                        }
                    }
                }
                else throw new NotSupportedException();
            }
            //
            FeSurface surfaceFromFaceIds = new FeSurface(surface.Name, faceIds.ToArray(), null);
            foreach (var entry in surface.ElementFaces) surfaceFromFaceIds.AddElementFace(entry.Key, entry.Value);
            // Get the surface face type
            surfaceFromFaceIds.SurfaceFaceTypes = GetSurfaceTypeFromFaceIds(surfaceFromFaceIds.FaceIds);
            // Node set
            string nodeSetName = GetNextFreeInternalName(_nodeSets, surfaceFromFaceIds.Name);
            FeNodeSet nodeSet = new FeNodeSet(nodeSetName, allNodeIds.ToArray());
            nodeSet.Internal = true;
            UpdateNodeSetCenterOfGravity(nodeSet);
            _nodeSets.Add(nodeSetName, nodeSet);
            surfaceFromFaceIds.NodeSetName = nodeSetName;
            //
            UpdateSurfaceArea(surfaceFromFaceIds);
            //
            surface = surfaceFromFaceIds;
        }
        private void AddSurfaceFromNodeSet(FeSurface surface)
        {
            CreateSurfaceItems(surface);
        }
        //
        public string[] AddMesh(FeMesh mesh, ICollection<string> reservedPartNames, bool forceRenameParts = true,
                                bool renumberNodesAndElements = true)
        {
            int numOfNodes = mesh.Nodes.Count;
            int numOfElements = mesh.Elements.Count;
            int firstNodeId = _nodes.FindFreeIntervalOfKeys(numOfNodes, _maxNodeId);
            int firstElementId = _elements.FindFreeIntervalOfKeys(numOfElements, _maxElementId);
            //
            if (!renumberNodesAndElements &&
                (_nodes.Keys.Intersect(mesh.Nodes.Keys).Count() > 0 || _elements.Keys.Intersect(mesh.Elements.Keys).Count() > 0))
            {
                renumberNodesAndElements = true;
            }
            //
            int count;
            string entryName;
            // Renumber nodes
            if (renumberNodesAndElements) mesh.RenumberNodes(firstNodeId);
            _nodes.AddRange(mesh.Nodes);
            if (_maxNodeId < mesh.MaxNodeId) _maxNodeId = mesh.MaxNodeId;
            // Renumber elements
            if (renumberNodesAndElements) mesh.RenumberElements(firstElementId);
            _elements.AddRange(mesh.Elements);
            if (_maxElementId < mesh.MaxElementId) _maxElementId = mesh.MaxElementId;
            // Add and rename nodeSets
            count = 1;
            foreach (var entry in mesh.NodeSets)
            {
                entryName = entry.Key;
                if (_nodeSets.ContainsKey(entryName))
                {
                    entryName += "_Im-";
                    while (_nodeSets.ContainsKey(entryName + count.ToString())) count++;
                    entryName += count.ToString();
                    entry.Value.Name = entryName;
                }
                _nodeSets.Add(entry.Value.Name, entry.Value);
            }
            // Add and rename elementSets
            count = 1;
            foreach (var entry in mesh.ElementSets)
            {
                entryName = entry.Key;
                if (_elementSets.ContainsKey(entryName))
                {
                    entryName += "_Im-";
                    while (_elementSets.ContainsKey(entryName + count.ToString())) count++;
                    entryName += count.ToString();
                    entry.Value.Name = entryName;
                }
                _elementSets.Add(entry.Value.Name, entry.Value);
            }
            // Add and rename surfaces
            count = 1;
            foreach (var entry in mesh.Surfaces)
            {
                entryName = entry.Key;
                if (_surfaces.ContainsKey(entryName))
                {
                    entryName += "_Im-";
                    while (_surfaces.ContainsKey(entryName + count.ToString())) count++;
                    entryName += count.ToString();
                    entry.Value.Name = entryName;
                }
                _surfaces.Add(entry.Value.Name, entry.Value);
            }
            // Renumber parts
            int maxPartID = 0;
            foreach (var entry in _parts)
            {
                if (entry.Value.PartId > maxPartID) maxPartID = entry.Value.PartId;
            }
            mesh.RenumberParts(maxPartID + 1);
            // Add and rename parts
            List<string> addedPartNames = new List<string>();
            HashSet<string> allNames = new HashSet<string>(_parts.Keys);
            if (reservedPartNames != null) allNames.UnionWith(reservedPartNames);
            //
            BasePart newPart;
            foreach (var entry in mesh.Parts)
            {
                newPart = entry.Value.DeepCopy();
                // Set color after renumbering
                SetPartColorFromColorTable(newPart);
                //
                entryName = entry.Key;
                if (forceRenameParts || allNames.Contains(entryName))
                {
                    entryName = NamedClass.GetNameWithoutLastValue(entryName);
                    entryName = allNames.GetNextNumberedKey(entryName);
                }
                //
                newPart.Name = entryName;
                _parts.Add(entryName, newPart);
                addedPartNames.Add(entryName);
                allNames.Add(entryName);
            }
            // Bounding box
            _boundingBox.IncludeBox(mesh.BoundingBox);
            //
            return addedPartNames.ToArray();
        }
        public string[] AddPartsFromMesh(FeMesh mesh, string[] partNames, ICollection<string> reservedPartNames,
                                         bool forceRenameParts = true, bool renumberNodesAndElements = true)
        {
            FeMesh partialMesh = new FeMesh(mesh, partNames);
            return AddMesh(partialMesh, reservedPartNames, forceRenameParts, renumberNodesAndElements);
        }
       
        #endregion #################################################################################################################

        #region Update entities ####################################################################################################

        public void UpdateSurfaceArea(FeSurface surface)
        {
            if (surface.Type == FeSurfaceType.Node)
            {
                surface.Area = 0;
            }
            else if (surface.Type == FeSurfaceType.Element)
            {
                double area;
                int[] nodeIds;
                FeSurfaceFaceTypes surfaceFaceTypes;
                Dictionary<FeFaceName, List<int>> elementSets;
                //
                if (surface.CreatedFrom == FeSurfaceCreatedFrom.Selection)
                {
                    CreateSurfaceFacesFromSelection(surface.FaceIds, out nodeIds, out elementSets,
                                                    out area, out surfaceFaceTypes);
                }
                else if (surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet)
                {
                    CreateSurfaceFacesFromNodeSet(surface, out nodeIds, out elementSets, out area,
                                                  out surfaceFaceTypes);
                }
                else throw new CaeException("The surface created from faces is not supported.");
                //
                surface.Area = area;
            }
            else throw new CaeException("The surface type is not supported.");
        }
        public void UpdateReferencePoint(FeReferencePoint referencePoint)
        {
            if (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.BoundingBoxCenter
                || referencePoint.CreatedFrom == FeReferencePointCreatedFrom.CenterOfGravity)
            {
                // Update reference point
                FeNodeSet nodeSet;
                string regionName = referencePoint.RegionName;
                if (regionName != null)
                {
                    if (referencePoint.RegionType == RegionTypeEnum.NodeSetName)
                    {
                        if (_nodeSets.TryGetValue(regionName, out nodeSet))
                        {
                            if (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.BoundingBoxCenter)
                                referencePoint.UpdateCoordinates(nodeSet.BoundingBox);
                            else if (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.CenterOfGravity)
                                referencePoint.UpdateCoordinates(nodeSet.CenterOfGravity);
                            else throw new NotSupportedException();
                        }
                    }
                    else if (referencePoint.RegionType == RegionTypeEnum.SurfaceName)
                    {
                        if (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.BoundingBoxCenter)
                        {
                            nodeSet = GetSurfaceNodeSet(regionName);
                            if (nodeSet != null) referencePoint.UpdateCoordinates(nodeSet.BoundingBox);
                        }
                        else if (referencePoint.CreatedFrom == FeReferencePointCreatedFrom.CenterOfGravity)
                        {
                            double[] cg = GetSurfaceCG(regionName);
                            referencePoint.UpdateCoordinates(cg);
                        }
                        else throw new NotSupportedException();
                    }
                    else throw new NotSupportedException();
                }
            }
        }

        #endregion #################################################################################################################

        #region Remove entities ####################################################################################################
        public HashSet<int> RemoveUnreferencedNodes(HashSet<int> possiblyUnrefNodeIds,
                                                    bool removeEmptyNodeSets, bool removeForRemeshing)
        {
            // For each node find it's connected elements
            Dictionary<int, List<FeElement>> nodeElements = new Dictionary<int, List<FeElement>>();
            foreach (var entry in _elements)
            {
                foreach (var nodeId in entry.Value.NodeIds)
                {
                    if (nodeElements.ContainsKey(nodeId)) nodeElements[nodeId].Add(entry.Value);
                    else nodeElements.Add(nodeId, new List<FeElement>() { entry.Value });
                }
            }
            // Get unreferenced nodes
            HashSet<int> unreferenced = new HashSet<int>();
            foreach (var nodeId in possiblyUnrefNodeIds)
            {
                if (!nodeElements.ContainsKey(nodeId)) unreferenced.Add(nodeId);
            }
            if (unreferenced.Count > 0)
            {
                // Remove unreferenced nodes
                foreach (var key in unreferenced) _nodes.Remove(key);
                // Remove unreferenced nodes from node sets
                List<int> newNodeSetLabels = new List<int>();
                List<string> emptyNodeSets = new List<string>();
                List<string> changedNodeSets = new List<string>();
                foreach (var entry in _nodeSets)
                {
                    newNodeSetLabels.Clear();
                    foreach (var id in entry.Value.Labels)
                    {
                        if (!unreferenced.Contains(id)) newNodeSetLabels.Add(id);
                    }
                    if (newNodeSetLabels.Count != entry.Value.Labels.Length)
                    {
                        entry.Value.Labels = newNodeSetLabels.ToArray();
                        changedNodeSets.Add(entry.Key);
                    }
                    if (entry.Value.Labels.Length == 0) emptyNodeSets.Add(entry.Key);
                }
                // Changed node sets
                bool geometryBased;
                FeNodeSet nodeSet;
                foreach (string name in changedNodeSets)
                {
                    nodeSet = _nodeSets[name];
                    geometryBased = nodeSet.CreationData != null && nodeSet.CreationData.IsGeometryBased();
                    // Do not change the geometry based node set if remeshing is done
                    if (!(removeForRemeshing && geometryBased))
                    {
                        UpdateNodeSetCenterOfGravity(nodeSet);
                        nodeSet.CreationData = new Selection();
                        nodeSet.CreationData.Add(new SelectionNodeIds(vtkSelectOperation.None, false, nodeSet.Labels));
                        nodeSet.Valid = false;          // mark it as unvalid to highlight it for the user
                    }
                }
                // Remove empty node sets
                if (removeEmptyNodeSets)
                {
                    foreach (var name in emptyNodeSets) _nodeSets.Remove(name);
                }
                // Remove unreferenced nodes from parts
                foreach (var entry in _parts)
                {
                    newNodeSetLabels.Clear();
                    foreach (var id in entry.Value.NodeLabels)
                    {
                        if (!unreferenced.Contains(id)) newNodeSetLabels.Add(id);
                    }
                    if (newNodeSetLabels.Count != entry.Value.NodeLabels.Length)
                    {
                        entry.Value.NodeLabels = newNodeSetLabels.ToArray();
                    }
                }
            }
            //
            UpdateMaxNodeAndElementIds();
            //
            return unreferenced;
        }
        public string[] RemoveElementsFromElementSets(HashSet<int> removedElementIds, bool removeEmptyElementSets, bool removeForRemeshing)
        {
            List<int> newElementSetLabels = new List<int>();
            List<string> emptyElementSets = new List<string>();
            List<string> changedElementSets = new List<string>();
            //
            foreach (var entry in _elementSets)
            {
                newElementSetLabels.Clear();
                foreach (var id in entry.Value.Labels)
                {
                    if (!removedElementIds.Contains(id)) newElementSetLabels.Add(id);
                }
                if (newElementSetLabels.Count != entry.Value.Labels.Length)
                {
                    entry.Value.Labels = newElementSetLabels.ToArray();
                    changedElementSets.Add(entry.Key);
                }
                if (entry.Value.Labels.Length == 0) emptyElementSets.Add(entry.Key);
            }
            // Changed element sets
            bool geometryBased;
            FeElementSet elementSet;
            foreach (string name in changedElementSets)
            {
                elementSet = _elementSets[name];
                geometryBased = elementSet.CreationData != null && elementSet.CreationData.IsGeometryBased();
                // Do not change the geometry based element set if remeshing is done
                if (!(removeForRemeshing && geometryBased))
                {
                    elementSet.CreationData = new Selection();
                    elementSet.CreationData.Add(new SelectionNodeIds(vtkSelectOperation.None, false, elementSet.Labels));
                    elementSet.Valid = false;          // mark it as unvalid to highlight it for the user
                }
            }
            // Remove empty element sets
            if (removeEmptyElementSets)
            {
                foreach (var name in emptyElementSets) _elementSets.Remove(name);
            }
            //
            return changedElementSets.ToArray();
        }
        public string[] RemoveElementsFromSurfaceFaces(HashSet<int> removedElementIds,
                                                       bool removeEmptySurfaces,
                                                       bool removeForRemeshing)
        {
            int elementId;
            List<int> newSurfaceFaceIds = new List<int>();
            List<string> changedSurfaces = new List<string>();
            List<string> emptySurfaces = new List<string>();
            foreach (var entry in _surfaces)
            {
                newSurfaceFaceIds.Clear();
                //
                if (entry.Value.FaceIds != null)
                {
                    foreach (var faceId in entry.Value.FaceIds)
                    {
                        elementId = faceId / 10;
                        if (!removedElementIds.Contains(elementId)) newSurfaceFaceIds.Add(faceId);
                    }
                    if (entry.Value.FaceIds.Length != newSurfaceFaceIds.Count)
                    {
                        entry.Value.FaceIds = newSurfaceFaceIds.ToArray();
                        changedSurfaces.Add(entry.Key);
                    }
                    if (entry.Value.FaceIds.Length == 0) emptySurfaces.Add(entry.Key);
                }
            }
            // Changed surfaces
            bool geometryBased;
            FeSurface surface;
            foreach (string name in changedSurfaces)
            {
                surface = _surfaces[name];
                geometryBased = surface.CreationData.IsGeometryBased();
                // Do not change the geometry based surface if remeshing is done
                if (!(removeForRemeshing && geometryBased))
                {
                    surface = new FeSurface(_surfaces[name]);
                    surface.CreationData = new Selection();
                    surface.CreationData.Add(new SelectionNodeIds(vtkSelectOperation.None, false, surface.FaceIds));
                    surface.Valid = false;      // mark it as unvalid to highlight it for the user
                    _surfaces[name] = surface;
                }
            }
            // Remove empty surfaces
            if (removeEmptySurfaces)
            {
                string[] nodeSets;
                string[] elementSets;
                RemoveSurfaces(emptySurfaces.ToArray(), out nodeSets, out elementSets);
            }
            //
            return changedSurfaces.ToArray();
        }
        //
        public int[] RemoveParts(string[] partNames, out string[] removedParts, bool removeForRemeshing)
        {
            int[] removedPartIds = new int[partNames.Length];
            HashSet<int> possiblyUnrefNodeIds = new HashSet<int>();
            List<string> removedPartsList = new List<string>();     // Use a list to keep the sorting of the input partNames
            HashSet<int> removedElementIds = new HashSet<int>();
            //
            int partCount = 0;
            foreach (var name in partNames)
            {
                if (!_parts.ContainsKey(name)) removedPartIds[partCount] = -1;
                else
                {
                    removedPartIds[partCount] = _parts[name].PartId;
                    // Remove elements
                    foreach (int elementId in _parts[name].Labels)
                    {
                        foreach (int nodeId in _elements[elementId].NodeIds)
                        {
                            possiblyUnrefNodeIds.Add(nodeId);
                            removedElementIds.Add(elementId);
                        }
                        _elements.Remove(elementId);
                    }
                    // Remove parts
                    _parts.Remove(name);
                    removedPartsList.Add(name);
                }
                partCount++;
            }
            removedParts = removedPartsList.ToArray();
            // Remove unreferenced nodes and keep empty node sets
            RemoveUnreferencedNodes(possiblyUnrefNodeIds, false, removeForRemeshing);
            // Remove elements from element sets and find empty element sets but do not remove them
            RemoveElementsFromElementSets(removedElementIds, false, removeForRemeshing);
            // Find changed surface
            RemoveElementsFromSurfaceFaces(removedElementIds, false, removeForRemeshing);
            // Bounding box
            ComputeBoundingBox();
            //
            return removedPartIds;
        }
        public FeSurface[] RemoveSurfaces(string[] surfaceNames, out string[] removedNodeSets, out string[] removedElementSets)
        {
            HashSet<string> removedElementSetsHashSet = new HashSet<string>();
            HashSet<string> removedNodeSetsHashSet = new HashSet<string>();
            FeSurface surface;
            List<FeSurface> removedSurfaces = new List<FeSurface>();
            //
            foreach (var name in surfaceNames)
            {
                // Remove old element sets
                if (Surfaces.TryGetValue(name, out surface))
                {
                    if (surface.ElementFaces != null)
                    {
                        foreach (var entry in surface.ElementFaces)
                        {
                            if (_elementSets.ContainsKey(entry.Value))
                            {
                                _elementSets.Remove(entry.Value);
                                removedElementSetsHashSet.Add(entry.Value);
                            }
                        }
                    }
                    //
                    if (!(surface.Type == FeSurfaceType.Node && surface.CreatedFrom == FeSurfaceCreatedFrom.NodeSet))
                    {
                        // Null is in the case when no elements were found to form a surface
                        if (surface.NodeSetName != null)
                        {
                            removedNodeSetsHashSet.Add(surface.NodeSetName);
                            _nodeSets.Remove(surface.NodeSetName);
                        }
                    }
                    // Remove surface
                    Surfaces.Remove(name);
                    removedSurfaces.Add(surface);
                }
            }
            removedNodeSets = removedNodeSetsHashSet.ToArray();
            removedElementSets = removedElementSetsHashSet.ToArray();
            //
            return removedSurfaces.ToArray();
        }
        public void RemoveElementsByType<T>()
        {
            HashSet<int> removedElementIds = new HashSet<int>();
            HashSet<int> possiblyUnrefNodeIds = new HashSet<int>();
            foreach (var entry in _elements)
            {
                if (entry.Value is T)
                {
                    // Get removed element ids
                    foreach (int nodeId in entry.Value.NodeIds) possiblyUnrefNodeIds.Add(nodeId);
                    removedElementIds.Add(entry.Key);
                }
            }
            //
            RemoveElementsByIds(removedElementIds, possiblyUnrefNodeIds, true, true, false);
        }
        public HashSet<int> RemoveElementsByIds(HashSet<int> elementIds, HashSet<int> possiblyUnrefNodeIds,
                                                bool removeEmptyParts,
                                                bool removeEmptyNodeSets,
                                                bool removeForRemeshing)
        {
            // Remove elements
            foreach (var elementId in elementIds) _elements.Remove(elementId);
            // Remove elements from element sets
            List<int> newLabels = new List<int>();
            List<string> emptyElementSets = new List<string>();
            foreach (var entry in _elementSets)
            {
                newLabels.Clear();
                foreach (var id in entry.Value.Labels)
                {
                    if (!elementIds.Contains(id)) newLabels.Add(id);
                }
                if (newLabels.Count == 0) emptyElementSets.Add(entry.Key);
                else entry.Value.Labels = newLabels.ToArray();
            }
            //
            foreach (var key in emptyElementSets) _elementSets.Remove(key);
            //
            List<string> emptyParts = new List<string>();
            // Remove elements from Parts
            foreach (var entry in _parts)
            {
                newLabels.Clear();
                foreach (var id in entry.Value.Labels)
                {
                    if (!elementIds.Contains(id)) newLabels.Add(id);
                }
                entry.Value.Labels = newLabels.ToArray();
                //
                if (newLabels.Count == 0) emptyParts.Add(entry.Key);
                else entry.Value.Labels = newLabels.ToArray();
            }
            //
            if (removeEmptyParts) foreach (var key in emptyParts) _parts.Remove(key);
            //
            HashSet<int> unreferenced = RemoveUnreferencedNodes(possiblyUnrefNodeIds, removeEmptyNodeSets, removeForRemeshing);
            //
            ComputeBoundingBox();
            //
            return unreferenced;
        }
        //
        #endregion #################################################################################################################


        // Nodes 
        public double[][] GetAllNodeCoor()
        {
            double[][] coor = new double[_nodes.Count][];
            int count = 0;
            foreach (int key in _nodes.Keys)
            {
                coor[count++] = _nodes[key].Coor;
            }
            return coor;
        }
        public string[] GetNodeSetNames()
        {
            return _nodeSets.Keys.ToArray();
        }
        public void UpdateNodeSetCenterOfGravity(FeNodeSet nodeSet)
        {
            double[] coor;
            double[] centerOfGravity = new double[3];
            double[][] boundingBox = new double[3][];

            for (int i = 0; i < 3; i++) boundingBox[i] = new double[2];

            boundingBox[0][0] = double.MaxValue;
            boundingBox[1][0] = double.MaxValue;
            boundingBox[2][0] = double.MaxValue;
            boundingBox[0][1] = -double.MaxValue;
            boundingBox[1][1] = -double.MaxValue;
            boundingBox[2][1] = -double.MaxValue;

            if (nodeSet.Labels != null && nodeSet.Labels.Length > 0)
            {
                foreach (var nodeId in nodeSet.Labels)
                {
                    coor = _nodes[nodeId].Coor;

                    for (int i = 0; i < 3; i++)
                    {
                        centerOfGravity[i] += coor[i];

                        if (coor[i] < boundingBox[i][0]) boundingBox[i][0] = coor[i];
                        if (coor[i] > boundingBox[i][1]) boundingBox[i][1] = coor[i];
                    }
                }
                centerOfGravity[0] /= nodeSet.Labels.Length;
                centerOfGravity[1] /= nodeSet.Labels.Length;
                centerOfGravity[2] /= nodeSet.Labels.Length;
            }

            nodeSet.CenterOfGravity = centerOfGravity;
            nodeSet.BoundingBox = boundingBox;
        }
        public double[][] GetNodeSetCoor(int[] nodeIds)
        {
            List<double[]> coor = new List<double[]>();
            for (int i = 0; i < nodeIds.Length; i++) coor.Add(_nodes[nodeIds[i]].Coor);
            return coor.ToArray();
        }
        public double[][] GetNodeSetCoor(int[] nodeIds, bool onlyVisible)
        {
            int nodeId;
            List<double[]> coor = new List<double[]>();
            //
            HashSet<int> visibleNodes = new HashSet<int>();
            foreach (var entry in _parts) if (entry.Value.Visible) visibleNodes.UnionWith(entry.Value.NodeLabels);
            //
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodeId = nodeIds[i];
                if (!(onlyVisible && !visibleNodes.Contains(nodeId))) coor.Add(_nodes[nodeIds[i]].Coor);
            }
            //
            return coor.ToArray();
        }
        public int[] GetVisibleNodeIds()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (var entry in _parts)
            {
                if (entry.Value.Visible) ids.UnionWith(entry.Value.NodeLabels);
            }
            return ids.ToArray();
        }
        public double[][] GetNodeNormals(int[] nodeIds)
        {
            VisualizationData vis;
            Vec3D normal;
            List<Vec3D> nodeNornals;
            Dictionary<int, List<Vec3D>> nodeIdNormals = new Dictionary<int, List<Vec3D>>();
            foreach (var entry in _parts)
            {
                vis = entry.Value.Visualization;
                for (int i = 0; i < vis.Cells.Length; i++)
                {
                    normal = new Vec3D(ComputeNormalFromFaceCellIndices(vis.Cells[i]).Coor);
                    for (int j = 0; j < vis.Cells[i].Length; j++)
                    {
                        if (nodeIdNormals.TryGetValue(vis.Cells[i][j], out nodeNornals)) nodeNornals.Add(normal);
                        else nodeIdNormals.Add(vis.Cells[i][j], new List<Vec3D> { normal });
                    }
                }
            }
            //
            double[][] normals = new double[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodeNornals = nodeIdNormals[nodeIds[i]];
                normal = new Vec3D();
                foreach(Vec3D n in nodeNornals) normal += n;
                normal.Normalize();
                normals[i] = normal.Coor;
            }
            return normals;
        }
        public void UpdateNodalCoordinatesFromMesh(FeMesh mesh)
        {
            FeNode node;
            foreach (var key in _nodes.Keys.ToArray())  // to copy
            {
                if (mesh.Nodes.TryGetValue(key, out node))
                {
                    _nodes[key] = node;
                }

                //if (mesh.Nodes.TryGetValue(entry.Key, out node))
                //{
                //    entry.Value.SetCoor(node.X, node.Y, node.Z);
                //    _nodes[entry.Key] = node.d()
                //}
            }
        }

        // Elements 
        public string[] GetElementSetNames()
        {
            return _elementSets.Keys.ToArray();
        }
        public void GetElementFaceNormal(int elementId, FeFaceName faceName, out double[] faceNormal)
        {
            GetElementFaceCenterAndNormal(elementId, faceName, out double[] faceCenter, out faceNormal,
                                          out bool shellElement);
        }
        public void GetElementFaceCenter(int elementId, FeFaceName faceName, out double[] faceCenter)
        {
            GetElementFaceCenterAndNormal(elementId, faceName, out faceCenter, out double[] faceNormal,
                                          out bool shellElement);
        }
        public void GetElementFaceNormalAndArea(int elementId, FeFaceName faceName, out double[] faceNormal, out double area)
        {
            GetElementFaceCenterAndNormal(elementId, faceName, out double[] faceCenter, out faceNormal,
                                          out bool shellElement);
            area = _elements[elementId].GetArea(faceName, _nodes);
        }
        public void GetElementFaceCenterAndNormal(int elementId, FeFaceName faceName, out double[] faceCenter,
                                                  out double[] faceNormal, out bool shellElement)
        {
            FeNode[] nodes;
            FeElement element = _elements[elementId];
            int[] nodeIds;
            faceCenter = null;
            faceNormal = null;
            //
            if (element is LinearTriangleElement || element is ParabolicTriangleElement)
            {
                // Shell surface face
                if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                {
                    nodeIds = element.GetNodeIdsFromFaceName(faceName);
                    nodes = new FeNode[3];
                    faceCenter = new double[3];
                    //
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i] = _nodes[nodeIds[i]];
                        faceCenter[0] += nodes[i].X;
                        faceCenter[1] += nodes[i].Y;
                        faceCenter[2] += nodes[i].Z;
                    }
                    faceCenter[0] /= nodes.Length;
                    faceCenter[1] /= nodes.Length;
                    faceCenter[2] /= nodes.Length;
                    //
                    faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2]).Coor;
                }
                // Shell edge face
                else
                {
                    nodeIds = element.GetNodeIdsFromFaceName(FeFaceName.S2); // the element normal
                    nodes = new FeNode[3];
                    for (int i = 0; i < nodes.Length; i++) nodes[i] = _nodes[nodeIds[i]];
                    faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2]).Coor;
                    //
                    nodeIds = element.GetNodeIdsFromFaceName(faceName);
                    nodes = new FeNode[2];
                    faceCenter = new double[3];
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i] = _nodes[nodeIds[i]];
                        faceCenter[0] += nodes[i].X;
                        faceCenter[1] += nodes[i].Y;
                        faceCenter[2] += nodes[i].Z;
                    }
                    faceCenter[0] /= nodes.Length;
                    faceCenter[1] /= nodes.Length;
                    faceCenter[2] /= nodes.Length;
                    //
                    double[] edgeDirection = ComputeDirectionFromEdgeCellNodes(nodes[0], nodes[1]);
                    // Return the normal towards element inside
                    Vec3D n = new Vec3D(faceNormal);
                    Vec3D d = new Vec3D(edgeDirection);
                    Vec3D en = Vec3D.CrossProduct(n, d);
                    en.Normalize();
                    faceNormal = en.Coor;
                }
                //
                shellElement = true;
            }
            else if (element is LinearQuadrilateralElement || element is ParabolicQuadrilateralElement)
            {
                // Shell surface face
                if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2)
                {
                    nodeIds = element.GetNodeIdsFromFaceName(faceName);
                    nodes = new FeNode[4];
                    faceCenter = new double[3];
                    //
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i] = _nodes[nodeIds[i]];
                        faceCenter[0] += nodes[i].X;
                        faceCenter[1] += nodes[i].Y;
                        faceCenter[2] += nodes[i].Z;
                    }
                    faceCenter[0] /= nodes.Length;
                    faceCenter[1] /= nodes.Length;
                    faceCenter[2] /= nodes.Length;
                    //
                    faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2], nodes[3]).Coor;
                }
                // Shell edge face
                else
                {
                    nodeIds = element.GetNodeIdsFromFaceName(FeFaceName.S2); // the element normal
                    nodes = new FeNode[4];
                    for (int i = 0; i < nodes.Length; i++) nodes[i] = _nodes[nodeIds[i]];
                    faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2], nodes[3]).Coor;
                    //
                    nodeIds = element.GetNodeIdsFromFaceName(faceName);
                    nodes = new FeNode[2];
                    faceCenter = new double[3];
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        nodes[i] = _nodes[nodeIds[i]];
                        faceCenter[0] += nodes[i].X;
                        faceCenter[1] += nodes[i].Y;
                        faceCenter[2] += nodes[i].Z;
                    }
                    faceCenter[0] /= nodes.Length;
                    faceCenter[1] /= nodes.Length;
                    faceCenter[2] /= nodes.Length;
                    //
                    double[] edgeDirection = ComputeDirectionFromEdgeCellNodes(nodes[0], nodes[1]);
                    // Return the normal towards element inside
                    Vec3D n = new Vec3D(faceNormal);
                    Vec3D d = new Vec3D(edgeDirection);
                    Vec3D en = Vec3D.CrossProduct(n, d);
                    en.Normalize();
                    faceNormal = en.Coor;
                }
                //
                shellElement = true;
            }
            else if(element is LinearTetraElement || element is ParabolicTetraElement)
            {
                nodeIds = element.GetNodeIdsFromFaceName(faceName);
                nodes = new FeNode[3];
                faceCenter = new double[3];
                //
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = _nodes[nodeIds[i]];
                    faceCenter[0] += nodes[i].X;
                    faceCenter[1] += nodes[i].Y;
                    faceCenter[2] += nodes[i].Z;
                }
                faceCenter[0] /= nodes.Length;
                faceCenter[1] /= nodes.Length;
                faceCenter[2] /= nodes.Length;
                // Element normal to inside
                faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2]).Coor;
                //
                shellElement = false;
            }
            else if (element is LinearWedgeElement || element is ParabolicWedgeElement)
            {
                nodeIds = element.GetNodeIdsFromFaceName(faceName);
                //
                if (faceName == FeFaceName.S1 || faceName == FeFaceName.S2) nodes = new FeNode[3];
                else nodes = new FeNode[4];
                //
                faceCenter = new double[3];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = _nodes[nodeIds[i]];
                    faceCenter[0] += nodes[i].X;
                    faceCenter[1] += nodes[i].Y;
                    faceCenter[2] += nodes[i].Z;
                }
                faceCenter[0] /= nodes.Length;
                faceCenter[1] /= nodes.Length;
                faceCenter[2] /= nodes.Length;
                // Element normal to inside
                if (faceName == FeFaceName.S1) faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2]).Coor;
                // Element normal to outside
                else if (faceName == FeFaceName.S2) faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[2], nodes[1]).Coor;
                // Element normal to outside
                else faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[3], nodes[2], nodes[1]).Coor;

                //
                shellElement = false;
            }
            else if (element is LinearHexaElement || element is ParabolicHexaElement)
            {
                nodeIds = element.GetNodeIdsFromFaceName(faceName);
                nodes = new FeNode[4];
                faceCenter = new double[3];
                //
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = _nodes[nodeIds[i]];
                    faceCenter[0] += nodes[i].X;
                    faceCenter[1] += nodes[i].Y;
                    faceCenter[2] += nodes[i].Z;
                }
                faceCenter[0] /= nodes.Length;
                faceCenter[1] /= nodes.Length;
                faceCenter[2] /= nodes.Length;
                // Element normal to inside
                faceNormal = ComputeNormalFromFaceCellNodes(nodes[0], nodes[1], nodes[2], nodes[3]).Coor;
                //
                shellElement = false;
            }
            else throw new NotSupportedException();
        }
        public int[] GetVisibleElementIds()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (var entry in _parts)
            {
                if (entry.Value.Visible) ids.UnionWith(entry.Value.Labels);
            }
            return ids.ToArray();
        }


        // Cells 
        public void GetAllNodesAndCells(out int[] nodeIds, out double[][] nodeCoor, out int[] cellIds,
                                        out int[][] cells, out int[] cellTypes)
        {
            cellIds = _elements.Keys.ToArray();
            cells = new int[cellIds.Length][];
            cellTypes = new int[cellIds.Length];
            int i = 0;
            //
            foreach (var entry in _elements)
            {
                cells[i] = entry.Value.GetVtkNodeIds();
                cellTypes[i] = entry.Value.GetVtkCellType();
                i++;
            }
            //
            nodeIds = GetRenumberedNodesAndCells(out nodeCoor, ref cells);
        }
        public void GetSetNodesAndCells(FeGroup elementSet, out int[] nodeIds, out double[][] nodeCoor, out int[] cellIds,
                                        out int[][] cells, out int[] cellTypes)
        {
            cellIds = elementSet.Labels;
            cells = new int[cellIds.Length][];
            cellTypes = new int[cellIds.Length];
            int i = 0;
            FeElement element;
            //
            if (elementSet is BasePart part)
            {
                foreach (var elemId in part.Labels)
                {
                    element = _elements[elemId];
                    // Copy the array because it will be renumbered
                    cells[i] = element.GetVtkNodeIds();
                    cellTypes[i] = element.GetVtkCellType();
                    i++;
                }
            }
            else
            {
                // Get all cells and all node ids for elementSet
                for (i = 0; i < cellIds.Length; i++)
                {
                    // Copy the array because it will be renumbered
                    element = _elements[cellIds[i]];
                    cells[i] = element.GetVtkNodeIds();
                    cellTypes[i] = element.GetVtkCellType();
                }
            }
            nodeIds = GetRenumberedNodesAndCells(out nodeCoor, ref cells);
        }
        public void GetVisualizationNodesAndCells(BasePart part, out int[] nodeIds, out double[][] nodeCoor, out int[] cellIds,
                                                  out int[][] cells, out int[] cellTypes)
        {
            cellIds = part.Visualization.CellIds.ToArray();
            int[][] visualizationCells = part.Visualization.Cells;
            //
            List<int> nodeIdsList = new List<int>();
            List<double[]> nodeCoorList = new List<double[]>();
            List<int> cellIdsList = new List<int>();
            List<int[]> cellsList = new List<int[]>();
            List<int> cellTypesList = new List<int>();
            int visualizationCellId;
            // Create visualization for each visualization face
            if (part.PartType == PartType.Solid || part.PartType == PartType.SolidAsShell || part.PartType == PartType.Shell)
            {
                for (int i = 0; i < part.Visualization.CellIdsByFace.Length; i++)
                {
                    cellIds = new int[part.Visualization.CellIdsByFace[i].Length];
                    cells = new int[part.Visualization.CellIdsByFace[i].Length][];
                    cellTypes = new int[part.Visualization.CellIdsByFace[i].Length];
                    //
                    for (int j = 0; j < part.Visualization.CellIdsByFace[i].Length; j++)
                    {
                        visualizationCellId = part.Visualization.CellIdsByFace[i][j];
                        cellIds[j] = part.Visualization.CellIds[visualizationCellId];
                        cells[j] = visualizationCells[visualizationCellId].ToArray(); // node ids in cells will be renumbered
                        //
                        if (cells[j].Length == 3) cellTypes[j] = (int)vtkCellType.VTK_TRIANGLE;
                        else if (cells[j].Length == 4) cellTypes[j] = (int)vtkCellType.VTK_QUAD;
                        else if (cells[j].Length == 6) cellTypes[j] = (int)vtkCellType.VTK_QUADRATIC_TRIANGLE;
                        else if (cells[j].Length == 8) cellTypes[j] = (int)vtkCellType.VTK_QUADRATIC_QUAD;
                        else throw new NotSupportedException();
                    }
                    nodeIds = GetRenumberedNodesAndCells(nodeIdsList.Count, out nodeCoor, ref cells);
                    //
                    nodeIdsList.AddRange(nodeIds);
                    nodeCoorList.AddRange(nodeCoor);
                    cellIdsList.AddRange(cellIds);
                    cellsList.AddRange(cells);
                    cellTypesList.AddRange(cellTypes);
                }
                //
                nodeIds = nodeIdsList.ToArray();
                nodeCoor = nodeCoorList.ToArray();
                cellIds = cellIdsList.ToArray();
                cells = cellsList.ToArray();
                cellTypes = cellTypesList.ToArray();
            }
            else if (part.PartType == PartType.Wire)
            {
                cellIds = part.Visualization.CellIds.ToArray();
                cells = new int[visualizationCells.Length][];
                cellTypes = new int[visualizationCells.Length];
                //
                for (int i = 0; i < visualizationCells.Length; i++)
                {
                    cells[i] = visualizationCells[i].ToArray();

                    if (cells[i].Length == 2) cellTypes[i] = (int)vtkCellType.VTK_LINE;
                    else if (cells[i].Length == 3) cellTypes[i] = (int)vtkCellType.VTK_QUADRATIC_EDGE;
                    else throw new NotSupportedException();
                }
                //
                nodeIds = GetRenumberedNodesAndCells(out nodeCoor, ref cells);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public void GetNodesAndCellsForModelEdges(FeGroup elementSet, out int[] nodeIds, out double[][] nodeCoor,
                                                  out int[][] cells, out int[] cellTypes)
        {
            nodeIds = null;
            nodeCoor = null;
            cells = null;
            cellTypes = null;
            //
            if (elementSet is BasePart part)
            {
                int[][] edgeCells;
                if (part.PartType.HasEdges()) edgeCells = part.Visualization.EdgeCells;
                else throw new Exception();
                //
                GetNodesAndCellsForEdges(edgeCells, out nodeIds, out nodeCoor, out cells, out cellTypes);
            }
        }
        public int[][] GetEdgeCellsFromGeometryEdgeIds(int[] geometryEdgeIds)
        {
            int[] itemTypePartIds;
            BasePart part;
            List<int[]> edgeCells = new List<int[]>();
            //
            foreach (int geometryEdgeId in geometryEdgeIds)
            {
                itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryEdgeId);
                //
                part = GetPartById(itemTypePartIds[2]);
                if (part == null) continue;
                //
                foreach (int edgeCellId in part.Visualization.EdgeCellIdsByEdge[itemTypePartIds[0]])
                {
                    edgeCells.Add(part.Visualization.EdgeCells[edgeCellId]);
                }
            }
            return edgeCells.ToArray();
        }
        public void GetNodesAndCellsForEdges(int[][] edgeCells, out int[] nodeIds, out double[][] nodeCoor, out int[][] cells,
                                             out int[] cellTypes)
        {
            cells = new int[edgeCells.Length][];
            cellTypes = new int[edgeCells.Length];
            for (int i = 0; i < edgeCells.Length; i++)
            {
                cells[i] = edgeCells[i].ToArray();
                if (cells[i].Length == 1) cellTypes[i] = (int)vtkCellType.VTK_VERTEX;
                else if (cells[i].Length == 2) cellTypes[i] = (int)vtkCellType.VTK_LINE;
                else if (cells[i].Length == 3) cellTypes[i] = (int)vtkCellType.VTK_QUADRATIC_EDGE;
                else throw new NotSupportedException();
            }

            nodeIds = GetRenumberedNodesAndCells(out nodeCoor, ref cells);
        }
        private int[] GetRenumberedNodesAndCells(out double[][] nodeCoor, ref int[][] cells)
        {
            return GetRenumberedNodesAndCells(0, out nodeCoor, ref cells);
        }
        private int[] GetSortedUniqueIds(int[][] cells)
        {
            //int maxId = -1;
            //for (int i = 0; i < cells.Length; i++)
            //{
            //    for (int j = 0; j < cells[i].Length; j++)
            //    {
            //        if (cells[i][j] > maxId) maxId = cells[i][j];
            //    }
            //}
            //maxId++;        // add 1
            ////
            int[] nodeIds;
            //int count = 0;
            //int twoGig = 2147483591;
            //if (maxId < twoGig)
            //{
            //    byte[] idArray = new byte[maxId];
            //    for (int i = 0; i < cells.Length; i++)
            //    {
            //        for (int j = 0; j < cells[i].Length; j++)
            //        {
            //            if (idArray[cells[i][j]] == 0)
            //            {
            //                idArray[cells[i][j]] = 1;
            //                count++;
            //            }
            //        }
            //    }
            //    //
            //    nodeIds = new int[count];
            //    count = 0;
            //    for (int i = 0; i < idArray.Length; i++)
            //    {
            //        if (idArray[i] == 1) nodeIds[count++] = i;
            //    }
            //}
            //else
            {
                HashSet<int> nodeIdsHash = new HashSet<int>();
                // Get all cells and all nodes ids for elementSet
                for (int i = 0; i < cells.Length; i++) nodeIdsHash.UnionWith(cells[i]);
                nodeIds = nodeIdsHash.ToArray();
                Array.Sort(nodeIds);
            }
            //
            return nodeIds;
        }
        private int[] GetRenumberedNodesAndCells(int firstNodeId, out double[][] nodeCoor, ref int[][] cells)
        {
            int[] nodeIds = GetSortedUniqueIds(cells);
            // Get all node coordinates and prepare re-numbering map
            Dictionary<int, int> oldIds = new Dictionary<int, int>();   // the order of items is not retained
            int[] orderedNodeIds = new int[nodeIds.Length];
            nodeCoor = new double[nodeIds.Length][];
            int count = 0;
            foreach (int nodeId in nodeIds)
            {
                nodeCoor[count] = _nodes[nodeId].Coor;
                oldIds.Add(nodeId, count);
                orderedNodeIds[count] = nodeId;
                count++;
            }
            //
            for (int i = 0; i < cells.Length; i++)
            {
                for (int j = 0; j < cells[i].Length; j++)
                {
                    cells[i][j] = oldIds[cells[i][j]] + firstNodeId;
                }
            }
            //
            return orderedNodeIds;       // return ordered node ids for the nodeCoords
        }
        public int[][] GetFreeEdgesFromVisualizationCells(int[][] cells, ElementFaceType[] elementFaceTypes)
        {
            CompareIntArray comparer = new CompareIntArray();
            HashSet<int[]> freeEdges = new HashSet<int[]>(comparer);
            //
            int[] key;
            int[][] cellEdges;
            ElementFaceType elementFaceType;
            // Get free edges
            for (int i = 0; i < cells.Length; i++)
            {
                if (elementFaceTypes != null) elementFaceType = elementFaceTypes[i];
                else elementFaceType = ElementFaceType.Unknown;
                //
                cellEdges = GetVisualizationEdgeCells(cells[i], elementFaceType);
                foreach (var cellEdge in cellEdges)
                {
                    key = cellEdge.ToArray();
                    Array.Sort(key);
                    if (!freeEdges.Add(key)) freeEdges.Remove(key);
                }
            }
            //
            return freeEdges.ToArray();
        }
        public static int[][] GetVisualizationEdgeCells(int[] cell, ElementFaceType elementFaceType)
        {
            //int[] lookUp3 = new int[] { 0, 1, 2, 0 };
            //int[] lookUp4 = new int[] { 0, 1, 2, 3, 0 };
            //int[] lookUp6 = new int[] { 0, 3, 1, 4, 2, 5, 0 };
            //int[] lookUp8 = new int[] { 0, 4, 1, 5, 2, 6, 3, 7, 0 };

            if (elementFaceType == ElementFaceType.Edge)
            {
                if (cell.Length == 2) return new int[][] { new int[] { cell[0], cell[1] } };
                else if (cell.Length == 3) return new int[][] { new int[] { cell[0], cell[1], cell[2] } };
                else throw new NotSupportedException();
            }
            else
            {
                if (cell.Length == 3)
                {
                    return new int[][] { new int[] { cell[0], cell[1] },
                                         new int[] { cell[1], cell[2] },
                                         new int[] { cell[2], cell[0] } };
                }
                else if (cell.Length == 4)
                {
                    return new int[][] { new int[] { cell[0], cell[1] },
                                         new int[] { cell[1], cell[2] },
                                         new int[] { cell[2], cell[3] },
                                         new int[] { cell[3], cell[0] } };
                }
                else if (cell.Length == 6)
                {
                    return new int[][] { new int[] { cell[0], cell[1], cell[3] },
                                         new int[] { cell[1], cell[2], cell[4] },
                                         new int[] { cell[2], cell[0], cell[5] } };

                    //return new int[][] {    new int[] { cell[0], cell[3], cell[1] },
                    //                        new int[] { cell[1], cell[4], cell[2] },
                    //                        new int[] { cell[2], cell[5], cell[0] } };
                }
                else if (cell.Length == 8)
                {
                    return new int[][] { new int[] { cell[0], cell[1], cell[4] },
                                         new int[] { cell[1], cell[2], cell[5] },
                                         new int[] { cell[2], cell[3], cell[6] },
                                         new int[] { cell[3], cell[0], cell[7] } };

                    //return new int[][] {    new int[] { cell[0], cell[4], cell[1] },
                    //                        new int[] { cell[1], cell[5], cell[2] },
                    //                        new int[] { cell[2], cell[6], cell[3] },
                    //                        new int[] { cell[3], cell[7], cell[0] } };
                }
                else throw new NotSupportedException();
            }
        }
        public static void InvertVisualizationCell(int[] cell)
        {
            int tmp;
            if (cell.Length == 3)
            {
                tmp = cell[1];
                cell[1] = cell[2];
                cell[2] = tmp;
            }
            else if (cell.Length == 4)
            {
                tmp = cell[1];
                cell[1] = cell[3];
                cell[3] = tmp;
            }
            else if (cell.Length == 6)
            {
                tmp = cell[1];
                cell[1] = cell[2];
                cell[2] = tmp;

                tmp = cell[3];
                cell[3] = cell[5];
                cell[5] = tmp;
            }
            else if (cell.Length == 8)
            {
                tmp = cell[1];
                cell[1] = cell[3];
                cell[3] = tmp;

                tmp = cell[4];
                cell[4] = cell[7];
                cell[7] = tmp;

                tmp = cell[5];
                cell[5] = cell[6];
                cell[6] = tmp;

            }
            else throw new NotSupportedException();
        }

        // Edges
        public double GetEdgeLength(int geometryEdgeId)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryEdgeId);
            if (itemTypePartIds[1] == (int)GeometryType.Edge)
            {
                BasePart part = GetPartById(itemTypePartIds[2]);
                return part.Visualization.EdgeLengths[itemTypePartIds[0]];
            }
            else throw new NotSupportedException();
        }
        public void GetEdgeNodeCoor(int geometryEdgeId, out int[] nodeIds, out double[][] nodeCoor)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryEdgeId);
            if (itemTypePartIds[1] == (int)GeometryType.Edge)
            {
                VisualizationData visualization = GetPartById(itemTypePartIds[2]).Visualization;
                GetEdgeNodeCoor(itemTypePartIds[0], visualization, out nodeIds, out nodeCoor);
            }
            else throw new NotSupportedException();
        }
        public void GetEdgeNodeCoor(int edgeId, VisualizationData visualization, out int[] nodeIds, out double[][] nodeCoor)
        {
            int edgeCellId;
            int[] oneEdgeNodeIds;
            List<double[]> edgeNodeCoor = new List<double[]>();
            List<int> allNodeIds = new List<int>();

            // each edge cell
            for (int i = 0; i < visualization.EdgeCellIdsByEdge[edgeId].Length; i++)
            {
                edgeCellId = visualization.EdgeCellIdsByEdge[edgeId][i];
                oneEdgeNodeIds = visualization.EdgeCells[edgeCellId];

                // for each cell add the first node
                allNodeIds.Add(oneEdgeNodeIds[0]);
                edgeNodeCoor.Add(_nodes[oneEdgeNodeIds[0]].Coor);
                // for parabolic cell add the the middle node
                if (oneEdgeNodeIds.Length == 3)
                {
                    allNodeIds.Add(oneEdgeNodeIds[2]);
                    edgeNodeCoor.Add(_nodes[oneEdgeNodeIds[2]].Coor);
                }
                // for the last cell add the last node
                if (i == visualization.EdgeCellIdsByEdge[edgeId].Length - 1)
                {
                    allNodeIds.Add(oneEdgeNodeIds[1]);
                    edgeNodeCoor.Add(_nodes[oneEdgeNodeIds[1]].Coor);
                }
            }
            nodeIds = allNodeIds.ToArray();
            nodeCoor = edgeNodeCoor.ToArray();
        }

        // Faces
        public void GetFaceNodes(int geometryFaceId, out int[] nodeIds)
        {
            int[] itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometryFaceId);
            VisualizationData vis = GetPartById(itemTypePartIds[2]).Visualization;
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            //
            if (geomType == GeometryType.SolidSurface ||
                geomType == GeometryType.ShellFrontSurface ||
                geomType == GeometryType.ShellBackSurface)
            {
                int cellId;
                int faceId = itemTypePartIds[0];
                HashSet<int> allNodeIds = new HashSet<int>();
                List<double[]> nodeCoor = new List<double[]>();
                //
                for (int i = 0; i < vis.CellIdsByFace[faceId].Length; i++)
                {
                    cellId = vis.CellIdsByFace[faceId][i];
                    allNodeIds.UnionWith(vis.Cells[cellId]);
                }
                //
                nodeIds = allNodeIds.ToArray();
            }
            else throw new NotSupportedException();
        }
        public void GetFaceNodeLumpedWeights(VisualizationData visualization, int faceId,
            Dictionary<int, FeNode> nodes, out Dictionary<int, double> weights, out double area)
        {
            int[] cell;
            int faceCellId;
            double faceArea;
            double[] faceWeights;
            double a = 1.0 / 3.0;
            double b = 1.0 / 4.0;
            double c = 1.0 / 12.0;
            double d = 3.0 / 12.0;
            double e = 1.0 / 16.0;
            double f = 3.0 / 16.0;
            area = 0;
            weights = new Dictionary<int, double>();
            // For each face cell
            for (int i = 0; i < visualization.CellIdsByFace[faceId].Length; i++)
            {
                faceArea = 0;
                faceCellId = visualization.CellIdsByFace[faceId][i];
                cell = visualization.Cells[faceCellId];
                //
                if (cell.Length == 3)
                {
                    faceArea = GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]]);
                    faceWeights = new double[] { faceArea * a, faceArea * a, faceArea * a };
                }
                else if (cell.Length == 4)
                {
                    faceArea += GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                            nodes[cell[3]]);
                    faceWeights = new double[] { faceArea * b, faceArea * b, faceArea * b, faceArea * b };
                }
                else if (cell.Length == 6)
                {
                    faceArea += GeometryTools.TriangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                           nodes[cell[3]], nodes[cell[4]], nodes[cell[5]]);
                    faceWeights = new double[] { faceArea * c, faceArea * c, faceArea * c,
                        faceArea * d, faceArea * d, faceArea * d };
                }
                else if (cell.Length == 8)
                {
                    faceArea += GeometryTools.RectangleArea(nodes[cell[0]], nodes[cell[1]], nodes[cell[2]],
                                                            nodes[cell[3]], nodes[cell[4]], nodes[cell[5]],
                                                            nodes[cell[6]], nodes[cell[7]]);
                    faceWeights = new double[] { faceArea * e, faceArea * e, faceArea * e, faceArea * e,
                        faceArea * f, faceArea * f, faceArea * f, faceArea * f  };
                }
                else throw new NotSupportedException();
                //
                for (int j = 0; j < cell.Length; j++)
                {
                    if (weights.ContainsKey(cell[j])) weights[cell[j]] += faceWeights[j];
                    else weights.Add(cell[j], faceWeights[j]);
                }
                //
                area += faceArea;
            }
        }

        // Analyze
        public double GetShortestEdgeLen(string[] partNames)
        {
            double min = double.MaxValue;
            VisualizationData visualization;
            BasePart part;
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // For each edge
                for (int i = 0; i < visualization.EdgeLengths.Length; i++)
                {
                    if (visualization.EdgeLengths[i] < min)
                        min = visualization.EdgeLengths[i];
                }
            }
            return min;
        }
        /// <summary>
        /// Returns an array of edges. Each edge is an array of connecteds points and each point is a 3D coordinate - array
        /// </summary>
        /// <param name="minEdgeLen"></param>
        /// <returns></returns>
        public double[][][] GetShortEdges(double minEdgeLen, string[] partNames)
        {
            double[][] nodeCoor;
            List<double[][]> allNodeCoor = new List<double[][]>();
            VisualizationData visualization;
            BasePart part;
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // For each edge
                for (int i = 0; i < visualization.EdgeLengths.Length; i++)
                {
                    if (visualization.EdgeLengths[i] < minEdgeLen)
                    {
                        GetEdgeNodeCoor(i, visualization, out int[] nodeIds, out nodeCoor);
                        allNodeCoor.Add(nodeCoor);
                    }
                }
            }
            return allNodeCoor.ToArray();
        }
        //
        public double GetClosestUnConnectedEdgesDistance(string[] partNames)
        {
            double min = double.MaxValue;
            VisualizationData visualization;
            BasePart part;
            double dist;
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // For each face
                for (int i = 0; i < visualization.FaceEdgeIds.Length; i++)
                {
                    dist = GetClosestUnConnectedEdgesDistanceOnFace(visualization, i);
                    if (dist < min) min = dist;
                }
            }
            return min;
        }
        public double GetClosestUnConnectedEdgesDistanceOnFace(VisualizationData visualization, int faceId)
        {
            double min = double.MaxValue;
            int edge1Id;
            int edge2Id;
            double dist;
            // For each face edge
            for (int i = 0; i < visualization.FaceEdgeIds[faceId].Length - 1; i++)
            {
                edge1Id = visualization.FaceEdgeIds[faceId][i];
                // Compare to each other face edge
                for (int j = i + 1; j < visualization.FaceEdgeIds[faceId].Length; j++)
                {
                    edge2Id = visualization.FaceEdgeIds[faceId][j];
                    // Get edge to edge distance; for connected edges the distance is -1
                    dist = EdgeToEdgeMinDistance(visualization, edge1Id, edge2Id);
                    //
                    if (dist != -1 && dist < min) min = dist;
                }
            }
            return min;
        }
        public double[][][] ShowCloseUnConnectedEdges(double minDistance, string[] partNames)
        {
            VisualizationData visualization;
            BasePart part;
            int edge1Id;
            int edge2Id;
            double dist;
            double[][] nodeCoor;
            List<double[][]> allNodeCoor = new List<double[][]>();
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // For each face
                for (int i = 0; i < visualization.FaceEdgeIds.Length; i++)
                {
                    // For each face edge
                    for (int j = 0; j < visualization.FaceEdgeIds[i].Length - 1; j++)
                    {
                        edge1Id = visualization.FaceEdgeIds[i][j];
                        // Compare to each other face edge
                        for (int k = j + 1; k < visualization.FaceEdgeIds[i].Length; k++)
                        {
                            edge2Id = visualization.FaceEdgeIds[i][k];
                            // Get edge to edge distance; for connected edges the distance is -1
                            dist = EdgeToEdgeMinDistance(visualization, edge1Id, edge2Id);
                            //
                            if (dist != -1 && dist <= minDistance)
                            {
                                GetEdgeNodeCoor(edge1Id, visualization, out int[] nodeIds1, out nodeCoor);
                                allNodeCoor.Add(nodeCoor);
                                GetEdgeNodeCoor(edge2Id, visualization, out int[] nodeIds2, out nodeCoor);
                                allNodeCoor.Add(nodeCoor);
                            }
                        }
                    }
                }
            }
            return allNodeCoor.ToArray();
        }
        private double EdgeToEdgeMinDistance(VisualizationData visualization, int loaclEdgeId1, int loaclEdgeId2)
        {
            HashSet<int> edge1NodeIds = new HashSet<int>();
            HashSet<int> edge2NodeIds = new HashSet<int>();
            double dist;
            double min = double.MaxValue;
            double[] a1;
            double[] a2;
            double[] b1;
            double[] b2;
            int[] edgeCell;
            int[][] edge1Cells = new int[visualization.EdgeCellIdsByEdge[loaclEdgeId1].Length][];
            int[][] edge2Cells = new int[visualization.EdgeCellIdsByEdge[loaclEdgeId2].Length][];
            // For each edge 1 cell
            for (int i = 0; i < visualization.EdgeCellIdsByEdge[loaclEdgeId1].Length; i++)
            {
                edgeCell = visualization.EdgeCells[visualization.EdgeCellIdsByEdge[loaclEdgeId1][i]];
                edge1Cells[i] = edgeCell;
                edge1NodeIds.UnionWith(edgeCell);
                if (edgeCell.Length != 2) throw new NotSupportedException();
            }
            // For each edge 2 cell
            for (int i = 0; i < visualization.EdgeCellIdsByEdge[loaclEdgeId2].Length; i++)
            {
                edgeCell = visualization.EdgeCells[visualization.EdgeCellIdsByEdge[loaclEdgeId2][i]];
                edge2Cells[i] = edgeCell;
                edge2NodeIds.UnionWith(edgeCell);
                if (edgeCell.Length != 2) throw new NotSupportedException();
            }
            // For connected edges
            if (edge1NodeIds.Intersect(edge2NodeIds).Count() != 0)
            {
                return -1;
            }
            // For un-connected edges
            else
            {
                for (int i = 0; i < edge1Cells.Length; i++)
                {
                    for (int j = 0; j < edge2Cells.Length; j++)
                    {
                        a1 = _nodes[edge1Cells[i][0]].Coor;
                        a2 = _nodes[edge1Cells[i][1]].Coor;
                        b1 = _nodes[edge2Cells[j][0]].Coor;
                        b2 = _nodes[edge2Cells[j][1]].Coor;
                        dist = Geometry.SegmentToSegmentDistance(a1, a2, b1, b2);
                        if (dist < min) min = dist;
                    }
                }
                return min;
            }
        }
        //
        public double GetSmallestFace(string[] partNames)
        {
            double min = double.MaxValue;
            VisualizationData visualization;
            BasePart part;
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // for each face
                for (int i = 0; i < visualization.FaceAreas.Length; i++)
                {
                    if (visualization.FaceAreas[i] < min) min = visualization.FaceAreas[i];
                }
            }
            return min;
        }
        public int[][] GetSmallestFaces(double minFaceArea, string[] partNames)
        {
            int cellId;
            int[] cell;
            List<int[]> cells = new List<int[]>();
            //
            VisualizationData visualization;
            BasePart part;
            //
            foreach (var partName in partNames)
            {
                part = _parts[partName];
                if (part is CaeMesh.CompoundGeometryPart) continue;
                //
                visualization = part.Visualization;
                // for each face
                for (int i = 0; i < visualization.FaceAreas.Length; i++)
                {
                    if (visualization.FaceAreas[i] < minFaceArea)
                    {
                        // each edge cell
                        for (int j = 0; j < visualization.CellIdsByFace[i].Length; j++)
                        {
                            cellId = visualization.CellIdsByFace[i][j];
                            cell = visualization.Cells[cellId];
                            cells.Add(cell.ToArray());
                        }
                    }
                }
            }
            return cells.ToArray();
        }

        // Read - Write
        public void WriteEdgeNodesToFile(GeometryPart part, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("int		number of additional edges");
            sb.AppendLine(part.Visualization.EdgeCells.Length.ToString());

            FeNode node1;
            FeNode node2;
            foreach (int[] cell in part.Visualization.EdgeCells)
            {
                if (cell.Length > 2) throw new NotSupportedException();
                node1 = Nodes[cell[0]];
                node2 = Nodes[cell[1]];
                sb.AppendLine(String.Format("{0} {1} {2}", (float)node1.X, (float)node1.Y, (float)node1.Z));
                sb.AppendLine(String.Format("{0} {1} {2}", (float)node2.X, (float)node2.Y, (float)node2.Z));
            }

            System.IO.File.WriteAllText(fileName, sb.ToString());
        }

        // Operations
        public string[] TranslateParts(string[] partNames, double[] translateVector, bool copy,
                                       ICollection<string> reservedPartNames)
        {
            string[] translatedPartNames = partNames.ToArray();
            //
            if (copy)
            {
                FeMesh mesh = this.DeepCopy();
                translatedPartNames = this.AddPartsFromMesh(mesh, translatedPartNames, reservedPartNames);
            }
            //
            HashSet<int> nodeLabels = new HashSet<int>();
            foreach (var partName in translatedPartNames) nodeLabels.UnionWith(_parts[partName].NodeLabels);
            // Translate nodes
            FeNode node;
            foreach (var nodeId in nodeLabels)
            {
                node = _nodes[nodeId];
                node.X += translateVector[0];
                node.Y += translateVector[1];
                node.Z += translateVector[2];
                _nodes[nodeId] = node;
            }
            // Update node sets
            foreach (var entry in _nodeSets) UpdateNodeSetCenterOfGravity(entry.Value);
            // Update reference points
            foreach (var entry in _referencePoints) UpdateReferencePoint(entry.Value);
            // Update bounding box
            ComputeBoundingBox();
            //
            if (copy) return translatedPartNames;
            else return null;
        }
        public string[] ScaleParts(string[] partNames, double[] scaleCenter, double[] scaleFactors, bool copy,
                                   ICollection<string> reservedPartNames)
        {
            string[] scaledPartNames = partNames.ToArray();
            //
            if (copy)
            {
                FeMesh mesh = this.DeepCopy();
                scaledPartNames = this.AddPartsFromMesh(mesh, scaledPartNames, reservedPartNames);
            }
            //
            HashSet<int> nodeLabels = new HashSet<int>();
            foreach (var partName in scaledPartNames) nodeLabels.UnionWith(_parts[partName].NodeLabels);
            // Scale nodes
            FeNode node;
            foreach (var nodeId in nodeLabels)
            {
                node = _nodes[nodeId];
                node.X = scaleCenter[0] + (node.X - scaleCenter[0]) * scaleFactors[0];
                node.Y = scaleCenter[1] + (node.Y - scaleCenter[1]) * scaleFactors[1];
                node.Z = scaleCenter[2] + (node.Z - scaleCenter[2]) * scaleFactors[2];
                _nodes[nodeId] = node;
            }
            // Update edge lengths and face areas
            BasePart part;
            foreach (var partName in scaledPartNames)
            {
                part = _parts[partName];
                // Recompute the areas and lengths
                ComputeEdgeLengths(part);
                ComputeFaceAreas(part);
            }
            // Update node sets
            foreach (var entry in _nodeSets) UpdateNodeSetCenterOfGravity(entry.Value);
            // Update reference points
            foreach (var entry in _referencePoints) UpdateReferencePoint(entry.Value);
            // Update bounding box
            ComputeBoundingBox();
            
            // Update visibility
            if (copy) return scaledPartNames;
            else return null;
        }
        public string[] RotateParts(string[] partNames, double[] rotateCenter, double[] rotateAxis, double rotateAngle, bool copy,
                                    ICollection<string> reservedPartNames)
        {
            string[] rotatedPartNames = partNames.ToArray();
            //
            if (copy)
            {
                FeMesh mesh = this.DeepCopy();
                rotatedPartNames = this.AddPartsFromMesh(mesh, rotatedPartNames, reservedPartNames);
            }
            //
            HashSet<int> nodeLabels = new HashSet<int>();
            foreach (var partName in rotatedPartNames) nodeLabels.UnionWith(_parts[partName].NodeLabels);
            // Rotate nodes
            FeNode node;
            double[] x;
            double[][] m = RotationMatrix(rotateAxis, rotateAngle);
            foreach (var nodeId in nodeLabels)
            {
                node = _nodes[nodeId];

                // translate to origin
                node.X -= rotateCenter[0];
                node.Y -= rotateCenter[1];
                node.Z -= rotateCenter[2];
                // copy
                x = node.Coor;
                // rotate
                node.X = m[0][0] * x[0] + m[0][1] * x[1] + m[0][2] * x[2];
                node.Y = m[1][0] * x[0] + m[1][1] * x[1] + m[1][2] * x[2];
                node.Z = m[2][0] * x[0] + m[2][1] * x[1] + m[2][2] * x[2];
                // translate to rotation center
                node.X += rotateCenter[0];
                node.Y += rotateCenter[1];
                node.Z += rotateCenter[2];
                // copy data
                _nodes[nodeId] = node;
            }
            // Update node sets
            foreach (var entry in _nodeSets) UpdateNodeSetCenterOfGravity(entry.Value);
            // Update reference points
            foreach (var entry in _referencePoints) UpdateReferencePoint(entry.Value);
            // Update bounding box
            ComputeBoundingBox();
            //
            if (copy) return rotatedPartNames;
            else return null;
        }

        private double[][] RotationMatrix(double[] rotateAxis, double rotateAngle)
        {
            //https://stackoverflow.com/questions/6721544/circular-rotation-around-an-arbitrary-axis
            //https://en.wikipedia.org/wiki/Rotation_matrix#Rotation_matrix_from_axis_and_angle

            double[][] m = new double[3][];
            m[0] = new double[3];
            m[1] = new double[3];
            m[2] = new double[3];

            Vec3D u = new Vec3D(rotateAxis);
            u.Normalize();

            double cosPhi = Math.Cos(rotateAngle);
            double sinPhi = Math.Sin(rotateAngle);

            m[0][0] = cosPhi + (Math.Pow(u.X, 2) * (1 - cosPhi));
            m[0][1] = u.X * u.Y * (1 - cosPhi) - u.Z * sinPhi;
            m[0][2] = u.X * u.Z * (1 - cosPhi) + u.Y * sinPhi;

            m[1][0] = u.Y * u.X * (1 - cosPhi) + u.Z * sinPhi;
            m[1][1] = cosPhi + (Math.Pow(u.Y, 2) * (1 - cosPhi));
            m[1][2] = u.Y * u.Z * (1 - cosPhi) - u.X * sinPhi;

            m[2][0] = u.Z * u.X * (1 - cosPhi) - u.Y * sinPhi;
            m[2][1] = u.Z * u.Y * (1 - cosPhi) + u.X * sinPhi;
            m[2][2] = cosPhi + (Math.Pow(u.Z, 2) * (1 - cosPhi));

            return m;
        }

        public string[] CreatePrismaticBoundaryLayer(int[] geometrySurfaceIds, double thickness)
        {
            // GeometryId = itemId * 100000 + typeId * 10000 + partId
            List<string> errors = new List<string>();
            int cellId;
            int partId;
            int[] cell;
            int[] surfaceCellIds;
            int[] itemTypePartIds;
            Vec3D normal;
            MeshPart part;
            VisualizationData vis;           
            List<Vec3D> normalsList;
            Dictionary<int, List<Vec3D>> nodeIdNormals;
            HashSet<int> surfaceIds;
            Dictionary<int, HashSet<int>> partIdSelectedSurfaceIds = new Dictionary<int, HashSet<int>>();
            // Get surface ids by part
            foreach (var geometrySurfaceId in geometrySurfaceIds)
            {
                itemTypePartIds = GetItemTypePartIdsFromGeometryId(geometrySurfaceId);
                // Use only solid geometry surface ids
                if (itemTypePartIds[1] == (int)GeometryType.SolidSurface)
                {
                    if (partIdSelectedSurfaceIds.TryGetValue(itemTypePartIds[2], out surfaceIds)) surfaceIds.Add(itemTypePartIds[0]);
                    else partIdSelectedSurfaceIds.Add(itemTypePartIds[2], new HashSet<int>() { itemTypePartIds[0] });
                }
            }
            //
            HashSet<int[]> modifiedCells;
            Dictionary<int, HashSet<int>> edgeIdNodeIds;
            Dictionary<int, HashSet<int>> surfaceIdNodeIds;
            HashSet<int> nodeSurfaceIds;
            Dictionary<int, HashSet<int>> nodeIdSelectedSurfaceIds;
            HashSet<int> selectedSurfaceAndNeighboursIds = new HashSet<int>();
            Dictionary<int, HashSet<int>> surfaceIdSurfaceNeighbourIds;
            // Create new layer for each part
            foreach (var partIdSelectedSurfaceIdEntry in partIdSelectedSurfaceIds)
            {
                modifiedCells = new HashSet<int[]>();
                nodeIdSelectedSurfaceIds = new Dictionary<int, HashSet<int>>();
                nodeIdNormals = new Dictionary<int, List<Vec3D>>();
                //
                partId = partIdSelectedSurfaceIdEntry.Key;
                part = (MeshPart)GetPartById(partId);
                if (part == null) continue;
                if (part.PartType == PartType.Solid)
                {
                    //
                    vis = part.Visualization;
                    edgeIdNodeIds = vis.GetNodeIdsByEdges();
                    surfaceIdNodeIds = vis.GetNodeIdsBySurfaces();
                    // Check that all selected surfaces are connected over edge                                                         
                    if (partIdSelectedSurfaceIdEntry.Value.Count > 1)
                    {
                        surfaceIdSurfaceNeighbourIds = vis.GetSurfaceNeighboursData();
                        selectedSurfaceAndNeighboursIds.Clear();
                        foreach (var surfaceId in partIdSelectedSurfaceIdEntry.Value)
                            selectedSurfaceAndNeighboursIds.UnionWith(surfaceIdSurfaceNeighbourIds[surfaceId]);
                        // If surfaces are connected all of them are also their neighbours
                        bool connected = true;
                        foreach (var surfaceId in partIdSelectedSurfaceIdEntry.Value)
                        {
                            if (!selectedSurfaceAndNeighboursIds.Contains(surfaceId))
                            {
                                connected = false;
                                break;
                            }
                        }
                        if (!connected)
                        {
                            string partName = GetPartNamesByIds(new int[] { partIdSelectedSurfaceIdEntry.Key })[0];
                            errors.Add("The surfaces selected on part '" + partName + "' are not connected." +
                                       " Boundary layer can not be created.");
                            continue;
                        }
                    }
                    // Collect all node normals                                                                                         
                    foreach (var surfaceId in partIdSelectedSurfaceIdEntry.Value)
                    {
                        surfaceCellIds = vis.CellIdsByFace[surfaceId];
                        foreach (var surfaceCellId in surfaceCellIds)
                        {
                            cell = vis.Cells[surfaceCellId];
                            cellId = vis.CellIds[surfaceCellId];
                            modifiedCells.Add(cell);
                            normal = -1 * new Vec3D(ComputeNormalFromFaceCellIndices(cell).Coor);       // only first three nodes are used
                                                                                                    //
                            foreach (var nodeId in cell)
                            {
                                if (nodeIdNormals.TryGetValue(nodeId, out normalsList)) normalsList.Add(normal);
                                else nodeIdNormals.Add(nodeId, new List<Vec3D>() { normal });
                                // Save surface id for each node - egde and vertex nodes can have multiple surface ids
                                if (nodeIdSelectedSurfaceIds.TryGetValue(nodeId, out nodeSurfaceIds)) nodeSurfaceIds.Add(surfaceId);
                                else nodeIdSelectedSurfaceIds.Add(nodeId, new HashSet<int>() { surfaceId });
                            }
                        }
                    }
                    // Collect nodeIds and compute their average normals                                                                
                    int count = 0;
                    int[] nodeIds = new int[nodeIdNormals.Count];
                    Vec3D[] normals = new Vec3D[nodeIdNormals.Count];
                    Vec3D[] allNodeNormals;
                    double alpha;
                    double maxAlpha;
                    double factor;
                    foreach (var entry in nodeIdNormals)
                    {
                        maxAlpha = 0;
                        allNodeNormals = entry.Value.ToArray();
                        for (int i = 0; i < allNodeNormals.Length - 1; i++)
                        {
                            for (int j = i + 1; j < allNodeNormals.Length; j++)
                            {
                                alpha = Math.Abs(Vec3D.DotProduct(allNodeNormals[i], allNodeNormals[j]));
                                alpha = alpha > 1 ? 1 : Math.Acos(alpha);
                                if (alpha > maxAlpha) maxAlpha = alpha;
                            }
                        }
                        factor = Math.Abs(Math.Cos(maxAlpha / 2));
                        factor = factor < 0.001 ? 1 : 1 / factor;
                        //
                        nodeIds[count] = entry.Key;
                        //
                        normal = new Vec3D();
                        foreach (var nodeNormal in entry.Value) normal += nodeNormal;
                        //normals[count] = normal * (1.0 / entry.Value.Count());
                        normal.Normalize();
                        normals[count] = normal * factor;
                        count++;
                    }
                    // Add new nodes pressed-into the part                                                                              
                    int oldNodeId;
                    FeNode node;
                    Vec3D newPosition;
                    Dictionary<int, int> oldNewNodeIds = new Dictionary<int, int>();
                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        _maxNodeId++;
                        oldNodeId = nodeIds[i];
                        newPosition = new Vec3D(_nodes[oldNodeId].Coor) + thickness * normals[i];
                        node = new FeNode(_maxNodeId, newPosition.Coor);
                        _nodes.Add(node.Id, node);
                        oldNewNodeIds.Add(oldNodeId, node.Id);
                    }
                    // Replace old element nodes for the nodes pressed-into the part                                                    
                    HashSet<int> modifiedElementNodeIds = new HashSet<int>();
                    int newNodeId;
                    FeElement element;
                    foreach (var entry in _elements)
                    {
                        element = entry.Value;
                        if (element.PartId == part.PartId)
                        {
                            modifiedElementNodeIds.Clear();
                            for (int i = 0; i < element.NodeIds.Length; i++)
                            {
                                if (oldNewNodeIds.TryGetValue(element.NodeIds[i], out newNodeId))
                                {
                                    element.NodeIds[i] = newNodeId;
                                    modifiedElementNodeIds.Add(newNodeId);
                                }
                            }
                            if (element.NodeIds.Length >= 10 && modifiedElementNodeIds.Count > 0)
                            {
                                RepositionMidNodes(element, modifiedElementNodeIds);
                            }
                        }
                    }
                    // Create and add new wedge elements                                                                                
                    FeElement3D element3D;
                    Dictionary<int, FeElement> newElements = new Dictionary<int, FeElement>();
                    List<int[]> newElementCells = new List<int[]>();
                    int[] elNodeIds;
                    HashSet<Type> newElementTypes = new HashSet<Type>();
                    FeNode midNode;
                    CompareIntArray comparer = new CompareIntArray();
                    Dictionary<int[], FeNode> midNodes = new Dictionary<int[], FeNode>(comparer);
                    foreach (var modifiedCell in modifiedCells)
                    {
                        _maxElementId++;
                        if (modifiedCell.Length == 3)
                        {
                            // Linear wedge element
                            elNodeIds = new int[6];
                            elNodeIds[0] = oldNewNodeIds[modifiedCell[0]];
                            elNodeIds[1] = oldNewNodeIds[modifiedCell[1]];
                            elNodeIds[2] = oldNewNodeIds[modifiedCell[2]];
                            elNodeIds[3] = modifiedCell[0];
                            elNodeIds[4] = modifiedCell[1];
                            elNodeIds[5] = modifiedCell[2];
                            element3D = new LinearWedgeElement(_maxElementId, partId, elNodeIds);
                        }
                        else if (modifiedCell.Length == 4)
                        {
                            // Linear hexa element
                            elNodeIds = new int[8];
                            elNodeIds[0] = oldNewNodeIds[modifiedCell[0]];
                            elNodeIds[1] = oldNewNodeIds[modifiedCell[1]];
                            elNodeIds[2] = oldNewNodeIds[modifiedCell[2]];
                            elNodeIds[3] = oldNewNodeIds[modifiedCell[3]];
                            elNodeIds[4] = modifiedCell[0];
                            elNodeIds[5] = modifiedCell[1];
                            elNodeIds[6] = modifiedCell[2];
                            elNodeIds[7] = modifiedCell[3];
                            element3D = new LinearHexaElement(_maxElementId, partId, elNodeIds);
                        }
                        else if (modifiedCell.Length == 6)
                        {
                            // Parabolic wedge element
                            elNodeIds = new int[15];
                            elNodeIds[0] = oldNewNodeIds[modifiedCell[0]];
                            elNodeIds[1] = oldNewNodeIds[modifiedCell[1]];
                            elNodeIds[2] = oldNewNodeIds[modifiedCell[2]];
                            elNodeIds[3] = modifiedCell[0];
                            elNodeIds[4] = modifiedCell[1];
                            elNodeIds[5] = modifiedCell[2];
                            elNodeIds[6] = oldNewNodeIds[modifiedCell[3]];
                            elNodeIds[7] = oldNewNodeIds[modifiedCell[4]];
                            elNodeIds[8] = oldNewNodeIds[modifiedCell[5]];
                            elNodeIds[9] = modifiedCell[3];
                            elNodeIds[10] = modifiedCell[4];
                            elNodeIds[11] = modifiedCell[5];
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[0]], _nodes[elNodeIds[3]], ref midNodes, ref _maxNodeId);
                            elNodeIds[12] = midNode.Id;
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[1]], _nodes[elNodeIds[4]], ref midNodes, ref _maxNodeId);
                            elNodeIds[13] = midNode.Id;
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[2]], _nodes[elNodeIds[5]], ref midNodes, ref _maxNodeId);
                            elNodeIds[14] = midNode.Id;
                            //
                            element3D = new ParabolicWedgeElement(_maxElementId, partId, elNodeIds);
                        }
                        else if (modifiedCell.Length == 8)
                        {
                            // Parabolic wedge element
                            elNodeIds = new int[20];
                            elNodeIds[0] = oldNewNodeIds[modifiedCell[0]];
                            elNodeIds[1] = oldNewNodeIds[modifiedCell[1]];
                            elNodeIds[2] = oldNewNodeIds[modifiedCell[2]];
                            elNodeIds[3] = oldNewNodeIds[modifiedCell[3]];
                            elNodeIds[4] = modifiedCell[0];
                            elNodeIds[5] = modifiedCell[1];
                            elNodeIds[6] = modifiedCell[2];
                            elNodeIds[7] = modifiedCell[3];

                            elNodeIds[8] = oldNewNodeIds[modifiedCell[4]];
                            elNodeIds[9] = oldNewNodeIds[modifiedCell[5]];
                            elNodeIds[10] = oldNewNodeIds[modifiedCell[6]];
                            elNodeIds[11] = oldNewNodeIds[modifiedCell[7]];

                            elNodeIds[12] = modifiedCell[4];
                            elNodeIds[13] = modifiedCell[5];
                            elNodeIds[14] = modifiedCell[6];
                            elNodeIds[15] = modifiedCell[7];

                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[0]], _nodes[elNodeIds[4]], ref midNodes, ref _maxNodeId);
                            elNodeIds[16] = midNode.Id;
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[1]], _nodes[elNodeIds[5]], ref midNodes, ref _maxNodeId);
                            elNodeIds[17] = midNode.Id;
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[2]], _nodes[elNodeIds[6]], ref midNodes, ref _maxNodeId);
                            elNodeIds[18] = midNode.Id;
                            midNode = GetOrCreateMidNode(_nodes[elNodeIds[3]], _nodes[elNodeIds[7]], ref midNodes, ref _maxNodeId);
                            elNodeIds[19] = midNode.Id;
                            //
                            element3D = new ParabolicHexaElement(_maxElementId, partId, elNodeIds);
                        }
                        else throw new NotSupportedException();
                        // Add elements
                        newElements.Add(element3D.Id, element3D);
                        _elements.Add(element3D.Id, element3D);
                        newElementTypes.Add(element3D.GetType());
                    }
                    // Add mid nodes to the nodes collection
                    foreach (var entry in midNodes) _nodes.Add(entry.Value.Id, entry.Value);

                    //


                    // Find outside node ids of all selected surfaces                                                                   
                    Dictionary<int, int[]> edgeIdEdgeCount = new Dictionary<int, int[]>();
                    int[] edgeCount;
                    HashSet<int> selectedEdgeIds = new HashSet<int>();
                    foreach (var surfaceId in partIdSelectedSurfaceIdEntry.Value)
                    {
                        foreach (var edgeId in vis.FaceEdgeIds[surfaceId])
                        {
                            if (edgeIdEdgeCount.TryGetValue(edgeId, out edgeCount)) edgeCount[0]++;
                            else edgeIdEdgeCount.Add(edgeId, new int[] { 1 });
                            //
                            selectedEdgeIds.Add(edgeId);
                        }
                    }
                    HashSet<int> outsideNodeIds = new HashSet<int>();
                    foreach (var entry in edgeIdEdgeCount)
                    {
                        if (entry.Value[0] == 1)
                        {
                            foreach (var edgeCellId in vis.EdgeCellIdsByEdge[entry.Key])
                                outsideNodeIds.UnionWith(vis.EdgeCells[edgeCellId]);
                        }
                    }
                    //
                    int[] neighbouringNodeIds;
                    List<Tuple<int, int>> neighSurfIdsCount;
                    Dictionary<int, List<Tuple<int, int>>> outsideNodeIdNeighSurfId = new Dictionary<int, List<Tuple<int, int>>>();
                    for (int i = 0; i < vis.CellIdsByFace.Length; i++)
                    {
                        // Skip selected surfaces
                        if (!partIdSelectedSurfaceIdEntry.Value.Contains(i))
                        {
                            // Get common nodes
                            neighbouringNodeIds = surfaceIdNodeIds[i].Intersect(outsideNodeIds).ToArray();
                            foreach (var neighNodeId in neighbouringNodeIds)
                            {
                                if (outsideNodeIdNeighSurfId.TryGetValue(neighNodeId, out neighSurfIdsCount))
                                    neighSurfIdsCount.Add(new Tuple<int, int>(i, neighbouringNodeIds.Length));
                                else
                                {
                                    neighSurfIdsCount = new List<Tuple<int, int>>();
                                    neighSurfIdsCount.Add(new Tuple<int, int>(i, neighbouringNodeIds.Length));
                                    outsideNodeIdNeighSurfId.Add(neighNodeId, neighSurfIdsCount);
                                }
                            }
                        }
                    }
                    // Modify the existing edge and surface node ids for the visualization extraction                                   
                    // Add new node ids or remove old id from appropriate surfaces and edges
                    HashSet<int> allVertexNodes = new HashSet<int>(vis.VertexNodeIds);
                    int[] sortedNodeIds;
                    HashSet<int> newNodeIds = new HashSet<int>();
                    HashSet<int[]> newEdgeNodeIds = new HashSet<int[]>(comparer);
                    Dictionary<int, HashSet<int>> neighEdgeNodeIds = new Dictionary<int, HashSet<int>>();
                    foreach (var nodeId in outsideNodeIds)
                    {
                        // The node lies on the selected surface - edge or vertex node
                        // Edge or vertex node: add it to the neighbouring surfaces
                        // Vertex node: add it to the neighbouring edge
                        newNodeId = oldNewNodeIds[nodeId];
                        // It is a vertex node
                        if (allVertexNodes.Contains(nodeId))
                        {
                            // Correct EDGES
                            // Count the neighbouring edges connected to the vertex
                            neighEdgeNodeIds.Clear();
                            foreach (var entry in edgeIdNodeIds)
                            {
                                // Neighbouring edge is not on selected edge
                                if (selectedEdgeIds.Contains(entry.Key)) continue;
                                //
                                if (entry.Value.Contains(nodeId)) neighEdgeNodeIds.Add(entry.Key, entry.Value);
                            }
                            //
                            foreach (var entry in neighEdgeNodeIds)
                            {
                                // Only one not selected edge is connected to the vertex
                                if (neighEdgeNodeIds.Count == 1)
                                {
                                    // Add node
                                    entry.Value.Add(newNodeId);
                                    // Add midnode
                                    if (midNodes.Count > 0) entry.Value.Add(midNodes[new int[] { nodeId, newNodeId }].Id);
                                }
                                // More than only one not selected edge is connected to the vertex - create new vertex and new edge
                                else if (neighEdgeNodeIds.Count > 1)
                                {
                                    // Add node
                                    entry.Value.Add(newNodeId);
                                    // Remove the old node from the edge
                                    entry.Value.Remove(nodeId);
                                    // Create and add the new edge
                                    newNodeIds.Clear();
                                    newNodeIds.Add(nodeId);
                                    newNodeIds.Add(newNodeId);
                                    if (midNodes.Count > 0) newNodeIds.Add(midNodes[new int[] { nodeId, newNodeId }].Id);
                                    sortedNodeIds = newNodeIds.ToArray();
                                    Array.Sort(sortedNodeIds);
                                    newEdgeNodeIds.Add(sortedNodeIds);
                                }
                            }
                            // Correct SURFACES
                            outsideNodeIdNeighSurfId.TryGetValue(nodeId, out neighSurfIdsCount);
                            foreach (var neighSurfIdCount in neighSurfIdsCount)
                            {
                                // Selection shares an edge
                                if (neighSurfIdCount.Item2 > 1)
                                {
                                    // Add node
                                    surfaceIdNodeIds[neighSurfIdCount.Item1].Add(newNodeId);
                                    // Add midnode - it exists only on element edges
                                    if (midNodes.TryGetValue(new int[] { nodeId, newNodeId }, out node))
                                        surfaceIdNodeIds[neighSurfIdCount.Item1].Add(node.Id);
                                }
                                // Selection shares only a node
                                else if (neighSurfIdCount.Item2 == 1)
                                {
                                    // Remove the old node from the surface sharing only one node
                                    surfaceIdNodeIds[neighSurfIdCount.Item1].Remove(nodeId);
                                }
                            }
                        }
                        // It is an edge node
                        else
                        {
                            outsideNodeIdNeighSurfId.TryGetValue(nodeId, out neighSurfIdsCount);
                            foreach (var neighSurfIdCount in neighSurfIdsCount)
                            {
                                // Selection shares an edge
                                if (neighSurfIdCount.Item2 > 1)
                                {
                                    // Add node
                                    surfaceIdNodeIds[neighSurfIdCount.Item1].Add(newNodeId);
                                    // Add midnode - it exists only on element edges
                                    if (midNodes.TryGetValue(new int[] { nodeId, newNodeId }, out node))
                                        surfaceIdNodeIds[neighSurfIdCount.Item1].Add(node.Id);
                                }
                                // Selection shares only a node
                                else if (neighSurfIdCount.Item2 == 1) throw new NotSupportedException();
                            }
                        }
                    }

                    // Create new visualization 
                    HashSet<int> newPartNodeIds = new HashSet<int>(part.NodeLabels);
                    newPartNodeIds.UnionWith(oldNewNodeIds.Values);
                    foreach (var entry in midNodes) newPartNodeIds.Add(entry.Value.Id);
                    part.NodeLabels = newPartNodeIds.ToArray();
                    Array.Sort(part.NodeLabels);
                    //
                    part.Labels = part.Labels.Union(newElements.Keys).ToArray();
                    Array.Sort(part.Labels);
                    //
                    part.AddElementTypes(newElementTypes.ToArray());
                    part.Visualization = new VisualizationData();
                    part.Visualization.ExtractVisualizationCellsFromElements3D(_elements, part.Labels);
                    // This creates part.Visualization.CellNeighboursOverCellEdge;
                    ExtractEdgesFromShellByAngle(part, -1);
                    // Add new created edges
                    foreach (var edgeNodeIds in newEdgeNodeIds)
                        edgeIdNodeIds.Add(edgeIdNodeIds.Count, new HashSet<int>(edgeNodeIds));
                    // This calls SplitVisualizationEdgesAndFaces(part);
                    ConvertEdgeNodesToEdges(part, edgeIdNodeIds);
                    RenumberVisualizationSurfaces(part, surfaceIdNodeIds);
                    RenumberVisualizationEdges(part, edgeIdNodeIds);
                }
                else
                {
                    errors.Add("The part '" + part.Name + "' is not a solid part.");
                }
            }
            return errors.ToArray();
        }

        private void RepositionMidNodes(FeElement element, HashSet<int> modifiedElNodeIds)
        {
            if (modifiedElNodeIds == null) modifiedElNodeIds = new HashSet<int>();
            int[] n = element.NodeIds;
            if (element is ParabolicTetraElement)
            {
                if (!modifiedElNodeIds.Contains(n[4])) _nodes[n[4]] = new FeNode(n[4], GetMidNodeCoor(_nodes[n[0]], _nodes[n[1]]));
                if (!modifiedElNodeIds.Contains(n[5])) _nodes[n[5]] = new FeNode(n[5], GetMidNodeCoor(_nodes[n[1]], _nodes[n[2]]));
                if (!modifiedElNodeIds.Contains(n[6])) _nodes[n[6]] = new FeNode(n[6], GetMidNodeCoor(_nodes[n[2]], _nodes[n[0]]));
                if (!modifiedElNodeIds.Contains(n[7])) _nodes[n[7]] = new FeNode(n[7], GetMidNodeCoor(_nodes[n[0]], _nodes[n[3]]));
                if (!modifiedElNodeIds.Contains(n[8])) _nodes[n[8]] = new FeNode(n[8], GetMidNodeCoor(_nodes[n[1]], _nodes[n[3]]));
                if (!modifiedElNodeIds.Contains(n[9])) _nodes[n[9]] = new FeNode(n[9], GetMidNodeCoor(_nodes[n[2]], _nodes[n[3]]));
            }
            else if (element is ParabolicWedgeElement)
            {
                if (!modifiedElNodeIds.Contains(n[6])) _nodes[n[6]] = new FeNode(n[6], GetMidNodeCoor(_nodes[n[0]], _nodes[n[1]]));
                if (!modifiedElNodeIds.Contains(n[7])) _nodes[n[7]] = new FeNode(n[7], GetMidNodeCoor(_nodes[n[1]], _nodes[n[2]]));
                if (!modifiedElNodeIds.Contains(n[8])) _nodes[n[8]] = new FeNode(n[8], GetMidNodeCoor(_nodes[n[2]], _nodes[n[0]]));
                if (!modifiedElNodeIds.Contains(n[9])) _nodes[n[9]] = new FeNode(n[9], GetMidNodeCoor(_nodes[n[3]], _nodes[n[4]]));
                if (!modifiedElNodeIds.Contains(n[10])) _nodes[n[10]] = new FeNode(n[10], GetMidNodeCoor(_nodes[n[4]], _nodes[n[5]]));
                if (!modifiedElNodeIds.Contains(n[11])) _nodes[n[11]] = new FeNode(n[11], GetMidNodeCoor(_nodes[n[5]], _nodes[n[3]]));
                if (!modifiedElNodeIds.Contains(n[12])) _nodes[n[12]] = new FeNode(n[12], GetMidNodeCoor(_nodes[n[0]], _nodes[n[3]]));
                if (!modifiedElNodeIds.Contains(n[13])) _nodes[n[13]] = new FeNode(n[13], GetMidNodeCoor(_nodes[n[1]], _nodes[n[4]]));
                if (!modifiedElNodeIds.Contains(n[14])) _nodes[n[14]] = new FeNode(n[14], GetMidNodeCoor(_nodes[n[2]], _nodes[n[5]]));
            }
            else if (element is ParabolicHexaElement)
            {
                if (!modifiedElNodeIds.Contains(n[8])) _nodes[n[8]] = new FeNode(n[8], GetMidNodeCoor(_nodes[n[0]], _nodes[n[1]]));
                if (!modifiedElNodeIds.Contains(n[9])) _nodes[n[9]] = new FeNode(n[9], GetMidNodeCoor(_nodes[n[1]], _nodes[n[2]]));
                if (!modifiedElNodeIds.Contains(n[10])) _nodes[n[10]] = new FeNode(n[10], GetMidNodeCoor(_nodes[n[2]], _nodes[n[3]]));
                if (!modifiedElNodeIds.Contains(n[11])) _nodes[n[11]] = new FeNode(n[11], GetMidNodeCoor(_nodes[n[3]], _nodes[n[0]]));
                if (!modifiedElNodeIds.Contains(n[12])) _nodes[n[12]] = new FeNode(n[12], GetMidNodeCoor(_nodes[n[4]], _nodes[n[5]]));
                if (!modifiedElNodeIds.Contains(n[13])) _nodes[n[13]] = new FeNode(n[13], GetMidNodeCoor(_nodes[n[5]], _nodes[n[6]]));
                if (!modifiedElNodeIds.Contains(n[14])) _nodes[n[14]] = new FeNode(n[14], GetMidNodeCoor(_nodes[n[6]], _nodes[n[7]]));
                if (!modifiedElNodeIds.Contains(n[15])) _nodes[n[15]] = new FeNode(n[15], GetMidNodeCoor(_nodes[n[7]], _nodes[n[4]]));
                if (!modifiedElNodeIds.Contains(n[16])) _nodes[n[16]] = new FeNode(n[16], GetMidNodeCoor(_nodes[n[0]], _nodes[n[4]]));
                if (!modifiedElNodeIds.Contains(n[17])) _nodes[n[17]] = new FeNode(n[17], GetMidNodeCoor(_nodes[n[1]], _nodes[n[5]]));
                if (!modifiedElNodeIds.Contains(n[18])) _nodes[n[18]] = new FeNode(n[18], GetMidNodeCoor(_nodes[n[2]], _nodes[n[6]]));
                if (!modifiedElNodeIds.Contains(n[19])) _nodes[n[19]] = new FeNode(n[19], GetMidNodeCoor(_nodes[n[3]], _nodes[n[7]]));
            }
            else throw new NotSupportedException();
        }

        // Clone
        public FeMesh DeepCopy()
        {
            FeMesh copy = this.DeepClone();
            //
            copy.Nodes = new Dictionary<int, FeNode>();
            foreach (var entry in _nodes)
            {
                copy.Nodes.Add(entry.Key, entry.Value.DeepCopy());
            }
            //
            copy.Elements = new Dictionary<int, FeElement>();
            foreach (var entry in _elements)
            {
                copy.Elements.Add(entry.Key, entry.Value.DeepCopy());
            }
            //
            if (_octree != null) copy._octree = _octree.DeepClone();
            else copy._octree = null;
            //
            if (_partOffsets != null) copy._partOffsets = _partOffsets.DeepClone();
            else copy._partOffsets = null;
            //
            return copy;
        }

        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_meshRepresentation", _meshRepresentation, typeof(MeshRepresentation));
            info.AddValue("_meshRefinements", _meshRefinements, typeof(OrderedDictionary<string, FeMeshRefinement>));
            info.AddValue("_parts", _parts, typeof(OrderedDictionary<string, BasePart>));
            info.AddValue("_nodeSets", _nodeSets, typeof(OrderedDictionary<string, FeNodeSet>));
            info.AddValue("_elementSets", _elementSets, typeof(OrderedDictionary<string, FeElementSet>));
            info.AddValue("_surfaces", _surfaces, typeof(OrderedDictionary<string, FeSurface>));
            info.AddValue("_referencePoints", _referencePoints, typeof(OrderedDictionary<string, FeReferencePoint>));
            info.AddValue("_maxNodeId", _maxNodeId, typeof(int));
            info.AddValue("_maxElementId", _maxElementId, typeof(int));
            info.AddValue("_boundingBox", _boundingBox, typeof(BoundingBox));
        }
    }
}
