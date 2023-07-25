using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.Runtime.Serialization;
using Calculix = FileInOut.Output.Calculix;
using System.IO;
using System.Drawing;
using System.Data;

namespace CaeModel
{
    [Serializable]
    public class FeModel : ISerializable
    {
        // Variables                                                                                                                
        private string _hashName;                                                               //ISerializable
        private FeMesh _geometry;                                                               //ISerializable
        private FeMesh _mesh;                                                                   //ISerializable
        private OrderedDictionary<string, Material> _materials;                                 //ISerializable
        private OrderedDictionary<string, Section> _sections;                                   //ISerializable
        private OrderedDictionary<string, Constraint> _constraints;                             //ISerializable
        private OrderedDictionary<string, SurfaceInteraction> _surfaceInteractions;             //ISerializable
        private OrderedDictionary<string, ContactPair> _contactPairs;                           //ISerializable
        private OrderedDictionary<string, Amplitude> _amplitudes;                               //ISerializable
        private OrderedDictionary<string, InitialCondition> _initialConditions;                 //ISerializable
        private StepCollection _stepCollection;                                                 //ISerializable
        private OrderedDictionary<int[], Calculix.CalculixUserKeyword> _calculixUserKeywords;   //ISerializable
        private ModelProperties _properties;                                                    //ISerializable
        private UnitSystem _unitSystem;                                                         //ISerializable


        // Properties                                                                                                               
        public string Name { get; set; }
        public string HashName { get { return _hashName; } }
        public FeMesh Geometry { get { return _geometry; } }
        public FeMesh Mesh { get { return _mesh; } }
        public OrderedDictionary<string, Material> Materials { get { return _materials; } }
        public OrderedDictionary<string, Section> Sections { get { return _sections; } }
        public OrderedDictionary<string, Constraint> Constraints { get { return _constraints; } }
        public OrderedDictionary<string, SurfaceInteraction> SurfaceInteractions { get { return _surfaceInteractions; } }
        public OrderedDictionary<string, ContactPair> ContactPairs { get { return _contactPairs; } }
        public OrderedDictionary<string, Amplitude> Amplitudes { get { return _amplitudes; } }
        public OrderedDictionary<string, InitialCondition> InitialConditions { get { return _initialConditions; } }
        public StepCollection StepCollection { get { return _stepCollection; } }
        public OrderedDictionary<int[], Calculix.CalculixUserKeyword> CalculixUserKeywords
        {
            get { return _calculixUserKeywords; }
            set
            {
                _calculixUserKeywords = value;
            }
        }
        public ModelProperties Properties { get { return _properties; } set { _properties = value; } }
        public UnitSystem UnitSystem
        {
            get { return _unitSystem; }
            set { _unitSystem = value; }
        }


        // Constructors                                                                                                             
        public FeModel(string name)
        {
            StringComparer sc = StringComparer.OrdinalIgnoreCase;
            //
            Name = name;
            _hashName = Tools.GetRandomString(8);
            _geometry = new FeMesh(MeshRepresentation.Geometry);
            _mesh = new FeMesh(MeshRepresentation.Mesh);
            _materials = new OrderedDictionary<string, Material>("Materials", sc);
            _sections = new OrderedDictionary<string, Section>("Sections", sc);
            _constraints = new OrderedDictionary<string, Constraint>("Constraints", sc);
            _surfaceInteractions = new OrderedDictionary<string, SurfaceInteraction>("Suface Tractions", sc);
            _contactPairs = new OrderedDictionary<string, ContactPair>("Contact Pairs", sc);
            _initialConditions = new OrderedDictionary<string, InitialCondition>("Initial Conditions", sc);
            _amplitudes = new OrderedDictionary<string, Amplitude>("Amplitudes", sc);
            _stepCollection = new StepCollection();
            _properties = new ModelProperties();
            _unitSystem = new UnitSystem();
        }

        // ISerialization
        public FeModel(SerializationInfo info, StreamingContext context)
        {
            StringComparer sc = StringComparer.OrdinalIgnoreCase;
            // Compatibility for version v.0.6.0
            _surfaceInteractions = new OrderedDictionary<string, SurfaceInteraction>("Suface Tractions", sc);
            _contactPairs = new OrderedDictionary<string, ContactPair>("Contact Pairs", sc);
            // Compatibility for version v.0.7.0
            _unitSystem = new UnitSystem();
            // Compatibility for version v.0.8.0
            _hashName = Tools.GetRandomString(8);
            // Compatibility for version v.1.0.0
            _initialConditions = new OrderedDictionary<string, InitialCondition>("Initial Conditions", sc);
            // Compatibility for version v.1.2.1
            _amplitudes = new OrderedDictionary<string, Amplitude>("Amplitudes", sc);
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "<Name>k__BackingField":               // Compatibility for version v.0.5.1
                    case "_name":
                        Name = (string)entry.Value; break;
                    case "_geometry":
                        _geometry = (FeMesh)entry.Value; break;
                    case "_mesh":
                        _mesh = (FeMesh)entry.Value; break;
                    case "_materials":
                        if (entry.Value is Dictionary<string, Material> md)
                        {
                            // Compatibility for version v.0.5.1
                            md.OnDeserialization(null);
                            _materials = new OrderedDictionary<string, Material>("", md, sc);
                        }
                        else if (entry.Value is OrderedDictionary<string, Material> mod) _materials = mod;
                        else if (entry.Value == null) _materials = null;
                        else throw new NotSupportedException();
                        break;
                    case "_sections":
                        if (entry.Value is Dictionary<string, Section> sd)
                        {
                            // Compatibility for version v.0.5.1
                            sd.OnDeserialization(null);
                            _sections = new OrderedDictionary<string, Section>("", sd, sc);
                        }
                        else if (entry.Value is OrderedDictionary<string, Section> sod) _sections = sod;
                        else if (entry.Value == null) _sections = null;
                        else throw new NotSupportedException();
                        break;
                    case "_constraints":
                        if (entry.Value is Dictionary<string, Constraint> cd)
                        {
                            // Compatibility for version v.0.5.1
                            cd.OnDeserialization(null);
                            _constraints = new OrderedDictionary<string, Constraint>("", cd, sc);
                        }
                        else if (entry.Value is OrderedDictionary<string, Constraint> cod) _constraints = cod;
                        else if (entry.Value == null) _constraints = null;
                        else throw new NotSupportedException();
                        break;
                    case "_surfaceInteractions":
                        _surfaceInteractions = (OrderedDictionary<string, SurfaceInteraction>)entry.Value; break;
                    case "_contactPairs":
                        _contactPairs = (OrderedDictionary<string, ContactPair>)entry.Value; break;
                    case "_amplitudes":
                        _amplitudes = (OrderedDictionary<string, Amplitude>)entry.Value; break;
                    case "_initialConditions":
                        _initialConditions = (OrderedDictionary<string, InitialCondition>)entry.Value; break;
                    case "_stepCollection":
                        _stepCollection = (StepCollection)entry.Value; break;
                    case "_calculixUserKeywords":
                        if (entry.Value is Dictionary<int[], Calculix.CalculixUserKeyword> cukd)
                        {
                            // Compatibility for version v.0.5.1
                            cukd.OnDeserialization(null);
                            _calculixUserKeywords = new OrderedDictionary<int[], Calculix.CalculixUserKeyword>("", cukd);
                        }
                        else if (entry.Value is OrderedDictionary<int[], Calculix.CalculixUserKeyword> cukod)
                            _calculixUserKeywords = cukod;
                        else if (entry.Value == null) _calculixUserKeywords = null;
                        else throw new NotSupportedException();
                        break;
                    case "_properties":
                        _properties = (ModelProperties)entry.Value;
                        break;
                    case "_unitSystem":
                        _unitSystem = (UnitSystem)entry.Value; break;
                    case "_hashName":
                        _hashName = (string)entry.Value; break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }


