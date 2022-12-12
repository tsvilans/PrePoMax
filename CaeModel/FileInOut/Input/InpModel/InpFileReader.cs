using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using CaeModel;
using FileInOut.Output.Calculix;

namespace FileInOut.Input
{
    [Serializable]
    static public class InpFileReader
    {
        // Variables                                                                                                                
        private static List<string> _errors;
        private static string[] _splitterComma = new string[] { "," };
        private static string[] _splitterEqual = new string[] { "=" };
        private static string[] _splitter = new string[] { " ", ",", "\t" };
        private static HashSet<string> _knownKeywords = new HashSet<string> { "*HEADING",
                                                                              "*INCLUDE",
                                                                              "*NODE",
                                                                              "*ELEMENT",
                                                                              "*NSET",
                                                                              "*ELSET",
                                                                              "*SURFACE",
                                                                              "*RIGID BODY",
                                                                              "*MATERIAL",
                                                                              "*DENSITY",
                                                                              "*ELASTIC",
                                                                              "*PLASTIC",
                                                                              "*SOLID SECTION",
                                                                              "*SHELL SECTION",
                                                                              "*STEP",
                                                                              "*STATIC",
                                                                              "*FREQUENCY",
                                                                              "*BUCKLE",
                                                                              "*HEAT TRANSFER",
                                                                              "*END STEP",
                                                                              "*BOUNDARY",
                                                                              "*CLOAD",
                                                                              "*DLOAD",
                                                                              "*NODE FILE",
                                                                              "*EL FILE",
                                                                              "*CONTACT FILE",
                                                                              "*NODE PRINT",
                                                                              "*EL PRINT",
                                                                              "*CONTACT PRINT"
        };
        private static HashSet<string> _materialKeywords = new HashSet<string> { "*CONDUCTIVITY",
                                                                                 "*CREEP",
                                                                                 "*CYCLIC HARDENING",
                                                                                 "*DAMPING",
                                                                                 "*DEFORMATION PLASTICITY",
                                                                                 "*DENSITY",
                                                                                 "*DEPVAR",
                                                                                 "*ELASTIC",
                                                                                 "*ELECTRICAL CONDUCTIVITY",
                                                                                 "*EXPANSION",
                                                                                 "*FLUID CONSTANTS",
                                                                                 "*HYPER ELASTIC",
                                                                                 "*HYPERFOAM",
                                                                                 "*MAGNETIC PERMEABILITY",
                                                                                 "*PLASTIC",
                                                                                 "*SLIP WEAR",
                                                                                 "*SPECIFIC GAS CONSTANT",
                                                                                 "*SPECIFIC HEAT",
                                                                                 "*USER MATERIAL"
        };
        private static HashSet<string> _surfaceInteractionKeywords = new HashSet<string> { "*FRICTION",
                                                                                           "*GAP CONDUCTANCE",
                                                                                           "*GAP HEAT GENERATION",
                                                                                           "*SURFACE BEHAVIOR"
        };

        // Callbacks                                                                                                                
        private static Action<string> WriteDataToOutputStatic;


        // Properties                                                                                                               
        static public List<string> Errors { get { return _errors; } }


