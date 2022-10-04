using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CaeResults
{
    [Serializable]
    public class FeResults //: ISerializable
    {
        // Variables                                                                                                                
        [NonSerialized]
        private Dictionary<int, int> _nodeIdsLookUp;            // [globalId][resultsId] for values
        [NonSerialized]
        private Dictionary<FieldData, Field> _fields;
        [NonSerialized]
        private Dictionary<int, FeNode> _undeformedNodes;
        [NonSerialized]
        private float _scale;
        [NonSerialized]
        private int _stepId;
        [NonSerialized]
        private int _stepIncrementId;
        //
        private string _hashName;
        private string _fileName;
        private FeMesh _mesh;
        private HistoryResults _history;
        private DateTime _dateTime;
        private UnitSystem _unitSystem;
        private string _deformationFieldOutputName;


        // Properties                                                                                                               
        public Dictionary<int, FeNode> UndeformedNodes { get { return _undeformedNodes; } }
        public string HashName { get { return _hashName; } set { _hashName = value; } }
        public string FileName { get { return _fileName; } set { _fileName = value; } }
        public FeMesh Mesh { get { return _mesh; } set { _mesh = value; } }        
        public DateTime DateTime { get { return _dateTime; } set { _dateTime = value; } }
        public UnitSystem UnitSystem { get { return _unitSystem; } set { _unitSystem = value; } }
        public string DeformationFieldOutputName
        {
            get
            {
                OrderedDictionary<string, string> possibleNames = GetPossibleDeformationFieldOutputNamesMap();
                foreach (var entry in possibleNames)
                {
                    if (entry.Value == _deformationFieldOutputName) return entry.Key;
                }
                return "";
            }
            set
            {
                OrderedDictionary<string, string> possibleNames = GetPossibleDeformationFieldOutputNamesMap();
                if (possibleNames.TryGetValue(value, out _deformationFieldOutputName)) { }
                else _deformationFieldOutputName = possibleNames.Values.First();
                // Reset global mesh deformation settings
                ResetScaleStepIncrement();
            }
        }


        // Constructor                                                                                                              
        public FeResults(string fileName)
        {
            _fileName = fileName;
            _hashName = Tools.GetRandomString(8);
            _mesh = null;
            _nodeIdsLookUp = null;
            _fields = new Dictionary<FieldData, Field>();
            _history = null;
            _unitSystem = new UnitSystem();
            _deformationFieldOutputName = FOFieldNames.Disp;
        }
        //public FeResults(SerializationInfo info, StreamingContext context)
        //{

        //}
        //// ISerialization
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    // Using typeof() works also for null fields
        //    //info.AddValue("_meshRepresentation", _meshRepresentation, typeof(MeshRepresentation));
        //    //info.AddValue("_meshRefinements", _meshRefinements, typeof(OrderedDictionary<string, FeMeshRefinement>));
        //    //info.AddValue("_parts", _parts, typeof(OrderedDictionary<string, BasePart>));
        //    //info.AddValue("_nodeSets", _nodeSets, typeof(OrderedDictionary<string, FeNodeSet>));
        //    //info.AddValue("_elementSets", _elementSets, typeof(OrderedDictionary<string, FeElementSet>));
        //    //info.AddValue("_surfaces", _surfaces, typeof(OrderedDictionary<string, FeSurface>));
        //    //info.AddValue("_referencePoints", _referencePoints, typeof(OrderedDictionary<string, FeReferencePoint>));
        //    //info.AddValue("_maxNodeId", _maxNodeId, typeof(int));
        //    //info.AddValue("_maxElementId", _maxElementId, typeof(int));
        //    //info.AddValue("_boundingBox", _boundingBox, typeof(BoundingBox));
        //}

        // Static methods                                                                                                           
        public static void WriteToFile(FeResults results, System.IO.BinaryWriter bw)
        {
            if (results == null)
            {
                bw.Write(-1);
            }
            else
            {
                bw.Write(1);
                // Mesh
                Dictionary<int, FeNode> tmp = results.Mesh.Nodes;
                results.Mesh.Nodes = results._undeformedNodes;
                FeMesh.WriteToBinaryFile(results.Mesh, bw);
                results.Mesh.Nodes = tmp;
                // Node lookup
                if (results._nodeIdsLookUp == null) bw.Write(-1);
                else
                {
                    bw.Write(1);
                    //
                    bw.Write(results._nodeIdsLookUp.Count);
                    foreach (var entry in results._nodeIdsLookUp)
                    {
                        bw.Write(entry.Key);
                        bw.Write(entry.Value);
                    }
                }
                // Fields
                if (results._fields == null) bw.Write(-1);
                else
                {
                    bw.Write(1);
                    //
                    bw.Write(results._fields.Count);
                    foreach (var entry in results._fields)
                    {
                        FieldData.WriteToFile(entry.Key, bw);
                        Field.WriteToFile(entry.Value, bw);
                    }
                }
            }
        }
        public static void ReadFromFile(FeResults results, System.IO.BinaryReader br, int version)
        {
            int numItems;
            FieldData fieldData;
            Field field;
            //
            int exist = br.ReadInt32();
            if (exist == 1)
            {
                // Mesh
                FeMesh.ReadFromBinaryFile(results.Mesh, br, version);
                results.InitializeUndeformedNodes();
                // Node lookup
                exist = br.ReadInt32();
                if (exist == 1)
                {
                    numItems = br.ReadInt32();
                    results._nodeIdsLookUp = new Dictionary<int, int>();
                    for (int i = 0; i < numItems; i++) results._nodeIdsLookUp.Add(br.ReadInt32(), br.ReadInt32());
                }
                // Fields
                exist = br.ReadInt32();
                if (exist == 1)
                {
                    numItems = br.ReadInt32();
                    results._fields = new Dictionary<FieldData, Field>();
                    for (int i = 0; i < numItems; i++)
                    {
                        fieldData = FieldData.ReadFromFile(br, version);
                        field = Field.ReadFromFile(br);
                        results._fields.Add(fieldData, field);
                    }
                }
            }
        }


        // Methods                                                                                                                  
        // Mesh                                     
        public void SetMesh(FeMesh mesh, Dictionary<int, int> nodeIdsLookUp)
        {
            _mesh = new FeMesh(mesh, mesh.Parts.Keys.ToArray());
            //
            List<BasePart> parts = new List<BasePart>();
            foreach (var entry in _mesh.Parts)
            {
                if (!(entry.Value is ResultPart)) parts.Add(entry.Value);
            }
            //
            foreach (var part in parts)
            {
                _mesh.Parts.Replace(part.Name, part.Name, new ResultPart(part));
            }
            //
            InitializeUndeformedNodes();
            //
            _nodeIdsLookUp = nodeIdsLookUp;
        }
        public string[] AddPartsFromMesh(FeMesh mesh, string[] partNames)
        {
            _mesh.Nodes = _undeformedNodes;
            string[] addedPartNames = _mesh.AddPartsFromMesh(mesh, partNames, null, false);
            InitializeUndeformedNodes();
            return addedPartNames;
        }
        public string[] RemoveParts(string[] partNames)
        {
            _mesh.Nodes = _undeformedNodes;
            string[] removedPartNames;
            _mesh.RemoveParts(partNames, out removedPartNames, false);
            InitializeUndeformedNodes();
            return removedPartNames;
        }
        private void InitializeUndeformedNodes()
        {
            _undeformedNodes = new Dictionary<int, FeNode>();
            foreach (var entry in _mesh.Nodes) _undeformedNodes.Add(entry.Key, new FeNode(entry.Value));
            ResetScaleStepIncrement();
        }
        private void ResetScaleStepIncrement()
        {
            _scale = -1;
            _stepId = -1;
            _stepIncrementId = -1;
        }
        public void AddResults(FeResults results)
        {
            int[] stepIds = GetAllStepIds();
            int lastStepId = stepIds.Last();
            int[] stepIncrementIds = GetIncrementIds(lastStepId);
            int lastStepIncrementId = stepIncrementIds.Last();
            string[] fieldNames = GetAllFieldNames();
            string[] componentNames;
            //
            float lastTime = 0;
            FieldData fieldData;
            foreach (var fieldName in fieldNames)
            {
                componentNames = GetFieldComponentNames(fieldName);
                foreach (var componentName in componentNames)
                {
                    fieldData = GetFieldData(fieldName, componentName, lastStepId, lastStepIncrementId);
                    if (fieldData.Valid)
                    {
                        if (fieldData.Time > lastTime) lastTime = fieldData.Time;
                    }
                }
            }
            //
            AddFieldOutputs(results, lastTime, lastStepId, lastStepIncrementId);
            AddHistoryOutputs(results, lastTime);
        }
        private void AddFieldOutputs(FeResults results, float lastTime, int lastStepId, int lastStepIncrementId)
        {
            Field lastWearDepthField = GetField(new FieldData(FOFieldNames.WearDepth, "", lastStepId, lastStepIncrementId));
            if (lastWearDepthField == null) throw new NotSupportedException();
            //
            Field currentField;
            FieldData fieldData;
            FieldComponent fc1;
            FieldComponent fc2;
            FieldComponent fc3;
            float[] values1;
            float[] values2;
            float[] values3;
            float[] magnitude;
            //float[][] vectors = GetLocalVectors(FOFieldNames.WearDepth);
            float[][] vectors = SubtractMeshPositions(results, this);
            float[][] vectorsWD = GetLocalVectors(FOFieldNames.WearDepth);
            // Add mesh deformation
            foreach (var entry in results._fields.ToArray())    // copy to modify
            {
                fieldData = entry.Key;
                currentField = entry.Value;
                //
                if (fieldData.StepId == 0 && fieldData.StepIncrementId == 0) continue; // Zero increment - Find all occurances!!!
                //
                if (fieldData.Name == FOFieldNames.WearDepth)
                {
                    // H1
                    values1 = currentField.GetComponentValues(FOComponentNames.H1);
                    magnitude = new float[values1.Length];
                    for (int i = 0; i < values1.Length; i++)
                    {
                        values1[i] += vectorsWD[0][i];
                        magnitude[i] += values1[i] * values1[i];
                    }
                    fc1 = new FieldComponent(FOComponentNames.H1, values1);
                    // H2
                    values2 = currentField.GetComponentValues(FOComponentNames.H2);
                    for (int i = 0; i < values2.Length; i++)
                    {
                        values2[i] += vectorsWD[1][i];
                        magnitude[i] += values2[i] * values2[i];
                    }
                    fc2 = new FieldComponent(FOComponentNames.H2, values2);
                    // H3
                    values3 = currentField.GetComponentValues(FOComponentNames.H3);
                    for (int i = 0; i < values3.Length; i++)
                    {
                        values3[i] += vectorsWD[2][i];
                        magnitude[i] += values3[i] * values3[i];
                    }
                    fc3 = new FieldComponent(FOComponentNames.H3, values3);
                    // Magnitude
                    for (int i = 0; i < magnitude.Length; i++) magnitude[i] = (float)Math.Sqrt(magnitude[i]);
                    // Create field
                    currentField = new Field(fieldData.Name);
                    currentField.AddComponent(FOComponentNames.All, magnitude);
                    currentField.AddComponent(fc1);
                    currentField.AddComponent(fc2);
                    currentField.AddComponent(fc3);
                    // Replace field
                    results.ReplaceField(fieldData, currentField);
                }
                else if (fieldData.Name == FOFieldNames.MeshDeformation || fieldData.Name == FOFieldNames.DispDeformationDepth)
                {
                    // U1
                    values1 = currentField.GetComponentValues(FOComponentNames.U1);
                    magnitude = new float[values1.Length];
                    for (int i = 0; i < values1.Length; i++)
                    {
                        values1[i] += vectors[0][i];
                        magnitude[i] += values1[i] * values1[i];
                    }
                    fc1 = new FieldComponent(FOComponentNames.U1, values1);
                    // U2
                    values2 = currentField.GetComponentValues(FOComponentNames.U2);
                    for (int i = 0; i < values2.Length; i++)
                    {
                        values2[i] += vectors[1][i];
                        magnitude[i] += values2[i] * values2[i];
                    }
                    fc2 = new FieldComponent(FOComponentNames.U2, values2);
                    // U3
                    values3 = currentField.GetComponentValues(FOComponentNames.U3);
                    for (int i = 0; i < values3.Length; i++)
                    {
                        values3[i] += vectors[2][i];
                        magnitude[i] += values3[i] * values3[i];
                    }
                    fc3 = new FieldComponent(FOComponentNames.U3, values3);
                    // Magnitude
                    for (int i = 0; i < magnitude.Length; i++) magnitude[i] = (float)Math.Sqrt(magnitude[i]);
                    // Create field
                    currentField = new Field(fieldData.Name);
                    currentField.AddComponent(FOComponentNames.All, magnitude);
                    currentField.AddComponent(fc1);
                    currentField.AddComponent(fc2);
                    currentField.AddComponent(fc3);
                    // Replace field
                    results.ReplaceField(fieldData, currentField);
                }
            }
            // Copy all fields
            foreach (var entry in results._fields)
            {
                fieldData = entry.Key;
                currentField = entry.Value;
                //
                if (fieldData.StepId == 0 && fieldData.StepIncrementId == 0) continue; // Zero increment - Find all occurances!!!
                //
                fieldData.Time += lastTime;
                fieldData.StepId += lastStepId;
                //
                _fields.Add(fieldData, currentField);
            }
        }
        private void AddHistoryOutputs(FeResults results, float lastTime)
        {
            // Append all entries
            HistoryResultEntries historyResultEntry;
            foreach (var setEntry in results._history.Sets)
            {
                foreach (var fieldEntry in setEntry.Value.Fields)
                {
                    foreach (var componentEntry in fieldEntry.Value.Components)
                    {
                        foreach (var entry in componentEntry.Value.Entries)
                        {
                            entry.Value.ShiftTime(lastTime);
                            historyResultEntry = GetHistoryResultEntry(setEntry.Key, fieldEntry.Key, componentEntry.Key, entry.Key);
                            if (historyResultEntry != null)
                            {
                                historyResultEntry.Append(entry.Value);
                            }
                        }
                    }
                }
            }
        }
        private static float[][] SubtractMeshPositions(FeResults results1, FeResults results2)
        {
            if (results1._undeformedNodes.Count == results2._undeformedNodes.Count)
            {
                int id;
                FeNode node1;
                FeNode node2;
                float[][] diff = new float[3][];
                diff[0] = new float[results1._undeformedNodes.Count];
                diff[1] = new float[results1._undeformedNodes.Count];
                diff[2] = new float[results1._undeformedNodes.Count];
                //
                foreach (var entry in results1._undeformedNodes)
                {
                    node1 = entry.Value;
                    node2 = results2._undeformedNodes[node1.Id];
                    id = results1._nodeIdsLookUp[node1.Id];
                    //
                    diff[0][id] = (float)(node1.X - node2.X);
                    diff[1][id] = (float)(node1.Y - node2.Y);
                    diff[2][id] = (float)(node1.Z - node2.Z);
                }
                return diff;
            }
            else  return null;
        }
        // Mesh deformation                         
        public void SetMeshDeformation(float scale, int stepId, int stepIncrementId)
        {
            if (scale != _scale || stepId != _stepId || stepIncrementId != _stepIncrementId)
            {
                _scale = scale;
                _stepId = stepId;
                _stepIncrementId = stepIncrementId;
                //
                int resultNodeId;
                double[] offset;
                FeNode node;
                FeNode deformedNode;
                Dictionary<int, FeNode> deformedNodes = null;
                //
                if (_scale != 0)
                {
                    float[][] deformations = GetNodalMeshDeformations(_stepId, _stepIncrementId);
                    if (deformations != null)
                    {
                        deformedNodes = new Dictionary<int, FeNode>();
                        //
                        foreach (var entry in _mesh.Parts)
                        {
                            offset = entry.Value.Offset;
                            //
                            foreach (var nodeId in entry.Value.NodeLabels)
                            {
                                node = _undeformedNodes[nodeId];
                                // Result parts
                                if (_nodeIdsLookUp.TryGetValue(node.Id, out resultNodeId))
                                {
                                    deformedNode = new FeNode(node.Id,
                                                              node.X + offset[0] + _scale * deformations[0][resultNodeId],
                                                              node.Y + offset[1] + _scale * deformations[1][resultNodeId],
                                                              node.Z + offset[2] + _scale * deformations[2][resultNodeId]);
                                }
                                // Geometry parts
                                else deformedNode = node;
                                // Check for merged nodes as in compound parts
                                if (!deformedNodes.ContainsKey(deformedNode.Id))
                                    deformedNodes.Add(deformedNode.Id, deformedNode);
                            }
                        }
                    }
                }
                if (deformedNodes == null)
                {
                    deformedNodes = new Dictionary<int, FeNode>();
                    foreach (var entry in _mesh.Parts)
                    {
                        offset = entry.Value.Offset;
                        //
                        foreach (var nodeId in entry.Value.NodeLabels)
                        {
                            node = _undeformedNodes[nodeId];
                            // Result parts
                            if (_nodeIdsLookUp.TryGetValue(node.Id, out resultNodeId))
                                deformedNode = new FeNode(node.Id, node.X + offset[0], node.Y + offset[1], node.Z + offset[2]);
                            // Geometry parts
                            else deformedNode = node;
                            // Check for merged nodes as in compound parts
                            if (!deformedNodes.ContainsKey(deformedNode.Id))
                                deformedNodes.Add(deformedNode.Id, deformedNode);
                        }
                    }
                }
                //
                _mesh.Nodes = deformedNodes;
            }
        }
        public void SetPartDeformation(BasePart part, float scale, int stepId, int stepIncrementId)
        {
            // Reset global mesh deformation settings
            ResetScaleStepIncrement();
            //
            bool scaled = false;
            double[] offset;
            FeNode node;
            FeNode deformedNode;
            //
            if (scale != 0)
            {
                float[][] deformations = GetNodalMeshDeformations(stepId, stepIncrementId);
                if (deformations != null)
                {
                    scaled = true;
                    int resultNodeId;
                    offset = part.Offset;
                    //
                    foreach (var nodeId in part.NodeLabels)
                    {
                        resultNodeId = _nodeIdsLookUp[nodeId];
                        node = _undeformedNodes[nodeId];
                        //
                        deformedNode = new FeNode(node.Id,
                                                  node.X + offset[0] + scale * deformations[0][resultNodeId],
                                                  node.Y + offset[1] + scale * deformations[1][resultNodeId],
                                                  node.Z + offset[2] + scale * deformations[2][resultNodeId]);
                        _mesh.Nodes[deformedNode.Id] = deformedNode;
                    }
                }
            }
            if (!scaled)
            {
                offset = part.Offset;
                foreach (var nodeId in part.NodeLabels)
                {
                    node = _undeformedNodes[nodeId];
                    deformedNode = new FeNode(node.Id, node.X + offset[0], node.Y + offset[1], node.Z + offset[2]);
                    _mesh.Nodes[node.Id] = deformedNode;
                }
            }
        }
        public float[][] GetNodalMeshDeformations(int stepId, int stepIncrementId)
        {
            return GetNodalMeshDeformations(_deformationFieldOutputName, stepId, stepIncrementId);
        }
        public float[][] GetNodalMeshDeformations(string deformationFieldOutputName, int stepId, int stepIncrementId)
        {
            float[][] deformations = null;
            string[] componentNames = GetDeformationFieldOutputComponentNames(deformationFieldOutputName);
            string[] existingComponentNames = GetFieldComponentNames(deformationFieldOutputName);
            //
            if (existingComponentNames != null &&
                existingComponentNames.Contains(componentNames[0]) &&
                existingComponentNames.Contains(componentNames[1]) &&
                existingComponentNames.Contains(componentNames[2]))
            {
                Field deformationField = GetField(new FieldData(deformationFieldOutputName, "", stepId, stepIncrementId));
                if (deformationField == null) return null;
                //
                deformations = new float[3][];
                deformations[0] = deformationField.GetComponentValues(componentNames[0]);
                if (deformations[0] == null) return null;
                deformations[1] = deformationField.GetComponentValues(componentNames[1]);
                if (deformations[1] == null) return null;
                deformations[2] = deformationField.GetComponentValues(componentNames[2]);
                if (deformations[2] == null) return null;
            }
            return deformations;
        }
        public static OrderedDictionary<string, string> GetPossibleDeformationFieldOutputNamesMap()
        {
            OrderedDictionary<string, string> names = new OrderedDictionary<string, string>("Deformation names");
            names.Add("Displacements", FOFieldNames.Disp);
            names.Add("Forces", FOFieldNames.Forc);
            names.Add("Surface normals", FOFieldNames.SurfaceNormal);
            names.Add("Wear depths", FOFieldNames.WearDepth);
            names.Add("Mesh deformation", FOFieldNames.MeshDeformation);
            names.Add("Disp&Def&Depth", FOFieldNames.DispDeformationDepth);
            return names;
        }
        public static string[] GetPossibleDeformationFieldOutputNames()
        {
            return GetPossibleDeformationFieldOutputNamesMap().Keys.ToArray();
        }
        public string[] GetExistingDeformationFieldOutputNames()
        {
            string[] fieldNames = GetAllFieldNames();
            OrderedDictionary<string, string> possibleNames = GetPossibleDeformationFieldOutputNamesMap();
            List<string> existingFieldNames = new List<string>();
            foreach (var entry in possibleNames)
            {
                if (fieldNames.Contains(entry.Value)) existingFieldNames.Add(entry.Key);
            }
            return existingFieldNames.ToArray();
        }
        private string[] GetDeformationFieldOutputComponentNames(string deformationFieldOutputName)
        {
            string[] componentNames = null;
            if (deformationFieldOutputName == FOFieldNames.Disp)
            {
                componentNames = new string[] { FOComponentNames.U1, FOComponentNames.U2, FOComponentNames.U3 };
            }
            else if (deformationFieldOutputName == FOFieldNames.Forc)
            {
                componentNames = new string[] { FOComponentNames.F1, FOComponentNames.F2, FOComponentNames.F3 };
            }
            else if (deformationFieldOutputName == FOFieldNames.SurfaceNormal)
            {
                componentNames = new string[] { FOComponentNames.N1, FOComponentNames.N2, FOComponentNames.N3 };
            }
            else if (deformationFieldOutputName == FOFieldNames.WearDepth)
            {
                componentNames = new string[] { FOComponentNames.H1, FOComponentNames.H2, FOComponentNames.H3 };
            }
            else if (deformationFieldOutputName == FOFieldNames.MeshDeformation)
            {
                componentNames = new string[] { FOComponentNames.U1, FOComponentNames.U2, FOComponentNames.U3 };
            }
            else if (deformationFieldOutputName == FOFieldNames.DispDeformationDepth)
            {
                componentNames = new string[] { FOComponentNames.U1, FOComponentNames.U2, FOComponentNames.U3 };
            }
            return componentNames;
        }
        // History                                  
        public HistoryResults GetHistory()
        {
            return _history;
        }
        public void SetHistory(HistoryResults historyResults)
        {
            _history = historyResults;            
        }
        // Units                                    
        public TypeConverter GetFieldUnitConverter(string fieldDataName, string componentName)
        {
            GetFieldUnitConverterAndAbbrevation(fieldDataName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitConverter;
        }
        public string GetFieldUnitAbbrevation(string fieldDataName, string componentName)
        {
            GetFieldUnitConverterAndAbbrevation(fieldDataName, componentName, out TypeConverter unitConverter,
                                                out string unitAbbreviation);
            return unitAbbreviation;
        }
        public void GetFieldUnitConverterAndAbbrevation(string fieldDataName, string componentName, out TypeConverter unitConverter,
                                                        out string unitAbbreviation)
        {
            unitConverter = new DoubleConverter();
            unitAbbreviation = "?";
            try
            {
                switch (fieldDataName.ToUpper())
                {
                    case FOFieldNames.None:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "";
                        break;
                    case FOFieldNames.Disp:
                    case FOFieldNames.Distance: // Imported pressure
                        unitConverter = new StringLengthConverter();
                        unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        break;
                    case FOFieldNames.Stress:
                    case FOFieldNames.ZZStr:
                    case FOFieldNames.Imported: // Imported pressure
                        unitConverter = new StringPressureConverter();
                        unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                        break;
                    case FOFieldNames.ToStrain: 
                    case FOFieldNames.MeStrain:
                    case FOFieldNames.Pe:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case FOFieldNames.Forc:
                        unitConverter = new StringForceConverter();
                        unitAbbreviation = _unitSystem.ForceUnitAbbreviation;
                        break;
                    case FOFieldNames.Ener:
                        unitConverter = new StringEnergyPerVolumeConverter();
                        unitAbbreviation = _unitSystem.EnergyPerVolumeUnitAbbreviation;
                        break;
                    case FOFieldNames.Error:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "%";
                        break;
                    case FOFieldNames.Contact:
                        {
                            switch (componentName.ToUpper())
                            {
                                case FOComponentNames.COpen:
                                case FOComponentNames.CSlip1:
                                case FOComponentNames.CSlip2:
                                    unitConverter = new StringLengthConverter();
                                    unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                                    break;
                                case FOComponentNames.CPress:
                                case FOComponentNames.CShear1:
                                case FOComponentNames.CShear2:
                                    unitConverter = new StringPressureConverter();
                                    unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    // Wear
                    case FOFieldNames.SlidingDistance:
                    case FOFieldNames.SurfaceNormal:
                    case FOFieldNames.WearDepth:
                    case FOFieldNames.MeshDeformation:
                    case FOFieldNames.DispDeformationDepth:
                        unitConverter = new StringLengthConverter();
                        unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        break;
                    // Thermal
                    case FOFieldNames.NdTemp:
                        unitConverter = new StringTemperatureConverter();
                        unitAbbreviation = _unitSystem.TemperatureUnitAbbreviation;
                        break;
                    case FOFieldNames.Flux:
                        unitConverter = new StringPowerPerAreaConverter();
                        unitAbbreviation = _unitSystem.PowerPerAreaUnitAbbreviation;
                        break;
                    case FOFieldNames.Rfl:
                        unitConverter = new StringPowerConverter();
                        unitAbbreviation = _unitSystem.PowerUnitAbbreviation;
                        break;
                    case FOFieldNames.HError:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "%";
                        break;
                    // Sensitivity
                    case FOFieldNames.Norm:
                        unitConverter = new StringLengthConverter();
                        unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        break;
                    case FOFieldNames.SenFreq:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    default:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "?";
                        // OpenFOAM
                        if (componentName.ToUpper().StartsWith("VAL")) { }
                        else if (_unitSystem.UnitSystemType == UnitSystemType.UNIT_LESS) unitAbbreviation = "";
                        else if (System.Diagnostics.Debugger.IsAttached) throw new NotSupportedException();
                        //
                        break;
                }
            }
            catch
            {
            }
        }
        //
        public TypeConverter GetHistoryUnitConverter(string fieldName, string componentName)
        {
            GetHistoryUnitConverterAndAbbrevation(fieldName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitConverter;
        }
        public string GetHistoryUnitAbbrevation(string fieldName, string componentName)
        {
            GetHistoryUnitConverterAndAbbrevation(fieldName, componentName, out TypeConverter unitConverter,
                                                  out string unitAbbreviation);
            return unitAbbreviation;
        }
        public void GetHistoryUnitConverterAndAbbrevation(string fieldName, string componentName, out TypeConverter unitConverter,
                                                          out string unitAbbreviation)
        {
            unitConverter = new DoubleConverter();
            unitAbbreviation = "?";
            try
            {
                switch (fieldName.ToUpper())
                {
                    case HOFieldNames.Time:
                        unitConverter = new StringTimeConverter();
                        unitAbbreviation = _unitSystem.TimeUnitAbbreviation;
                        break;
                    case HOFieldNames.Displacements:
                    case HOFieldNames.RelativeContactDisplacement:
                    case HOFieldNames.CenterOgGravityCG:
                    case HOFieldNames.MeanSurfaceNormal:
                        if (componentName.ToUpper().StartsWith("UR"))
                        {
                            unitConverter = new StringAngleConverter();
                            unitAbbreviation = _unitSystem.AngleUnitAbbreviation;
                        }
                        else
                        {
                            unitConverter = new StringLengthConverter();
                            unitAbbreviation = _unitSystem.LengthUnitAbbreviation;
                        }
                        break;
                    case HOFieldNames.SurfaceArea:
                        unitConverter = new StringAreaConverter();
                        unitAbbreviation = _unitSystem.AreaUnitAbbreviation;
                        break;
                    case HOFieldNames.Volume:
                    case HOFieldNames.TotalVolume:
                        unitConverter = new StringVolumeConverter();
                        unitAbbreviation = _unitSystem.VolumeUnitAbbreviation;
                        break;
                    case HOFieldNames.Forces:
                    case HOFieldNames.TotalForce:
                    case HOFieldNames.NormalSurfaceForce:
                    case HOFieldNames.ShearSurfaceForce:
                    case HOFieldNames.TotalSurfaceForce:
                        if (componentName.ToUpper().StartsWith("RM"))
                        {
                            unitConverter = new StringMomentConverter();
                            unitAbbreviation = _unitSystem.MomentUnitAbbreviation;
                        }
                        else
                        {
                            unitConverter = new StringForceConverter();
                            unitAbbreviation = _unitSystem.ForceUnitAbbreviation;
                        }
                        break;
                    case HOFieldNames.Stresses:
                    case HOFieldNames.ContactStress:
                        unitConverter = new StringPressureConverter();
                        unitAbbreviation = _unitSystem.PressureUnitAbbreviation;
                        break;
                    case HOFieldNames.Strains:
                    case HOFieldNames.MechanicalStrains:
                    case HOFieldNames.EquivalentPlasticStrains:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case HOFieldNames.InternalEnergy:
                    case HOFieldNames.TotalInternalEnergy:
                    case HOFieldNames.ContactPrintEnergy:
                        unitConverter = new StringEnergyConverter();
                        unitAbbreviation = _unitSystem.EnergyUnitAbbreviation;
                        break;
                    case HOFieldNames.InternalEnergyDensity:
                        unitConverter = new StringEnergyPerVolumeConverter();
                        unitAbbreviation = _unitSystem.EnergyPerVolumeUnitAbbreviation;
                        break;
                    case HOFieldNames.TotalNumberOfContactElements:
                        unitConverter = new DoubleConverter();
                        unitAbbreviation = "/";
                        break;
                    case HOFieldNames.MomentAboutOrigin:
                    case HOFieldNames.MomentAboutCG:
                        unitConverter = new StringMomentConverter();
                        unitAbbreviation = _unitSystem.MomentUnitAbbreviation;
                        break;
                    // Thermal
                    case HOFieldNames.Temperatures:
                        unitConverter = new StringTemperatureConverter();
                        unitAbbreviation = _unitSystem.TemperatureUnitAbbreviation;
                        break;
                    case HOFieldNames.HeatGeneration:
                    case HOFieldNames.TotalHeatGeneration:
                    case HOFieldNames.BodyHeating:
                    case HOFieldNames.TotalBodyHeating:
                        unitConverter = new StringPowerConverter();
                        unitAbbreviation = _unitSystem.PowerUnitAbbreviation;
                        break;
                    case HOFieldNames.HeatFlux:
                        unitConverter = new StringPowerPerAreaConverter();
                        unitAbbreviation = _unitSystem.PowerPerAreaUnitAbbreviation;
                        break;
                    // Error
                    case FOFieldNames.Error:
                    default:
                        string noSpacesName = fieldName.Replace(' ', '_');
                        GetFieldUnitConverterAndAbbrevation(noSpacesName.ToUpper(), componentName,
                                                            out unitConverter, out unitAbbreviation);
                        if (unitAbbreviation == "?" && System.Diagnostics.Debugger.IsAttached)
                            throw new NotSupportedException();
                        break;
                }
            }
            catch
            {
            }
        }
        //                                          
        public void CopyPartsFromMesh(FeMesh mesh)
        {
            _mesh.Parts.Clear();

            ResultPart part;
            foreach (var entry in mesh.Parts)
            {
                part = new ResultPart(entry.Value);
                _mesh.Parts.Add(entry.Key, part);

                foreach (var elementId in part.Labels) _mesh.Elements[elementId].PartId = part.PartId;
            }
        }
        public void CopyMeshitemsFromMesh(FeMesh mesh)
        {
            foreach (var nodeSet in mesh.NodeSets)
            {
                _mesh.NodeSets.Add(nodeSet.Key, nodeSet.Value.DeepClone());
            }
            //
            foreach (var elementSet in mesh.ElementSets)
            {
                _mesh.ElementSets.Add(elementSet.Key, elementSet.Value.DeepClone());
            }
            //
            foreach (var surface in mesh.Surfaces)
            {
                _mesh.Surfaces.Add(surface.Key, surface.Value.DeepClone());
            }
        }
        //
        public void AddFiled(FieldData fieldData, Field field)
        {
            fieldData = new FieldData(fieldData);   // copy
            _fields.Add(fieldData, field);
        }
        public Field GetField(FieldData fieldData)
        {
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                    entry.Key.StepId == fieldData.StepId &&
                    entry.Key.StepIncrementId == fieldData.StepIncrementId)
                {
                    return entry.Value;
                }
            }
            return null;
        }
        public bool ReplaceField(FieldData fieldData, Field field)
        {
            // Find the result
            FieldData key = null;
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                    entry.Key.StepId == fieldData.StepId &&
                    entry.Key.StepIncrementId == fieldData.StepIncrementId)
                {
                    key = entry.Key;
                    break;
                }
            }
            //
            if (key != null)
            {
                _fields[key] = field;
                return true;
            }
            else return false;
        }
        //
        public HistoryResultSet AddResultHistoryOutput(ResultHistoryOutput resultHistoryOutput)
        {
            // Compatibility for version v1.2.1
            if (_history == null) _history = new HistoryResults("Tmp");
            //
            HistoryResultSet historyResultSet = null;
            //
            if (resultHistoryOutput is ResultHistoryOutputFromField rhoff)
            {
                int[] nodeIds = null;
                if (rhoff.RegionType == RegionTypeEnum.NodeSetName)
                {
                    nodeIds = _mesh.NodeSets[rhoff.RegionName].Labels;
                }
                else if (rhoff.RegionType == RegionTypeEnum.SurfaceName)
                {
                    string nodeSetName = _mesh.Surfaces[rhoff.RegionName].NodeSetName;
                    nodeIds = _mesh.NodeSets[nodeSetName].Labels;
                }
                else if (rhoff.RegionType == RegionTypeEnum.Selection)
                {
                    nodeIds = rhoff.CreationIds;
                }
                //
                if (nodeIds != null)
                {                    
                    // Prepare entries
                    string name;
                    HistoryResultComponent historyResultComponent = new HistoryResultComponent(rhoff.ComponentName);
                    historyResultComponent.Entries = new Dictionary<string, HistoryResultEntries>();
                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        name = nodeIds[i].ToString();
                        historyResultComponent.Entries.Add(name, new HistoryResultEntries(name, false));
                    }
                    // Get all existing increments
                    Dictionary<int, int[]> existingStepIncrementIds =
                        GetExistingIncrementIds(rhoff.FieldName, rhoff.ComponentName);
                    Field field;
                    float[] values;
                    int resultNodeId;
                    FieldData fieldData;
                    //
                    foreach (var entry in existingStepIncrementIds)
                    {
                        foreach (var incrementId in entry.Value)
                        {
                            fieldData = GetFieldData(rhoff.FieldName, rhoff.ComponentName, entry.Key, incrementId);
                            field = GetField(fieldData);
                            if (field != null)
                            {
                                //
                                values = field.GetComponentValues(rhoff.ComponentName);
                                //
                                for (int i = 0; i < nodeIds.Length; i++)
                                {
                                    name = nodeIds[i].ToString();
                                    resultNodeId = _nodeIdsLookUp[nodeIds[i]];
                                    historyResultComponent.Entries[name].Add(fieldData.Time, values[resultNodeId]);
                                }
                            }
                        }
                    }
                    //
                    HistoryResultField historyResultField = new HistoryResultField(rhoff.FieldName);
                    historyResultField.Components.Add(historyResultComponent.Name, historyResultComponent);
                    //
                    historyResultSet = new HistoryResultSet(rhoff.Name);
                    historyResultSet.Fields.Add(historyResultField.Name, historyResultField);
                    //
                    _history.Sets.Add(historyResultSet.Name, historyResultSet);
                }
            }
            return historyResultSet;
        }
        //
        public string[] GetAllComponentNames()
        {
            HashSet<string> componentNames = new HashSet<string>();
            foreach (var entry in _fields)
            {
                componentNames.UnionWith(entry.Value.GetCmponentNames());
            }
            return componentNames.ToArray();
        }
        public string[] GetFieldComponentNames(string fieldName)
        {
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldName.ToUpper())
                {
                    return entry.Value.GetCmponentNames();
                }
            }
            return null;
        }
        public FieldData[] GetAllFieldData()
        {
            return _fields.Keys.ToArray();
        }
        public FieldData GetFieldData(string name, string component, int stepId, int stepIncrementId)
        {
            FieldData result;
            // Empty results
            if (stepId == -1 && stepIncrementId == -1)
            {
                result = new FieldData(name, component, stepId, stepIncrementId);
                result.Type = StepType.Static;
                result.Valid = false;
                return result;
            }
            // Zero increment - Find all occurances!!!
            else if (stepId == 1 && stepIncrementId == 0)
            {
                result = new FieldData(name, component, stepId, stepIncrementId);
                result.Type = StepType.Static;
                return result;
            }
            // Find the result
            foreach (var entry in _fields)
            {
                if (entry.Key.Name == name && entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId)
                {
                    result = new FieldData(entry.Key);
                    result.Component = component;
                    return result;
                }
            }
            // Find other existing result
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId)
                {
                    result = new FieldData(entry.Key);
                    result.Name = name;
                    result.Component = component;
                    result.Valid = false;
                    return result;
                }
            }
            // Nothing found
            result = new FieldData(name, component, stepId, stepIncrementId);
            result.Type = StepType.Static;
            result.Valid = false;
            return result;
            //return null;
        }
        public FieldData GetFirstComponentOfTheFirstFieldAtLastIncrement()
        {
            string name = GetAllFieldNames()[0];
            string component = GetFieldComponentNames(name)[0];
            int stepId = GetAllStepIds().Last();
            int stepIncrementId = GetIncrementIds(stepId).Last();
            //
            return GetFieldData(name, component, stepId, stepIncrementId);
        }
        public FieldData GetFirstComponentOfTheFirstFieldAtDefaultIncrement()
        {
            string[] names = GetAllFieldNames();
            FieldData fieldData;
            if (names.Length == 0)
            {
                // There is no data
                fieldData = GetFieldData(FOFieldNames.None, FOComponentNames.None, -1, -1);
                fieldData.Valid = false;
            }
            else
            {
                int stepId = GetAllStepIds().Last();
                int stepIncrementId = GetIncrementIds(stepId).Last();
                string name = GetStepFieldNames(stepId)[0];
                string component = GetFieldComponentNames(name)[0];
                //
                fieldData = GetFieldData(name, component, stepId, stepIncrementId);
                if (fieldData == null)
                {
                    // There is no data
                    fieldData = GetFieldData(FOFieldNames.None, FOComponentNames.None, -1, -1);
                }
                else if (fieldData.Type == StepType.Frequency)
                {
                    stepIncrementId = GetIncrementIds(stepId).First();
                    fieldData = GetFieldData(name, component, stepId, stepIncrementId);
                }
            }
            return fieldData;
        }
        public string[] GetAllFieldNames()
        {
            List<string> names = new List<string>();
            foreach (var entry in _fields)
            {
                if (!names.Contains(entry.Key.Name)) names.Add(entry.Key.Name);
            }
            return names.ToArray();
        }
        public NamedClass[] GetFieldsAsNamedItems()
        {
            string[] names = GetAllFieldNames();
            NamedClass[] fields = new NamedClass[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                fields[i] = new EmptyNamedClass(names[i]);
            }
            return fields;
        }
        public string[] GetStepFieldNames(int stepId)
        {
            List<string> names = new List<string>();
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && !names.Contains(entry.Key.Name))
                    names.Add(entry.Key.Name);
            }
            return names.ToArray();
        }
        public int[] GetAllStepIds()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (var entry in _fields) ids.Add(entry.Key.StepId);
            //
            int[] sortedIds = ids.ToArray();
            Array.Sort(sortedIds);
            //
            return sortedIds;
        }
        public int[] GetIncrementIds(int stepId)
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId)
                {
                    if (entry.Key.Type == StepType.Static && stepId == 1 && ids.Count == 0)
                        ids.Add(0);   // Zero increment - Find all occurances!!!
                    //
                    ids.Add(entry.Key.StepIncrementId);
                }
            }
            //
            int[] sortedIds = ids.ToArray();
            Array.Sort(sortedIds);
            //
            return sortedIds;
        }
        public Dictionary<int, int[]> GetExistingIncrementIds(string fieldName, string component)
        {
            int[] stepIds = GetAllStepIds();
            Dictionary<int, int[]> existingIncrementIds = new Dictionary<int, int[]>();
            if (stepIds.Length == 0)
            {
                existingIncrementIds.Add(-1, new int[] { -1 });
            }
            else
            {
                List<int> stepIncrementIds;
                string fieldHash;
                HashSet<string> fieldHashes = GetAllFieldHashes();
                //
                foreach (int stepId in stepIds)
                {
                    stepIncrementIds = new List<int>();
                    foreach (int incrementId in GetIncrementIds(stepId))
                    {
                        fieldHash = GetFieldHash(fieldName, component, stepId, incrementId);
                        //if (FieldExists(fieldName, component, stepId, incrementId)) stepIncrementIds.Add(incrementId);
                        if (fieldHashes.Contains(fieldHash)) stepIncrementIds.Add(incrementId);
                    }
                    existingIncrementIds.Add(stepId, stepIncrementIds.ToArray());
                }
            }
            return existingIncrementIds;
        }
        public Dictionary<int, int[]> GetAllExistingIncrementIds()
        {
            int[] stepIds = GetAllStepIds();
            Dictionary<int, int[]> existingIncrementIds = new Dictionary<int, int[]>();
            if (stepIds.Length == 0)
            {
                existingIncrementIds.Add(-1, new int[] { -1 });
            }
            else
            {
                if (!stepIds.Contains(1))
                {
                    existingIncrementIds.Add(1, new int[] { 0 });   // Zero increment - Find all occurances!!!
                }
                foreach (int stepId in stepIds)
                {
                    existingIncrementIds.Add(stepId, GetIncrementIds(stepId));
                }
            }
            return existingIncrementIds;
        }
        public Dictionary<int, float> GetMaxStepTime()
        {
            int[] stepIds = GetAllStepIds();
            Dictionary<int, float> stepMaxTime = new Dictionary<int, float>();
            //
            if (stepIds.Length == 0)
            {
                stepMaxTime.Add(-1, -1);
            }
            else
            {
                foreach (var stepId in stepIds) stepMaxTime.Add(stepId, 0);
                if (!stepMaxTime.ContainsKey(0)) stepMaxTime.Add(0, 0); // Zero increment - Find all occurances!!!
                //
                float time;
                foreach (var entry in _fields)
                {
                    stepMaxTime.TryGetValue(entry.Key.StepId, out time);
                    if (entry.Key.Time > time) stepMaxTime[entry.Key.StepId] = entry.Key.Time;
                }
            }
            return stepMaxTime;
        }
        public Dictionary<string, string[]> GetAllFiledNameComponentNames()
        {
            HashSet<string> componentNames;
            Dictionary<string, HashSet<string>> filedNameComponentNames = new Dictionary<string, HashSet<string>>();
            foreach (var entry in _fields)
            {
                if (filedNameComponentNames.TryGetValue(entry.Key.Name, out componentNames))
                    componentNames.UnionWith(entry.Value.GetCmponentNames());
                else
                    filedNameComponentNames.Add(entry.Key.Name, new HashSet<string>(entry.Value.GetCmponentNames()));
            }
            //
            Dictionary<string, string[]> filedNameComponentNamesArr = new Dictionary<string, string[]>();
            foreach (var entry in filedNameComponentNames)
            {
                filedNameComponentNamesArr.Add(entry.Key, entry.Value.ToArray());
            }
            return filedNameComponentNamesArr;
        }
        public bool FieldExists(string fieldName, string component, int stepId, int stepIncrementId)
        {
            foreach (var entry in _fields)
            {
                if ((entry.Key.Name.ToUpper() == fieldName.ToUpper() &&
                     entry.Value.ContainsComponent(component) &&
                     stepId == 1 && stepIncrementId == 0) // Zero increment - Find all occurances!!!
                     ||
                    (entry.Key.Name.ToUpper() == fieldName.ToUpper() &&
                     entry.Value.ContainsComponent(component) &&
                     entry.Key.StepId == stepId &&
                     entry.Key.StepIncrementId == stepIncrementId))
                    return true;
            }
            return false;
        }
        private HashSet<string> GetAllFieldHashes()
        {
            HashSet<string> fieldHashes = new HashSet<string>();
            foreach (var fieldEntry in _fields)
            {
                foreach (var componentName in fieldEntry.Value.GetCmponentNames())
                {
                    fieldHashes.Add(GetFieldHash(fieldEntry.Key.Name, componentName, 1, 0));
                    fieldHashes.Add(GetFieldHash(fieldEntry.Key.Name, componentName, fieldEntry.Key.StepId,
                                                 fieldEntry.Key.StepIncrementId));
                }
            }
            return fieldHashes;
        }
        private string GetFieldHash(string fieldName, string component, int stepId, int stepIncrementId)
        {
            return fieldName.ToUpper() + "_" + component.ToUpper() + "_" + stepId.ToString() + "_" + stepIncrementId.ToString();
        }
        //
        public float GetIncrementTime(int stepId, int stepIncrementId)
        {
            if (stepIncrementId == 0) return 0; // Zero increment - Find all occurances!!!
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId)
                {
                    return entry.Key.Time;
                }
            }
            return -1;
        }
        public float[] GetValues(FieldData fieldData, int[] globalNodeIds)
        {
            float[] values = null;
            bool zeroIncrement = false;
            //
            if (fieldData.Valid)
            {
                if (fieldData.StepIncrementId == 0)     // Zero increment - Find all occurances!!!
                {
                    if (fieldData.StepId == 1)          // first step
                    {
                        values = new float[globalNodeIds.Length];
                        zeroIncrement = true;
                    }
                }
                //
                if (!zeroIncrement)
                {
                    Field field = GetField(fieldData);
                    //
                    float[] allValues = field.GetComponentValues(fieldData.Component);
                    values = new float[globalNodeIds.Length];
                    int globalId;
                    int localId;
                    for (int i = 0; i < globalNodeIds.Length; i++)
                    {
                        globalId = globalNodeIds[i];
                        if (_nodeIdsLookUp.TryGetValue(globalId, out localId) && localId >= 0 && localId < allValues.Length)
                            values[i] = allValues[localId];
                        else
                            return null;
                    }

                    //// Remove frst step
                    //fieldData.StepIncrementId = 1;
                    ////
                    //foreach (var entry in _fields)
                    //{
                    //    if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() &&
                    //        entry.Key.StepId == fieldData.StepId &&
                    //        entry.Key.StepIncrementId == fieldData.StepIncrementId)
                    //    {
                    //        float[] allValues = entry.Value.GetComponentValues(fieldData.Component);
                    //        int globalId;
                    //        int localId;
                    //        for (int i = 0; i < globalNodeIds.Length; i++)
                    //        {
                    //            globalId = globalNodeIds[i];
                    //            if (_nodeIdsLookUp.TryGetValue(globalId, out localId) && localId >= 0 && localId < allValues.Length)
                    //                values[i] -= allValues[localId];
                    //            else
                    //                return null;
                    //        }
                    //        break;
                    //    }
                    //}
                }
            }
            return values;
        }
        public bool IsComponentInvariant(FieldData fieldData)
        {
            if (fieldData.StepIncrementId == 0)   // Zero increment - Find all occurances!!!
            {
                if (fieldData.StepId == 1)        // first step
                    return false;
            }
            //
            foreach (var entry in _fields)
            {
                if (entry.Key.Name.ToUpper() == fieldData.Name.ToUpper() && 
                    entry.Key.StepId == fieldData.StepId && 
                    entry.Key.StepIncrementId == fieldData.StepIncrementId)
                {
                    return entry.Value.IsComponentInvariant(fieldData.Component);
                }
            }
            return false;
        }
        public NodesExchangeData GetExtremeValues(string partName, FieldData fieldData)
        {
            return GetScaledExtremeValues(partName, fieldData, 0);
        }
        public NodesExchangeData GetScaledExtremeValues(string partName, FieldData fieldData, float relativeScale)
        {
            if (!fieldData.Valid) return null;
            //
            NodesExchangeData nodesData = new NodesExchangeData();
            nodesData.Ids = new int[2];
            nodesData.Coor = new double[2][];
            nodesData.Values = new float[2];
            int minId = -1;
            int maxId = -1;
            bool zeroIncrement = false;
            //
            if (fieldData.StepIncrementId == 0)     // Zero increment - Find all occurances!!!
            {
                if (fieldData.StepId == 1)          // first step / zero increment
                {
                    minId = _nodeIdsLookUp.Keys.First();
                    maxId = minId;
                    nodesData.Ids[0] = minId;
                    nodesData.Ids[1] = maxId;
                    nodesData.Coor[0] = _mesh.Nodes[minId].Coor;
                    nodesData.Coor[1] = _mesh.Nodes[maxId].Coor;
                    nodesData.Values[0] = 0;
                    nodesData.Values[1] = 0;
                    //
                    zeroIncrement = true;
                }
            }
            //
            if (!zeroIncrement)
            {                
                foreach (var fieldEntry in _fields)
                {
                    if (fieldEntry.Key.Name == fieldData.Name && fieldEntry.Key.StepId == fieldData.StepId &&
                        fieldEntry.Key.StepIncrementId == fieldData.StepIncrementId)
                    {
                        int id;
                        float value;
                        BasePart basePart;
                        bool firstNaN = true;
                        float[] values = fieldEntry.Value.GetComponentValues(fieldData.Component);
                        //
                        basePart = _mesh.Parts[partName];
                        // Initialize   
                        nodesData.Values[0] = float.MaxValue;
                        nodesData.Values[1] = -float.MaxValue;
                        // Get first value
                        if (_nodeIdsLookUp.TryGetValue(basePart.NodeLabels[0], out id) && id < values.Length)
                        {
                            value = values[id];
                            if (!float.IsNaN(value))
                            {
                                firstNaN = false;
                                nodesData.Values[0] = value;
                                nodesData.Values[1] = value;
                            }
                            minId = basePart.NodeLabels[0];
                            maxId = basePart.NodeLabels[0];
                        }
                        //
                        foreach (var nodeId in basePart.NodeLabels)
                        {
                            if (_nodeIdsLookUp.TryGetValue(nodeId, out id) && id < values.Length)
                            {
                                value = values[id];
                                if (!float.IsNaN(value))
                                {
                                    if (value < nodesData.Values[0])
                                    {
                                        nodesData.Values[0] = value;
                                        minId = nodeId;
                                    }
                                    else if (value > nodesData.Values[1])
                                    {
                                        nodesData.Values[1] = value;
                                        maxId = nodeId;
                                    }
                                }
                            }
                        }
                        //
                        if (relativeScale < 0)  // swap min and max
                        {
                            int tmp = minId;
                            minId = maxId;
                            maxId = tmp;
                            float tmpD = nodesData.Values[0];
                            nodesData.Values[0] = nodesData.Values[1];
                            nodesData.Values[1] = tmpD;
                        }
                        // Ids
                        nodesData.Ids[0] = minId;
                        nodesData.Ids[1] = maxId;
                        // Coordinates
                        nodesData.Coor[0] = _mesh.Nodes[minId].Coor;
                        nodesData.Coor[1] = _mesh.Nodes[maxId].Coor;
                        // Values
                        if (firstNaN && minId == maxId) // all values are NaN
                        {
                            nodesData.Values[0] = 0;
                            nodesData.Values[1] = 0;
                        }
                        else
                        {
                            nodesData.Values[0] *= relativeScale;
                            nodesData.Values[1] *= relativeScale;
                        }
                        //
                        break;
                    }
                }
            }
            return nodesData;
        }
        // History
        public HistoryResultSet GetHistoryResultSet(string setName)
        {
            HistoryResultSet set = null;
            //
            foreach (var setEntry in _history.Sets)
            {
                if (setEntry.Key.ToUpper() == setName.ToUpper())
                {
                    set = setEntry.Value;
                    break;
                }
            }
            //
            return set;
        }
        public HistoryResultField GetHistoryResultField(string setName, string fieldName)
        {
            HistoryResultSet set = GetHistoryResultSet(setName);
            HistoryResultField field = null;
            //
            if (set != null)
            {
                foreach (var fieldEntry in set.Fields)
                {
                    if (fieldEntry.Key.ToUpper() == fieldName.ToUpper())
                    {
                        field = fieldEntry.Value;
                        break;
                    }
                }
            }
            //
            return field;
        }
        public HistoryResultComponent GetHistoryResultComponent(string setName, string fieldName, string componentName)
        {
            HistoryResultField field = GetHistoryResultField(setName, fieldName);
            HistoryResultComponent component = null;
            //
            if (field != null)
            {
                foreach (var componentEntry in field.Components)
                {
                    if (componentEntry.Key.ToUpper() == componentName.ToUpper())
                    {
                        component = componentEntry.Value;
                        break;
                    }
                }
            }
            //
            return component;
        }
        public HistoryResultEntries GetHistoryResultEntry(string setName, string fieldName, string componentName, string entryName)
        {
            HistoryResultComponent component = GetHistoryResultComponent(setName, fieldName, componentName);
            HistoryResultEntries historyResultEntry = null;
            //
            if (component != null)
            {
                foreach (var entry in component.Entries)
                {
                    if (entry.Key.ToUpper() == entryName.ToUpper())
                    {
                        historyResultEntry = entry.Value;
                        break;
                    }
                }
            }
            //
            return historyResultEntry;
        }
        public NamedClass[] GetHistoriyOutputsAsNamedItems()
        {
            string[] names = _history.Sets.Keys.ToArray();
            NamedClass[] historyOutputs = new NamedClass[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                historyOutputs[i] = new EmptyNamedClass(names[i]);
            }
            return historyOutputs;
        }
        public void GetHistoryOutputData(HistoryResultData historyData, out string[] columnNames, out object[][] rowBasedData)
        {
            HistoryResultSet set = _history.Sets[historyData.SetName];
            HistoryResultField field = set.Fields[historyData.FieldName];
            HistoryResultComponent component = field.Components[historyData.ComponentName];
            string unit = "\n[" + GetHistoryUnitAbbrevation(field.Name, component.Name) + "]";
            string timeUnit = "\n[" + GetHistoryUnitAbbrevation("Time", null) + "]";
            // Sorted time
            double[] sortedTime;
            Dictionary<double, int> timeRowId;
            GetSortedTime(new HistoryResultComponent[] { component }, out sortedTime, out timeRowId);
            // Create the data array
            int numRow = sortedTime.Length;
            int numCol = component.Entries.Count + 1; // +1 for the time column
            columnNames = new string[numCol];
            rowBasedData = new object[numRow][];
            // Create rows
            for (int i = 0; i < numRow; i++) rowBasedData[i] = new object[numCol];
            // Add time column name
            columnNames[0] = "Time" + timeUnit;
            // Fill the data array
            for (int i = 0; i < sortedTime.Length; i++) rowBasedData[i][0] = sortedTime[i];
            // Add data column
            //
            int col = 1;
            int row;
            double[] timePoints;
            double[] values;
            foreach (var entry in component.Entries)
            {
                columnNames[col] = entry.Key + unit;
                if (entry.Value.Local) columnNames[col] += "\nLocal";
                //
                row = 0;
                timePoints = entry.Value.Time.ToArray();
                values = entry.Value.Values.ToArray();
                for (int i = 0; i < timePoints.Length; i++)
                {
                    row = timeRowId[timePoints[i]];
                    rowBasedData[row][col] = values[i];
                }
                col++;
            }
        }
        public void GetSortedTime(HistoryResultComponent[] components, out double[] sortedTime,
                                  out Dictionary<double, int> timeRowId)
        {
            // Collect all time points
            HashSet<double> timePointsHash = new HashSet<double>();
            foreach (var component in components)
            {
                foreach (var entry in component.Entries)
                {
                    foreach (var time in entry.Value.Time) timePointsHash.Add(time);
                }
            }
            // Sort time points
            sortedTime = timePointsHash.ToArray();
            Array.Sort(sortedTime);
            // Create a map of time point vs row id
            timeRowId = new Dictionary<double, int>();
            for (int i = 0; i < sortedTime.Length; i++) timeRowId.Add(sortedTime[i], i);
        }
        // Remove
        public void RemoveResultFieldOutputs(string[] fieldOutputNames)
        {
            if (_fields != null)
            {
                List<FieldData> fieldsToRemove = new List<FieldData>();
                foreach (var entry in _fields)
                {
                    if (fieldOutputNames.Contains(entry.Key.Name)) fieldsToRemove.Add(entry.Key);
                }
                //
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    _fields.Remove(fieldToRemove);
                }
            }
        }
        public void RemoveResultFieldOutputComponents(string fieldOutputName, string[] componentNames)
        {
            if (_fields != null)
            {
                foreach (var entry in _fields)
                {
                    if (entry.Key.Name == fieldOutputName)
                    {
                        foreach (var componentName in componentNames)
                        {
                            entry.Value.RemoveComponent(componentName);
                        }
                    }
                }
            }
        }
        //
        public void RemoveResultHistoryResultSets(string[] historyResultSetNames)
        {
            if (_history != null)
            {
                List<string> setsToRemove = new List<string>();
                foreach (var entry in _history.Sets)
                {
                    if (historyResultSetNames.Contains(entry.Key)) setsToRemove.Add(entry.Key);
                }
                //
                foreach (var setToRemove in setsToRemove)
                {
                    _history.Sets.Remove(setToRemove);
                }
            }
        }
        public void RemoveResultHistoryResultFields(string historyResultSetName, string[] historyResultFieldNames)
        {
            if (_history != null)
            {
                List<string> fieldsToRemove = new List<string>();
                foreach (var entry in _history.Sets[historyResultSetName].Fields)
                {
                    if (historyResultFieldNames.Contains(entry.Key)) fieldsToRemove.Add(entry.Key);
                }
                //
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    _history.Sets[historyResultSetName].Fields.Remove(fieldToRemove);
                }
            }
        }
        public void RemoveResultHistoryResultCompoments(string historyResultSetName,
                                                        string historyResultFieldName,
                                                        string[] historyResultComponentNames)
        {
            if (_history != null)
            {
                List<string> componentsToRemove = new List<string>();
                foreach (var entry in _history.Sets[historyResultSetName].Fields[historyResultFieldName].Components)
                {
                    if (historyResultComponentNames.Contains(entry.Key)) componentsToRemove.Add(entry.Key);
                }
                //
                foreach (var componentToRemove in componentsToRemove)
                {
                    _history.Sets[historyResultSetName].Fields[historyResultFieldName].Components.Remove(componentToRemove);
                }
            }
        }


        public void GetClosestFieldComponent(string fieldName, string componentName,
                                             out string closestFieldName, out string closestComponentName)
        {
            closestFieldName = null;
            closestComponentName = null;
            string[] existingComponentNames = GetFieldComponentNames(fieldName);
            //
            for (int i = 0; i < existingComponentNames.Length; i++)
            {
                if (existingComponentNames[i] == componentName)
                {
                    if (i + 1 < existingComponentNames.Length) closestComponentName = existingComponentNames[i + 1];
                    else if (i > 1) closestComponentName = existingComponentNames[i - 1];
                }
            }
            // Component found
            if (closestComponentName != null)
            {
                closestFieldName = fieldName;
            }
            // No components found in the same field
            else
            {
                string[] existingFieldNames = GetAllFieldNames();
                for (int i = 0; i < existingFieldNames.Length; i++)
                {
                    if (existingFieldNames[i] == fieldName)
                    {
                        // Find the next field output with at least one component
                        while (++i < existingFieldNames.Length)
                        {
                            existingComponentNames = GetFieldComponentNames(existingFieldNames[i]);
                            if (existingComponentNames.Length > 0)
                            {
                                closestFieldName = existingFieldNames[i];
                                closestComponentName = existingComponentNames[0];
                                return;
                            }
                        }
                        // Find the prevous field output with at least one component
                        while (--i > 0)
                        {
                            existingComponentNames = GetFieldComponentNames(existingFieldNames[i]);
                            if (existingComponentNames.Length > 0)
                            {
                                closestFieldName = existingFieldNames[i];
                                closestComponentName = existingComponentNames[0];
                                return;
                            }
                        }
                    }
                }
                // No field with at least one component found
                closestFieldName = null;
                closestComponentName = null;
            }
        }
        // Scaled results values
        public void GetNodesAndValues(FieldData fieldData, int[] nodeIds, out double[][] nodeCoor, out float[] values)
        {
            nodeCoor = new double[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                nodeCoor[i] = Mesh.Nodes[nodeIds[i]].Coor;
            }
            values = GetValues(fieldData, nodeIds);
        }
        //
        public PartExchangeData GetAllNodesCellsAndValues(FieldData fData)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetAllNodesAndCells(out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                      out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
            return pData;
        }
        public PartExchangeData GetSetNodesCellsAndValues(FeGroup elementSet, FieldData fData)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetSetNodesAndCells(elementSet, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                      out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
            return pData;
        }
        public PartExchangeData GetVisualizationNodesCellsAndValues(BasePart part, FieldData fData)
        {
            PartExchangeData pData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out pData.Nodes.Ids, out pData.Nodes.Coor, out pData.Cells.Ids,
                                                out pData.Cells.CellNodeIds, out pData.Cells.Types);
            if (!fData.Valid) pData.Nodes.Values = null;
            else
            {
                pData.Nodes.Values = GetValues(fData, pData.Nodes.Ids);
                pData.ExtremeNodes = GetScaledExtremeValues(part.Name, fData, 1);
            }
            return pData;
        }
        public PartExchangeData GetEdgesNodesAndCells(FeGroup elementSet, FieldData fData)  // must use fData
        {
            PartExchangeData resultData = new PartExchangeData();
            _mesh.GetNodesAndCellsForModelEdges(elementSet, out resultData.Nodes.Ids, out resultData.Nodes.Coor,
                                                out resultData.Cells.CellNodeIds, out resultData.Cells.Types);
            return resultData;
        }
        public void GetUndeformedNodesAndCells(BasePart part, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes)
        {
            int[] nodeIds, cellIds;
            Dictionary<int, FeNode> tmp = _mesh.Nodes;
            _mesh.Nodes = _undeformedNodes;
            _mesh.GetVisualizationNodesAndCells(part, out nodeIds, out nodeCoor, out cellIds, out cells, out cellTypes);
            _mesh.Nodes = tmp;
        }
        public void GetUndeformedModelEdges(BasePart part, out double[][] nodeCoor, out int[][] cells, out int[] cellTypes)
        {
            int[] nodeIds;
            Dictionary<int, FeNode> tmp = _mesh.Nodes;
            _mesh.Nodes = _undeformedNodes;
            _mesh.GetNodesAndCellsForModelEdges(part, out nodeIds, out nodeCoor, out cells, out cellTypes);
            _mesh.Nodes = tmp;
        }
        // Animation        
        public void GetScaleFactorAnimationData(BasePart part, FieldData fData, float scale, int numFrames,
                                                out PartExchangeData modelResultData,
                                                out PartExchangeData modelEdgesResultData,
                                                out PartExchangeData locatorResultData)
        {
            modelEdgesResultData = null;
            bool modelEdges = part.PartType.HasEdges() && part.Visualization.EdgeCells != null;
            // Undeforme part
            SetPartDeformation(part, 0, 0, 0);
            // Model
            modelResultData = new PartExchangeData();
            _mesh.GetVisualizationNodesAndCells(part, out modelResultData.Nodes.Ids, out modelResultData.Nodes.Coor,
                                                out modelResultData.Cells.Ids, out modelResultData.Cells.CellNodeIds,
                                                out modelResultData.Cells.Types);
            // Edges
            if (modelEdges)
            {
                modelEdgesResultData = new PartExchangeData();
                _mesh.GetNodesAndCellsForModelEdges(part, out modelEdgesResultData.Nodes.Ids,
                                                    out modelEdgesResultData.Nodes.Coor,
                                                    out modelEdgesResultData.Cells.CellNodeIds,
                                                    out modelEdgesResultData.Cells.Types);
            }
            // Locator
            locatorResultData = new PartExchangeData();
            _mesh.GetSetNodesAndCells(part, out locatorResultData.Nodes.Ids, out locatorResultData.Nodes.Coor, 
                                      out locatorResultData.Cells.Ids, out locatorResultData.Cells.CellNodeIds,
                                      out locatorResultData.Cells.Types);
            // Values
            if (!fData.Valid)
            {
                modelResultData.Nodes.Values = null;
                locatorResultData.Nodes.Values = null;
            }
            else
            {
                modelResultData.Nodes.Values = GetValues(fData, modelResultData.Nodes.Ids);
                modelResultData.ExtremeNodes = GetExtremeValues(part.Name, fData);
                locatorResultData.Nodes.Values = GetValues(fData, locatorResultData.Nodes.Ids);
            }
            // Aniumation
            modelResultData.NodesAnimation = new NodesExchangeData[numFrames];
            modelResultData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
            if (modelEdges) modelEdgesResultData.NodesAnimation = new NodesExchangeData[numFrames];
            locatorResultData.NodesAnimation = new NodesExchangeData[numFrames];
            //
            float[] ratios;
            if (fData.Type == StepType.Frequency || fData.Type == StepType.Buckling) ratios = GetRelativeModalScales(numFrames);
            else ratios = GetRelativeScales(numFrames);
            //
            float absoluteScale;
            float relativeScale;
            bool invariant = IsComponentInvariant(fData);
            //
            for (int i = 0; i < numFrames; i++)
            {
                relativeScale = ratios[i];
                absoluteScale = relativeScale * scale;
                // Deform mesh
                SetPartDeformation(part, absoluteScale, fData.StepId, fData.StepIncrementId);
                // Get scaled model
                modelResultData.NodesAnimation[i] = new NodesExchangeData();
                modelResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(modelResultData.Nodes.Ids);
                // Get scaled edges
                if (modelEdges)
                {
                    modelEdgesResultData.NodesAnimation[i] = new NodesExchangeData();
                    modelEdgesResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(modelEdgesResultData.Nodes.Ids);
                }
                // Get scaled locator
                locatorResultData.NodesAnimation[i] = new NodesExchangeData();
                locatorResultData.NodesAnimation[i].Coor = _mesh.GetNodeSetCoor(locatorResultData.Nodes.Ids);
                // Scale values
                if (invariant) relativeScale = Math.Abs(relativeScale);
                // Model
                ScaleValues(relativeScale, modelResultData.Nodes.Values, out modelResultData.NodesAnimation[i].Values);
                modelResultData.ExtremeNodesAnimation[i] = GetScaledExtremeValues(part.Name, fData, relativeScale);
                // Locator
                ScaleValues(relativeScale, locatorResultData.Nodes.Values, out locatorResultData.NodesAnimation[i].Values);
            }
        }
        public void GetTimeIncrementAnimationData(BasePart part, FieldData fData, float scale,
                                                  out PartExchangeData modelResultData,
                                                  out PartExchangeData modelEdgesResultData,
                                                  out PartExchangeData locatorResultData)
        {
            modelEdgesResultData = null;
            bool modelEdges = part.PartType.HasEdges() && part.Visualization.EdgeCells != null;
            // Undeforme part
            SetPartDeformation(part, 0, 0, 0);
            // Model
            modelResultData = GetVisualizationNodesCellsAndValues(part, fData);
            // Edges
            if (modelEdges) modelEdgesResultData = GetEdgesNodesAndCells(part, fData);
            // Locator
            locatorResultData = GetSetNodesCellsAndValues(part, fData);
            // Get all existing increments
            Dictionary<int, int[]> existingStepIncrementIds = GetExistingIncrementIds(fData.Name, fData.Component);
            // Count all existing increments
            int numFrames = 0;
            foreach (var entry in existingStepIncrementIds) numFrames += entry.Value.Length;
            // Create animation frames
            modelResultData.NodesAnimation = new NodesExchangeData[numFrames];
            modelResultData.ExtremeNodesAnimation = new NodesExchangeData[numFrames];
            if (modelEdges) modelEdgesResultData.NodesAnimation = new NodesExchangeData[numFrames];
            locatorResultData.NodesAnimation = new NodesExchangeData[numFrames];
            //
            PartExchangeData data;
            FieldData tmpFieldData = new FieldData(fData);  // create a copy
            int count = 0;
            foreach (var entry in existingStepIncrementIds)
            {
                tmpFieldData.StepId = entry.Key;
                foreach (var incrementId in entry.Value)
                {
                    tmpFieldData.StepIncrementId = incrementId;
                    // Deform part
                    SetPartDeformation(part, scale, tmpFieldData.StepId, tmpFieldData.StepIncrementId);
                    // Model
                    data = GetVisualizationNodesCellsAndValues(part, tmpFieldData);
                    modelResultData.NodesAnimation[count] = data.Nodes;
                    modelResultData.ExtremeNodesAnimation[count] = data.ExtremeNodes;
                    // Edges
                    if (modelEdges)
                    {
                        data = GetEdgesNodesAndCells(part, tmpFieldData);
                        modelEdgesResultData.NodesAnimation[count] = data.Nodes;
                    }
                    // Locator
                    data = GetSetNodesCellsAndValues(part, tmpFieldData);
                    locatorResultData.NodesAnimation[count] = data.Nodes;
                    //
                    count++;
                }
            }
        }
        //
        private float[] GetRelativeScales(int numFrames)
        {
            float ratio = 1f / (numFrames - 1);
            float[] ratios = new float[numFrames];
            for (int i = 0; i < numFrames; i++) ratios[i] = i * ratio;
            return ratios;
        }
        private float[] GetRelativeModalScales(int numFrames)
        {
            float ratio = 2f / (numFrames - 1);
            float[] ratios = new float[numFrames];
            for (int i = 0; i < numFrames; i++)
            {
                ratios[i] = (float)Math.Sin((-1 + i * ratio) * Math.PI / 2);     // end with 1
            }
            return ratios;
        }
        //
        public void ScaleNodeCoordinates(string deformationFieldOutputName, float scale, int stepId, int stepIncrementId,
                                         int[] globalNodeIds, ref double[][] nodes)
        {
            if (deformationFieldOutputName == FOFieldNames.Default) deformationFieldOutputName = _deformationFieldOutputName;
            if (scale != 0)
            {
                float[][] deformations = GetNodalMeshDeformations(deformationFieldOutputName, stepId, stepIncrementId);
                if (deformations != null)
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        int resultId = _nodeIdsLookUp[globalNodeIds[i]];
                        //
                        for (int j = 0; j < 3; j++)
                        {
                            nodes[i][j] += scale * deformations[j][resultId];
                        }
                    }
                }
            }
        }
        //
        public void ScaleValues(float scale, float[] values, out float[] scaledValues)
        {
            if (values == null) scaledValues = null;
            else
            {
                scaledValues = new float[values.Length];
                if (scale != 0)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        scaledValues[i] = values[i] * scale;
                    }
                }
            }
        }
        //
        public float GetMaxDeformation()
        {
            double fieldMax;
            double max = -double.MaxValue;
            string[] componentNames = GetFieldComponentNames(_deformationFieldOutputName);
            //
            foreach (var entry in _fields)
            {
                if (entry.Key.Name == _deformationFieldOutputName)
                {
                    fieldMax = 0;
                    //
                    foreach (var componentName in componentNames)
                        fieldMax += Math.Pow(entry.Value.GetComponentAbsMax(componentName), 2);
                    //
                    if (fieldMax > 0) fieldMax = Math.Sqrt(fieldMax);
                    //
                    if (fieldMax > max) max = fieldMax;
                }
            }
            return (float)max;
        }
        public float GetMaxDeformation(int stepId, int stepIncrementId)
        {
            double max = -double.MaxValue;
            foreach (var entry in _fields)
            {
                if (entry.Key.StepId == stepId && entry.Key.StepIncrementId == stepIncrementId &&
                    entry.Key.Name == _deformationFieldOutputName)
                {
                    max = 0;
                    string[] componentNames = GetFieldComponentNames(_deformationFieldOutputName);
                    //
                    foreach (var componentName in componentNames)
                        max += Math.Pow(entry.Value.GetComponentAbsMax(componentName), 2);
                    //
                    if (max > 0) max = Math.Sqrt(max);
                    //
                    break;
                }
            }
            return (float)max;
        }
        //
        public double GetEdgeLength(int geometryEdgeId)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryEdgeId);
            BasePart part = _mesh.GetPartById(itemTypePartIds[2]);
            int[] nodeIds = part.Visualization.GetOrderedNodeIdsForEdge(itemTypePartIds[0]);
            //
            return 0;
        }

        // Wear         
        public bool ComputeWear(int[] slipStepIds, Dictionary<int, double> nodeIdCoefficient,
                                Dictionary<int, bool[]> nodeIdZeroDisplacements)
        {
            if (slipStepIds != null && slipStepIds.Length > 0 && CheckFieldAndhistoryTimes())
            {
                ComputeHistoryWearSlidingDistance();
                //
                HistoryResultComponent slidingDistanceAll =
                    GetHistoryResultComponent(HOSetNames.ContactWear, HOFieldNames.SlidingDistance, HOComponentNames.All);
                if (slidingDistanceAll != null)
                {
                    CreateAveragedFieldFromElementFaceHistory(FOFieldNames.SlidingDistance, slidingDistanceAll, true);
                }
                //
                HistoryResultField surfaceNormalField = GetHistoryResultField(HOSetNames.ContactWear, HOFieldNames.SurfaceNormal);
                if (surfaceNormalField != null)
                {
                    HistoryResultComponent all = surfaceNormalField.Components[HOComponentNames.All];
                    HistoryResultComponent n1 = surfaceNormalField.Components[HOComponentNames.N1];
                    HistoryResultComponent n2 = surfaceNormalField.Components[HOComponentNames.N2];
                    HistoryResultComponent n3 = surfaceNormalField.Components[HOComponentNames.N3];
                    //
                    CreateAveragedFieldFromElementFaceHistory(FOFieldNames.SurfaceNormal, all, false);
                    CreateAveragedFieldFromElementFaceHistory(FOFieldNames.SurfaceNormal, n1, false);
                    CreateAveragedFieldFromElementFaceHistory(FOFieldNames.SurfaceNormal, n2, false);
                    CreateAveragedFieldFromElementFaceHistory(FOFieldNames.SurfaceNormal, n3, false);
                }
                //
                return ComputeSlidingWearFields(slipStepIds, nodeIdCoefficient, nodeIdZeroDisplacements);
            }
            else return false;
        }
        private bool CheckFieldAndhistoryTimes()
        {
            HistoryResultField relativeContactDisplacement =
                GetHistoryResultField(HOSetNames.AllContactElements, HOFieldNames.RelativeContactDisplacement);
            if (relativeContactDisplacement != null)
            {
                HistoryResultComponent tang1 = relativeContactDisplacement.Components[HOComponentNames.Tang1];
                if (tang1 != null)
                {
                    // Sorted time
                    double[] sortedTime;
                    Dictionary<double, int> timeRowId;
                    GetSortedTime(new HistoryResultComponent[] { tang1 }, out sortedTime, out timeRowId);
                    //
                    int count = 0;
                    int[] stepIds = GetAllStepIds();
                    for (int i = 0; i < stepIds.Length; i++)
                    {
                        int[] stepIncrementIds = GetIncrementIds(stepIds[i]);
                        count += stepIncrementIds.Length;
                    }
                    //
                    if (sortedTime.Length == count - 1) return true; // -1 for Zero increment - Find all occurances!!!
                }
            }
            return false;
        }
        private void ComputeHistoryWearSlidingDistance()
        {
            HistoryResultField relativeContactDisplacement =
                GetHistoryResultField(HOSetNames.AllContactElements, HOFieldNames.RelativeContactDisplacement);
            //
            if (relativeContactDisplacement != null)
            {
                HistoryResultComponent tang1 = relativeContactDisplacement.Components[HOComponentNames.Tang1];
                HistoryResultComponent s1 = ComputeRelativeDisplacement(HOComponentNames.S1, tang1);
                //
                HistoryResultComponent tang2 = relativeContactDisplacement.Components[HOComponentNames.Tang2];
                HistoryResultComponent s2 = ComputeRelativeDisplacement(HOComponentNames.S2, tang2);
                //
                HistoryResultComponent all =
                    ComputeVectorMagnitude(HOComponentNames.All, new HistoryResultComponent[] { s1, s2 });
                //
                relativeContactDisplacement = new HistoryResultField(HOFieldNames.SlidingDistance);
                relativeContactDisplacement.Components.Add(all.Name, all);
                relativeContactDisplacement.Components.Add(s1.Name, s1);
                relativeContactDisplacement.Components.Add(s2.Name, s2);
               
                // Use tang1 since it contains only values != 0
                HistoryResultComponent[] normalComponents = GetNormalsFromFromElementFaceHistory(tang1);
                //
                HistoryResultField surfaceNormalsField = new HistoryResultField(HOFieldNames.SurfaceNormal);
                surfaceNormalsField.Components.Add(normalComponents[0].Name, normalComponents[0]);  // All
                surfaceNormalsField.Components.Add(normalComponents[1].Name, normalComponents[1]);  // N1
                surfaceNormalsField.Components.Add(normalComponents[2].Name, normalComponents[2]);  // N2
                surfaceNormalsField.Components.Add(normalComponents[3].Name, normalComponents[3]);  // N3
                //
                HistoryResultSet contactWear = new HistoryResultSet(HOSetNames.ContactWear);
                contactWear.Fields.Add(relativeContactDisplacement.Name, relativeContactDisplacement);
                contactWear.Fields.Add(surfaceNormalsField.Name, surfaceNormalsField);
                //
                _history.Sets.Add(contactWear.Name, contactWear);
            }
        }
        public bool ComputeSlidingWearFields(int[] slipStepIds, Dictionary<int, double> nodeIdCoefficient,
                                             Dictionary<int, bool[]> nodeIdZeroDisplacements)
        {
            HistoryResultComponent slidingDistanceAll =
                GetHistoryResultComponent(HOSetNames.ContactWear, HOFieldNames.SlidingDistance, HOComponentNames.All);
            //
            if (slidingDistanceAll != null)
            {
                float[] coefficients = new float[_nodeIdsLookUp.Count()]; 
                if (nodeIdCoefficient != null)
                {
                    foreach (var entry in _nodeIdsLookUp) coefficients[entry.Value] = (float)nodeIdCoefficient[entry.Key];
                }
                else
                {
                    foreach (var entry in _nodeIdsLookUp) coefficients[entry.Value] = 0;
                }
                //
                float dh;
                float[] pressureValues;
                float[] slidingDistanceValues;
                float[] normalN1Values;
                float[] normalN2Values;
                float[] normalN3Values;
                float[] depthValuesMag;
                float[] depthValuesH1;
                float[] depthValuesH2;
                float[] depthValuesH3;
                float[] prevDepthValuesMag = null;
                float[] prevDepthValuesH1 = null;
                float[] prevDepthValuesH2 = null;
                float[] prevDepthValuesH3 = null;
                float[] dispValuesMag;
                float[] dispValuesU1;
                float[] dispValuesU2;
                float[] dispValuesU3;
                //
                Field pressureField;
                Field slidingDistanceField;
                Field depthField;
                Field normalField;
                Field dispField;
                Field meshUpdateField;
                //
                FieldData pressureData;
                FieldData slidingDistanceData;
                FieldData depthData;
                FieldData normalData;
                FieldData dispData;
                FieldData meshUpdateData;
                //
                int id;
                Vec3D normal = new Vec3D();
                //
                for (int i = 0; i < slipStepIds.Length; i++)
                {
                    int[] stepIncrementIds = GetIncrementIds(slipStepIds[i]);
                    //
                    for (int j = 0; j < stepIncrementIds.Length; j++)
                    {
                        if (slipStepIds[i] == 0 && stepIncrementIds[j] == 0) continue;
                        // Pressure
                        pressureData = GetFieldData(FOFieldNames.Contact, "", slipStepIds[i], stepIncrementIds[j]);
                        pressureField = GetField(pressureData);
                        if (pressureField != null)
                        {
                            pressureValues = pressureField.GetComponentValues(FOComponentNames.CPress);
                            // Disp
                            dispData = GetFieldData(FOFieldNames.Disp, "", slipStepIds[i], stepIncrementIds[j]);
                            dispField = GetField(dispData);
                            dispValuesMag = new float[pressureValues.Length];
                            dispValuesU1 = dispField.GetComponentValues(FOComponentNames.U1).ToArray(); // copy
                            dispValuesU2 = dispField.GetComponentValues(FOComponentNames.U2).ToArray(); // copy
                            dispValuesU3 = dispField.GetComponentValues(FOComponentNames.U3).ToArray(); // copy
                            // Sliding distance
                            slidingDistanceData = GetFieldData(FOFieldNames.SlidingDistance, "",
                                                               slipStepIds[i], stepIncrementIds[j]);
                            slidingDistanceField = GetField(slidingDistanceData);
                            slidingDistanceValues = slidingDistanceField.GetComponentValues(FOComponentNames.All);
                            // Normal
                            normalData = GetFieldData(FOFieldNames.SurfaceNormal, "", slipStepIds[i], stepIncrementIds[j]);
                            normalField = GetField(normalData);
                            normalN1Values = normalField.GetComponentValues(FOComponentNames.N1).ToArray();
                            normalN2Values = normalField.GetComponentValues(FOComponentNames.N2).ToArray();
                            normalN3Values = normalField.GetComponentValues(FOComponentNames.N3).ToArray();
                            // Adjust normals based on zero BCs
                            if (nodeIdZeroDisplacements != null)
                            {
                                foreach (var entry in nodeIdZeroDisplacements)
                                {
                                    id = _nodeIdsLookUp[entry.Key];
                                    if (entry.Value[0]) normal.X = 0;
                                    else normal.X = normalN1Values[id];
                                    if (entry.Value[1]) normal.Y = 0;
                                    else normal.Y = normalN2Values[id];
                                    if (entry.Value[2]) normal.Z = 0;
                                    else normal.Z = normalN3Values[id];
                                    //
                                    normal.Normalize();
                                    //
                                    normalN1Values[id] = (float)normal.X;
                                    normalN2Values[id] = (float)normal.Y;
                                    normalN3Values[id] = (float)normal.Z;
                                }
                            }
                            // Wear depth
                            depthValuesMag = new float[pressureValues.Length];
                            depthValuesH1 = new float[pressureValues.Length];
                            depthValuesH2 = new float[pressureValues.Length];
                            depthValuesH3 = new float[pressureValues.Length];
                            if (prevDepthValuesMag == null)
                            {
                                prevDepthValuesMag = new float[pressureValues.Length];
                                prevDepthValuesH1 = new float[pressureValues.Length];
                                prevDepthValuesH2 = new float[pressureValues.Length];
                                prevDepthValuesH3 = new float[pressureValues.Length];
                            }
                            //
                            for (int k = 0; k < depthValuesMag.Length; k++)
                            {
                                dh = coefficients[k] * pressureValues[k] * slidingDistanceValues[k];
                                depthValuesMag[k] = dh + prevDepthValuesMag[k];
                                depthValuesH1[k] = dh * normalN1Values[k] + prevDepthValuesH1[k];
                                depthValuesH2[k] = dh * normalN2Values[k] + prevDepthValuesH2[k];
                                depthValuesH3[k] = dh * normalN3Values[k] + prevDepthValuesH3[k];
                                // Disp with wear depth
                                dispValuesU1[k] += depthValuesH1[k];
                                dispValuesU2[k] += depthValuesH2[k];
                                dispValuesU3[k] += depthValuesH3[k];
                                dispValuesMag[k] = (float)Math.Sqrt(Math.Pow(dispValuesU1[k], 2) +
                                                                    Math.Pow(dispValuesU2[k], 2) +
                                                                    Math.Pow(dispValuesU3[k], 2));
                            }
                            // Wear depth
                            depthData = new FieldData(FOFieldNames.WearDepth);
                            depthData.StepId = slipStepIds[i];
                            depthData.StepIncrementId = stepIncrementIds[j];
                            depthData.Time = dispData.Time;
                            depthData.Type = StepType.Static;
                            depthField = new Field(depthData.Name);
                            depthField.AddComponent(FOComponentNames.All, depthValuesMag);
                            depthField.AddComponent(FOComponentNames.H1, depthValuesH1);
                            depthField.AddComponent(FOComponentNames.H2, depthValuesH2);
                            depthField.AddComponent(FOComponentNames.H3, depthValuesH3);
                            AddFiled(depthData, depthField);
                            // Mesh update
                            meshUpdateData = new FieldData(FOFieldNames.MeshDeformation);
                            meshUpdateData.StepId = slipStepIds[i];
                            meshUpdateData.StepIncrementId = stepIncrementIds[j];
                            meshUpdateData.Time = dispData.Time;
                            meshUpdateData.Type = StepType.Static;
                            meshUpdateField = new Field(meshUpdateData.Name);
                            meshUpdateField.AddComponent(FOComponentNames.All, new float[pressureValues.Length]);
                            meshUpdateField.AddComponent(FOComponentNames.U1, new float[pressureValues.Length]);
                            meshUpdateField.AddComponent(FOComponentNames.U2, new float[pressureValues.Length]);
                            meshUpdateField.AddComponent(FOComponentNames.U3, new float[pressureValues.Length]);
                            AddFiled(meshUpdateData, meshUpdateField);
                            // Disp with wear depth
                            dispData.Name = FOFieldNames.DispDeformationDepth;
                            dispField = new Field(dispData.Name);
                            dispField.AddComponent(FOComponentNames.All, dispValuesMag);
                            dispField.AddComponent(FOComponentNames.U1, dispValuesU1);
                            dispField.AddComponent(FOComponentNames.U2, dispValuesU2);
                            dispField.AddComponent(FOComponentNames.U3, dispValuesU3);
                            AddFiled(dispData, dispField);
                            //
                            prevDepthValuesMag = depthValuesMag;
                            prevDepthValuesH1 = depthValuesH1;
                            prevDepthValuesH2 = depthValuesH2;
                            prevDepthValuesH3 = depthValuesH3;
                        }
                    }
                }
                //
                return true;
            }
            else return false;
        }
        private float[][] GetLocalVectors(string fieldName)
        {
            string prevFieldOutputName = _deformationFieldOutputName;
            _deformationFieldOutputName = fieldName;
            //
            int[] stepIds = GetAllStepIds();
            int stepId = stepIds.Last();
            int[] stepIncrementIds = GetIncrementIds(stepId);
            int stepIncrementId = stepIncrementIds.Last();
            //
            float[][] vectors = GetNodalMeshDeformations(stepId, stepIncrementId);
            //
            _deformationFieldOutputName = prevFieldOutputName;
            //
            return vectors;
        }
        public Dictionary<int, double[]> GetGlobalNonZeroVectors(string fieldName)
        {
            double[] xyz = new double[3];
            float[][] vectors = GetLocalVectors(fieldName);
            Dictionary<int, double[]> globalVectors = new Dictionary<int, double[]>();
            foreach (var entry in _nodeIdsLookUp)
            {
                xyz[0] = vectors[0][entry.Value];
                xyz[1] = vectors[1][entry.Value];
                xyz[2] = vectors[2][entry.Value];
                //
                if (xyz[0] != 0 || xyz[1] != 0 || xyz[2] != 0)
                    globalVectors.Add(entry.Key, new double[] { xyz[0], xyz[1], xyz[2] });
            }
            //
            return globalVectors;
        }
        //
        private HistoryResultComponent ComputeRelativeDisplacement(string componentName, HistoryResultComponent tang1)
        {
            double[] time;
            double[] values;
            HistoryResultEntries itemData;
            HistoryResultComponent component = new HistoryResultComponent(componentName);
            //
            foreach (var entry in tang1.Entries)
            {
                time = entry.Value.Time.ToArray();
                values = entry.Value.Values.ToArray();
                itemData = new HistoryResultEntries(entry.Key, false);
                //
                itemData.Add(time[0], 0);   // must be here to keep the time in the component
                for (int i = 1; i < values.Length; i++)
                {
                    // Add only values != 0
                    if (values[i - 1] != 0 && values[i] != 0) itemData.Add(time[i], values[i] - values[i - 1]);
                }
                if (itemData.Time.Count > 0) component.Entries.Add(itemData.Name, itemData);
            }
            //
            return component;
        }
        private HistoryResultComponent ComputeVectorMagnitude(string componentName, HistoryResultComponent[] components)
        {
            // Sorted time
            double[] sortedTime;
            Dictionary<double, int> timeRowId;
            GetSortedTime(components, out sortedTime, out timeRowId);
            // Get all entry names
            HashSet<string> entryNamesHash = new HashSet<string>();
            foreach (var component in components)
            {
                if (entryNamesHash.Count == 0) entryNamesHash.UnionWith(component.Entries.Keys);
                else entryNamesHash.IntersectWith(component.Entries.Keys);
            }
            //
            double value;
            double[][] values = new double[components.Length][];
            HistoryResultEntries historyEntry;
            HistoryResultComponent magnitudeComponent = new HistoryResultComponent(componentName);
            foreach (var entryName in entryNamesHash)
            {
                // Get all entry values from all components
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].Entries.TryGetValue(entryName, out historyEntry))
                        values[i] = GetAllEntryValues(historyEntry, timeRowId);
                    else values[i] = new double[timeRowId.Count];
                }
                // Compute the magnitude
                historyEntry = new HistoryResultEntries(entryName, false);
                for (int i = 0; i < sortedTime.Length; i++)
                {
                    value = 0;
                    for (int j = 0; j < components.Length; j++)
                    {
                        value += Math.Pow(values[j][i], 2);
                    }
                    historyEntry.Add(sortedTime[i], Math.Sqrt(value));
                }
                //
                if (historyEntry.Time.Count > 0) magnitudeComponent.Entries.Add(historyEntry.Name, historyEntry);
            }
            //
            return magnitudeComponent;
        }
        private HistoryResultComponent[] GetNormalsFromFromElementFaceHistory(HistoryResultComponent historyResultComponent)
        {
            HistoryResultComponent[] normalComponents = new HistoryResultComponent[4];
            normalComponents[0] = new HistoryResultComponent(HOComponentNames.All);
            normalComponents[1] = new HistoryResultComponent(HOComponentNames.N1);
            normalComponents[2] = new HistoryResultComponent(HOComponentNames.N2);
            normalComponents[3] = new HistoryResultComponent(HOComponentNames.N3);
            // Sorted time
            double[] sortedTime;
            Dictionary<double, int> timeRowId;
            GetSortedTime(new HistoryResultComponent[] { historyResultComponent }, out sortedTime, out timeRowId);
            // Get all values data for each node
            int faceId;
            int elementId;
            int count = 0;
            string[] tmp;
            double faceArea;
            double[] faceNormal;
            FeElement element;
            FeFaceName faceName;
            HistoryResultEntries normalAllEntry;
            HistoryResultEntries normal1Entry;
            HistoryResultEntries normal2Entry;
            HistoryResultEntries normal3Entry;
            //
            foreach (var entry in historyResultComponent.Entries)
            {
                tmp = entry.Key.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2 && int.TryParse(tmp[0], out elementId) && int.TryParse(tmp[1], out faceId))
                {
                    switch (faceId)
                    {
                        case 1: faceName = FeFaceName.S1; break;
                        case 2: faceName = FeFaceName.S2; break;
                        case 3: faceName = FeFaceName.S3; break;
                        case 4: faceName = FeFaceName.S4; break;
                        case 5: faceName = FeFaceName.S5; break;
                        case 6: faceName = FeFaceName.S6; break;
                        default: throw new NotSupportedException();
                    }
                    element = _mesh.Elements[elementId];
                    _mesh.GetElementFaceNormalAndArea(elementId, faceName, out faceNormal, out faceArea);
                    //
                    normalAllEntry = new HistoryResultEntries(entry.Key, false);
                    normal1Entry = new HistoryResultEntries(entry.Key, false);
                    normal2Entry = new HistoryResultEntries(entry.Key, false);
                    normal3Entry = new HistoryResultEntries(entry.Key, false);
                    //
                    foreach (var time in entry.Value.Time)
                    {
                        normalAllEntry.Add(time, 1);
                        normal1Entry.Add(time, faceNormal[0]);
                        normal2Entry.Add(time, faceNormal[1]);
                        normal3Entry.Add(time, faceNormal[2]);
                    }
                    //
                    normalComponents[0].Entries.Add(normalAllEntry.Name, normalAllEntry);
                    normalComponents[1].Entries.Add(normal1Entry.Name, normal1Entry);
                    normalComponents[2].Entries.Add(normal2Entry.Name, normal2Entry);
                    normalComponents[3].Entries.Add(normal3Entry.Name, normal3Entry);
                }
                count++;
            }
            //
            return normalComponents;
        }
        private void CreateAveragedFieldFromElementFaceHistory(string fieldName,
                                                               HistoryResultComponent historyResultComponent,
                                                               bool areaAverage)
        {
            if (historyResultComponent != null)
            {
                // Sorted time
                double[] sortedTime;
                Dictionary<double, int> timeRowId;
                GetSortedTime(new HistoryResultComponent[] { historyResultComponent }, out sortedTime, out timeRowId);
                Dictionary<double, float[]> timeAvgValues =
                    GetNodalValuesFromElementFaceHistory(historyResultComponent, areaAverage);
                //
                bool newField;
                float[] values;
                Field field;
                FieldData fieldData = new FieldData(fieldName, historyResultComponent.Name, 0, 0);
                int count = 0;
                int[] stepIds = GetAllStepIds();
                int[] stepIncrementIds;
                //
                fieldData.Type = StepType.Static;
                //
                for (int i = 0; i < stepIds.Length; i++)
                {
                    stepIncrementIds = GetIncrementIds(stepIds[i]);
                    //
                    for (int j = 0; j < stepIncrementIds.Length; j++)
                    {
                        if (i == 0 && j == 0) continue; // Zero increment - Find all occurances!!!
                                                        // Field
                        values = timeAvgValues[sortedTime[count]];
                        fieldData.StepId = stepIds[i];
                        fieldData.StepIncrementId = stepIncrementIds[j];
                        fieldData.Time = (float)sortedTime[count];
                        //
                        newField = false;
                        field = GetField(fieldData);
                        if (field == null)
                        {
                            field = new Field(fieldData.Name);
                            newField = true;
                        }
                        field.AddComponent(new FieldComponent(fieldData.Component, values));
                        if (newField) AddFiled(fieldData, field);
                        //
                        count++;
                    }
                }
            }
        }
        private double[] GetAllEntryValues(HistoryResultEntries historyEntry, Dictionary<double, int> timeRowId)
        {
            int id;
            double[] values;
            double[] timePoints;
            double[] allValues = new double[timeRowId.Count];
            //
            timePoints = historyEntry.Time.ToArray();
            values = historyEntry.Values.ToArray();
            for (int i = 0; i < timePoints.Length; i++)
            {
                id = timeRowId[timePoints[i]];
                allValues[id] = values[i];
            }
            return allValues;
        }
        private Dictionary<double, float[]> GetNodalValuesFromElementFaceHistory(HistoryResultComponent historyResultComponent,
                                                                                 bool areaAverage)
        {
            // Sorted time
            double[] sortedTime;
            Dictionary<double, int> timeRowId;
            GetSortedTime(new HistoryResultComponent[] { historyResultComponent }, out sortedTime, out timeRowId);
            // Average data structure
            AvgData avgData = new AvgData();
            foreach (var entry in _mesh.Parts)
            {
                avgData.AddRange(entry.Value.Visualization.GetAvgData().Nodes);
            }
            // Get all values data for each node
            int faceId;
            int elementId;
            int count = 0;
            string[] tmp;
            double faceArea;
            double[] faceNormal;
            double[][] historyValues = new double[historyResultComponent.Entries.Count][];
            FeElement element;
            FeFaceName faceName;
            AvgEntryData avgEntryData;
            Dictionary<int, AvgEntryData> valueIdAvgEntryData = new Dictionary<int, AvgEntryData>();
            //
            foreach (var entry in historyResultComponent.Entries)
            {
                tmp = entry.Key.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2 && int.TryParse(tmp[0], out elementId) && int.TryParse(tmp[1], out faceId))
                {
                    switch (faceId)
                    {
                        case 1: faceName = FeFaceName.S1; break;
                        case 2: faceName = FeFaceName.S2; break;
                        case 3: faceName = FeFaceName.S3; break;
                        case 4: faceName = FeFaceName.S4; break;
                        case 5: faceName = FeFaceName.S5; break;
                        case 6: faceName = FeFaceName.S6; break;
                        default: throw new NotSupportedException();
                    }
                    element = _mesh.Elements[elementId];
                    _mesh.GetElementFaceNormalAndArea(elementId, faceName, out faceNormal, out faceArea);
                    //
                    avgEntryData = new AvgEntryData();
                    avgEntryData.NodeIds = element.GetNodeIdsFromFaceName(faceName);
                    avgEntryData.SurfaceId = 0;
                    avgEntryData.ElementId = elementId;
                    avgEntryData.Area = faceArea;
                    //
                    valueIdAvgEntryData.Add(count, avgEntryData);
                    historyValues[count] = GetAllEntryValues(entry.Value, timeRowId);
                }
                count++;
            }
            // Average
            float[] averagedValues;
            AvgData avgDataClone;
            Dictionary<int, double> averagedNodalValues;
            Dictionary<double, float[]> timeValues = new Dictionary<double, float[]>();
            for (int i = 0; i < sortedTime.Length; i++)
            {
                avgDataClone = new AvgData(avgData);
                //
                for (int j = 0; j < historyValues.Length; j++)
                {
                    avgEntryData = valueIdAvgEntryData[j];
                    for (int k = 0; k < avgEntryData.NodeIds.Length; k++)
                    {
                        avgDataClone.Nodes[avgEntryData.NodeIds[k]].Elements[avgEntryData.ElementId].Data.Add(
                            new Tuple<double, double>(historyValues[j][i], avgEntryData.Area));
                    }
                }
                //
                averagedValues = new float[_nodeIdsLookUp.Count];
                averagedNodalValues = avgDataClone.GetAveragedValues(areaAverage);
                foreach (var entry in averagedNodalValues)
                {
                    averagedValues[_nodeIdsLookUp[entry.Key]] = (float)entry.Value;
                }
                timeValues.Add(sortedTime[i], averagedValues);
            }
            //
            return timeValues;
        }
        // Select slip wear results
        public void KeepOnlySelectedSlipWearResults(OrderedDictionary<int, double> stepIdDuration, int[] slipStepIds,
                                                    SlipWearResultsEnum slipWearResultsToKeep)
        {
            Dictionary<int, float> stepIdMaxTime = GetMaxStepTime();
            //
            KeepOnlySelectedFieldSlipWearResults(slipStepIds, slipWearResultsToKeep);
            KeepOnlySelectedHistorySlipWearResults(stepIdDuration, slipStepIds, slipWearResultsToKeep);
        }
        private void KeepOnlySelectedFieldSlipWearResults(int[] slipStepIds, SlipWearResultsEnum slipWearResultsToKeep)
        {
            HashSet<int> slipStepIdsHash = new HashSet<int>(slipStepIds);
            //
            if (slipWearResultsToKeep == SlipWearResultsEnum.All) { }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.SlipWearSteps)
            {
                Dictionary<FieldData, Field> fields = new Dictionary<FieldData, Field>();
                //
                foreach (var entry in _fields)
                {
                    if (slipStepIdsHash.Contains(entry.Key.StepId))
                    {
                        fields.Add(entry.Key, entry.Value);
                    }
                }
                _fields = fields;
            }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.LastIncrementOfSlipWearSteps)
            {
                int lastIncrementId;
                Dictionary<FieldData, Field> fields = new Dictionary<FieldData, Field>();
                //
                foreach (var entry in _fields)
                {
                    if (slipStepIdsHash.Contains(entry.Key.StepId))
                    {
                        lastIncrementId = GetIncrementIds(entry.Key.StepId).Last();
                        //
                        if (entry.Key.StepIncrementId == lastIncrementId)
                            fields.Add(entry.Key, entry.Value);
                    }
                }
                _fields = fields;
            }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.LastIncrementOfLastSlipWearStep)
            {
                int lastIncrementId;
                int lastStepId = slipStepIds.Last();
                Dictionary<FieldData, Field> fields = new Dictionary<FieldData, Field>();
                //
                foreach (var entry in _fields)
                {
                    if (slipStepIdsHash.Contains(entry.Key.StepId))
                    {
                        if (entry.Key.StepId == lastStepId)
                        {
                            lastIncrementId = GetIncrementIds(entry.Key.StepId).Last();
                            //
                            if (entry.Key.StepIncrementId == lastIncrementId)
                                fields.Add(entry.Key, entry.Value);
                        }
                    }
                }
                _fields = fields;
            }
            else throw new NotSupportedException();
        }
        private void KeepOnlySelectedHistorySlipWearResults(OrderedDictionary<int, double> stepIdDuration, int[] slipStepIds,
                                                            SlipWearResultsEnum slipWearResultsToKeep)
        {
            HashSet<int> slipStepIdsHash = new HashSet<int>(slipStepIds);
            //
            double sumTime = 0;
            OrderedDictionary<int, double> stepIdStartTime = new OrderedDictionary<int, double>("Step id - step time");
            foreach (var entry in stepIdDuration)
            {
                stepIdStartTime.Add(entry.Key, sumTime);
                sumTime += entry.Value;
            }
            //
            int stepId;
            double startTime;
            double endTime;
            List<double[]> minMaxList = new List<double[]>();
            //
            if (slipWearResultsToKeep == SlipWearResultsEnum.All) 
            {
                minMaxList.Add(new double[] { 0, sumTime });
            }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.SlipWearSteps)
            {
                foreach (var entry in stepIdDuration)
                {
                    stepId = entry.Key;
                    //
                    if (slipStepIdsHash.Contains(stepId))
                    {
                        startTime = stepIdStartTime[stepId];
                        endTime = startTime + entry.Value;
                        minMaxList.Add(new double[] { startTime, endTime });
                    }
                }
            }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.LastIncrementOfSlipWearSteps)
            {
                foreach (var entry in stepIdDuration)
                {
                    stepId = entry.Key;
                    //
                    if (slipStepIdsHash.Contains(stepId))
                    {
                        startTime = stepIdStartTime[stepId];
                        endTime = startTime + entry.Value;
                        minMaxList.Add(new double[] { endTime, endTime });
                    }
                }
            }
            else if (slipWearResultsToKeep == SlipWearResultsEnum.LastIncrementOfLastSlipWearStep)
            {
                stepId = slipStepIds.Last();
                //
                if (slipStepIdsHash.Contains(stepId))
                {
                    startTime = stepIdStartTime[stepId];
                    endTime = startTime + stepIdDuration[stepId];
                    minMaxList.Add(new double[] { endTime, endTime });
                }
            }
            else throw new NotSupportedException();
            //
            double[][] minMax = minMaxList.ToArray();
            //
            List<string> entryNamesToRemove = new List<string>();
            foreach (var setEntry in _history.Sets)
            {
                foreach (var fieldEntry in setEntry.Value.Fields)
                {
                    foreach (var componentEntry in fieldEntry.Value.Components)
                    {
                        entryNamesToRemove.Clear();
                        //
                        foreach (var entry in componentEntry.Value.Entries)
                        {
                            entry.Value.KeepOnly(minMax);
                            if (entry.Value.Time.Count == 0) entryNamesToRemove.Add(entry.Key);
                        }
                        // Remove empty
                        foreach (var entryName in entryNamesToRemove) componentEntry.Value.Entries.Remove(entryName);
                    }
                }
            }
        }

    }
}