        // Methods                                                                                                                  
        public static void WriteToFile(FeModel model, BinaryWriter bw)
        {
            // Write geometry
            if (model == null || model.Geometry == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                FeMesh.WriteToBinaryFile(model.Geometry, bw);
            }
            // Write mesh
            if (model == null || model.Mesh == null)
            {
                bw.Write((int)0);
            }
            else
            {
                bw.Write((int)1);
                FeMesh.WriteToBinaryFile(model.Mesh, bw);
            }
        }
        public static void ReadFromFile(FeModel model, BinaryReader br, int version)
        {
            // Read geometry
            int geometryExists = br.ReadInt32();
            if (geometryExists == 1)
            {
                FeMesh.ReadFromBinaryFile(model.Geometry, br, version);
            }
            // Read mesh
            int meshExists = br.ReadInt32();
            if (meshExists == 1)
            {
                FeMesh.ReadFromBinaryFile(model.Mesh, br, version);
            }
        }
        //
        public string[] CheckValidity(List<Tuple<NamedClass, string>> items)
        {
            // Tuple<NamedClass, string>   ...   Tuple<invalidItem, stepName>
            if (_mesh == null) return new string[0];
            //
            List<string> invalidItems = new List<string>();
            bool valid = false;
            invalidItems.AddRange(_geometry.CheckValidity(items));
            invalidItems.AddRange(_mesh.CheckValidity(items));
            // Sections
            Section section;
            foreach (var entry in _sections)
            {
                section = entry.Value;
                if (section is SolidSection ss)
                {
                    valid = _materials.ContainsValidKey(section.MaterialName)
                        && ((ss.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(section.RegionName))
                        || (ss.RegionType == RegionTypeEnum.ElementSetName
                            && _mesh.ElementSets.ContainsValidKey(section.RegionName)));
                }
                else if (section is ShellSection shs)
                {
                    valid = _materials.ContainsValidKey(section.MaterialName)
                        && ((shs.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(section.RegionName))
                        || (shs.RegionType == RegionTypeEnum.ElementSetName
                            && _mesh.ElementSets.ContainsValidKey(section.RegionName)));
                }
                else if (section is MembraneSection ms)
                {
                    valid = _materials.ContainsValidKey(section.MaterialName)
                        && ((ms.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(section.RegionName))
                        || (ms.RegionType == RegionTypeEnum.ElementSetName
                            && _mesh.ElementSets.ContainsValidKey(section.RegionName)));
                }
                else throw new NotSupportedException();
                //
                SetItemValidity(null, section, valid, items);
                if (!valid && section.Active) invalidItems.Add("Section: " + section.Name);
            }
            // Constraints
            Constraint constraint;
            foreach (var entry in _constraints)
            {
                constraint = entry.Value;
                if (constraint is PointSpring ps)
                {
                    valid = (ps.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(ps.RegionName))
                            || (ps.RegionType == RegionTypeEnum.ReferencePointName
                               && _mesh.ReferencePoints.ContainsValidKey(ps.RegionName));
                }
                else if (constraint is SurfaceSpring ss)
                {
                    valid = ss.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsValidKey(ss.RegionName);
                }
                else if (constraint is RigidBody rb)
                {
                    valid = rb.ReferencePointName != null
                            && _mesh.ReferencePoints.ContainsValidKey(rb.ReferencePointName)
                            && ((rb.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(rb.RegionName))
                            || (rb.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsValidKey(rb.RegionName)));
                }
                else if (constraint is Tie t)
                {
                    valid = _mesh.Surfaces.ContainsValidKey(t.SlaveRegionName)
                            && _mesh.Surfaces.ContainsValidKey(t.MasterRegionName)
                            && (t.SlaveRegionName != t.MasterRegionName);
                }
                else throw new NotSupportedException();
                //
                SetItemValidity(null, constraint, valid, items);
                if (!valid && constraint.Active) invalidItems.Add("Constraint: " + constraint.Name);
            }
            // Contact pairs
            ContactPair contactPair;
            foreach (var entry in _contactPairs)
            {
                contactPair = entry.Value;
                valid = _surfaceInteractions.ContainsValidKey(contactPair.SurfaceInteractionName)
                        && _mesh.Surfaces.ContainsValidKey(contactPair.SlaveRegionName)
                        && _mesh.Surfaces.ContainsValidKey(contactPair.MasterRegionName)
                        && (contactPair.SlaveRegionName != contactPair.MasterRegionName);
                //
                SetItemValidity(null, contactPair, valid, items);
                if (!valid && contactPair.Active) invalidItems.Add("Contact pair: " + contactPair.Name);
            }
            // Initial conditions
            InitialCondition initialCondition;
            foreach (var entry in _initialConditions)
            {
                initialCondition = entry.Value;
                if (initialCondition is InitialTemperature it)
                {
                    valid = (it.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(it.RegionName))
                            || (it.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsValidKey(it.RegionName));
                }
                SetItemValidity(null, initialCondition, valid, items);
                if (!valid && initialCondition.Active) invalidItems.Add("Initial condition: " + initialCondition.Name);
            }
            // Steps
            HistoryOutput historyOutput;
            BoundaryCondition bc;
            Load load;
            //
            foreach (var step in _stepCollection.StepsList)
            {
                // History output
                foreach (var hoEntry in step.HistoryOutputs)
                {
                    historyOutput = hoEntry.Value;
                    if (historyOutput is NodalHistoryOutput nho)
                    {
                        valid = (nho.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(nho.RegionName))
                                || (nho.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsValidKey(nho.RegionName))
                                || (nho.RegionType == RegionTypeEnum.ReferencePointName
                                && _mesh.ReferencePoints.ContainsValidKey(nho.RegionName));
                    }
                    else if (historyOutput is ElementHistoryOutput eho)
                    {
                        valid = _mesh.ElementSets.ContainsValidKey(eho.RegionName);
                    }
                    else if (historyOutput is ContactHistoryOutput cho)
                    {
                        valid = _contactPairs.ContainsValidKey(cho.RegionName);
                    }
                    else throw new NotSupportedException();
                    //
                    SetItemValidity(step.Name, historyOutput, valid, items);
                    if (!valid && historyOutput.Active) invalidItems.Add("History output: " + step.Name + ", " + historyOutput.Name);
                }
                // Boundary conditions
                foreach (var bcEntry in step.BoundaryConditions)
                {
                    bc = bcEntry.Value;
                    // Region
                    valid = IsBoundaryConditionRegionValid(bc);
                    // Amplitude
                    if (bc.AmplitudeName != BoundaryCondition.DefaultAmplitudeName &&
                        !_amplitudes.ContainsValidKey(bc.AmplitudeName)) valid = false;
                    //
                    SetItemValidity(step.Name, bc, valid, items);
                    if (!valid && bc.Active) invalidItems.Add("Boundary condition: " + step.Name + ", " + bc.Name);
                }
                // Loads
                foreach (var loadEntry in step.Loads)
                {
                    load = loadEntry.Value;
                    // Region
                    valid = IsLoadRegionValid(load);
                    // Amplitude
                    if (load.AmplitudeName != BoundaryCondition.DefaultAmplitudeName &&
                        !_amplitudes.ContainsValidKey(load.AmplitudeName)) valid = false;
                    //
                    SetItemValidity(step.Name, load, valid, items);
                    if (!valid && load.Active) invalidItems.Add("Load: " + step.Name + ", " + load.Name);
                }
            }
            //
            return invalidItems.ToArray();
        }
        private void Check2D(FeMesh mesh)
        {
            // Check for 2D geometry - the same check in ImportGeometry
            if (_properties.ModelSpace.IsTwoD())
            {
                if (!mesh.BoundingBox.Is2D())
                    throw new CaeException("The selected file does not contain 2D geometry in x-y plane.");
                else if (!Check2DSurfaceNormals(mesh))
                {
                    MessageBoxes.ShowWarning("The imported geometry contains faces oriented in the wrong direction (negative Z). " +
                                             "To use them in the analysis, change their orientation.");
                }
            }
        }
        private bool Check2DSurfaceNormals(FeMesh mesh)
        {
            int cellId;
            int[] cell;
            FeNode normal;
            foreach (var entry in mesh.Parts)
            {
                for (int i = 0; i < entry.Value.Visualization.CellIdsByFace.Length; i++)
                {
                    if (entry.Value.Visualization.CellIdsByFace[i].Length > 0)
                    {
                        cellId = entry.Value.Visualization.CellIdsByFace[i][0];
                        cell = entry.Value.Visualization.Cells[cellId];
                        normal = mesh.ComputeNormalFromFaceCellIndices(cell);
                        if (normal.Z < 0) return false;
                    }
                }
            }
            return true;
        }
        public bool IsBoundaryConditionRegionValid(BoundaryCondition bc)
        {
            bool valid;
            //
            if (bc is FixedBC fix)
            {
                valid = (fix.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(fix.RegionName))
                        || (fix.RegionType == RegionTypeEnum.SurfaceName && (_mesh.Surfaces.ContainsValidKey(fix.RegionName)))
                        || (fix.RegionType == RegionTypeEnum.ReferencePointName
                        && (_mesh.ReferencePoints.ContainsValidKey(fix.RegionName)));
            }
            else if (bc is DisplacementRotation dr)
            {
                valid = (dr.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(dr.RegionName))
                        || (dr.RegionType == RegionTypeEnum.SurfaceName && (_mesh.Surfaces.ContainsValidKey(dr.RegionName)))
                        || (dr.RegionType == RegionTypeEnum.ReferencePointName
                        && (_mesh.ReferencePoints.ContainsValidKey(dr.RegionName)));
            }
            else if (bc is SubmodelBC sm)
            {
                valid = (sm.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(sm.RegionName))
                        || (sm.RegionType == RegionTypeEnum.SurfaceName && (_mesh.Surfaces.ContainsValidKey(sm.RegionName)));
            }
            else if (bc is TemperatureBC tmp)
            {
                valid = (tmp.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(tmp.RegionName))
                        || (tmp.RegionType == RegionTypeEnum.SurfaceName && (_mesh.Surfaces.ContainsValidKey(tmp.RegionName)));
            }
            else throw new NotSupportedException();
            //
            return valid;
        }
        public bool IsLoadRegionValid(Load load)
        {
            bool valid;
            FeSurface s;
            //
            if (load is CLoad cl)
            {
                valid = (cl.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(cl.RegionName))
                        || (cl.RegionType == RegionTypeEnum.ReferencePointName
                        && (_mesh.ReferencePoints.ContainsValidKey(cl.RegionName)));
            }
            else if (load is MomentLoad ml)
            {
                valid = (ml.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(ml.RegionName))
                        || (ml.RegionType == RegionTypeEnum.ReferencePointName
                        && (_mesh.ReferencePoints.ContainsValidKey(ml.RegionName)));
            }
            else if (load is DLoad dl)
            {
                valid = (_mesh.Surfaces.TryGetValue(dl.SurfaceName, out s) && s.Valid);
            }
            else if (load is HydrostaticPressure hpl)
            {
                valid = (_mesh.Surfaces.TryGetValue(hpl.SurfaceName, out s) && s.Valid);
            }
            else if (load is ImportedPressure ip)
            {
                valid = (_mesh.Surfaces.TryGetValue(ip.SurfaceName, out s) && s.Valid);
            }
            else if (load is STLoad stl)
            {
                valid = (_mesh.Surfaces.TryGetValue(stl.SurfaceName, out s) && s.Valid);
            }
            else if (load is ShellEdgeLoad sel)
            {
                valid = (_mesh.Surfaces.TryGetValue(sel.SurfaceName, out s) && s.Valid);
            }
            else if (load is GravityLoad gl)
            {
                valid = (gl.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(gl.RegionName))
                        || (gl.RegionType == RegionTypeEnum.ElementSetName
                        && _mesh.ElementSets.ContainsValidKey(gl.RegionName)
                        || (gl.RegionType == RegionTypeEnum.Selection && _mesh.GetPartNamesByIds(gl.CreationIds) != null &&
                           _mesh.GetPartNamesByIds(gl.CreationIds).Length == gl.CreationIds.Length));
            }
            else if (load is CentrifLoad cf)
            {
                valid = (cf.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(cf.RegionName))
                        || (cf.RegionType == RegionTypeEnum.ElementSetName
                        && _mesh.ElementSets.ContainsValidKey(cf.RegionName)
                        || (cf.RegionType == RegionTypeEnum.Selection && _mesh.GetPartNamesByIds(cf.CreationIds) != null &&
                           _mesh.GetPartNamesByIds(cf.CreationIds).Length == cf.CreationIds.Length));
            }
            else if (load is PreTensionLoad ptl)
            {
                valid = (_mesh.Surfaces.TryGetValue(ptl.SurfaceName, out s) && s.Valid && s.Type == FeSurfaceType.Element);
            }
            // Thermal
            else if (load is CFlux cfl)
            {
                valid = (cfl.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsValidKey(cfl.RegionName));
            }
            else if (load is DFlux df)
            {
                valid = (_mesh.Surfaces.TryGetValue(df.SurfaceName, out s) && s.Valid);
            }
            else if (load is BodyFlux bf)
            {
                valid = (bf.RegionType == RegionTypeEnum.PartName && _mesh.Parts.ContainsValidKey(bf.RegionName))
                        || (bf.RegionType == RegionTypeEnum.ElementSetName
                        && _mesh.ElementSets.ContainsValidKey(bf.RegionName)
                        || (bf.RegionType == RegionTypeEnum.Selection && _mesh.GetPartNamesByIds(bf.CreationIds) != null &&
                           _mesh.GetPartNamesByIds(bf.CreationIds).Length == bf.CreationIds.Length));
            }
            else if (load is FilmHeatTransfer fht)
            {
                valid = (_mesh.Surfaces.TryGetValue(fht.SurfaceName, out s) && s.Valid);
            }
            else if (load is RadiationHeatTransfer rht)
            {
                valid = (_mesh.Surfaces.TryGetValue(rht.SurfaceName, out s) && s.Valid);
            }
            else throw new NotSupportedException();
            //
            return valid;
        }
        private void SetItemValidity(string stepName, NamedClass item, bool validity, List<Tuple<NamedClass, string>> items)
        {
            if (item.Valid != validity)
            {
                item.Valid = validity;
                items.Add(new Tuple<NamedClass, string>(item, stepName));
            }
        }
        public bool RegionValid(IMultiRegion multiRegion)
        {
            if (multiRegion.RegionType == RegionTypeEnum.NodeSetName && _mesh.NodeSets.ContainsKey(multiRegion.RegionName))
                return _mesh.NodeSets[multiRegion.RegionName].Valid;
            else if (multiRegion.RegionType == RegionTypeEnum.ElementSetName && _mesh.ElementSets.ContainsKey(multiRegion.RegionName))
                return _mesh.ElementSets[multiRegion.RegionName].Valid;
            else if (multiRegion.RegionType == RegionTypeEnum.SurfaceName && _mesh.Surfaces.ContainsKey(multiRegion.RegionName))
                return _mesh.Surfaces[multiRegion.RegionName].Valid;
            //
            return false;
        }
        //
        public int[] GetSectionAssignments(out Dictionary<int, int> elementIdSectionId)
        {
            int sectionId = 0;
            elementIdSectionId = new Dictionary<int, int>();
            //
            foreach (var entry in _sections)
            {
                if (entry.Value.RegionType == RegionTypeEnum.PartName)
                {
                    foreach (var elementId in _mesh.Parts[entry.Value.RegionName].Labels)
                    {
                        if (elementIdSectionId.ContainsKey(elementId)) elementIdSectionId[elementId] = sectionId;
                        else elementIdSectionId.Add(elementId, sectionId);
                    }
                }
                else if (entry.Value.RegionType == RegionTypeEnum.ElementSetName)
                {
                    if (entry.Value is SolidSection)
                    {
                        FeElementSet elementSet = _mesh.ElementSets[entry.Value.RegionName];
                        if (elementSet.CreatedFromParts)
                        {
                            string[] partNames = _mesh.GetPartNamesByIds(elementSet.Labels);
                            foreach (var partName in partNames)
                            {
                                foreach (var elementId in _mesh.Parts[partName].Labels)
                                {
                                    if (elementIdSectionId.ContainsKey(elementId)) elementIdSectionId[elementId] = sectionId;
                                    else elementIdSectionId.Add(elementId, sectionId);
                                }
                            }
                        }
                        else
                        {
                            foreach (var elementId in elementSet.Labels)
                            {
                                if (elementIdSectionId.ContainsKey(elementId)) elementIdSectionId[elementId] = sectionId;
                                else elementIdSectionId.Add(elementId, sectionId);
                            }
                        }
                    }
                    else if (entry.Value is ShellSection || entry.Value is MembraneSection)
                    {
                        FeElementSet elementSet = _mesh.ElementSets[entry.Value.RegionName];
                        foreach (var elementId in elementSet.Labels)
                        {
                            if (elementIdSectionId.ContainsKey(elementId)) elementIdSectionId[elementId] = sectionId;
                            else elementIdSectionId.Add(elementId, sectionId);
                        }
                    }
                    else throw new NotSupportedException();
                }
                else throw new NotSupportedException();
                //
                sectionId++;
            }
            // Not assigned
            IEnumerable<int> unAssignedElementIds = _mesh.Elements.Keys.Except(elementIdSectionId.Keys);
            int[] unAssignedElementIdsArray = new int[unAssignedElementIds.Count()];
            int count = 0;
            foreach (var elementId in unAssignedElementIds)
            {
                elementIdSectionId.Add(elementId, -1);
                unAssignedElementIdsArray[count++] = elementId;
            }
            //
            return unAssignedElementIdsArray;
        }
        public void GetMaterialAssignments(out Dictionary<int, int> elementIdMaterialId)
        {
            // Get element section ids
            Dictionary<int, int> elementIdSectionId;
            GetSectionAssignments(out elementIdSectionId);
            // Get material ids
            int count = 0;
            Dictionary<string, int> materialId = new Dictionary<string, int>();
            foreach (var entry in _materials) materialId.Add(entry.Value.Name, count++);
            // Get a map of section materials
            count = 0;
            Dictionary<int, int> sectionIdMaterialId = new Dictionary<int, int>();
            sectionIdMaterialId.Add(-1, -1);    // add missing section
            foreach (var entry in _sections) sectionIdMaterialId.Add(count++, materialId[entry.Value.MaterialName]);
            // Use the map
            elementIdMaterialId = new Dictionary<int, int>();
            foreach (var entry in elementIdSectionId) elementIdMaterialId.Add(entry.Key, sectionIdMaterialId[entry.Value]);
        }
        public void GetNodalSafetyFactorLimits()
        {
            Dictionary<int, int> elementIdMaterialId;
            GetMaterialAssignments(out elementIdMaterialId);
            //
            int count = 0;
            Dictionary<int, double> materialIdSafetyFactor = new Dictionary<int, double>();
            foreach (var entry in _materials)
            {
                //if (safetyFactor != null) materialIdSafetyFactor.Add(count++, safetyFactor.SafetyFactorLimitValue);
                //else materialIdSafetyFactor.Add(count++, double.NaN);
            }
            //
            Dictionary<int, double> nodeIdSafetyFactorLimit = new Dictionary<int, double>();
        }
        public bool AreSlipWearCoefficientsDefined(out Dictionary<int, double> materialIdCoefficient)
        {
            int count = 0;
            bool containsWear = false;
            double coefficient;
            materialIdCoefficient = new Dictionary<int, double>();
            // For each material check if the material has wear coefficients defined
            foreach (var entry in _materials)
            {
                coefficient = 0;
                foreach (var property in entry.Value.Properties)
                {
                    if (property is SlipWear sw)
                    {
                        coefficient = sw.WearCoefficient / sw.Hardness * _properties.CyclesIncrement;
                        containsWear = true;
                        break;
                    }
                }
                //
                materialIdCoefficient.Add(count++, coefficient);
            }
            return containsWear;
        }
        public Dictionary<int, double> GetNodalSlipWearCoefficients()
        {
            double coefficient;
            Dictionary<int, double> materialIdCoefficient;
            bool containsWear = AreSlipWearCoefficientsDefined(out materialIdCoefficient);
            // If wear coefficients are defined
            if (containsWear)
            {
                Dictionary<int, int> elementIdMaterialId;
                GetMaterialAssignments(out elementIdMaterialId);
                // Get wear coefficient for each element
                Dictionary<int, double> elementIdCoefficient = new Dictionary<int, double>();
                foreach (var entry in elementIdMaterialId)
                {
                    elementIdCoefficient.Add(entry.Key, materialIdCoefficient[entry.Value]);
                }
                // Get wear coefficients for each node
                HashSet<double> allCoefficients;
                Dictionary<int, HashSet<double>> nodeIdAllCoefficients = new Dictionary<int, HashSet<double>>();
                FeElement element;
                foreach (var entry in elementIdCoefficient)
                {
                    element = _mesh.Elements[entry.Key];
                    foreach (var nodeId in element.NodeIds)
                    {
                        if (nodeIdAllCoefficients.TryGetValue(nodeId, out allCoefficients)) allCoefficients.Add(entry.Value);
                        else nodeIdAllCoefficients.Add(nodeId, new HashSet<double>() { entry.Value });
                    }
                }
                // Compute the average
                Dictionary<int, double> nodeIdCoefficient = new Dictionary<int, double>();
                foreach (var entry in nodeIdAllCoefficients)
                {
                    coefficient = 0;
                    foreach (var value in entry.Value) coefficient += value;
                    coefficient /= entry.Value.Count();
                    //
                    nodeIdCoefficient.Add(entry.Key, coefficient);
                }
                //
                return nodeIdCoefficient;
            }
            else return null;
        }
        public void GetSectionThicknessAssignments(out Dictionary<int, int> elementIdSectionThicknessId)
        {
            // Get element section ids
            Dictionary<int, int> elementIdSectionId;
            GetSectionAssignments(out elementIdSectionId);
            // Get thicknesses
            int count = 0;
            double thickness;
            List<int> sectionIds;
            Dictionary<double, List<int>> thicknessSectionIds = new Dictionary<double, List<int>>();
            foreach (var entry in _sections)
            {
                if (entry.Value is ShellSection ss) thickness = ss.Thickness;
                else thickness = -1;
                //
                if (thicknessSectionIds.TryGetValue(thickness, out sectionIds)) sectionIds.Add(count);
                else thicknessSectionIds.Add(thickness, new List<int>() { count });
                count++;
            }
            double[] sortedThicknesses = thicknessSectionIds.Keys.ToArray();
            Array.Sort(sortedThicknesses);
            // Get a map of section thicknesses
            Dictionary<int, int> sectionIdSectionThicknessId = new Dictionary<int, int>();
            sectionIdSectionThicknessId.Add(-1, -1);
            if (sortedThicknesses.Length > 0 && sortedThicknesses[0] == -1) count = -1;
            else count = 0;
            foreach (var sortedThickness in sortedThicknesses)
            {
                foreach (var sectionId in thicknessSectionIds[sortedThickness])
                {
                    sectionIdSectionThicknessId.Add(sectionId, count);
                }
                count++;
            }
            // Use the map
            elementIdSectionThicknessId = new Dictionary<int, int>();
            foreach (var entry in elementIdSectionId)
                elementIdSectionThicknessId.Add(entry.Key, sectionIdSectionThicknessId[entry.Value]);
        }
        //
        public void RemoveLostUserKeywords(Action<int> SetNumberOfUserKeywords)
        {
            try
            {
                FileInOut.Output.CalculixFileWriter.RemoveLostUserKeywords(this);
                SetNumberOfUserKeywords?.Invoke(_calculixUserKeywords.Count);
            }
            catch { }
        }
        // Import
        public string[] ImportGeometryFromStlFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.StlFileReader.Read(fileName);
            //
            string[] addedPartNames = ImportGeometry(mesh, GetReservedPartNames());
            //
            return addedPartNames;
        }
        public string[] ImportGeometryFromBrepFile(string visFileName, string brepFileName)
        {
            FeMesh mesh = FileInOut.Input.VisFileReader.Read(visFileName);
            //
            if (mesh.Parts.Count > 1)
                throw new Exception("The geometry contains more than one part.");
            else if (mesh.Parts.Count <= 0)
            {
                //throw new Exception("The geometry contains less than one part."); 
            }
            else
            {
                if (mesh.Parts.GetValueByIndex(0) is GeometryPart gp)
                {
                    gp.CADFileDataFromFile(brepFileName);
                    gp.SmoothShaded = true;
                }
            }
            //
            string[] addedPartNames = ImportGeometry(mesh, GetReservedPartNames());
            //
            return addedPartNames;
        }
        public void ImportMeshFromVolFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.VolFileReader.Read(fileName, FileInOut.Input.ElementsToImport.Shell |
                                                                       FileInOut.Input.ElementsToImport.Solid);
            //
            ImportMesh(mesh, GetReservedPartNames(), true, false);
        }
        public bool ImportMeshFromMmgFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.MmgFileReader.Read(fileName, MeshRepresentation.Mesh);
            //
            bool noErrors = true;
            foreach (var entry in mesh.Parts)
            {
                if (entry.Value is GeometryPart gp && gp.HasErrors)
                {
                    noErrors = false;
                    break;
                }
            }
            //
            ImportMesh(mesh, GetReservedPartNames(), true, false);
            //
            return noErrors;
        }
        public void ImportGeneratedMeshFromMeshFile(string fileName, BasePart part, bool convertToSecondorder,
                                                    bool splitCompoundMesh)
        {
            FileInOut.Input.ElementsToImport elementsToImport;
            GeometryPart subPart;
            if (part.PartType == PartType.SolidAsShell) elementsToImport = FileInOut.Input.ElementsToImport.Solid;
            else if (part.PartType == PartType.Shell) elementsToImport = FileInOut.Input.ElementsToImport.Shell;
            else if (part.PartType == PartType.Compound)
            {
                subPart = _geometry.Parts[(part as CompoundGeometryPart).SubPartNames[0]] as GeometryPart;
                if (subPart.PartType == PartType.SolidAsShell) elementsToImport = FileInOut.Input.ElementsToImport.Solid;
                else if (subPart.PartType == PartType.Shell) elementsToImport = FileInOut.Input.ElementsToImport.Shell;
                else throw new NotSupportedException();
            }
            else throw new NotSupportedException();
            // Called after meshing in PrePoMax - the parts are sorted by id
            FeMesh mesh;
            if (Path.GetExtension(fileName) == ".vol")
                mesh = FileInOut.Input.VolFileReader.Read(fileName, elementsToImport, convertToSecondorder);
            else if (Path.GetExtension(fileName) == ".mesh")
                mesh = FileInOut.Input.MmgFileReader.Read(fileName, MeshRepresentation.Mesh, convertToSecondorder);
            else throw new NotSupportedException();
            // Split compound mesh
            if (splitCompoundMesh) mesh.SplitCompoundMesh();
            // Get part names
            string[] partNames = mesh.Parts.Keys.ToArray();
            string[] prevPartNames;
            if (part is CompoundGeometryPart cgp) prevPartNames = cgp.SubPartNames.ToArray();
            else prevPartNames = new string[] { part.Name };
            // Rename the imported part
            BasePart[] importedParts = new BasePart[partNames.Length];
            for (int i = 0; i < partNames.Length; i++)
            {
                importedParts[i] = mesh.Parts[partNames[i]];
                importedParts[i].Name = prevPartNames[i];
            }
            mesh.Parts.Clear();
            for (int i = 0; i < partNames.Length; i++) mesh.Parts.Add(importedParts[i].Name, importedParts[i]);
            //
            ImportMesh(mesh, null, false);
            // Recolor parts at the end of the import
            foreach (var importedPart in importedParts)
            {
                if (_geometry.Parts.ContainsKey(importedPart.Name) && _mesh.Parts.ContainsKey(importedPart.Name))
                    _mesh.Parts[importedPart.Name].Color = _geometry.Parts[importedPart.Name].Color;
            }
        }
        public void ImportGeneratedRemeshFromMeshFile(string fileName, int[] elementIds, BasePart part,
                                                      bool convertToSecondOrder, Dictionary<int[], FeNode> midNodes)
        {
            // Remove elements from the mesh
            HashSet<int> possiblyUnrefNodeIds = new HashSet<int>();
            foreach (var elId in elementIds) possiblyUnrefNodeIds.UnionWith(_mesh.Elements[elId].NodeIds);  // contains midside nodes
            HashSet<int> removedNodeIds =
                _mesh.RemoveElementsByIds(new HashSet<int>(elementIds), possiblyUnrefNodeIds, false, false, true);
            HashSet<int> borderNodeIds = new HashSet<int>(possiblyUnrefNodeIds.Except(removedNodeIds));     // contains midside nodes
            Dictionary<int, FeNode> borderNodes = new Dictionary<int, FeNode>();
            foreach (var ndId in borderNodeIds) borderNodes.Add(ndId, _mesh.Nodes[ndId]);                   // contains midside nodes
            HashSet<int> remainingNodeIds = new HashSet<int>(_mesh.Nodes.Keys.Except(removedNodeIds));
            // Read the mmg file and renumber nodes and elements
            double epsilon = 1E-6;
            double max = part.BoundingBox.GetDiagonal();
            Dictionary<string, Dictionary<int, int>> partIdNewSurfIdOldSurfId = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<string, Dictionary<int, int>> partIdNewEdgeIdOldEdgeId = new Dictionary<string, Dictionary<int, int>>();
            FeMesh mmgMmesh = FileInOut.Input.MmgFileReader.Read(fileName, MeshRepresentation.Mesh, convertToSecondOrder,
                                                                 _mesh.MaxNodeId + 1, _mesh.MaxElementId + 1,
                                                                 borderNodes, midNodes,
                                                                 epsilon * max,
                                                                 partIdNewSurfIdOldSurfId, partIdNewEdgeIdOldEdgeId);
            // Get surface nodes before modification
            Dictionary<int, HashSet<int>> surfaceIdNodeIds = part.Visualization.GetNodeIdsBySurfaces();
            foreach (var entry in surfaceIdNodeIds) entry.Value.ExceptWith(removedNodeIds);
            // Get edge nodes before modification
            Dictionary<int, HashSet<int>> edgeIdNodeIds = part.Visualization.GetNodeIdsByEdges();
            foreach (var entry in edgeIdNodeIds) entry.Value.ExceptWith(removedNodeIds);
            // Add elements to mesh                                                                                     
            FeElement element;
            foreach (var entry in mmgMmesh.Elements)
            {
                element = entry.Value;
                element.PartId = part.PartId;
                _mesh.Elements.Add(entry.Key, element);
            }
            // Add elements to part
            HashSet<int> newPartElementIds = new HashSet<int>(part.Labels);
            newPartElementIds.UnionWith(mmgMmesh.Elements.Keys);
            part.Labels = newPartElementIds.ToArray();
            Array.Sort(part.Labels);
            // Add nodes to mesh                                                                                        
            foreach (var entry in mmgMmesh.Nodes)
            {
                if (!_mesh.Nodes.ContainsKey(entry.Key)) _mesh.Nodes.Add(entry.Key, entry.Value);
            }
            // Add nodes to part
            HashSet<int> newPartNodeIds = new HashSet<int>(part.NodeLabels);
            newPartNodeIds.UnionWith(_mesh.Nodes.Keys);
            part.NodeLabels = newPartNodeIds.ToArray();
            Array.Sort(part.NodeLabels);
            // Update node ids
            _mesh.UpdateMaxNodeAndElementIds();
            // Model vertices                                                                                           
            HashSet<int> vertexNodeIds = new HashSet<int>();
            // Get vertices from part - only those that were not removed
            foreach (var nodeId in part.Visualization.VertexNodeIds)
            {
                if (!removedNodeIds.Contains(nodeId)) vertexNodeIds.Add(nodeId);
            }
            // Get vertices from mmgPart
            foreach (var entry in mmgMmesh.Parts) vertexNodeIds.UnionWith(entry.Value.Visualization.VertexNodeIds);
            // Model edges                                                                                              
            int elementId = mmgMmesh.MaxElementId + 1;
            LinearBeamElement beamElement;
            List<FeElement1D> edgeElements = new List<FeElement1D>();
            // Get model edges from part - only edges that are not completely removed
            int[] key;
            CompareIntArray comparer = new CompareIntArray();
            HashSet<int[]> edgeKeys = new HashSet<int[]>(comparer);
            //
            foreach (var edgeCell in part.Visualization.EdgeCells)
            {
                if (remainingNodeIds.Contains(edgeCell[0]) && remainingNodeIds.Contains(edgeCell[1]))
                {
                    key = Tools.GetSortedKey(edgeCell[0], edgeCell[1]);
                    if (!edgeKeys.Contains(key))
                    {
                        beamElement = new LinearBeamElement(elementId++, key);
                        edgeElements.Add(beamElement);
                        edgeKeys.Add(key);
                    }
                }
            }
            // Get model edges from mmgPart
            foreach (var entry in mmgMmesh.Parts)
            {
                foreach (var edgeCell in entry.Value.Visualization.EdgeCells)
                {
                    key = Tools.GetSortedKey(edgeCell[0], edgeCell[1]);
                    if (!edgeKeys.Contains(key))
                    {
                        beamElement = new LinearBeamElement(elementId++, key);
                        edgeElements.Add(beamElement);
                        edgeKeys.Add(key);
                    }
                }
            }
            // Add edge elements to mesh
            foreach (var edgeElement in edgeElements) _mesh.Elements.Add(edgeElement.Id, edgeElement);
            // Extract visualization                                                                                    
            _mesh.ExtractShellPartVisualization(part, false, -1);
            //
            _mesh.ConvertLineFeElementsToEdges(vertexNodeIds, false, part.Name);
            // Renumber surfaces                                                                                        
            BasePart mmgPart;
            Dictionary<int, HashSet<int>> itemIdNodeIds;
            foreach (var partNewSurfIdOldSurfId in partIdNewSurfIdOldSurfId)
            {
                mmgPart = mmgMmesh.Parts[partNewSurfIdOldSurfId.Key];
                itemIdNodeIds = mmgPart.Visualization.GetNodeIdsBySurfaces();
                //
                foreach (var newSurfIdOldSurfId in partNewSurfIdOldSurfId.Value)
                {
                    surfaceIdNodeIds[newSurfIdOldSurfId.Value].UnionWith(itemIdNodeIds[newSurfIdOldSurfId.Key]);
                    surfaceIdNodeIds[newSurfIdOldSurfId.Value].IntersectWith(part.NodeLabels);
                }
            }
            _mesh.RenumberVisualizationSurfaces(part, surfaceIdNodeIds);
            // Renumber edges                                                                                           
            foreach (var partNewEdgeIdOldEdgeId in partIdNewEdgeIdOldEdgeId)
            {
                mmgPart = mmgMmesh.Parts[partNewEdgeIdOldEdgeId.Key];
                itemIdNodeIds = mmgPart.Visualization.GetNodeIdsByEdges();
                //
                foreach (var newEdgeIdOldEdgeId in partNewEdgeIdOldEdgeId.Value)
                {
                    edgeIdNodeIds[newEdgeIdOldEdgeId.Value].UnionWith(itemIdNodeIds[newEdgeIdOldEdgeId.Key]);
                    edgeIdNodeIds[newEdgeIdOldEdgeId.Value].IntersectWith(part.NodeLabels);
                }
            }
            _mesh.RenumberVisualizationEdges(part, edgeIdNodeIds);
            //
            _mesh.RemoveElementsByType<FeElement1D>();
        }
        public List<string> ImportModelFromInpFile(string fileName, Action<string> WriteDataToOutput)
        {
            OrderedDictionary<int[], Calculix.CalculixUserKeyword> indexedUserKeywords;
            FileInOut.Input.InpFileReader.Read(fileName,
                                               FileInOut.Input.ElementsToImport.Solid | FileInOut.Input.ElementsToImport.Shell,
                                               this,
                                               WriteDataToOutput,
                                               out indexedUserKeywords);
            //
            CalculixUserKeywords = indexedUserKeywords;
            //
            return FileInOut.Input.InpFileReader.Errors;
        }
        public List<string> ImportMaterialsFromInpFile(string fileName, Action<string> WriteDataToOutput)
        {
            FeModel model = new FeModel("Imported Materials");
            model.Properties.ModelSpace = _properties.ModelSpace;
            model.UnitSystem = _unitSystem;
            //
            FileInOut.Input.InpFileReader.Read(fileName,
                                               FileInOut.Input.ElementsToImport.Solid | FileInOut.Input.ElementsToImport.Shell,
                                               model,
                                               WriteDataToOutput,
                                               out OrderedDictionary<int[], Calculix.CalculixUserKeyword> indexedUserKeywords);
            //
            string name;
            foreach (var entry in model.Materials)
            {
                name = entry.Key;
                if (_materials.ContainsKey(name))
                {
                    name += "_Imported";
                    if (_materials.ContainsKey(name))
                    {
                        name = _materials.GetNextNumberedKey(name);
                    }
                }
                entry.Value.Name = name;
                _materials.Add(name, entry.Value);
            }
            //
            return FileInOut.Input.InpFileReader.Errors;
        }
        public void ImportMeshFromUnvFile(string fileName)
        {
            FeMesh mesh = FileInOut.Input.UnvFileReader.Read(fileName, FileInOut.Input.ElementsToImport.Shell |
                                                                       FileInOut.Input.ElementsToImport.Solid);
            //
            ImportMesh(mesh, GetReservedPartNames(), true, false);
        }
        //
        private string[] ImportGeometry(FeMesh mesh, ICollection<string> reservedPartNames)
        {
            Check2D(mesh);
            //
            if (_geometry == null)
            {
                _geometry = new FeMesh(MeshRepresentation.Geometry);
                mesh.ResetPartsColor();
            }
            //
            string[] addedPartNames = _geometry.AddMesh(mesh, reservedPartNames);
            return addedPartNames;
        }
        public void ImportMesh(FeMesh mesh, ICollection<string> reservedPartNames, bool forceRenameParts = true,
                               bool renumberNodesAndElements = true)
        {
            Check2D(mesh);
            //
            if (_mesh == null)
            {
                _mesh = new FeMesh(MeshRepresentation.Mesh);
                mesh.ResetPartsColor();
            }
            _mesh.AddMesh(mesh, reservedPartNames, forceRenameParts, renumberNodesAndElements);
        }
        // Setters
        public void SetMesh(FeMesh mesh)
        {
            _mesh = mesh;
        }
        // Getters
        public string[] GetAllMeshEntityNames()
        {
            if (_mesh != null) return _mesh.GetAllMeshEntityNames();
            return new string[0];
        }
        public HashSet<string> GetReservedPartNames()
        {
            HashSet<string> reservedPartNames = new HashSet<string>();
            if (_geometry != null && _geometry.Parts != null) reservedPartNames.UnionWith(_geometry.Parts.Keys);
            reservedPartNames.UnionWith(GetAllMeshEntityNames());
            return reservedPartNames;
        }
        // Springs
        public PointSpring[] GetPointSpringsFromSurfaceSpring(SurfaceSpring spring)
        {
            Dictionary<int, double> nodalStiffnesses;
            double area;
            GetDistributedNodalValuesFromSurface(spring.RegionName, out nodalStiffnesses, out area);
            //
            if (spring.StiffnessPerArea) area = 1;      // account for the stiffness type
            //
            List<PointSpring> springs = new List<PointSpring>();
            foreach (var entry in nodalStiffnesses)
            {
                if (entry.Value != 0 && (spring.GetSpringDirections().Length > 0))
                {
                    springs.Add(new PointSpring(spring.Name + "_" + entry.Key.ToString(),
                                entry.Key,
                                spring.K1 / area * entry.Value,
                                spring.K2 / area * entry.Value,
                                spring.K3 / area * entry.Value,
                                spring.TwoD));
                }
            }
            //
            return springs.ToArray();
        }
        // Loads
        public CLoad[] GetNodalLoadsFromSurfaceTraction(STLoad load)
        {
            Dictionary<int, double> nodalForces;
            double area;
            GetDistributedNodalValuesFromSurface(load.SurfaceName, out nodalForces, out area);
            //
            List<CLoad> loads = new List<CLoad>();
            CLoad cLoad;
            foreach (var entry in nodalForces)
            {
                if (entry.Value != 0 && (load.F1 != 0 || load.F2 != 0 || load.F3 != 0))
                {
                    cLoad = new CLoad("_CLoad_" + entry.Key.ToString(), entry.Key,
                                      load.F1 / area * entry.Value,
                                      load.F2 / area * entry.Value,
                                      load.F3 / area * entry.Value,
                                      load.TwoD, load.Complex,
                                      load.PhaseDeg);
                    cLoad.AmplitudeName = load.AmplitudeName;
                    loads.Add(cLoad);
                }
            }
            //
            return loads.ToArray();
        }
        public void GetDistributedNodalValuesFromSurface(string surfaceName,
                                                         out Dictionary<int, double> nodalValues,
                                                         out double aSum)
        {
            nodalValues = new Dictionary<int, double>();
            aSum = 0;
            //
            int nodeId;
            int sectionId = 0;
            int[] nodeIds;
            double A;
            double thickness;
            double[] equValue;
            FeElement element;
            Dictionary<int, int> elementIdSectionId;
            Dictionary<int, double> sectionIdThickness = new Dictionary<int, double>();
            // Get element thicknesses
            GetSectionAssignments(out elementIdSectionId);
            foreach (var entry in _sections)
            {
                if (entry.Value is SolidSection solid) thickness = solid.Thickness;
                else if (entry.Value is ShellSection shell) thickness = shell.Thickness;
                else throw new NotSupportedException();
                //
                sectionIdThickness.Add(sectionId++, thickness);
            }
            //
            FeSurface surface = _mesh.Surfaces[surfaceName];
            if (surface.ElementFaces == null) return;
            //
            foreach (var entry in surface.ElementFaces)
            {
                foreach (var elementId in _mesh.ElementSets[entry.Value].Labels)
                {
                    element = _mesh.Elements[elementId];
                    A = element.GetArea(entry.Key, _mesh.Nodes);
                    if (element is FeElement2D element2D)
                    {
                        sectionId = elementIdSectionId[elementId];
                        if (sectionId == -1) throw new CaeException("Missing section assignment at element " + elementId +
                                                                    " from part " + _mesh.GetPartById(element.PartId) + ".");
                        thickness = sectionIdThickness[sectionId];
                        A *= thickness;
                    }
                    aSum += A;
                    nodeIds = element.GetNodeIdsFromFaceName(entry.Key);
                    equValue = element.GetEquivalentForcesFromFaceName(entry.Key);
                    //
                    for (int i = 0; i < nodeIds.Length; i++)
                    {
                        nodeId = nodeIds[i];
                        if (nodalValues.ContainsKey(nodeId)) nodalValues[nodeId] += A * equValue[i];
                        else nodalValues.Add(nodeId, A * equValue[i]);
                    }
                }
            }
        }
        public CLoad[] GetNodalCLoadsFromVariablePressureLoad(VariablePressure load)
        {
            Dictionary<int, int> elementIdSectionId;
            double[] sectionIdThickness = new double[_sections.Count];
            // Get element thicknesses
            GetSectionAssignments(out elementIdSectionId);
            //
            int sectionId = 0;
            double thickness;
            string surfaceName = load.SurfaceName;
            foreach (var entry in _sections)
            {
                if (entry.Value is SolidSection solid) thickness = solid.Thickness;
                else if (entry.Value is ShellSection shell) thickness = shell.Thickness;
                else throw new NotSupportedException();
                //
                sectionIdThickness[sectionId++] = thickness;
            }
            // Surface
            FeSurface surface = _mesh.Surfaces[surfaceName];
            if (surface.ElementFaces == null) return null;
            //
            int nodeId = -1;
            int[] nodeIds;
            double A;
            double sign;
            double pressure;
            double[] force;
            double[] nodalForce;
            double[] faceNormal;
            double[] nodalPressures;
            double[] nodalForceMagnitudes;
            FeElement element;
            Dictionary<int, double> nodeIdPressure = new Dictionary<int, double>();
            Dictionary<int, double[]> nodeIdForce = new Dictionary<int, double[]>();
            //
            int count;
            FeFaceName faceName;
            FeElement expandedElement;
            int[] expandedNodeIds;
            Dictionary<int, FeNode> expandedNodes;
            //
            foreach (var entry in surface.ElementFaces)
            {
                foreach (var elementId in _mesh.ElementSets[entry.Value].Labels)
                {
                    element = _mesh.Elements[elementId];
                    // Node ids
                    nodeIds = element.GetNodeIdsFromFaceName(entry.Key);
                    //
                    _mesh.GetElementFaceCenterAndNormal(elementId, entry.Key, out double[] faceCenter, out faceNormal,
                                                        out bool shellElement);
                    //
                    if (Properties.ModelSpace == ModelSpaceEnum.Axisymmetric)
                    {
                        GetExpandedAxisymmetricElementFromNodeIds(nodeIds, out expandedElement, out expandedNodes);
                        expandedNodeIds = expandedElement.NodeIds;
                        faceName = FeFaceName.S1;
                        //
                        A = expandedElement.GetArea(faceName, expandedNodes) * 180 / 0.01;
                        // Pressure
                        nodalPressures = new double[expandedNodeIds.Length];
                        for (int i = 0; i < nodalPressures.Length; i++)
                        {
                            nodeId = expandedNodeIds[i];
                            pressure = load.GetPressureForPoint(expandedNodes[nodeId].Coor);
                            nodalPressures[i] = pressure;
                        }
                        // Force magnitudes without area
                        nodalForceMagnitudes = expandedElement.GetEquivalentForcesFromFaceName(faceName, nodalPressures);
                        // Invert face normal in case of S1 or S2 shell face
                        sign = 1;
                        // Force vectors
                        for (int i = 0; i < expandedNodeIds.Length; i++)
                        {
                            force = new double[] {
                                sign * A * nodalForceMagnitudes[i] * faceNormal[0],
                                sign * A * nodalForceMagnitudes[i] * faceNormal[1],
                                sign * A * nodalForceMagnitudes[i] * faceNormal[2]};
                            //
                            if (expandedElement.Id == 31)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[1];
                            }
                            else if (expandedElement.Id == 32)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[0];
                            }
                            else if (expandedElement.Id == 41)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 4) nodeId = nodeIds[0];
                            }
                            else if (expandedElement.Id == 61)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 4) nodeId = nodeIds[2];
                                else if (expandedNodeIds[i] == 5) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 6) nodeId = nodeIds[2];
                            }
                            else if (expandedElement.Id == 62)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 4) nodeId = nodeIds[2];
                                else if (expandedNodeIds[i] == 5) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 6) nodeId = nodeIds[2];
                            }
                            else if (expandedElement.Id == 81)
                            {
                                if (expandedNodeIds[i] == 1) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 2) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 3) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 4) nodeId = nodeIds[0];
                                else if (expandedNodeIds[i] == 5) nodeId = nodeIds[2];
                                else if (expandedNodeIds[i] == 6) nodeId = nodeIds[1];
                                else if (expandedNodeIds[i] == 7) nodeId = nodeIds[2];
                                else if (expandedNodeIds[i] == 8) nodeId = nodeIds[0];
                            }
                            //
                            if (!nodeIdForce.TryGetValue(nodeId, out nodalForce))
                            {
                                nodalForce = new double[3];
                                nodeIdForce.Add(nodeId, nodalForce);
                            }
                            nodalForce[0] += force[0];
                            nodalForce[1] += force[1];
                            nodalForce[2] += force[2];
                        }
                    }
                    else
                    {
                        A = element.GetArea(entry.Key, _mesh.Nodes);
                        // Account for 2D area when an edge is selected
                        if (element is FeElement2D element2D && entry.Key != FeFaceName.S1 && entry.Key != FeFaceName.S2)
                        {
                            sectionId = elementIdSectionId[elementId];
                            if (sectionId == -1) throw new CaeException("Missing section assignment at element " + elementId +
                                                                        " from part " + _mesh.GetPartById(element.PartId) + ".");
                            thickness = sectionIdThickness[sectionId];
                            A *= thickness;
                        }
                        // Pressure
                        nodalPressures = new double[nodeIds.Length];
                        for (int i = 0; i < nodalPressures.Length; i++)
                        {
                            nodeId = nodeIds[i];
                            if (!nodeIdPressure.TryGetValue(nodeId, out pressure))
                            {
                                pressure = load.GetPressureForPoint(_mesh.Nodes[nodeId].Coor);
                                nodeIdPressure.Add(nodeId, pressure);
                            }
                            nodalPressures[i] = pressure;
                        }
                        // Force magnitudes without area
                        nodalForceMagnitudes = element.GetEquivalentForcesFromFaceName(entry.Key, nodalPressures);
                        // Invert face normal in case of S1 or S2 shell face
                        if (shellElement && (entry.Key == FeFaceName.S1 || entry.Key == FeFaceName.S2)) sign = -1;
                        else sign = 1;
                        // Force vectors
                        for (int i = 0; i < nodeIds.Length; i++)
                        {
                            force = new double[] {
                                sign * A * nodalForceMagnitudes[i] * faceNormal[0],
                                sign * A * nodalForceMagnitudes[i] * faceNormal[1],
                                sign * A * nodalForceMagnitudes[i] * faceNormal[2]};
                            //
                            if (!nodeIdForce.TryGetValue(nodeIds[i], out nodalForce))
                            {
                                nodalForce = new double[3];
                                nodeIdForce.Add(nodeIds[i], nodalForce);
                            }
                            nodalForce[0] += force[0];
                            nodalForce[1] += force[1];
                            nodalForce[2] += force[2];
                        }
                    }
                }
            }
            // Concentrated loads
            CLoad cLoad;
            List<CLoad> loads = new List<CLoad>();
            foreach (var entry in nodeIdForce)
            {
                if (entry.Value[0] != 0 || entry.Value[1] != 0 || entry.Value[2] != 0)
                {
                    cLoad = new CLoad("_CLoad_" + entry.Key.ToString(), entry.Key,
                                      entry.Value[0],
                                      entry.Value[1],
                                      entry.Value[2],
                                      load.TwoD, load.Complex,
                                      load.PhaseDeg);
                    cLoad.AmplitudeName = load.AmplitudeName;
                    loads.Add(cLoad);
                }
            }
            //
            return loads.ToArray();
        }
        public void GetExpandedAxisymmetricElementFromNodeIds(int[] nodeIds, out FeElement expandedElement,
                                                             out Dictionary<int, FeNode> expandedNodes)
        {
            expandedElement = null;
            expandedNodes = new Dictionary<int, FeNode>();
            // Node coor
            double[][] coor = new double[nodeIds.Length][];
            for (int i = 0; i < nodeIds.Length; i++) coor[i] = _mesh.Nodes[nodeIds[i]].Coor;
            //
            double cos = Math.Cos(Math.PI / 180 * 0.01);
            double sin = Math.Sin(Math.PI / 180 * 0.01);
            //
            if (nodeIds.Length == 2)
            {
                // Triangle
                if (coor[0][0] == 0)
                {
                    expandedNodes.Add(1, new FeNode(1, coor[0]));
                    expandedNodes.Add(2, new FeNode(2, coor[1][0] * cos, coor[1][1], coor[1][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[1][0] * cos, coor[1][1], -coor[1][0] * sin));
                    expandedElement = new LinearTriangleElement(31, new int[] { 1, 2, 3 });
                }
                // Triangle
                else if (coor[1][0] == 0)
                {
                    expandedNodes.Add(1, new FeNode(1, coor[1]));
                    expandedNodes.Add(2, new FeNode(2, coor[0][0] * cos, coor[0][1], coor[0][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[0][0] * cos, coor[0][1], -coor[0][0] * sin));
                    expandedElement = new LinearTriangleElement(32, new int[] { 1, 2, 3 });
                }
                // Quad
                else
                {
                    expandedNodes.Add(1, new FeNode(1, coor[0][0] * cos, coor[0][1], coor[0][0] * sin));
                    expandedNodes.Add(2, new FeNode(2, coor[1][0] * cos, coor[1][1], coor[1][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[1][0] * cos, coor[1][1], -coor[1][0] * sin));
                    expandedNodes.Add(4, new FeNode(4, coor[0][0] * cos, coor[0][1], -coor[0][0] * sin));
                    expandedElement = new LinearQuadrilateralElement(41, new int[] { 1, 2, 3, 4 });
                }
            }
            else if (nodeIds.Length == 3)
            {
                // Triangle
                if (coor[0][0] == 0)
                {
                    expandedNodes.Add(1, new FeNode(1, coor[0]));
                    expandedNodes.Add(2, new FeNode(2, coor[1][0] * cos, coor[1][1], coor[1][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[1][0] * cos, coor[1][1], -coor[1][0] * sin));
                    //
                    expandedNodes.Add(4, new FeNode(4, coor[2][0] * cos, coor[2][1], coor[2][0] * sin));
                    expandedNodes.Add(5, new FeNode(5, coor[1]));
                    expandedNodes.Add(6, new FeNode(6, coor[2][0] * cos, coor[2][1], -coor[2][0] * sin));
                    //
                    expandedElement = new ParabolicTriangleElement(61, new int[] { 1, 2, 3, 4, 5, 6 });
                }
                // Triangle
                else if (coor[1][0] == 0)
                {
                    expandedNodes.Add(1, new FeNode(1, coor[1]));
                    expandedNodes.Add(2, new FeNode(2, coor[0][0] * cos, coor[0][1], coor[0][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[0][0] * cos, coor[0][1], -coor[0][0] * sin));
                    //
                    expandedNodes.Add(4, new FeNode(4, coor[2][0] * cos, coor[2][1], coor[2][0] * sin));
                    expandedNodes.Add(5, new FeNode(5, coor[0]));
                    expandedNodes.Add(6, new FeNode(6, coor[2][0] * cos, coor[2][1], -coor[2][0] * sin));
                    //
                    expandedElement = new ParabolicTriangleElement(62, new int[] { 1, 2, 3, 4, 5, 6 });
                }
                // Quad
                else
                {
                    expandedNodes.Add(1, new FeNode(1, coor[0][0] * cos, coor[0][1], coor[0][0] * sin));
                    expandedNodes.Add(2, new FeNode(2, coor[1][0] * cos, coor[1][1], coor[1][0] * sin));
                    expandedNodes.Add(3, new FeNode(3, coor[1][0] * cos, coor[1][1], -coor[1][0] * sin));
                    expandedNodes.Add(4, new FeNode(4, coor[0][0] * cos, coor[0][1], -coor[0][0] * sin));
                    //
                    expandedNodes.Add(5, new FeNode(5, coor[2][0] * cos, coor[2][1], coor[2][0] * sin));
                    expandedNodes.Add(6, new FeNode(6, coor[1]));
                    expandedNodes.Add(7, new FeNode(7, coor[2][0] * cos, coor[2][1], -coor[2][0] * sin));
                    expandedNodes.Add(8, new FeNode(8, coor[0]));
                    //
                    expandedElement = new ParabolicQuadrilateralElement(81, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                }
            }
            else throw new NotSupportedException();
        }
        public DLoad[] GetNodalDLoadsFromVariablePressureLoad_(VariablePressure load)
        {
            // Surface
            FeSurface surface = _mesh.Surfaces[load.SurfaceName];
            if (surface.ElementFaces == null) return null;
            //
            double sign;
            double pressure;
            double[] faceNormal;
            FeElement element;
            DLoad dLoad;
            List<DLoad> loads = new List<DLoad>();
            //
            foreach (var entry in surface.ElementFaces)
            {
                foreach (var elementId in _mesh.ElementSets[entry.Value].Labels)
                {
                    element = _mesh.Elements[elementId];
                    //
                    _mesh.GetElementFaceCenterAndNormal(elementId, entry.Key, out double[] faceCenter, out faceNormal,
                                                        out bool shellElement);
                    // Pressure
                    pressure = load.GetPressureForPoint(faceCenter);
                    // Pressure loads
                    if (pressure != 0)
                    {
                        dLoad = new DLoad(entry.Key.ToString(), elementId.ToString(), RegionTypeEnum.ElementId,
                                          pressure, load.TwoD, load.Complex, load.PhaseDeg);
                        dLoad.AmplitudeName = load.AmplitudeName;
                        loads.Add(dLoad);
                    }
                }
            }
            //
            return loads.ToArray();
        }
        public DLoad[] GetNodalDLoadsFromVariablePressureLoad(VariablePressure load)
        {
            // Surface
            FeSurface surface = _mesh.Surfaces[load.SurfaceName];
            if (surface.ElementFaces == null) return null;
            //
            int elementCount = 0;
            foreach (var entry in surface.ElementFaces) elementCount += _mesh.ElementSets[entry.Value].Labels.Length;
            DLoad[] loads = new DLoad[elementCount];
            //
            elementCount = 0;
            foreach (var entry in surface.ElementFaces) // this are faces S1, S2, ...
            {
                // Parallel
                Parallel.For(0, _mesh.ElementSets[entry.Value].Labels.Length, i =>
                //for (int i = 0; i < _mesh.ElementSets[entry.Value].Labels.Length; i++)
                {
                    int elementId = _mesh.ElementSets[entry.Value].Labels[i];
                    _mesh.GetElementFaceCenterAndNormal(elementId, entry.Key, out double[] faceCenter, out double[] faceNormal,
                                                        out bool shellElement);
                    // Pressure
                    double pressure = load.GetPressureForPoint(faceCenter);
                    // Pressure loads
                    if (pressure != 0)
                    {
                        DLoad dLoad = new DLoad(entry.Key.ToString(), elementId.ToString(), RegionTypeEnum.ElementId,
                                          pressure, load.TwoD, load.Complex, load.PhaseDeg);
                        dLoad.AmplitudeName = load.AmplitudeName;
                        loads[elementCount + i] = dLoad;
                    }
                }
                );
                elementCount += _mesh.ElementSets[entry.Value].Labels.Length;
            }
            //
            return loads.ToArray();
        }
        // 3D - 2D
        public void UpdateMeshPartsElementTypes()
        {
            Dictionary<Type, HashSet<Enum>> elementTypeEnums = _properties.ModelSpace.GetAvailableElementTypes();
            if (_mesh != null) _mesh.UpdatePartsElementTypes(elementTypeEnums);
        }
        // Boundary displacement method
        public FeModel PrepareBdmModel(Dictionary<int, double[]> deformations)
        {
            // Mesh
            FeModel bdmModel = new FeModel("BDMmodel");
            bdmModel.Properties = _properties;
            bdmModel.SetMesh(_mesh.DeepCopy());
            // Materials
            Material materialElastic = new Material("Elastic");
            materialElastic.AddProperty(new Elastic(new double[][] { new double[] { 1000, 0, 0 } }));
            bdmModel._materials.Add(materialElastic.Name, materialElastic);
            // Sections
            Section section;
            foreach (var entry in _sections)
            {
                section = entry.Value.DeepClone();
                section.MaterialName = materialElastic.Name;
                bdmModel.Sections.Add(section.Name, section);
            }
            // Constraints
            Constraint constraint;
            foreach (var entry in _constraints)
            {
                constraint = entry.Value.DeepClone();
                bdmModel.Constraints.Add(constraint.Name, constraint);
            }
            // Amplitudes
            Amplitude amplitude;
            foreach (var entry in _amplitudes)
            {
                amplitude = entry.Value.DeepClone();
                bdmModel.Amplitudes.Add(amplitude.Name, amplitude);
            }
            // Steps
            StaticStep staticStep = new StaticStep("Step-1");
            bdmModel._stepCollection.AddStep(staticStep, false);
            BoundaryDisplacementStep boundaryDisplacementStep = _stepCollection.GetBoundaryDisplacementStep();
            if (boundaryDisplacementStep == null)
                throw new CaeException("The boundary displacement step is missing.");
            // Add existing boundary conditions
            bool twoD = false;
            BoundaryCondition bc;
            foreach (var entry in boundaryDisplacementStep.BoundaryConditions)
            {
                bc = entry.Value.DeepClone();
                staticStep.AddBoundaryCondition(bc);
                twoD = bc.TwoD;
            }
            // Create BDM boundary conditions
            string name;
            double[] xyz;
            FeNodeSet nodeSet;
            DisplacementRotation displacementRotation;
            //
            if (deformations != null)
            {
                foreach (var entry in deformations)
                {
                    // Node set
                    name = bdmModel.Mesh.NodeSets.GetNextNumberedKey("BDM", "_" + entry.Key);
                    nodeSet = new FeNodeSet(name, new int[] { entry.Key });
                    bdmModel.Mesh.NodeSets.Add(nodeSet.Name, nodeSet);
                    // Boundary condition
                    xyz = entry.Value;
                    name = staticStep.BoundaryConditions.GetNextNumberedKey("BDM-" + entry.Key);
                    displacementRotation = new DisplacementRotation(name, nodeSet.Name, RegionTypeEnum.NodeSetName, twoD,
                                                                    false, 0);
                    if (xyz[0] != 0) displacementRotation.U1 = xyz[0];
                    if (xyz[1] != 0) displacementRotation.U2 = xyz[1];
                    if (xyz[2] != 0) displacementRotation.U3 = xyz[2];
                    staticStep.AddBoundaryCondition(displacementRotation);
                }
            }
            //
            return bdmModel;
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_name", Name, typeof(string));
            info.AddValue("_geometry", _geometry, typeof(FeMesh));
            info.AddValue("_mesh", _mesh, typeof(FeMesh));
            info.AddValue("_materials", _materials, typeof(OrderedDictionary<string, Material>));
            info.AddValue("_sections", _sections, typeof(OrderedDictionary<string, Section>));
            info.AddValue("_constraints", _constraints, typeof(OrderedDictionary<string, Constraint>));
            info.AddValue("_surfaceInteractions", _surfaceInteractions, typeof(OrderedDictionary<string, SurfaceInteraction>));
            info.AddValue("_contactPairs", _contactPairs, typeof(OrderedDictionary<string, ContactPair>));
            info.AddValue("_amplitudes", _amplitudes, typeof(OrderedDictionary<string, Amplitude>));
            info.AddValue("_initialConditions", _initialConditions, typeof(OrderedDictionary<string, InitialCondition>));
            info.AddValue("_stepCollection", _stepCollection, typeof(StepCollection));
            info.AddValue("_calculixUserKeywords", _calculixUserKeywords, typeof(OrderedDictionary<int[], Calculix.CalculixUserKeyword>));
            info.AddValue("_properties", _properties, typeof(ModelProperties));
            info.AddValue("_unitSystem", _unitSystem, typeof(UnitSystem));
            info.AddValue("_hashName", _hashName, typeof(string));
        }


        public FeSurface TestCreateSurface(int partId = 1, int surfaceId = 1, string name = "UserSurface")
        {
            int surfaceType = (int)GeometryType.SolidSurface;
            int geometryId = surfaceId * 100000 + surfaceType * 10000 + partId;
            int[] faceIds = this.Mesh.GetIdsFromGeometryIds(new int[] { geometryId }, vtkSelectItem.Surface);
            //
            FeSurface surface = new FeSurface(this.Mesh.Surfaces.GetNextNumberedKey(name));
            surface.CreatedFrom = FeSurfaceCreatedFrom.Faces;
            surface.FaceIds = faceIds;

            this.Mesh.CreateSurfaceItems(surface);
            this.Mesh.Surfaces.Add(surface.Name, surface);

            return surface;
        }
    }
}