        // Methods                                                                                                                  
        static public void Read(string fileName, ElementsToImport elementsToImport, FeModel model,
                                Action<string> WriteDataToOutput,
                                out OrderedDictionary<int[], CalculixUserKeyword> indexedUserKeywords)
        {
            WriteDataToOutputStatic = WriteDataToOutput;
            _errors = new List<string>();
            indexedUserKeywords = new OrderedDictionary<int[], CalculixUserKeyword>("User calculix keywords");
            //
            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = Tools.ReadAllLines(fileName);
                lines = ReadIncludes(lines, 0, Path.GetDirectoryName(fileName));
                //
                string[] dataSet;
                string[][] dataSets = GetDataSets(lines);
                //
                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                //
                int[] ids;
                string name;
                string keyword;
                List<InpElementSet> inpElementTypeSets = new List<InpElementSet>();
                // Nodes and elements
                for (int i = 0; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i];
                    keyword = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                    //
                    if (keyword == "*NODE") // nodes
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        nodes.AddRange(GetNodes(dataSet));
                    }
                    else if (keyword == "*ELEMENT") // elements
                    {
                        WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                        AddElements(dataSet, ref elements, ref inpElementTypeSets);
                    }
                }
                // Element sets are used to separate elements into parts
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh, inpElementTypeSets);
                // Element sets from element keywords
                int[] labels;
                foreach (var inpElementTypeSet in inpElementTypeSets)
                {
                    if (inpElementTypeSet.Name != null) // name is null when no elset is defined by *Element keyword
                    {
                        labels = inpElementTypeSet.ElementLabels.ToArray();
                        mesh.AddElementSet(new FeElementSet(inpElementTypeSet.Name, labels));
                        //mesh.AddNodeSetFromElementSet(inpElementTypeSet.Name);
                    }
                }
                //

                OrderedDictionary<string, FeReferencePoint> referencePoints = new OrderedDictionary<string, FeReferencePoint>("Reference points");
                OrderedDictionary<string, Material> materials = new OrderedDictionary<string, Material>("Materials");
                OrderedDictionary<string, Section> sections = new OrderedDictionary<string, Section>("Sections");
                OrderedDictionary<string, Constraint> constraints = new OrderedDictionary<string, Constraint>("Constraints");
                OrderedDictionary<string, SurfaceInteraction> surfaceInteractions = 
                    new OrderedDictionary<string, SurfaceInteraction>("Surface interactions");
                OrderedDictionary<string, ContactPair> contactPairs = new OrderedDictionary<string, ContactPair>("Contact pairs");
                OrderedDictionary<string, Amplitude> amplitudes = new OrderedDictionary<string, Amplitude>("Amplitudes");
                OrderedDictionary<string, Step> steps = new OrderedDictionary<string, Step>("Steps");
                List<CalculixUserKeyword> userKeywords = new List<CalculixUserKeyword>();
                //
                for (int i = 0; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i];
                    keyword = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                    //
                    WriteDataToOutputStatic("Reading keyword line: " + dataSet[0]);
                    //
                    if (keyword == "*PART")
                    {
                        // Files created in Gmesh containing only one part can have a name
                        string newName = GetPartName(dataSet);
                        if (newName != null && mesh.Parts.Count == 1)
                        {
                            string oldName = mesh.Parts.Keys.First();
                            BasePart part = mesh.Parts[oldName];
                            part.Name = newName;
                            mesh.Parts.Replace(oldName, newName, part);
                        }
                    }
                    else if (keyword == "*NSET")
                    {
                        GetNodeOrElementSet("NSET", dataSet, mesh, out name, out ids);
                        if (NamedClass.CheckNameError(name) != null) AddError(NamedClass.CheckNameError(name));
                        else if (ids != null) mesh.AddNodeSet(new FeNodeSet(name, ids));
                    }
                    else if (keyword == "*ELSET")
                    {
                        GetNodeOrElementSet("ELSET", dataSet, mesh, out name, out ids);
                        if (NamedClass.CheckNameError(name) != null) AddError(NamedClass.CheckNameError(name));
                        else if (ids != null) mesh.AddElementSet(new FeElementSet(name, ids));
                    }
                    else if (keyword == "*SURFACE")
                    {
                        FeSurface surface = GetSurface(dataSet);
                        if (surface != null) mesh.AddSurface(surface);
                    }
                    else if (keyword == "*RIGID BODY")
                    {
                        Constraint constraint = GetRigidBody(dataSet, nodes, constraints, referencePoints);
                        if (constraint != null) constraints.Add(constraint.Name, constraint);
                    }
                    else if (keyword == "*MATERIAL")
                    {
                        Material material = GetMaterial(dataSets, ref i, userKeywords);
                        if (material != null) materials.Add(material.Name, material);
                    }
                    else if (keyword == "*SOLID SECTION")
                    {
                        SolidSection section = GetSolidSection(dataSet, sections);
                        if (section != null) sections.Add(section.Name, section);
                    }
                    else if (keyword == "*SHELL SECTION")
                    {
                        ShellSection section = GetShellSection(dataSet, sections);
                        if (section != null) sections.Add(section.Name, section);
                    }
                    else if (keyword == "*AMPLITUDE")
                    {
                        Amplitude amplitude = GetAmplitudeTabular(dataSet);
                        if (amplitude != null) amplitudes.Add(amplitude.Name, amplitude);
                    }
                    else if (keyword == "*SURFACE INTERACTION")
                    {
                        SurfaceInteraction surfaceInteraction = GetSurfaceInteraction(dataSets, ref i, userKeywords);
                        if (surfaceInteraction != null) surfaceInteractions.Add(surfaceInteraction.Name, surfaceInteraction);
                    }
                    else if (keyword == "*STEP")
                    {
                        Step step = GetStep(dataSets, ref i, mesh, steps, contactPairs, userKeywords);
                        if (step != null) steps.Add(step.Name, step);
                    }
                    else if (!_knownKeywords.Contains(keyword))
                    {
                        // User keyword
                        CalculixUserKeyword userKeyword = new CalculixUserKeyword(dataSet.ToRows(dataSet.Length));
                        userKeyword.Parent = "Model";
                        userKeywords.Add(userKeyword);
                    }
                }
                //
                if (elementsToImport != ElementsToImport.All)
                {
                    if (!elementsToImport.HasFlag(ElementsToImport.Beam)) mesh.RemoveElementsByType<FeElement1D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Shell)) mesh.RemoveElementsByType<FeElement2D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Solid)) mesh.RemoveElementsByType<FeElement3D>();
                }
                //
                model.ImportMesh(mesh, null, false, false);
                // Add model items
                foreach (var entry in referencePoints) mesh.ReferencePoints.Add(entry.Key, entry.Value);
                foreach (var entry in materials) model.Materials.Add(entry.Key, entry.Value);
                foreach (var entry in sections) model.Sections.Add(entry.Key, entry.Value);
                foreach (var entry in constraints) model.Constraints.Add(entry.Key, entry.Value);
                foreach (var entry in surfaceInteractions) model.SurfaceInteractions.Add(entry.Key, entry.Value);
                foreach (var entry in contactPairs) model.ContactPairs.Add(entry.Key, entry.Value);
                foreach (var entry in amplitudes) model.Amplitudes.Add(entry.Key, entry.Value);
                foreach (var entry in steps) model.StepCollection.AddStep(entry.Value, false);
                // Add indices of user keywords
                int[] indices;
                Stack<int> indexStack = new Stack<int>();
                List<CalculixKeyword> keywords = Output.CalculixFileWriter.GetModelKeywords(model);
                indexedUserKeywords = new OrderedDictionary<int[], CalculixUserKeyword>("User Calculix keywords");
                foreach (CalculixUserKeyword userKeyword in userKeywords)
                {
                    indexStack.Clear();
                    indices = GetKeywordIndices(userKeyword, keywords, indexStack);
                    if (indices != null)
                    {
                        Output.CalculixFileWriter.AddUserKeywordByIndices(keywords, indices, userKeyword);
                        indexedUserKeywords.Add(indices, userKeyword);
                    }
                }
            }
        }
        static public void ReadCel(string fileName,
                                   out Dictionary<int, FeElement> uniqueElements,
                                   out Dictionary<string, FeElementSet> elementSets)
        {
            _errors = new List<string>();
            uniqueElements = null;
            elementSets = null;
            //
            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = Tools.ReadAllLines(fileName);
                //lines = ReadIncludes(lines, 0, Path.GetDirectoryName(fileName));
                //
                string[] dataSet;
                string[][] dataSets = GetDataSets(lines);
                //
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                //
                string keyword;
                List<InpElementSet> inpElementTypeSets = new List<InpElementSet>();
                // Elements
                for (int i = 0; i < dataSets.Length; i++)
                {
                    dataSet = dataSets[i];
                    keyword = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                    //
                    if (keyword == "*ELEMENT") // elements
                    {
                        //if (elements.Count >= 1) continue;
                        AddElements(dataSet, ref elements, ref inpElementTypeSets);
                    }
                }
                // Remove duplicate elements
                RemoveDuplicateElements(elements, inpElementTypeSets, out uniqueElements, out elementSets);
            }
        }
        private static void RemoveDuplicateElements(Dictionary<int, FeElement> elements,
                                                    List<InpElementSet> inpElementTypeSets,
                                                    out Dictionary<int, FeElement> uniqueElements,
                                                    out Dictionary<string, FeElementSet> elementSets)
        {
            uniqueElements = new Dictionary<int, FeElement>();
            elementSets = new Dictionary<string, FeElementSet>();
            //
            CompareIntArray comparer = new CompareIntArray();
            HashSet<int[]> elementNodeLabels = new HashSet<int[]>(comparer);
            FeElement element;
            List<int> labels = new List<int>();
            FeElementSet elementSet;
            int newId = int.MaxValue;
            //
            foreach (var entry in elements)
            {
                if (entry.Value.Id < newId) newId = entry.Value.Id;
            }
            //
            foreach (var inpElementSet in inpElementTypeSets)
            {
                labels.Clear();
                elementNodeLabels.Clear();
                //
                foreach (var elementId in inpElementSet.ElementLabels)
                {
                    element = elements[elementId];
                    if (!elementNodeLabels.Contains(element.NodeIds))
                    {
                        element.Id = newId;
                        uniqueElements.Add(element.Id, element);
                        labels.Add(element.Id);
                        elementNodeLabels.Add(element.NodeIds);
                        newId++;
                    }
                }
                elementSet = new FeElementSet(inpElementSet.Name, labels.ToArray());
                elementSets.Add(elementSet.Name, elementSet);
            }
        }
        private static void ReorientElements(Dictionary<int, FeElement> elements, Dictionary<int, FeNode> nodes)
        {
            foreach (var entry in elements)
            {

            }
        }
        private static string[] ReadIncludes(string[] lines, int firstLine, string workDirectoryName)
        {
            int includeRowNumber = -1;
            string[] includeLines = null;
            //
            for (int i = firstLine; i < lines.Length; i++)
            {
                if (lines[i].Length > 1 && lines[i][0] == '*' && lines[i][1] != '*')            // keyword line
                {
                    if (lines[i].ToUpper().StartsWith("*INCLUDE"))
                    {
                        string[] record1 = lines[i].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                        if (record1.Length == 2)
                        {
                            string fileName = record1[1].Trim();
                            if (!File.Exists(fileName)) fileName = Path.Combine(workDirectoryName, fileName);
                            if (File.Exists(fileName))
                            {
                                includeRowNumber = i;
                                includeLines = Tools.ReadAllLines(fileName);
                                break;
                            }
                            else _errors.Add("Line " + i + ": The include file does not exist.");
                        }
                        else _errors.Add("Line " + i + ": Unsupported line formatting for the include keyword");
                    }
                }
            }
            //
            if (includeRowNumber != -1)
            {
                int count = 0;
                string[] allLines;
                allLines = new string[lines.Length - 1 + includeLines.Length];
                for (int i = 0; i < includeRowNumber; i++) allLines[count++] = lines[i];
                for (int i = 0; i < includeLines.Length; i++) allLines[count++] = includeLines[i];
                for (int i = includeRowNumber + 1; i < lines.Length; i++) allLines[count++] = lines[i];
                lines = allLines;
                //
                lines = ReadIncludes(lines, includeRowNumber + 1, workDirectoryName);
            }
            return lines;
        }
        private static string[][] GetDataSets(string[] lines)
        {
            List<string> dataSet = null;
            List<string[]> dataSets = new List<string[]>();
            //
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;                                             // empty line
                if (lines[i].Length > 1 && lines[i][0] == '*' && lines[i][1] == '*') continue;  // comment line
                //
                if (lines[i].Length > 1 && lines[i][0] == '*' && lines[i][1] != '*')            // keyword line
                {
                    // First keyword
                    if (dataSet != null) dataSets.Add(dataSet.ToArray());
                    dataSet = new List<string>();
                }
                if (dataSet != null) dataSet.Add(lines[i]);                                     // other lines
            }
            // Add last data set
            if (dataSet != null) dataSets.Add(dataSet.ToArray());
            //
            return dataSets.ToArray();
        }
        //
        private static int[] GetKeywordIndices(CalculixUserKeyword userKeyword, List<CalculixKeyword> calculixKeywords,
                                               Stack<int> stackIndices)
        {
            int i = 0;
            int count;
            int[] indices;
            foreach (var keyword in calculixKeywords)
            {
                stackIndices.Push(i);
                //
                if (userKeyword.Parent.ToString() == "Model" && keyword is CalStep)
                {
                    userKeyword.Parent = null;
                    while (stackIndices.Peek() == 0) stackIndices.Pop();
                    return stackIndices.ToArray().Reverse().ToArray();
                }
                else if ((keyword is CalMaterial calMat && calMat.GetBase == userKeyword.Parent) ||
                         (keyword is CalSurfaceInteraction calSI && calSI.GetBase == userKeyword.Parent))
                {
                    userKeyword.Parent = null;
                    count = keyword.Keywords.Count();
                    stackIndices.Push(count);
                    return stackIndices.ToArray().Reverse().ToArray();
                }
                else if (keyword is CalStep calS && calS.GetBase == userKeyword.Parent)
                {
                    userKeyword.Parent = null;
                    count = keyword.Keywords.Count();
                    stackIndices.Push(count - 1);   // Last one in *End step
                    return stackIndices.ToArray().Reverse().ToArray();
                }
                else
                {
                    indices = GetKeywordIndices(userKeyword, keyword.Keywords, stackIndices);
                    if (indices != null) return indices;
                }
                //
                stackIndices.Pop();
                i++;
            }
            return null;
        }
       
        //
        private static Dictionary<int, FeNode> GetNodes(string[] lines)
        {
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            int id;
            FeNode node;
            string[] record;
            string[] splitter = new string[] { " ", ",", "\t" };

            // line 0 is the line with the keyword
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("*")) continue;
                //
                record = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                //
                id = int.Parse(record[0]);
                node = new FeNode();
                node.Id = id;
                node.X = double.Parse(record[1]);
                node.Y = double.Parse(record[2]);
                if (record.Length == 4) node.Z = double.Parse(record[3]);
                //
                nodes.Add(id, node);
            }
            //
            return nodes;
        }
        private static void AddElements(string[] lines, ref Dictionary<int, FeElement> elements,
                                        ref List<InpElementSet> inpElementTypeSets)
        {
            try
            {
                FeElement element;
                //
                string elementType = null;
                string elementSetName = null;
                string[] record1;
                string[] record2;
                // *Element, type=C3D4, ELSET=PART1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "TYPE") elementType = record2[1].Trim().ToUpper();
                        else if (record2[0].Trim().ToUpper() == "ELSET") elementSetName = record2[1].Trim();
                    }
                }
                if (elementType == null) return;
                //
                List<int> elementIds = new List<int>();
                // Line 0 is the line with the keyword
                for (int i = 1; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("*")) continue;
                    //
                    switch (elementType)
                    {
                        // LINEAR ELEMENTS                                                                                          
                        // Linear triangle element
                        case "S3":
                        case "S3R":
                        case "M3D3":
                        case "CPS3":
                        case "CPE3":
                        case "CAX3":
                            element = GetLinearTriangleElement(ref i, lines, _splitter);
                            break;
                        // Linear quadrilateral element
                        case "S4":
                        case "S4R":
                        case "M3D4":
                        case "M3D4R":
                        case "CPS4":
                        case "CPS4R":
                        case "CPE4":
                        case "CPE4R":
                        case "CAX4":
                        case "CAX4R":
                            element = GetLinearQuadrilateralElement(ref i, lines, _splitter);
                            break;
                        // Linear tetrahedron element
                        case "C3D4":
                        case "DC3D4":
                            element = GetLinearTetraElement(ref i, lines, _splitter);
                            break;
                        // Linear wedge element
                        case "C3D6":
                        case "DC3D6":
                            element = GetLinearWedgeElement(ref i, lines, _splitter);
                            break;
                        // Linear hexahedron element
                        case "C3D8":
                        case "C3D8R":
                        case "C3D8I":
                        case "DC3D8":
                            element = GetLinearHexaElement(ref i, lines, _splitter);
                            break;
                        // PARABOLIC ELEMENTS                                                                                       
                        // Parabolic triangle element
                        case "S6":
                        case "M3D6":
                        case "CPS6":
                        case "CPE6":
                        case "CAX6":
                            element = GetParabolicTriangleElement(ref i, lines, _splitter);
                            break;
                        // Parabolic quadrilateral element
                        case "S8":
                        case "S8R":
                        case "M3D8":
                        case "M3D8R":
                        case "CPS8":
                        case "CPS8R":
                        case "CPE8":
                        case "CPE8R":
                        case "CAX8":
                        case "CAX8R":
                            element = GetParabolicQuadrilateralElement(ref i, lines, _splitter);
                            break;
                        // Parabolic tetrahedron element
                        case "C3D10":
                        case "C3D10T":
                        case "DC3D10":
                            element = GetParabolicTetraElement(ref i, lines, _splitter);
                            break;
                        // Parabolic wedge element
                        case "C3D15":
                        case "DC3D15":
                            element = GetParabolicWedgeElement(ref i, lines, _splitter);
                            break;
                        // Parabolic hexahedron element
                        case "C3D20":
                        case "C3D20R":
                        case "DC3D20":
                            element = GetParabolicHexaElement(ref i, lines, _splitter);
                            break;
                        default:
                            throw new Exception("The element type '" + elementType + "' is not supported.");
                    }
                    elementIds.Add(element.Id);
                    elements.Add(element.Id, element);
                }
                // Find element set
                InpElementSet inpSet = null;
                foreach (var entry in inpElementTypeSets)
                {
                    if (entry.Name == elementSetName)   // both names can be null - must be like this for sets without names
                    {
                        inpSet = entry;
                        break;
                    }
                }
                // Create new set
                if (inpSet == null)
                {
                    inpSet = new InpElementSet(elementSetName, new HashSet<string>(), new HashSet<int>());
                    inpElementTypeSets.Add(inpSet);
                }
                // Add type and labels
                inpSet.InpElementTypeNames.Add(elementType);
                inpSet.ElementLabels.UnionWith(elementIds);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
        // Part name
        private static string GetPartName(string[] lines)
        {
            if (lines.Length > 0)
            {
                string[] tmp = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var keyword in tmp)
                {
                    if (keyword.Trim().ToUpper().StartsWith("NAME="))
                    {
                        tmp = tmp[1].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length == 2) return tmp[1].Trim();
                    }
                }
            }
            return null;
        }
        // Node/Element set
        private static void GetNodeOrElementSet(string keywordName, string[] lines, FeMesh mesh, out string name, out int[] ids)
        {
            name = null;
            ids = null;
            //
            try
            {
                bool generate = false;
                string[] record1;
                string[] record2;
                // *NSET,NSET=SET1,GENERATE
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 1)
                    {
                        if (record2[0].Trim().ToUpper() == "GENERATE") generate = true;
                    }
                    else if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == keywordName.ToUpper()) name = record2[1].Trim();
                    }
                }
                if (name == null) WriteDataToOutputStatic("There is a set name missing in the line: " + lines[0]);
                //
                List<int> setIds = new List<int>();
                if (generate)   // can be written in more than one line
                {
                    // Line 0 is the line with the keyword
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                        //
                        if (record1.Length <= 3)
                        {
                            int start = int.Parse(record1[0]);
                            int end = int.Parse(record1[1]);
                            int step = 1;
                            if (record1.Length == 3) step = int.Parse(record1[2]);

                            for (int j = start; j <= end; j += step) setIds.Add(j);
                        }
                        else throw new CaeGlobals.CaeException("When node set is defined using GENERATE parameter at most three node ids are expected.");
                    }
                    ids = setIds.ToArray();
                }
                else
                {
                    int intId;
                    string setName;
                    FeNodeSet nodeSet;
                    FeElementSet elementSet;
                    BasePart part;
                    //
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(_splitter, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string id in record1)
                        {
                            if (int.TryParse(id, out intId)) setIds.Add(intId);
                            else
                            {
                                setName = id;
                                if (keywordName == "NSET" && mesh.NodeSets.TryGetValue(setName, out nodeSet))
                                {
                                    setIds.AddRange(nodeSet.Labels);
                                }
                                else if (keywordName == "ELSET" && mesh.ElementSets.TryGetValue(setName, out elementSet))
                                {
                                    setIds.AddRange(elementSet.Labels);
                                }
                                else if (keywordName == "ELSET" && mesh.Parts.TryGetValue(setName, out part))
                                {
                                    setIds.AddRange(part.Labels);
                                }
                                else WriteDataToOutputStatic("The set named '" + setName + "' was not found.");
                            }
                        }
                    }
                    ids = setIds.ToArray();
                    Array.Sort(ids);
                }
            }
            catch (Exception ex)
            {
                WriteDataToOutputStatic(ex.Message);
            }
        }
        // Surface
        private static FeSurface GetSurface(string[] lines)
        {
            // *Surface, name=Surface-2, type=Element
            // internal-2_Surface-2_S2, S2
            // internal-3_Surface-2_S6, S6
            // 15, S5
            // 81, S5
            //
            // *Surface, type = NODE, name = _T0_Part - 1 - 1_SN, internal
            // _T0_Part-1-1_SN
            string name = null;
            FeSurfaceType type = FeSurfaceType.Element;
            string[] record1;
            string[] record2;
            //
            try
            {
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "NAME") name = record2[1].Trim();
                        else if (record2[0].Trim().ToUpper() == "TYPE")
                        {
                            if (record2[1].Trim().ToUpper() == "ELEMENT") type = FeSurfaceType.Element;
                            else if (record2[1].Trim().ToUpper() == "NODE") type = FeSurfaceType.Node;
                            else
                            {
                                _errors.Add("Surface type is not defined: " + lines[0]);
                                return null;
                            }
                        }
                    }
                }
                if (name == null)
                {
                    _errors.Add("Surface name is not defined: " + lines[0]);
                    return null;
                }
                //
                FeSurface surface = new FeSurface(name);
                surface.Type = type;
                //
                if (type == FeSurfaceType.Element)
                {
                    surface.CreatedFrom = FeSurfaceCreatedFrom.Faces;
                    // Line 0 is the line with the keyword
                    FeFaceName faceName;
                    string elementSetName;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        record1 = lines[i].Split(_splitter, StringSplitOptions.RemoveEmptyEntries);
                        //
                        if (record1.Length == 2)
                        {
                            elementSetName = record1[0];
                            faceName = FeFaceName.Empty;
                            Enum.TryParse<FeFaceName>(record1[1].ToUpper(), out faceName);
                            surface.AddElementFace(faceName, elementSetName);
                        }
                        else
                        {
                            _errors.Add("When surface face is defined an element set name and face type are expected: " + lines[0]);
                            return null;
                        }
                    }
                }
                else if (type == FeSurfaceType.Node)
                {
                    surface.CreatedFrom = FeSurfaceCreatedFrom.NodeSet;
                    //
                    if (lines.Length != 2)
                    {
                        _errors.Add("When surface with type NODE is defined one node set name is expected: " + lines[0]);
                        return null;
                    }
                    // Line 0 is the line with the keyword    
                    record1 = lines[1].Split(_splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (record1.Length == 1)
                    {
                        surface.CreatedFromNodeSetName = record1[0];
                    }
                    else
                    {
                        _errors.Add("When surface with type NODE is defined one node set name is expected: " + lines[0]);
                        return null;
                    }
                }
                //
                return surface;
            }
            catch
            {
                if (name != null) name = " " + name;
                else name = "";
                _errors.Add("Failed to import surface " + name + ": " + lines[0]);
                return null;
            }
        }
        // Rigid body
        private static Constraint GetRigidBody(string[] lines, Dictionary<int, FeNode> nodes,
                                               OrderedDictionary<string, Constraint> constraints,
                                               OrderedDictionary<string, FeReferencePoint> referencePoints)
        {
            try
            {
                if (lines.Length > 1)
                {
                    _errors.Add("Only one line expected for the rigid body: " + lines[0]);
                    return null;
                }
                //
                string nodeSetName;
                int nodeId1 = -1;
                int nodeId2 = -1;
                string[] record1;
                string[] record2;
                //*Rigid body, Nset=internal-2_Element-Selection, Ref node=57885, Rot node=57886
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length != 4)
                {
                    _errors.Add("Unsupported line formatting for the rigid body: " + lines[0]);
                    return null;
                }
                //
                record2 = record1[1].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeSetName = record2[1];
                //
                record2 = record1[2].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeId1 = int.Parse(record2[1]);
                //
                record2 = record1[3].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                nodeId2 = int.Parse(record2[1]);
                // Check for existing reference point
                string referencePointName = null;
                foreach (var entry in referencePoints)
                {
                    if (entry.Value.CreatedFromRefNodeId1 == nodeId1)
                    {
                        referencePointName = entry.Key;
                        break;
                    }
                }
                // Create a new reference point if it was not found
                if (referencePointName == null)
                {
                    referencePointName = referencePoints.GetNextNumberedKey("RP");
                    FeNode node = nodes[nodeId1];
                    FeReferencePoint referencePoint = new FeReferencePoint(referencePointName, node, nodeId2);
                    referencePoints.Add(referencePointName, referencePoint);
                }
                //
                string rigidBodyName = constraints.GetNextNumberedKey("Constraint");
                RigidBody rigidBody = new RigidBody(rigidBodyName, referencePointName, nodeSetName,
                                                    RegionTypeEnum.NodeSetName, false);
                return rigidBody;                
            }
            catch
            {
                _errors.Add("Failed to import rigid body: " + lines[0]);
                return null;
            }
        }
        // Material
        private static Material GetMaterial(string[][] dataSets, ref int dataSetId, List<CalculixUserKeyword> userKeywords)
        {
            // *Material, Name=S235
            Material material = null;
            string name = null;
            string[] record1;
            string[] record2;
            string[] dataSet = null;
            //
            try
            {
                dataSet = dataSets[dataSetId];
                record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "NAME")
                        {
                            name = record2[1].Trim();
                            break;
                        }
                    }
                }
                dataSetId++;
                //
                if (name != null)
                {
                    string keyword;
                    bool temperatureDependent;
                    material = new Material(name);
                    //
                    for (int i = dataSetId; i < dataSets.Length; i++)
                    {
                        dataSetId = i;
                        dataSet = dataSets[dataSetId];
                        keyword = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                        //
                        if (keyword == "*DENSITY")
                        {
                            Density density = GetMaterialDensity(dataSet, out temperatureDependent);
                            if (density != null) material.AddProperty(density);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (keyword == "*SLIP WEAR")
                        {
                            SlipWear slipWear = GetMaterialSlipWear(dataSet, out temperatureDependent);
                            if (slipWear != null) material.AddProperty(slipWear);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (keyword == "*ELASTIC")
                        {
                            Elastic elastic = GetMaterialElasticity(dataSet, out temperatureDependent);
                            if (elastic != null) material.AddProperty(elastic);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (keyword == "*PLASTIC")
                        {
                            Plastic plastic = GetMaterialPlasticity(dataSet, out temperatureDependent);
                            if (plastic != null) material.AddProperty(plastic);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        // Thermal
                        else if (keyword == "*EXPANSION")
                        {
                            ThermalExpansion thermalExpansion = GetMaterialThermalExpansion(dataSet, out temperatureDependent);
                            if (thermalExpansion != null) material.AddProperty(thermalExpansion);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (keyword == "*CONDUCTIVITY")
                        {
                            ThermalConductivity thermalConductivity =
                                GetMaterialThermalConductivity(dataSet, out temperatureDependent);
                            if (thermalConductivity != null) material.AddProperty(thermalConductivity);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (keyword == "*SPECIFIC HEAT")
                        {
                            SpecificHeat specificHeat = GetMaterialSpecificHeat(dataSet, out temperatureDependent);
                            if (specificHeat != null) material.AddProperty(specificHeat);
                            if (temperatureDependent) material.TemperatureDependent = true;
                        }
                        else if (_materialKeywords.Contains(keyword))
                        {
                            // User keyword
                            CalculixUserKeyword userKeyword = new CalculixUserKeyword(dataSet.ToRows(dataSet.Length));
                            userKeyword.Parent = material;
                            userKeywords.Add(userKeyword);
                        }
                        else
                        {
                            dataSetId--;
                            break;
                        }
                    }
                }
                else _errors.Add("Material name not found: " + dataSet[0]);

                //
                return material;
            }
            catch
            {
                _errors.Add("Failed to import material: " + dataSet != null ? dataSet.ToRows() : "");
                return null;
            }
        }
        private static Density GetMaterialDensity(string[] lines, out bool temperatureDependent)
        {
            // *Density     
            // 7.85E-09, 0  
            // 7.80E-09, 100
            string[] record1;
            temperatureDependent = false;
            try
            {
                double rho;
                double T;
                double[][] densityTemp = new double[lines.Length - 1][];
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    rho = double.Parse(record1[0]);
                    T = 0;
                    if (record1.Length == 2)
                    {
                        T = double.Parse(record1[1]);
                        temperatureDependent = true;
                    }
                    densityTemp[i - 1] = new double[] { rho, T };
                }
                //
                Density density = new Density(densityTemp);
                return density;
            }
            catch
            {
                _errors.Add("Failed to import density: " + lines.ToRows());
                return null;
            }
        }
        private static SlipWear GetMaterialSlipWear(string[] lines, out bool temperatureDependent)
        {
            // *Slip wear     
            // 60, 0.01
            string[] record1;
            temperatureDependent = false;
            try
            {
                double hardness = 0;
                double wearCoefficient = 0;
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    hardness = double.Parse(record1[0]);
                    wearCoefficient = double.Parse(record1[1]);
                }
                //
                SlipWear slipWear = new SlipWear(hardness, wearCoefficient);
                return slipWear;
            }
            catch
            {
                _errors.Add("Failed to import slip wear: " + lines.ToRows());
                return null;
            }
        }
        private static Elastic GetMaterialElasticity(string[] lines, out bool temperatureDependent)
        {
            // *Elastic        
            // 210000, 0.3, 0  
            // 180000, 0.3, 100
            string[] record1;
            temperatureDependent = false;
            try
            {
                double E;
                double v;
                double T;
                double[][] youngsPoissonsTemp = new double[lines.Length - 1][];
                //
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    //
                    E = double.Parse(record1[0]);
                    v = double.Parse(record1[1]);
                    T = 0;
                    if (record1.Length == 3)
                    {
                        T = double.Parse(record1[2]);
                        temperatureDependent = true;
                    }
                    //
                    youngsPoissonsTemp[i - 1] = new double[] { E, v, T };
                }
                //
                Elastic elastic = new Elastic(youngsPoissonsTemp);
                return elastic;
            }
            catch
            {
                _errors.Add("Failed to import elasticity: " + lines.ToRows());
                return null;
            }
        }
        private static Plastic GetMaterialPlasticity(string[] lines, out bool temperatureDependent)
        {
            // *Plastic, Hardening=Kinematic
            // 235, 0,   0  
            // 400, 0.2, 0  
            // 200, 0,   100
            // 350, 0.2, 100
            string[] record1;
            temperatureDependent = false;
            try
            {
                PlasticHardening hardening = PlasticHardening.Isotropic;
                double stress;
                double strain;
                double T;
                double[][] stressStrainTemp = new double[lines.Length - 1][];
                //
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 1)
                {
                    record1 = record1[1].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    hardening = (PlasticHardening)Enum.Parse(typeof(PlasticHardening), record1[1]);
                }
                //
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    stress = double.Parse(record1[0]);
                    strain = double.Parse(record1[1]);
                    T = 0;
                    if (record1.Length == 3)
                    {
                        T = double.Parse(record1[2]);
                        temperatureDependent = true;
                    }
                    stressStrainTemp[i - 1] = new double[] { stress, strain, T };
                }
                //
                Plastic plastic = new Plastic(stressStrainTemp);
                plastic.Hardening = hardening;
                return plastic;
            }
            catch
            {
                _errors.Add("Failed to import plasticity: " + lines.ToRows());
                return null;
            }
        }
        // Thermal
        private static ThermalExpansion GetMaterialThermalExpansion(string[] lines, out bool temperatureDependent)
        {
            // *Expansion   
            // 4.E-6,-1300
            // 12.E-6,-100
            // 20.E-6,1100
            // 28.E-6,2300
            string[] record1;
            temperatureDependent = false;
            try
            {
                double exp;
                double T;
                double[][] thermalExpansionTemp = new double[lines.Length - 1][];
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    exp = double.Parse(record1[0]);
                    T = 0;
                    if (record1.Length == 2)
                    {
                        T = double.Parse(record1[1]);
                        temperatureDependent = true;
                    }
                    thermalExpansionTemp[i - 1] = new double[] { exp, T };
                }
                //
                ThermalExpansion thermalExpansion = new ThermalExpansion(thermalExpansionTemp);
                return thermalExpansion;
            }
            catch
            {
                _errors.Add("Failed to import thermal expansion: " + lines.ToRows());
                return null;
            }
        }
        private static ThermalConductivity GetMaterialThermalConductivity(string[] lines, out bool temperatureDependent)
        {
            // *Conductivity
            // 14, 0
            // 20, 100
            string[] record1;
            temperatureDependent = false;
            try
            {
                double con;
                double T;
                double[][] thermalConductivityTemp = new double[lines.Length - 1][];
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    con = double.Parse(record1[0]);
                    T = 0;
                    if (record1.Length == 2)
                    {
                        T = double.Parse(record1[1]);
                        temperatureDependent = true;
                    }
                    thermalConductivityTemp[i - 1] = new double[] { con, T };
                }
                //
                ThermalConductivity thermalConductivity = new ThermalConductivity(thermalConductivityTemp);
                return thermalConductivity;
            }
            catch
            {
                _errors.Add("Failed to import thermal conductivity: " + lines.ToRows());
                return null;
            }
        }
        private static SpecificHeat GetMaterialSpecificHeat(string[] lines, out bool temperatureDependent)
        {
            // *Specific heat
            // 440000000, 0
            // 460000000, 100
            string[] record1;
            temperatureDependent = false;
            try
            {
                double sh;
                double T;
                double[][] specificHeatTemp = new double[lines.Length - 1][];
                for (int i = 1; i < lines.Length; i++)
                {
                    record1 = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    sh = double.Parse(record1[0]);
                    T = 0;
                    if (record1.Length == 2)
                    {
                        T = double.Parse(record1[1]);
                        temperatureDependent = true;
                    }
                    specificHeatTemp[i - 1] = new double[] { sh, T };
                }
                //
                SpecificHeat specificHeat = new SpecificHeat(specificHeatTemp);
                return specificHeat;
            }
            catch
            {
                _errors.Add("Failed to import specific heat: " + lines.ToRows());
                return null;
            }
        }
        // Section
        private static SolidSection GetSolidSection(string[] dataSet, OrderedDictionary<string, Section> sections)
        {
            // Solid section can exist in two (2) formats as shown below:
            // *Solid Section, ELSET=STEEL_A, MATERIAL=STEEL_A
            // *Solid Section, MATERIAL=STEEL_A, ELSET=STEEL_A
            string regionName = null;
            string materialName = null;
            try
            {
                string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    string[] record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length != 2) continue;
                    else if (record2[0].Trim().ToUpper() == "ELSET") regionName = record2[1].Trim();
                    else if (record2[0].Trim().ToUpper() == "MATERIAL") materialName = record2[1].Trim();
                }
                //
                string name = sections.GetNextNumberedKey("Section");
                var section = new SolidSection(name, materialName, regionName, RegionTypeEnum.ElementSetName, 0, false);
                //
                return section;
            }
            catch
            {
                _errors.Add("Failed to import section: " + dataSet.ToRows());
                return null;
            }
        }
        private static ShellSection GetShellSection(string[] dataSet, OrderedDictionary<string, Section> sections)
        {
            // *Shell section, Elset=S4R, Material=Material-1, Offset=0
            // 5
            string regionName = null;
            string materialName = null;
            double offset = 0;
            try
            {
                string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    string[] record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length != 2) continue;
                    else if (record2[0].Trim().ToUpper() == "ELSET") regionName = record2[1].Trim();
                    else if (record2[0].Trim().ToUpper() == "MATERIAL") materialName = record2[1].Trim();
                    else if (record2[0].Trim().ToUpper() == "OFFSET") offset = double.Parse(record2[1]);
                }
                //
                if (dataSet.Length == 2)
                {
                    record1 = dataSet[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    double thickness = double.Parse(record1[0]);
                    //
                    string name = sections.GetNextNumberedKey("Section");
                    ShellSection section = new ShellSection(name, materialName, regionName, RegionTypeEnum.ElementSetName,
                                                            thickness, false);
                    section.Offset = offset;
                    //
                    return section;
                }
                else throw new Exception();
            }
            catch
            {
                _errors.Add("Failed to import section: " + dataSet.ToRows());
                return null;
            }
        }
        // Surface interaction
        private static SurfaceInteraction GetSurfaceInteraction(string[][] dataSets, ref int dataSetId,
                                                                List<CalculixUserKeyword> userKeywords)
        {
            // *Surface interaction, Name=Surface_interaction-1
            SurfaceInteraction surfaceInteraction = null;
            string name = null;
            string[] record1;
            string[] record2;
            string[] dataSet = null;
            //
            try
            {
                dataSet = dataSets[dataSetId];
                record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2)
                    {
                        if (record2[0].Trim().ToUpper() == "NAME")
                        {
                            name = record2[1].Trim();
                            break;
                        }
                    }
                }                
                dataSetId++;
                //
                if (name != null)
                {
                    string keyword;
                    surfaceInteraction = new SurfaceInteraction(name);
                    //
                    for (int i = dataSetId; i < dataSets.Length; i++)
                    {
                        dataSetId = i;
                        dataSet = dataSets[i];
                        keyword = dataSet[0].Split(_splitter, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                        //
                        if (_surfaceInteractionKeywords.Contains(keyword))
                        {
                            // User keyword
                            CalculixUserKeyword userKeyword = new CalculixUserKeyword(dataSet.ToRows(dataSet.Length));
                            userKeyword.Parent = surfaceInteraction;
                            userKeywords.Add(userKeyword);
                        }
                        else
                        {
                            dataSetId--;
                            break;
                        }                        
                    }
                }
                else _errors.Add("Surface interaction name not found: " + dataSet.ToRows());
                //
                return surfaceInteraction;
            }
            catch
            {
                _errors.Add("Failed to import surface interaction: " + dataSet != null ? dataSet.ToRows() : "");
                return null;
            }
        }
        // Amplitude
        private static AmplitudeTabular GetAmplitudeTabular(string[] dataSet)
        {
            //*Amplitude, name=Amp-1
            string amplitudeName = null;
            bool totalTime = false;
            double shiftX = 0;
            double shiftY = 0;
            try
            {
                string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                string[] record2;
                //
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length != 2) continue;
                    else if (record2[0].Trim().ToUpper() == "NAME") amplitudeName = record2[1].Trim();
                    else if (record2[0].Trim().ToUpper() == "TIME") totalTime = true;
                    else if (record2[0].Trim().ToUpper() == "SHIFTX") shiftX = double.Parse(record2[1].Trim());
                    else if (record2[0].Trim().ToUpper() == "SHIFTY") shiftY = double.Parse(record2[1].Trim());
                }
                // Read amplitude points
                List<double> values = new List<double>();
                for (int i = 1; i < dataSet.Length; i++)
                {
                    record1 = dataSet[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var rec in record1) values.Add(double.Parse(rec));
                }
                if (values.Count % 2 != 0) throw new Exception("There is an odd number of values defined for the amplitude.");
                //
                int count = 0;
                double[][] timeAmplitude = new double[values.Count / 2][];
                foreach (var value in values)
                {
                    if (count % 2 == 0)
                    {
                        timeAmplitude[count / 2] = new double[] { value, 0 };
                    }
                    else
                    {
                        timeAmplitude[count / 2][1] = value;
                    }
                    count++;
                }
                //
                AmplitudeTabular amplitude = new AmplitudeTabular(amplitudeName, timeAmplitude);
                if (totalTime) amplitude.TimeSpan = AmplitudeTimeSpanEnum.TotalTime;
                if (shiftX != 0) amplitude.ShiftX = shiftX;
                if (shiftY != 0) amplitude.ShiftY = shiftY;
                //
                return amplitude;
            }
            catch
            {
                _errors.Add("Failed to import amplitude: " + dataSet.ToRows());
                return null;
            }
        }
        // Step
        private static Step GetStep(string[][] dataSets, ref int dataSetId, FeMesh mesh, OrderedDictionary<string, Step> steps,
                                    OrderedDictionary<string, ContactPair> contactPairs, List<CalculixUserKeyword> userKeywords)
        {
            // *STEP, NAME=STEP-1, NLGEOM=NO, PERTURBATION -- ABAQUS
            //
            // *STEP, NLGEOM,INC = 1000 -- CALCULIX
            bool nlgeom = false;
            int? maxIncrements = null;
            bool perturbation = false;
            string[] dataSet = null;
            try
            {
                dataSet = dataSets[dataSetId];
                string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                string[] record2;
                // Capture the options on the step line
                foreach (var rec in record1)
                {
                    record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2.Length == 2) 
                    {
                        if (record2[0].Trim().ToUpper() == "NLGEOM")    // Look for ABAQUS Style "NLGEOM = YES"
                        {
                            if (record2[1].Trim().ToUpper() == "YES") nlgeom = true;
                        }
                        else if (record2[0].Trim().ToUpper() == "INC")
                        {
                            maxIncrements = int.Parse(record2[1].Trim());
                        }
                    }
                    else if (record2.Length == 1) 
                    {
                        if (record2[0] == "NLGEOM") nlgeom = true;      // Look for Calculix stye "NLGEOM"
                        else if (record2[0] == "PERTURBATION") perturbation = true;
                    }
                }
                // Go to the next keyword in Step
                dataSetId++;
                // Get step type first
                Step step = null;
                int prevDataSetId = dataSetId;
                while (dataSetId < dataSets.Length)
                {
                    dataSet = dataSets[dataSetId];
                    //
                    record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    string keyword = record1[0].Trim().ToUpper();
                    // *Static, Solver=Spooles, Direct
                    if (keyword == "*STATIC") step = GetStaticStep(dataSet);
                    else if (keyword == "*FREQUENCY") step = GetFrequencyStep(dataSet);
                    else if (keyword == "*BUCKLE") step = GetBuckleStep(dataSet);
                    else if (keyword == "*HEAT TRANSFER") step = GetHeatTransferStep(dataSet);
                    else if (keyword == "*END STEP") break;
                    //
                    dataSetId++;
                }
                step.FieldOutputs.Clear();              // Clear default field outputs
                step.Name = steps.GetNextNumberedKey("Step");
                // Get step features
                dataSetId = prevDataSetId;              // Go back to the beginning of the step
                dataSetId++;
                //
                while (dataSetId < dataSets.Length)
                {
                    dataSet = dataSets[dataSetId];      // Next keyword                    
                    //
                    record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    string keyword = record1[0].Trim().ToUpper();
                    //
                    if (keyword == "*BOUNDARY") AddStepBoundaryCondition(step, dataSet, mesh);
                    else if (keyword == "*CLOAD") AddStepCLoad(step, mesh, dataSet);
                    else if (keyword == "*DLOAD") AddStepDLoad(step, dataSet);
                    //
                    else if (keyword == "*NODE FILE") AddStepNodalFieldOutput(step, dataSet);
                    else if (keyword == "*EL FILE") AddStepElementFieldOutput(step, dataSet);
                    else if (keyword == "*CONTACT FILE") AddStepContactFieldOutput(step, dataSet);
                    //
                    else if (keyword == "*NODE PRINT") AddStepNodalHistoryOutput(step, dataSet);
                    else if (keyword == "*EL PRINT") AddStepElementHistoryOutput(step, dataSet);
                    else if (keyword == "*CONTACT PRINT") AddStepContactHistoryOutput(step, dataSet, contactPairs);
                    else if (keyword == "*END STEP") break;
                    else
                    {
                        // User keyword
                        CalculixUserKeyword userKeyword = new CalculixUserKeyword(dataSet.ToRows(dataSet.Length));
                        userKeyword.Parent = step;
                        userKeywords.Add(userKeyword);
                    }
                    //
                    dataSetId++;
                }
                step.Nlgeom = nlgeom;
                if (maxIncrements != null) step.MaxIncrements = (int)maxIncrements;
                step.Perturbation = perturbation;
                //
                return step;
            }
            catch
            {
                _errors.Add("Failed to import step: " + dataSet != null ? dataSet.ToRows() : "");
                return null;
            }
        }
        private static StaticStep GetStaticStep(string[] dataSet)
        {
            string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
            string[] record2;
            StaticStep staticStep = new StaticStep("Static");
            //
            for (int i = 1; i < record1.Length; i++)
            {
                record2 = record1[i].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                if (record2.Length == 2)
                {
                    if (record2[0].Trim().ToUpper() == "SOLVER") staticStep.SolverType = GetSolverType(record2[1]);
                }
                else if (record2.Length == 1)
                {
                    if (record2[0].Trim().ToUpper() == "DIRECT") staticStep.Direct = true;
                }
            }
            if (dataSet.Length == 2)
            {
                // If the second line exists the incrementation is Direct or Automatic
                if (!staticStep.Direct) staticStep.IncrementationType = IncrementationTypeEnum.Automatic;
                //
                record2 = dataSet[1].Split(_splitterComma, StringSplitOptions.None); // must be none
                //
                if (record2.Length > 0 && record2[0].Trim().Length > 0)
                    staticStep.InitialTimeIncrement = double.Parse(record2[0]);
                if (record2.Length > 1 && record2[1].Trim().Length > 0)
                    staticStep.TimePeriod = double.Parse(record2[1]);
                if (record2.Length > 2 && record2[2].Trim().Length > 0)
                    staticStep.MinTimeIncrement = double.Parse(record2[2]);
                if (record2.Length > 3 && record2[3].Trim().Length > 0)
                    staticStep.MaxTimeIncrement = double.Parse(record2[3]);
            }
            //
            return staticStep;
        }
        private static FrequencyStep GetFrequencyStep(string[] dataSet)
        {
            string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
            string[] record2;
            FrequencyStep frequencyStep = new FrequencyStep("Frequency");
            //
            for (int i = 1; i < record1.Length; i++)
            {
                record2 = record1[i].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                if (record2.Length == 2)
                {
                    if (record2[0].Trim().ToUpper() == "SOLVER")
                        frequencyStep.SolverType = GetSolverType(record2[1]);
                    else if (record2[0].Trim().ToUpper() == "STORAGE" && record2[1].Trim().ToUpper() == "YES")
                        frequencyStep.Storage = true;
                }
            }
            if (dataSet.Length == 2)
            {
                record2 = dataSet[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                if (record2.Length > 0) frequencyStep.NumOfFrequencies = int.Parse(record2[0]);
            }
            //
            return frequencyStep;
        }
        private static BuckleStep GetBuckleStep(string[] dataSet)
        {
            string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
            string[] record2;
            BuckleStep buckleStep = new BuckleStep("Buckle");
            //
            for (int i = 1; i < record1.Length; i++)
            {
                record2 = record1[i].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                if (record2.Length == 2)
                {
                    if (record2[0].Trim().ToUpper() == "SOLVER") buckleStep.SolverType = GetSolverType(record2[1]);
                }
            }
            if (dataSet.Length == 2)
            {
                record2 = dataSet[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                if (record2.Length > 0) buckleStep.NumOfBucklingFactors = int.Parse(record2[0]);
            }
            //
            return buckleStep;
        }
        private static HeatTransferStep GetHeatTransferStep(string[] dataSet)
        {
            string[] record1 = dataSet[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
            string[] record2;
            HeatTransferStep heatTransferStep = new HeatTransferStep("Heat");
            for (int i = 1; i < record1.Length; i++)
            {
                record2 = record1[i].Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                if (record2.Length == 2)
                {
                    if (record2[0].Trim().ToUpper() == "SOLVER")
                        heatTransferStep.SolverType = GetSolverType(record2[1]);
                    else if (record2[0].Trim().ToUpper() == "DELTMX")
                        heatTransferStep.Deltmx = double.Parse(record2[1]);
                }
                else if (record2.Length == 1)
                {
                    if (record2[0].Trim().ToUpper() == "DIRECT") heatTransferStep.Direct = true;
                    else if (record2[0].Trim().ToUpper() == "STEADY STATE") heatTransferStep.SteadyState = true;
                }
            }
            if (dataSet.Length == 2)
            {
                // If the second line exists the incrementation is Direct or Automatic
                if (!heatTransferStep.Direct) heatTransferStep.IncrementationType = IncrementationTypeEnum.Automatic;
                //
                record2 = dataSet[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                //
                if (record2.Length > 0) heatTransferStep.InitialTimeIncrement = double.Parse(record2[0]);
                if (record2.Length > 1) heatTransferStep.TimePeriod = double.Parse(record2[1]);
                if (record2.Length > 2) heatTransferStep.MinTimeIncrement = double.Parse(record2[2]);
                if (record2.Length > 3) heatTransferStep.MaxTimeIncrement = double.Parse(record2[3]);
            }
            //
            return heatTransferStep;
        }
        private static SolverTypeEnum GetSolverType(string solverName)
        {
            SolverTypeEnum solverType = (SolverTypeEnum)Enum.Parse(typeof(SolverTypeEnum), solverName.Replace(" ", ""));
            return solverType;
        }
        //
        private static void AddStepBoundaryCondition(Step step, string[] lines, FeMesh mesh)
        {
            // *Boundary, Amplitude=Amp-1
            // Set_1, 2, 2
            // Set_1, 3, 3, 0.001
            // Set_1, 1, 3, 0
            try
            {
                string name;
                string nodeSetName;
                int dof;
                int nodeId;
                double value;
                Dictionary<string, FeNodeSet> nodeSets = new Dictionary<string, FeNodeSet>();
                Dictionary<string, BoundaryCondition> boundaryConditions = new Dictionary<string, BoundaryCondition>();
                HashSet<string> allBCNames = new HashSet<string>(step.BoundaryConditions.Keys);
                // Amplitude
                string amplitudeName = GetAmplitudeName(lines[0]);
                //
                for (var i = 1; i < lines.Length; i++)
                {
                    string[] recordBC = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    // Create Boundary Condition
                    // Get regionName
                    string regionName = recordBC[0].Trim();
                    // Create new internal node set if it does not exist
                    if (!mesh.NodeSets.Keys.Contains(regionName) && int.TryParse(regionName, out nodeId))
                    {
                        nodeSetName = nodeSets.GetNextNumberedKey(Globals.InternalName + "_" + regionName);
                        FeNodeSet nodeSet = new FeNodeSet(nodeSetName, new int[] { nodeId });
                        nodeSets.Add(nodeSet.Name, nodeSet);
                        //
                        regionName = nodeSetName;
                    }
                    // Get the prescribed displacement
                    int dofStart = int.Parse(recordBC[1]);
                    int dofEnd = dofStart;
                    if (int.TryParse(recordBC[2], out dof)) dofEnd = dof;
                    double dofValue = 0;
                    if (recordBC.Length == 4 && double.TryParse(recordBC[3], out value)) dofValue = value;
                    //
                    if (dofStart == 11)
                    {
                        name = allBCNames.GetNextNumberedKey("Temperature");
                        TemperatureBC tempBC = new TemperatureBC(name, regionName, RegionTypeEnum. NodeSetName, dofValue, false);
                        // Amplitude
                        if (amplitudeName != null) tempBC.AmplitudeName = amplitudeName;
                        // Add
                        boundaryConditions.Add(name, tempBC);
                    }
                    else
                    {
                        name = allBCNames.GetNextNumberedKey("Displacement_rotation");
                        DisplacementRotation dispRotBC = new DisplacementRotation(name, regionName, RegionTypeEnum.NodeSetName,
                                                                                  false);
                        // Amplitude
                        if (amplitudeName != null) dispRotBC.AmplitudeName = amplitudeName;
                        // Assign DOF prescribed displacement
                        for (var j = dofStart; j <= dofEnd; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    dispRotBC.U1 = dofValue;
                                    break;
                                case 2:
                                    dispRotBC.U2 = dofValue;
                                    break;
                                case 3:
                                    dispRotBC.U3 = dofValue;
                                    break;
                                case 4:
                                    dispRotBC.UR1 = dofValue;
                                    break;
                                case 5:
                                    dispRotBC.UR2 = dofValue;
                                    break;
                                case 6:
                                    dispRotBC.UR3 = dofValue;
                                    break;
                            }
                        }
                        // Add
                        boundaryConditions.Add(name, dispRotBC);
                    }
                    allBCNames.Add(name);
                }
                //
                //MergeStepBoudaryConditions(boundaryConditions, nodeSets, step, mesh);
                //
                foreach (var entry in boundaryConditions) step.AddBoundaryCondition(entry.Value);
                foreach (var entry in nodeSets) mesh.AddNodeSet(entry.Value);
            }
            catch
            {
                _errors.Add("Failed to import boundary condition: " + lines.ToRows());
            }
        }
        private static void MergeStepBoudaryConditions(Dictionary<string, BoundaryCondition> boundaryConditions,
                                                       Dictionary<string, FeNodeSet> nodeSets,
                                                       Step step, FeMesh mesh)
        {
            try
            {
                double[] key;
                CompareDoubleArray coparer = new CompareDoubleArray();
                List<BoundaryCondition> groupedBoundaryConditions;
                Dictionary<double[], List<BoundaryCondition>> keyBoundaryConditions = 
                    new Dictionary<double[], List<BoundaryCondition>>(coparer);
                //
                foreach (var bc in boundaryConditions.Values)
                {
                    key = new double[2];
                    if (bc is TemperatureBC tempBC)
                    {
                        key[0] = 11;
                        key[1] = tempBC.Temperature;
                    }
                    else if (bc is DisplacementRotation dispRotBC)
                    {
                        key[0] = dispRotBC.GetConstraintHash();
                        if (key[0] > 0) key[1] = dispRotBC.GetConstrainValues()[0]; // at least one direction is prescribed
                    }
                    else throw new NotSupportedException();
                    //
                    if (keyBoundaryConditions.TryGetValue(key, out groupedBoundaryConditions)) groupedBoundaryConditions.Add(bc);
                    else keyBoundaryConditions.Add(key, new List<BoundaryCondition>() { bc });
                }
                //
                boundaryConditions.Clear();
                // Merge
                BoundaryCondition boundaryCondition;
                HashSet<int> nodeIds = new HashSet<int>();
                FeNodeSet nodeSet;
                FeNodeSet mergedNodeSet;
                Dictionary<string, FeNodeSet> mergedNodeSets = new Dictionary<string, FeNodeSet>();
                HashSet<string> allNodeSetNames = new HashSet<string>(mesh.NodeSets.Keys);
                allNodeSetNames.UnionWith(nodeSets.Keys);
                HashSet<string> allBCNames = new HashSet<string>(step.BoundaryConditions.Keys);
                //
                foreach (var keyBcEntry in keyBoundaryConditions)
                {
                    boundaryCondition = keyBcEntry.Value.First();
                    nodeIds.Clear();
                    //
                    if (keyBcEntry.Value.Count > 1)
                    {
                        foreach (var bcEntry in keyBcEntry.Value)
                        {
                            if (nodeSets.TryGetValue(bcEntry.RegionName, out nodeSet)) nodeIds.UnionWith(nodeSet.Labels);
                            else if (mesh.NodeSets.TryGetValue(bcEntry.RegionName, out nodeSet)) nodeIds.UnionWith(nodeSet.Labels);
                            else throw new NotSupportedException();
                        }
                        // Node set
                        mergedNodeSet = new FeNodeSet(boundaryCondition.RegionName, nodeIds.ToArray());
                        mergedNodeSet.Internal = true;
                        // Rename
                        mergedNodeSet.Name = allNodeSetNames.GetNextNumberedKey(Globals.InternalName + "_merged");
                        // Add
                        allNodeSetNames.Add(mergedNodeSet.Name);
                        mergedNodeSets.Add(mergedNodeSet.Name, mergedNodeSet);
                        // Boundary condition
                        boundaryCondition.RegionName = mergedNodeSet.Name;
                        // Rename
                        boundaryCondition.Name = allBCNames.GetNextNumberedKey("BC_merged");
                    }
                    // Add
                    allBCNames.Add(boundaryCondition.Name);
                    boundaryConditions.Add(boundaryCondition.Name, boundaryCondition);
                }
                //
                nodeSets.Clear();
                nodeSets.AddRange(mergedNodeSets);
            }
            catch
            {
                _errors.Add("Failed to merge boundary conditions.");
            }
        }
        private static void AddStepCLoad(Step step, FeMesh mesh, string[] lines)
        {
            // *CLoad, Amplitude=Amp-1
            // LD_BRTIP, 2, 10000 - Concentrated Force (f1,f2,f3)
            // 6623, 1, 22.99137  - Concentrated Force (f1,f2,f3)
            // LD_BRTIP, 6, 10000 - Moment (m1,m2,m3)
            try
            {
                // CLoad can either be a concentrated force or moment
                string nameCF;
                string nameMom;
                int nodeId;
                FeNodeSet nodeSet;
                // Amplitude
                string amplitudeName = GetAmplitudeName(lines[0]);
                //
                for (var i = 1; i < lines.Length; i++)
                {
                    nameCF = step.Loads.GetNextNumberedKey("Concentrated_force");
                    nameMom = step.Loads.GetNextNumberedKey("Moment");
                    //
                    string[] recordCL = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    // Get regionName name or nodeId
                    nodeId = -1;
                    string regionName = recordCL[0].Trim();
                    if (!mesh.NodeSets.ContainsKey(regionName)) int.TryParse(regionName, out nodeId);
                    if (nodeId != -1)
                    {
                        regionName = mesh.NodeSets.GetNextNumberedKey("Auto_created");
                        nodeSet = new FeNodeSet(regionName, new int[] { nodeId });
                        mesh.NodeSets.Add(regionName, nodeSet);
                    }
                    // Get degree of freedom and value
                    int dof = int.Parse(recordCL[1]); ;
                    double dofValue = double.Parse(recordCL[2]);
                    CLoad cfLoad = new CLoad(nameCF, regionName, RegionTypeEnum.NodeSetName, 0.0, 0.0, 0.0, false);
                    MomentLoad momentLoad = new MomentLoad(nameMom, regionName, RegionTypeEnum.NodeSetName, 0.0, 0.0, 0.0, false);
                    // Amplitude
                    if (amplitudeName != null)
                    {
                        cfLoad.AmplitudeName = amplitudeName;
                        momentLoad.AmplitudeName = amplitudeName;
                    }
                    //
                    switch (dof)
                    {
                        case 1:
                            cfLoad.F1 = dofValue;
                            step.AddLoad(cfLoad);
                            break;
                        case 2:
                            cfLoad.F2 = dofValue;
                            step.AddLoad(cfLoad);
                            break;
                        case 3:
                            cfLoad.F3 = dofValue;
                            step.AddLoad(cfLoad);
                            break;
                        case 4:
                            momentLoad.M1 = dofValue;
                            step.AddLoad(momentLoad);
                            break;
                        case 5:
                            momentLoad.M2 = dofValue;
                            step.AddLoad(momentLoad);
                            break;
                        case 6:
                            momentLoad.M3 = dofValue;
                            step.AddLoad(momentLoad);
                            break;
                        default:
                            _errors.Add("Failed to import load: " + lines.ToRows());
                            break;
                    }
                }
            }
            catch
            {
                _errors.Add("Failed to import load: " + lines.ToRows());
            }
        }
        private static void AddStepDLoad(Step step, string[] lines)
        {
            // *DLoad, Amplitude=Amp-1
            // Eall, GRAV, 9.81, 0, 0, -1
            try
            {
                // DLoad as gravity force is the only force covered at this point
                string[] recordDL;
                string regionName;
                string loadingType;
                double gValue;
                string name;
                // Amplitude
                string amplitudeName = GetAmplitudeName(lines[0]);
                //
                for (var i = 1; i < lines.Length; i++)
                {
                    recordDL = lines[i].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                    // Get regionName
                    regionName = recordDL[0].Trim();
                    // Get loading type
                    loadingType = recordDL[1].Trim();
                    if (loadingType.ToUpper() == "GRAV")
                    {
                        // Get Gravity value
                        gValue = double.Parse(recordDL[2]);
                        name = step.Loads.GetNextNumberedKey("Grav");
                        //
                        GravityLoad gLoad = new GravityLoad(name, regionName, RegionTypeEnum.ElementSetName,
                                                            0.0, 0.0, 0.0, false);
                        //
                        gLoad.F1 = double.Parse(recordDL[3]) * gValue;
                        gLoad.F2 = double.Parse(recordDL[4]) * gValue;
                        gLoad.F3 = double.Parse(recordDL[5]) * gValue;
                        //
                        step.AddLoad(gLoad);
                    }
                    else _errors.Add("Failed to import distributed load: " + lines.ToRows());
                }
            }
            catch
            {
                _errors.Add("Failed to import gravity load: " + lines.ToRows());
            }
        }
        private static string GetAmplitudeName(string line)
        {
            //
            string amplitudeName = null;
            string[] record1 = line.Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
            string[] record2;
            //
            foreach (var rec in record1)
            {
                record2 = rec.Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                if (record2.Length != 2) continue;
                else if (record2[0].Trim().ToUpper() == "AMPLITUDE") amplitudeName = record2[1].Trim();
            }
            //
            return amplitudeName;
        }
        // Field output
        private static void AddStepNodalFieldOutput(Step step, string[] lines)
        {
            // *Node file, Frequency=2
            // U, RF, NT
            try
            {
                string name = step.FieldOutputs.GetNextNumberedKey("NF-Output");
                NodalFieldVariable variables;
                int? frequency = null;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (NodalFieldVariable)Enum.Parse(typeof(NodalFieldVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (NodalFieldVariable)Enum.Parse(typeof(NodalFieldVariable), record1[i].ToUpper());
                    }
                    NodalFieldOutput nodalFieldOutput = new NodalFieldOutput(name, variables);
                    if (frequency != null) nodalFieldOutput.Frequency = (int)frequency;
                    // Add to step
                    step.FieldOutputs.Add(name, nodalFieldOutput);
                }
            }
            catch
            {
                _errors.Add("Failed to import nodal field output: " + lines.ToRows());
            }
        }
        private static void AddStepElementFieldOutput(Step step, string[] lines)
        {
            // *Element file, Frequency=2
            // S, E
            try
            {
                string name = step.FieldOutputs.GetNextNumberedKey("EF-Output");
                ElementFieldVariable variables;
                int? frequency = null;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (ElementFieldVariable)Enum.Parse(typeof(ElementFieldVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (ElementFieldVariable)Enum.Parse(typeof(ElementFieldVariable), record1[i].ToUpper());
                    }
                    ElementFieldOutput elementFieldOutput = new ElementFieldOutput(name, variables);
                    if (frequency != null) elementFieldOutput.Frequency = (int)frequency;
                    // Add to step
                    step.FieldOutputs.Add(name, elementFieldOutput);
                }
            }
            catch
            {
                _errors.Add("Failed to import element field output: " + lines.ToRows());
            }
        }
        private static void AddStepContactFieldOutput(Step step, string[] lines)
        {
            // *Contact file, Frequency=2
            // CDIS, CSTR
            try
            {
                string name = step.FieldOutputs.GetNextNumberedKey("CF-Output");
                ContactFieldVariable variables;
                int? frequency = null;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (ContactFieldVariable)Enum.Parse(typeof(ContactFieldVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (ContactFieldVariable)Enum.Parse(typeof(ContactFieldVariable), record1[i].ToUpper());
                    }
                    ContactFieldOutput contactFieldOutput = new ContactFieldOutput(name, variables);
                    if (frequency != null) contactFieldOutput.Frequency = (int)frequency;
                    // Add to step
                    step.FieldOutputs.Add(name, contactFieldOutput);
                }
            }
            catch
            {
                _errors.Add("Failed to import contact field output: " + lines.ToRows());
            }
        }
        // History output
        private static void AddStepNodalHistoryOutput(Step step, string[] lines)
        {
            //*Node print, Nset=Internal_Selection-7_Fz, Totals=Only, Totals=Yes
            // U, RF, NT
            try
            {
                string name = step.HistoryOutputs.GetNextNumberedKey("NH-Output");
                NodalHistoryVariable variables;
                string regionName = null;
                int? frequency = null;
                RegionTypeEnum regionType = RegionTypeEnum.None;
                TotalsTypeEnum totalsType = TotalsTypeEnum.No;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("NSET"))
                    {
                        regionName = record2[1];
                        regionType = RegionTypeEnum.NodeSetName;
                    }
                    else if (record2[0].ToUpper().StartsWith("TOTALS"))
                    {
                        totalsType = (TotalsTypeEnum)Enum.Parse(typeof(TotalsTypeEnum), record2[1]);
                    }
                    else if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (NodalHistoryVariable)Enum.Parse(typeof(NodalHistoryVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (NodalHistoryVariable)Enum.Parse(typeof(NodalHistoryVariable), record1[i].ToUpper());
                    }
                    if (regionName != null)
                    {
                        NodalHistoryOutput nodalHistoryOutput = new NodalHistoryOutput(name, variables, regionName, regionType);
                        if (frequency != null) nodalHistoryOutput.Frequency = (int)frequency;
                        if (totalsType != TotalsTypeEnum.No) nodalHistoryOutput.TotalsType = totalsType;
                        // Add to step
                        step.HistoryOutputs.Add(name, nodalHistoryOutput);
                    }
                }
            }
            catch
            {
                _errors.Add("Failed to import nodal history output: " + lines.ToRows());
            }
        }
        private static void AddStepElementHistoryOutput(Step step, string[] lines)
        {
            //*El print, Elset=Internal_Selection-7_Fz, Totals=Only, Totals=Yes
            // S, E, ME, PEEQ
            try
            {
                string name = step.HistoryOutputs.GetNextNumberedKey("EH-Output");
                ElementHistoryVariable variables;
                string regionName = null;
                int? frequency = null;
                RegionTypeEnum regionType = RegionTypeEnum.None;
                TotalsTypeEnum totalsType = TotalsTypeEnum.No;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("ELSET"))
                    {
                        regionName = record2[1];
                        regionType = RegionTypeEnum.ElementSetName;
                    }
                    else if (record2[0].ToUpper().StartsWith("TOTALS"))
                    {
                        totalsType = (TotalsTypeEnum)Enum.Parse(typeof(TotalsTypeEnum), record2[1]);
                    }
                    else if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (ElementHistoryVariable)Enum.Parse(typeof(ElementHistoryVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (ElementHistoryVariable)Enum.Parse(typeof(ElementHistoryVariable), record1[i].ToUpper());
                    }
                    if (regionName != null)
                    {
                        ElementHistoryOutput elementHistoryOutput = new ElementHistoryOutput(name, variables, regionName,
                                                                                             regionType);
                        if (frequency != null) elementHistoryOutput.Frequency = (int)frequency;
                        if (totalsType != TotalsTypeEnum.No) elementHistoryOutput.TotalsType = totalsType;
                        // Add to step
                        step.HistoryOutputs.Add(name, elementHistoryOutput);
                    }
                }
            }
            catch
            {
                _errors.Add("Failed to import element history output: " + lines.ToRows());
            }
        }
        private static void AddStepContactHistoryOutput(Step step, string[] lines,
                                                        OrderedDictionary<string, ContactPair> contactPairs)
        {
            //*Contact print, Elset=Internal_Selection-7_Fz, Totals=Only, Totals=Yes
            // S, E, ME, PEEQ
            try
            {
                string name = step.HistoryOutputs.GetNextNumberedKey("CH-Output");
                ContactHistoryVariable variables;
                string masterName = null;
                string slaveName = null;
                int? frequency = null;
                TotalsTypeEnum totalsType = TotalsTypeEnum.No;
                string[] record1;
                string[] record2;
                // Line 1
                record1 = lines[0].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rec in record1)
                {
                    record2 = rec.Trim().Split(_splitterEqual, StringSplitOptions.RemoveEmptyEntries);
                    if (record2[0].ToUpper().StartsWith("MASTER"))
                    {
                        masterName = record2[1];
                        //regionType = RegionTypeEnum.ElementSetName;
                    }
                    else if (record2[0].ToUpper().StartsWith("SLAVE"))
                    {
                        slaveName = record2[1];
                        //regionType = RegionTypeEnum.ElementSetName;
                    }
                    else if (record2[0].ToUpper().StartsWith("TOTALS"))
                    {
                        totalsType = (TotalsTypeEnum)Enum.Parse(typeof(TotalsTypeEnum), record2[1]);
                    }
                    else if (record2[0].ToUpper().StartsWith("FREQUENCY"))
                    {
                        frequency = int.Parse(record2[1]);
                    }
                }
                // Line 2
                record1 = lines[1].Split(_splitterComma, StringSplitOptions.RemoveEmptyEntries);
                if (record1.Length > 0)
                {
                    variables = (ContactHistoryVariable)Enum.Parse(typeof(ContactHistoryVariable), record1[0].ToUpper());
                    for (int i = 1; i < record1.Length; i++)
                    {
                        variables |= (ContactHistoryVariable)Enum.Parse(typeof(ContactHistoryVariable), record1[i].ToUpper());
                    }
                    // Find the contact pair
                    if (masterName != null && slaveName != null)
                    {
                        string contactPairName = null;
                        foreach (var entry in contactPairs)
                        {
                            if (entry.Value.MasterRegionName == masterName && entry.Value.SlaveRegionName == slaveName)
                            {
                                contactPairName = entry.Key;
                                break;
                            }
                        }
                        if (contactPairName != null)
                        {
                            ContactHistoryOutput contactHistoryOutput = new ContactHistoryOutput(name, variables, contactPairName);
                            if (frequency != null) contactHistoryOutput.Frequency = (int)frequency;
                            if (totalsType != TotalsTypeEnum.No) contactHistoryOutput.TotalsType = totalsType;
                            // Add to step
                            step.HistoryOutputs.Add(name, contactHistoryOutput);
                        }
                    }
                }
            }
            catch
            {
                _errors.Add("Failed to import contact history output: " + lines.ToRows());
            }
        }

        //  LINEAR ELEMENTS                                                                                                         
        private static LinearTriangleElement GetLinearTriangleElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 598, 1368, 1306
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(3, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new LinearTriangleElement(id, nodeIds);
        }
        private static LinearQuadrilateralElement GetLinearQuadrilateralElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 598, 1368, 1306, 16
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(4, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new LinearQuadrilateralElement(id, nodeIds);
        }
        private static LinearTetraElement GetLinearTetraElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 598, 1368, 1306, 1291
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(4, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new LinearTetraElement(id, nodeIds);
        }
        private static LinearWedgeElement GetLinearWedgeElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1,  132,  133,  495,  646,  647, 1009
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(6, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new LinearWedgeElement(id, nodeIds);
        }
        private static LinearHexaElement GetLinearHexaElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 598, 1368, 1306, 1291, 3, 4, 5, 6
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(8, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new LinearHexaElement(id, nodeIds);
        }


        //  PARABOLIC ELEMENTS                                                                                                      
        private static ParabolicTriangleElement GetParabolicTriangleElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 598, 1368, 1306, 1291, 15 ,16
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(6, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new ParabolicTriangleElement(id, nodeIds);
        }
        private static ParabolicQuadrilateralElement GetParabolicQuadrilateralElement(ref int lineId, string[] lines,
                                                                                      string[] splitter)
        {
            // 1, 5518, 5519, 4794, 5898, 19815, 19819, 19817, 
            // 19816
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(8, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new ParabolicQuadrilateralElement(id, nodeIds);
        }
        private static ParabolicTetraElement GetParabolicTetraElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 5518, 5519, 4794, 5898, 19815, 19819, 19817, 
            // 19816, 19818, 19820
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(10, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new ParabolicTetraElement(id, nodeIds);
        }
        private static ParabolicWedgeElement GetParabolicWedgeElement(ref int lineId, string[] lines, string[] splitter)
        {
            // 1, 5518, 5519, 4794, 5898, 19815, 19819, 19817, 
            // 19816, 19818, 19820, 1, 2, 3, 4, 5
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(15, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new ParabolicWedgeElement(id, nodeIds);
        }
        private static ParabolicHexaElement GetParabolicHexaElement(ref int lineId, string[] lines, string[] splitter)
        {
            // *ELEMENT, TYPE = C3D20R
            // 1,1,10,47,19,37,57,78,72,9,45,
            // 46,20,56,76,77,73,38,55,75,70
            int id;
            int[] nodeIds;
            GetElementIdAndNodeIds(20, ref lineId, lines, splitter, out id, out nodeIds);
            //
            return new ParabolicHexaElement(id, nodeIds);
        }
        private static void GetElementIdAndNodeIds(int numOfNodes, ref int lineId, string[] lines,
                                                   string[] splitter, out int elementId, out int[] nodeIds)
        {
            int numOfRecords = numOfNodes + 1;
            string[] splitStrings = new string[numOfRecords];
            //
            int count = 0;
            string[] record;
            while (count < numOfRecords)
            {
                record = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                //
                for (int i = 0; i < record.Length; i++)
                {
                    splitStrings[count++] = record[i];
                }
                lineId++;
            }
            lineId--;   // set the lineId to the last line
            // Element id
            elementId = int.Parse(splitStrings[0]);
            // Node ids
            nodeIds = new int[numOfNodes];
            for (int i = 1; i < splitStrings.Length; i++) nodeIds[i - 1] = int.Parse(splitStrings[i]);
        }
        //
        private static void AddError(string error)
        {
            Errors.Add(error);
            WriteDataToOutputStatic(error);
        }










    }
}
