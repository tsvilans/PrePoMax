using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using System.IO;
using FileInOut.Output.Calculix;
using CaeGlobals;
using Microsoft.SqlServer.Server;

namespace FileInOut.Output
{
    [Serializable]
    public static class CalculixFileWriter
    {
        // Methods                                                                                                                  
        static public void Write(string fileName, FeModel model, Dictionary<int, double[]> deformations = null)
        {
            List<CalculixKeyword> keywords = GetAllKeywords(model, deformations);
            // Write file
            StringBuilder sb = new StringBuilder();
            foreach (var keyword in keywords)
            {
                WriteKeywordRecursively(sb, keyword);
            }
            File.WriteAllText(fileName, sb.ToString());
        }
        static public void WriteMaterials(string fileName, FeModel model, string[] materialNames)
        {
            List<CalculixKeyword> keywords = new List<CalculixKeyword>();
            // Heading
            CalTitle title = new CalTitle("Heading", "");
            keywords.Add(title);
            AppendHeading(model, title);
            // Materials
            title = new CalTitle("Materials", "");
            keywords.Add(title);
            AppendMaterials(model, title, materialNames);
            // Write file
            StringBuilder sb = new StringBuilder();
            foreach (var keyword in keywords)
            {
                WriteKeywordRecursively(sb, keyword);
            }
            File.WriteAllText(fileName, sb.ToString());
        }
        //
        static public List<CalculixKeyword> GetAllKeywords(FeModel model, Dictionary<int, double[]> deformations = null)
        {
            List<CalculixKeyword> keywords = GetModelKeywords(model, deformations);
            // Add user keywords
            if (model.CalculixUserKeywords != null)
            {
                foreach (var entry in model.CalculixUserKeywords)
                {
                    // Deep clone to prevent the changes in user keywords
                    AddUserKeywordByIndices(keywords, entry.Key, entry.Value.DeepClone());
                }
            }
            //
            return keywords;
        }
        static public List<CalculixKeyword> GetModelKeywords(FeModel model, Dictionary<int, double[]> deformations = null)
        {
            // Only keywords from the model, not user keywords
            // Allways add a title keyword to get all possible keyword types to the keyword editor
            //
            // Collect pre-tension loads
            string name;
            List<PreTensionLoad> preTensionLoadsList;
            OrderedDictionary<string, List<PreTensionLoad>> preTensionLoads =
                new OrderedDictionary<string, List<PreTensionLoad>>("Pretension loads", StringComparer.OrdinalIgnoreCase);
            foreach (var step in model.StepCollection.StepsList)
            {
                foreach (var entry in step.Loads)
                {
                    if (entry.Value is PreTensionLoad ptl)
                    {
                        name = ptl.SurfaceName;
                        if (!ptl.AutoComputeDirection) name += "@" + ptl.X.ToString() + ptl.Y.ToString() + ptl.Z.ToString();
                        //
                        if (preTensionLoads.TryGetValue(name, out preTensionLoadsList)) preTensionLoadsList.Add(ptl);
                        else preTensionLoads.Add(name, new List<PreTensionLoad>() { ptl });
                    }
                }
            }
            // Prepare reference points
            Dictionary<string, int[]> referencePointsNodeIds = new Dictionary<string, int[]>();
            if (model.Mesh != null)
            {
                // Fill reference point nodes
                int id = model.Mesh.MaxNodeId;
                foreach (var entry in model.Mesh.ReferencePoints)
                {
                    referencePointsNodeIds.Add(entry.Key, new int[] { id + 1, id + 2 });
                    id += 2;
                }
                foreach (var entry in preTensionLoads)
                {
                    referencePointsNodeIds.Add(entry.Key, new int[] { id + 1 });
                    id++;
                }
            }
            // Prepare point springs
            List<CalElement> springElements;
            List<CalElementSet> springElementSets;
            List<CalLinearSpringSection> linearSpringSections;
            GetPointSprings(model, referencePointsNodeIds, out springElements, out springElementSets, out linearSpringSections);
            //
            CalTitle title;
            List<CalculixKeyword> keywords = new List<CalculixKeyword>();
            // Heading
            title = new CalTitle("Heading", "");
            keywords.Add(title);
            AppendHeading(model, title);
            // Submodel
            string[] nodeSetNames = GetAllSubmodelNodeSetNames(model);
            if (nodeSetNames.Length > 0)
            {
                title = new CalTitle("Submodel", "");
                keywords.Add(title);
                AppendSubmodel(model, nodeSetNames, title);
            }
            // Nodes
            title = new CalTitle("Nodes", "");
            keywords.Add(title);
            AppendNodes(model, referencePointsNodeIds, deformations, title);
            // Elements
            title = new CalTitle("Elements", "");
            keywords.Add(title);
            AppendElements(model, springElements, title);
            // Distributions
            title = new CalTitle("Distributions", "");
            keywords.Add(title);
            AppendDistributions(model, null, title);
            // Orientations
            title = new CalTitle("Orientations", "");
            keywords.Add(title);
            AppendOrientations(model, null, title);
            // Node sets
            title = new CalTitle("Node sets", "");
            keywords.Add(title);
            AppendNodeSets(model, referencePointsNodeIds, title);
            // Element sets
            title = new CalTitle("Element sets", "");
            keywords.Add(title);
            AppendElementSets(model, springElementSets, title);
            // Surfaces
            title = new CalTitle("Surfaces", "");
            keywords.Add(title);
            AppendSurfaces(model, title);
            // Physical constants
            title = new CalTitle("Physical constants", "");
            keywords.Add(title);
            AppendPhysicalConstants(model, title);
            // Materials
            title = new CalTitle("Materials", "");
            keywords.Add(title);
            AppendMaterials(model, title);
            // Sections
            title = new CalTitle("Sections", "");
            keywords.Add(title);
            AppendSections(model, linearSpringSections, title);
            // Pre-tension sections
            title = new CalTitle("Pre-tension sections", "");
            keywords.Add(title);
            AppendPreTensionSections(preTensionLoads, referencePointsNodeIds, title);
            // Constraints
            title = new CalTitle("Constraints", "");
            keywords.Add(title);
            AppendConstraints(model, referencePointsNodeIds, title);
            // Surface interactions
            title = new CalTitle("Surface interactions", "");
            keywords.Add(title);
            AppendSurfaceInteractions(model, title);
            // Contact pairs
            title = new CalTitle("Contact pairs", "");
            keywords.Add(title);
            AppendContactPairs(model, title);
            // Amplitudess
            title = new CalTitle("Amplitudes", "");
            keywords.Add(title);
            AppendAmplitudes(model, title);
            // Initial conditions
            title = new CalTitle("Initial conditions", "");
            keywords.Add(title);
            AppendInitialConditions(model, title);
            // Steps
            title = new CalTitle("Steps", "");
            keywords.Add(title);
            AppendSteps(model, referencePointsNodeIds, title);
            //
            return keywords;
        }
        static private void GetPointSprings(FeModel model, Dictionary<string, int[]> referencePointsNodeIds,
                                            out List<CalElement> springElements,
                                            out List<CalElementSet> springElementSets,
                                            out List<CalLinearSpringSection> linearSpringSections)
        {
            springElements = new List<CalElement>();
            springElementSets = new List<CalElementSet>();
            linearSpringSections = new List<CalLinearSpringSection>();
            //
            if (model.Mesh != null)
            {
                int count;
                int[] elementIds;
                int[] refPointIds;
                int[] directions;
                double[] stiffnesses;
                string name;
                int elementId = model.Mesh.MaxElementId;
                FeNodeSet nodeSet;
                List<FeElement> newElements;
                List<FeElement> oneSpringElements;
                HashSet<string> elementSetNames = new HashSet<string>(model.Mesh.ElementSets.Keys);
                    
                elementSetNames.UnionWith(model.Mesh.Parts.Keys);
                // Collect point and surface springs
                Dictionary<string, PointSpring[]> activeSprings = new Dictionary<string, PointSpring[]>();
                foreach (var entry in model.Constraints)
                {
                    if (entry.Value is PointSpring ps && ps.Active)
                        activeSprings.Add(ps.Name, new PointSpring[] { ps });
                    else if (entry.Value is SurfaceSpring ss && ss.Active)
                        activeSprings.Add(ss.Name, model.GetPointSpringsFromSurfaceSpring(ss));
                }
                //
                foreach (var entry in activeSprings)
                {
                    oneSpringElements = new List<FeElement>();
                    //
                    foreach (PointSpring ps in entry.Value)
                    {
                        directions = ps.GetSpringDirections();
                        stiffnesses = ps.GetSpringStiffnessValues();
                        //
                        if (directions.Length == 0) continue;
                        //
                        for (int i = 0; i < directions.Length; i++)
                        {
                            // Name
                            name = ps.Name + "_DOF_" + directions[i];
                            if (elementSetNames.Contains(name)) name = elementSetNames.GetNextNumberedKey(name);
                            elementSetNames.Add(name);
                            //
                            newElements = new List<FeElement>();
                            // Node id
                            if (ps.RegionType == RegionTypeEnum.NodeId)
                            {
                                newElements.Add(new LinearSpringElement(elementId + 1, new int[] { ps.NodeId }));
                                elementId++;
                            }
                            // Node set
                            else if (ps.RegionType == RegionTypeEnum.NodeSetName)
                            {
                                if (model.Mesh.NodeSets.TryGetValue(ps.RegionName, out nodeSet))
                                {
                                    foreach (var label in nodeSet.Labels)
                                    {
                                        newElements.Add(new LinearSpringElement(elementId + 1, new int[] { label }));
                                        elementId++;
                                    }
                                }
                            }
                            // Reference point
                            else if (ps.RegionType == RegionTypeEnum.ReferencePointName)
                            {
                                if (referencePointsNodeIds.TryGetValue(ps.RegionName, out refPointIds))
                                {
                                    newElements.Add(new LinearSpringElement(elementId + 1, new int[] { refPointIds[0] }));
                                    elementId++;
                                }
                            }
                            else throw new NotSupportedException();
                            // Get element ids
                            count = 0;
                            elementIds = new int[newElements.Count];
                            foreach (var element in newElements) elementIds[count++] = element.Id;
                            // Add items
                            springElementSets.Add(new CalElementSet(new FeGroup(name, elementIds)));
                            linearSpringSections.Add(new CalLinearSpringSection(name, directions[i], stiffnesses[i]));
                            oneSpringElements.AddRange(newElements);
                        }
                    }
                    // Add elements in sets
                    name = entry.Key + "_All";
                    if (elementSetNames.Contains(name)) name = elementSetNames.GetNextNumberedKey(name);
                    elementSetNames.Add(name);
                    springElements.Add(new CalElement(FeElementTypeSpring.SPRING1.ToString(), name, oneSpringElements));
                }
            }
        }
        static public bool AddUserKeywordByIndices(List<CalculixKeyword> keywords, int[] indices, CalculixKeyword userKeyword)
        {
            if (indices.Length == 1)
            {
                keywords.Insert(indices[0], userKeyword);
            }
            else
            {
                bool deactivated = false;
                CalculixKeyword keywordParent = keywords[indices[0]];
                if (keywordParent is CalDeactivated) deactivated = true;
                // Find a parent
                for (int i = 1; i < indices.Length - 1; i++)
                {
                    if (indices[i] < keywordParent.Keywords.Count)
                    {
                        keywordParent = keywordParent.Keywords[indices[i]];
                        if (keywordParent is CalDeactivated) deactivated = true;
                    }
                    else return false;
                }

                // Add the keyword
                if (keywordParent.Keywords.Count < indices[indices.Length - 1]) return false;

                if (!deactivated) keywordParent.Keywords.Insert(indices[indices.Length - 1], userKeyword);
                else keywordParent.Keywords.Insert(indices[indices.Length - 1], new CalDeactivated("User keyword"));
            }
            return true;
        }
        //
        static public void RemoveLostUserKeywords(FeModel model)
        {
            List<CalculixKeyword> keywords = GetModelKeywords(model);
            // Add user keywords
            List<int[]> keywordKeysToRemove = new List<int[]>();
            if (model.CalculixUserKeywords != null)
            {
                foreach (var entry in model.CalculixUserKeywords)
                {
                    if (!AddUserKeywordByIndices(keywords, entry.Key, entry.Value.DeepClone())) keywordKeysToRemove.Add(entry.Key);
                }
            }
            // Remove lost user keywords
            foreach (var indices in keywordKeysToRemove)
            {
                model.CalculixUserKeywords.Remove(indices);
            }
        }
        //
        static public string GetShortKeywordData(CalculixKeyword keyword)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(keyword.GetKeywordString());
            if (!(keyword is CalNode) && !(keyword is CalElement) && !(keyword is CalNodeSet) && !(keyword is CalElementSet))
                sb.Append(keyword.GetDataString());
            else
            {
                //int n = 10;
                //string[] lines = keyword.GetDataString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                //for (int i = 0; i < Math.Min(n,  lines.Length); i++) sb.AppendLine(lines[i]);
                //if (lines.Length > n) sb.AppendLine("... hidden data ...");
                sb.AppendLine("... hidden data ...");
            }
            return sb.ToString();
        }
        static private void WriteKeywordRecursively(StringBuilder sb, CalculixKeyword keyword)
        {
            sb.Append(keyword.GetKeywordString());
            sb.Append(keyword.GetDataString());
            //
            foreach (var childkeyword in keyword.Keywords)
            {
                WriteKeywordRecursively(sb, childkeyword);
            }
        }
        //
        static private string[] GetAllSubmodelNodeSetNames(FeModel model)
        {
            List<string> nodeSetNames = new List<string>();
            foreach (var step in model.StepCollection.StepsList)
            {
                foreach (var entry in step.BoundaryConditions)
                {
                    if (entry.Value is SubmodelBC sm)
                    {
                        if (sm.RegionType == RegionTypeEnum.SurfaceName) 
                            nodeSetNames.Add(model.Mesh.Surfaces[sm.RegionName].NodeSetName);
                        else nodeSetNames.Add(sm.RegionName);
                    }
                } 
            }
            return nodeSetNames.ToArray();
        }
        //
        static private void AppendHeading(FeModel model, CalculixKeyword parent)
        {
            CalHeading heading = new CalHeading(model.Name, model.HashName, model.UnitSystem.UnitSystemType);
            parent.AddKeyword(heading);
        }
        static private void AppendSubmodel(FeModel model, string[] nodeSetNames, CalculixKeyword parent)
        {
            //*Submodel, TYPE = NODE, INPUT = Model.frd
            CalSubmodel submodel = new CalSubmodel(model.Properties.GlobalResultsFileName, nodeSetNames);
            parent.AddKeyword(submodel);
        }
        static private void AppendNodes(FeModel model, Dictionary<string, int[]> referencePointsNodeIds,
                                        Dictionary<int, double[]> deformations, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalNode node = new CalNode(model, referencePointsNodeIds, deformations);
                parent.AddKeyword(node);
            }

        }
        static private void AppendElements(FeModel model, List<CalElement> additionalElements,
                                           CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                Dictionary<string, List<FeElement>> elementTypes = new Dictionary<string, List<FeElement>>();
                List<FeElement> elements;
                //
                string type;
                FeElement element;
                MeshPart part;
                CalElement elementKeyword;
                //
                foreach (var entry in model.Mesh.Parts)
                {
                    elementTypes.Clear();
                    part = (MeshPart)entry.Value;
                    //
                    foreach (int elementId in part.Labels)
                    {
                        element = model.Mesh.Elements[elementId];
                        if (part.LinearTriaType != FeElementTypeLinearTria.None && element is LinearTriangleElement)
                            type = part.LinearTriaType.ToString();
                        else if (part.LinearQuadType != FeElementTypeLinearQuad.None && element is LinearQuadrilateralElement)
                            type = part.LinearQuadType.ToString();
                        else if (part.LinearTetraType != FeElementTypeLinearTetra.None && element is LinearTetraElement)
                            type = part.LinearTetraType.ToString();
                        else if (part.LinearWedgeType != FeElementTypeLinearWedge.None && element is LinearWedgeElement)
                            type = part.LinearWedgeType.ToString();
                        else if (part.LinearHexaType != FeElementTypeLinearHexa.None && element is LinearHexaElement)
                            type = part.LinearHexaType.ToString();
                        else if (part.ParabolicTriaType != FeElementTypeParabolicTria.None && element is ParabolicTriangleElement)
                            type = part.ParabolicTriaType.ToString();
                        else if (part.ParabolicQuadType != FeElementTypeParabolicQuad.None &&
                                 element is ParabolicQuadrilateralElement)
                            type = part.ParabolicQuadType.ToString();
                        else if (part.ParabolicTetraType != FeElementTypeParabolicTetra.None && element is ParabolicTetraElement)
                            type = part.ParabolicTetraType.ToString();
                        else if (part.ParabolicWedgeType != FeElementTypeParabolicWedge.None && element is ParabolicWedgeElement)
                            type = part.ParabolicWedgeType.ToString();
                        else if (part.ParabolicHexaType != FeElementTypeParabolicHexa.None && element is ParabolicHexaElement)
                            type = part.ParabolicHexaType.ToString();
                        else throw new NotImplementedException();
                        // Add element to the coresponding type
                        if (elementTypes.TryGetValue(type, out elements)) elements.Add(element);
                        else elementTypes.Add(type, new List<FeElement>() { element });
                    }
                    //
                    foreach (var typeEntry in elementTypes)
                    {
                        elementKeyword = new CalElement(typeEntry.Key, part.Name, typeEntry.Value);
                        parent.AddKeyword(elementKeyword);
                    }
                }
                // Additional elements
                foreach (var additionalElementKeyword in additionalElements) parent.AddKeyword(additionalElementKeyword);
            }
        }
        static private void AppendNodeSets(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                HashSet<string> nodeSetNames = new HashSet<string>();
                CalNodeSet calNodeSet;
                foreach (var entry in model.Mesh.NodeSets)
                {
                    if (entry.Value.Active)
                    {
                        calNodeSet = new CalNodeSet(entry.Value);
                        parent.AddKeyword(calNodeSet);
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
                nodeSetNames.UnionWith(model.Mesh.NodeSets.Keys);
                // Reference points
                FeReferencePoint rp;
                FeNodeSet rpNodeSet;
                foreach (var entry in referencePointsNodeIds)
                {
                    if (model.Mesh.ReferencePoints.TryGetValue(entry.Key, out rp))
                    {
                        rp.RefNodeSetName = rp.Name + FeReferencePoint.RefName + entry.Value[0];
                        rp.RotNodeSetName = rp.Name + FeReferencePoint.RotName + entry.Value[1];
                        // Check name
                        if (nodeSetNames.Contains(rp.RefNodeSetName))
                        {
                            rp.RefNodeSetName = nodeSetNames.GetNextNumberedKey(rp.RefNodeSetName);
                            nodeSetNames.Add(rp.RefNodeSetName);
                        }
                        if (nodeSetNames.Contains(rp.RotNodeSetName))
                        {
                            rp.RotNodeSetName = nodeSetNames.GetNextNumberedKey(rp.RotNodeSetName);
                            nodeSetNames.Add(rp.RotNodeSetName);
                        }
                        //
                        rpNodeSet = new FeNodeSet(rp.RefNodeSetName, new int[] { entry.Value[0] });
                        calNodeSet = new CalNodeSet(rpNodeSet);
                        parent.AddKeyword(calNodeSet);
                        //
                        rpNodeSet = new FeNodeSet(rp.RotNodeSetName, new int[] { entry.Value[1] });
                        calNodeSet = new CalNodeSet(rpNodeSet);
                        parent.AddKeyword(calNodeSet);
                    }
                }
                // Initial conditions
                FeNodeSet nodeSet;
                foreach (var entry in model.InitialConditions)
                {
                    if (entry.Value is InitialVelocity iv && iv.Active)
                    {
                        nodeSet = model.Mesh.GetNodeSetFromPartOrElementSetName(iv.RegionName, false);
                        // Check name
                        if (nodeSetNames.Contains(nodeSet.Name))
                        {
                            nodeSet.Name = nodeSetNames.GetNextNumberedKey(nodeSet.Name);
                            nodeSetNames.Add(nodeSet.Name);
                        }
                        // Add temp name
                        iv.NodeSetName = nodeSet.Name;
                        //
                        calNodeSet = new CalNodeSet(nodeSet);
                        parent.AddKeyword(calNodeSet);
                    }
                }
            }
        }
        static private void AppendParts(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalElementSet part;
                foreach (var entry in model.Mesh.Parts)
                {
                    if (entry.Value.Active)
                    {
                        part = new CalElementSet(entry.Value, model);
                        parent.AddKeyword(part);
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendElementSets(FeModel model, List<CalElementSet> additionalElementSets, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalElementSet elementSet;
                foreach (var entry in model.Mesh.ElementSets)
                {
                    if (entry.Value.Active)
                    {
                        elementSet = new CalElementSet(entry.Value, model);
                        parent.AddKeyword(elementSet);
                    }
                }
                // Additional element sets
                foreach (var additionalElementSet in additionalElementSets)
                    parent.AddKeyword(additionalElementSet);
            }
        }

        static private void AppendDistributions(FeModel model, List<CalDistribution> additionalDistributions, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalDistribution distribution;
                foreach (var entry in model.Mesh.Distributions)
                {
                    distribution = new CalDistribution(entry.Value, model);
                    parent.AddKeyword(distribution);
                }

                if (additionalDistributions != null)
                    foreach (var additionalDistribution in additionalDistributions)
                        parent.AddKeyword(additionalDistribution);
            }
        }

        static private void AppendOrientations(FeModel model, List<CalOrientation> additionalOrientations, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalOrientation orientation;
                foreach(var entry in model.Mesh.Orientations)
                {
                    orientation = new CalOrientation(entry.Value);
                    parent.AddKeyword(orientation);
                }

                if (additionalOrientations != null)
                    foreach (var additionalOrientation in additionalOrientations)
                        parent.AddKeyword(additionalOrientation);

            }
        }
        static private void AppendSurfaces(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                CalSurface surface;
                foreach (var entry in model.Mesh.Surfaces)
                {
                    if (entry.Value.Active && entry.Value.ElementFaces != null)
                    {
                        surface = new CalSurface(entry.Value, model.Properties.ModelSpace.IsTwoD());
                        parent.AddKeyword(surface);
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }



        static private void AppendMaterials(FeModel model, CalculixKeyword parent, string[] materialNames = null)
        {
            CalMaterial material;
            HashSet<string> activeMaterialNames = MaterialNamesUsedInActiveSections(model);
            //
            foreach (var entry in model.Materials)
            {
                if ((entry.Value.Active && activeMaterialNames.Contains(entry.Key)) ||
                    (materialNames != null && materialNames.Contains(entry.Value.Name)))
                {
                    material = new CalMaterial(entry.Value);
                    parent.AddKeyword(material);
                    //
                    foreach (var property in entry.Value.Properties)
                    {
                        if (property is Density de)
                        {
                            material.AddKeyword(new CalDensity(de, entry.Value.TemperatureDependent));
                        }
                        else if (property is SlipWear sw)
                        {
                            if (materialNames != null)  // must be here
                                material.AddKeyword(new CalSlipWear(sw, entry.Value.TemperatureDependent));
                        }
                        else if (property is Elastic el)
                        {
                            material.AddKeyword(new CalElastic(el, entry.Value.TemperatureDependent));
                        }
                        else if (property is ElasticWithDensity ewd)
                        {
                            Density density = new Density(new double[][] { new double[] { ewd.Density } });
                            material.AddKeyword(new CalDensity(density, entry.Value.TemperatureDependent));
                            //
                            Elastic elastic = new Elastic(new double[][] { new double[] { ewd.YoungsModulus, ewd.PoissonsRatio } });
                            material.AddKeyword(new CalElastic(elastic, entry.Value.TemperatureDependent));
                        }
                        else if (property is EngineeringConstants eng)
                        {
                            material.AddKeyword(new CalEngineeringConstants(eng, entry.Value.TemperatureDependent));
                        }
                        else if (property is Plastic pl)
                        {
                            material.AddKeyword(new CalPlastic(pl, entry.Value.TemperatureDependent));
                        }
                        else if (property is ThermalExpansion te)
                        {
                            material.AddKeyword(new CalThermalExpansion(te, entry.Value.TemperatureDependent));
                        }
                        else if (property is ThermalConductivity tc)
                        {
                            material.AddKeyword(new CalThermalConductivity(tc, entry.Value.TemperatureDependent));
                        }
                        else if (property is SpecificHeat sh)
                        {
                            material.AddKeyword(new CalSpecificHeat(sh, entry.Value.TemperatureDependent));
                        }
                        else throw new NotImplementedException();
                    }
                }
                else if (materialNames == null) parent.AddKeyword(new CalDeactivated(entry.Value.Name));
            }
            
        }
        static private void AppendSections(FeModel model, List<CalLinearSpringSection> linearSpringSections,
                                           CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.Sections)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is SolidSection ss) parent.AddKeyword(new CalSolidSection(ss));                        
                        else if (entry.Value is ShellSection shs) parent.AddKeyword(new CalShellSection(shs));
                        else if (entry.Value is MembraneSection ms) parent.AddKeyword(new CalMembraneSection(ms));
                        else throw new NotImplementedException();
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
                //
                foreach (var calLinearSpringSection in linearSpringSections)
                {
                    parent.AddKeyword(calLinearSpringSection);
                }
            }
        }
        static private HashSet<string> MaterialNamesUsedInActiveSections(FeModel model)
        {
            HashSet<string> materialNames = new HashSet<string>();
            if (model.Mesh != null)
            {
                foreach (var entry in model.Sections)
                {
                    if (entry.Value.Active) materialNames.Add(entry.Value.MaterialName);
                }
            }
            return materialNames;
        }
        static private void AppendPreTensionSections(OrderedDictionary<string, List<PreTensionLoad>> preTensionLoads,
                                                     Dictionary<string, int[]> referencePointsNodeIds,
                                                     CalculixKeyword parent)
        {
            if (preTensionLoads != null)
            {
                int nodeId;
                bool atLeastOneActive;
                PreTensionLoad ptl;
                foreach (var preTensionOnSurfaceEntry in preTensionLoads)
                {
                    atLeastOneActive = false;
                    foreach (var item in preTensionOnSurfaceEntry.Value) atLeastOneActive |= item.Active;
                    // Take the first one since all the others are the same
                    ptl = preTensionOnSurfaceEntry.Value[0];
                    if (atLeastOneActive)
                    {
                        nodeId = referencePointsNodeIds[preTensionOnSurfaceEntry.Key][0];
                        CalPreTensionSection preTension;
                        if (ptl.AutoComputeDirection) preTension = new CalPreTensionSection(ptl.SurfaceName, nodeId);
                        else preTension = new CalPreTensionSection(ptl.SurfaceName, nodeId, ptl.X, ptl.Y, ptl.Z);
                        parent.AddKeyword(preTension);
                    }
                    else parent.AddKeyword(new CalDeactivated("Pre-tension " + ptl.SurfaceName));
                }
            }
        }
        static private void AppendConstraints(FeModel model, Dictionary<string, int[]> referencePointsNodeIds,
                                              CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.Constraints)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is PointSpring) { } // this contraint is split into elements and a section
                        else if (entry.Value is SurfaceSpring) { } // this contraint is split into point springs
                        else if (entry.Value is RigidBody rb)
                        {
                            string surfaceNodeSetName = null;
                            if (rb.RegionType == RegionTypeEnum.SurfaceName)
                                surfaceNodeSetName = model.Mesh.Surfaces[rb.RegionName].NodeSetName;
                            CalRigidBody calRigidBody = new CalRigidBody(rb, referencePointsNodeIds, surfaceNodeSetName);
                            parent.AddKeyword(calRigidBody);
                        }
                        else if (entry.Value is Tie tie)
                        {
                            CalTie calTie = new CalTie(tie);
                            parent.AddKeyword(calTie);
                        }
                        else throw new NotImplementedException();
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendSurfaceInteractions(FeModel model, CalculixKeyword parent)
        {
            CalSurfaceInteraction surfaceInteraction;
            foreach (var entry in model.SurfaceInteractions)
            {
                if (entry.Value.Active)
                {
                    surfaceInteraction = new CalSurfaceInteraction(entry.Value);
                    parent.AddKeyword(surfaceInteraction);
                    //
                    foreach (var property in entry.Value.Properties)
                    {
                        if (property is SurfaceBehavior sb) surfaceInteraction.AddKeyword(new CalSurfaceBehavior(sb));
                        else if (property is Friction fr) surfaceInteraction.AddKeyword(new CalFriction(fr));
                        else if (property is GapConductance gc) surfaceInteraction.AddKeyword(new CalGapConductance(gc));
                        else throw new NotImplementedException();
                    }
                }
                else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
            }
        }
        static private void AppendContactPairs(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.ContactPairs)
                {
                    if (entry.Value.Active)
                    {
                        CalContactPair calContactPair = new CalContactPair(entry.Value);
                        parent.AddKeyword(calContactPair);
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendAmplitudes(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.Amplitudes)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is Amplitude a)
                        {
                            CalAmplitude calAmplitude = new CalAmplitude(a);
                            parent.AddKeyword(calAmplitude);
                        }
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendInitialConditions(FeModel model, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                foreach (var entry in model.InitialConditions)
                {
                    if (entry.Value.Active)
                    {
                        if (entry.Value is InitialTemperature it)
                        {
                            CalInitialTemperature calInitialTemperature = new CalInitialTemperature(model, it);
                            parent.AddKeyword(calInitialTemperature);
                        }
                        else if (entry.Value is InitialVelocity iv)
                        {
                            CalInitialVelocity calInitialVelocity = new CalInitialVelocity(model, iv);
                            parent.AddKeyword(calInitialVelocity);
                        }
                    }
                    else parent.AddKeyword(new CalDeactivated(entry.Value.Name));
                }
            }
        }
        static private void AppendPhysicalConstants(FeModel model, CalculixKeyword parent)
        {
            if (model != null)
            {
                CalPhysicalConstants calPhysicalConstants = new CalPhysicalConstants(model.Properties);
                if (calPhysicalConstants.GetKeywordString().Length > 0) parent.AddKeyword(calPhysicalConstants);
            }
        }
        //
        static private void AppendSteps(FeModel model, Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            CalTitle title;
            HashSet<Type> loadTypes = model.StepCollection.GetAllLoadTypes();
            //
            foreach (var step in model.StepCollection.StepsList)
            {
                if (step is InitialStep) continue;
                else if (step is BoundaryDisplacementStep boundaryDisplacementStep) continue;
                //
                title = new CalTitle(step.Name, "");
                parent.AddKeyword(title);
                //
                CalculixKeyword calStep;
                if (step.Active) calStep = new CalStep(step);
                else calStep = new CalDeactivated(step.Name);
                title.AddKeyword(calStep);
                // Step type
                if (step.Active)
                {
                    if (step.GetType() == typeof(StaticStep) || step is SlipWearStep)
                    {
                        StaticStep staticStep = step as StaticStep;
                        CalStaticStep calStaticStep = new CalStaticStep(staticStep);
                        calStep.AddKeyword(calStaticStep);
                    }
                    else if (step is FrequencyStep frequencyStep)
                    {
                        CalFrequencyStep calFrequencyStep = new CalFrequencyStep(frequencyStep);
                        calStep.AddKeyword(calFrequencyStep);
                    }
                    else if (step is BuckleStep buckleStep)
                    {
                        CalBuckleStep calBuckleStep = new CalBuckleStep(buckleStep);
                        calStep.AddKeyword(calBuckleStep);
                    }
                    else if (step.GetType() == typeof(DynamicStep))
                    {
                        DynamicStep dynamicStep = step as DynamicStep;
                        CalDynamicStep calDynamicStep = new CalDynamicStep(dynamicStep);
                        calStep.AddKeyword(calDynamicStep);
                    }
                    else if (step.GetType() == typeof(HeatTransferStep))
                    {
                        HeatTransferStep heatTransferStep = step as HeatTransferStep;
                        CalHeatTransferStep calHeatTransferStep = new CalHeatTransferStep(heatTransferStep);
                        calStep.AddKeyword(calHeatTransferStep);
                    }
                    else if (step.GetType() == typeof(UncoupledTempDispStep))
                    {
                        UncoupledTempDispStep uncoupledTempDispStep = step as UncoupledTempDispStep;
                        CalUncoupledTempDispStep calUncoupledTempDispStep = new CalUncoupledTempDispStep(uncoupledTempDispStep);
                        calStep.AddKeyword(calUncoupledTempDispStep);
                    }
                    else if (step.GetType() == typeof(CoupledTempDispStep))
                    {
                        CoupledTempDispStep coupledTempDispStep = step as CoupledTempDispStep;
                        CalCoupledTempDispStep calCoupledTempDispStep = new CalCoupledTempDispStep(coupledTempDispStep);
                        calStep.AddKeyword(calCoupledTempDispStep);
                    }
                    else throw new NotImplementedException();
                }
                else calStep.AddKeyword(new CalDeactivated(step.GetType().ToString()));
                //
                // Boundary conditions
                if (step.Active) title = new CalTitle("Boundary conditions", "*Boundary, op=New");
                else title = new CalTitle("Boundary conditions", "");
                calStep.AddKeyword(title);
                //
                foreach (var bcEntry in step.BoundaryConditions)
                {
                    if (step.Active && bcEntry.Value.Active)
                        AppendBoundaryCondition(model, bcEntry.Value, referencePointsNodeIds, title);
                    else title.AddKeyword(new CalDeactivated(bcEntry.Value.Name));
                }
                // Loads
                if (step.Active)
                {
                    string data = "";
                    if (step.IsLoadTypeSupported(typeof(CLoad))) data += "*Cload, op=New";
                    if (step.IsLoadTypeSupported(typeof(DLoad)))
                    {
                        if (data.Length > 0) data += Environment.NewLine;
                        data += "*Dload, op=New";
                    }
                    if (step.IsLoadTypeSupported(typeof(CFlux)))
                    {
                        if (data.Length > 0) data += Environment.NewLine;
                        data += "*Cflux, op=New";
                    }
                    if (step.IsLoadTypeSupported(typeof(DFlux)) && loadTypes.Contains(typeof(DFlux)))
                    {
                        if (data.Length > 0) data += Environment.NewLine;
                        data += "*Dflux, op=New";
                    }
                    if (step.IsLoadTypeSupported(typeof(FilmHeatTransfer)) && loadTypes.Contains(typeof(FilmHeatTransfer)))
                    {
                        if (data.Length > 0) data += Environment.NewLine;
                        data += "*Film, op=New";
                    }
                    if (step.IsLoadTypeSupported(typeof(RadiationHeatTransfer)) &&
                        loadTypes.Contains(typeof(RadiationHeatTransfer)))
                    {
                        if (data.Length > 0) data += Environment.NewLine;
                        data += "*Radiate, op=New";
                    }
                    title = new CalTitle("Loads", data);
                }
                else title = new CalTitle("Loads", "");
                calStep.AddKeyword(title);
                //
                foreach (var loadEntry in step.Loads)
                {
                    if (step.Active && loadEntry.Value.Active) AppendLoad(model, loadEntry.Value, referencePointsNodeIds, title);
                    else title.AddKeyword(new CalDeactivated(loadEntry.Value.Name));
                }
                // Defined fields
                if (step.Active && step.DefinedFields.Count > 0) title = new CalTitle("Defined fields", "*Temperature, op=New");
                else title = new CalTitle("Defined fields", "");
                calStep.AddKeyword(title);
                //
                foreach (var definedFieldEntry in step.DefinedFields)
                {
                    if (step.Active && definedFieldEntry.Value.Active) AppendDefinedField(model, definedFieldEntry.Value, title);
                    else title.AddKeyword(new CalDeactivated(definedFieldEntry.Value.Name));
                }
                // History outputs
                title = new CalTitle("History outputs", "");
                calStep.AddKeyword(title);
                //
                foreach (var historyOutputEntry in step.HistoryOutputs)
                {
                    if (step.Active && historyOutputEntry.Value.Active) AppendHistoryOutput(model, historyOutputEntry.Value, title);
                    else title.AddKeyword(new CalDeactivated(historyOutputEntry.Value.Name));
                }
                // Field outputs
                title = new CalTitle("Field outputs", "");
                calStep.AddKeyword(title);
                //
                foreach (var fieldOutputEntry in step.FieldOutputs)
                {
                    if (step.Active && fieldOutputEntry.Value.Active) AppendFieldOutput(model, fieldOutputEntry.Value, title);
                    else title.AddKeyword(new CalDeactivated(fieldOutputEntry.Value.Name));
                }
                //
                title = new CalTitle("End step", "");
                calStep.AddKeyword(title);
                if (step.Active)
                {
                    CalEndStep endStep = new CalEndStep();
                    title.AddKeyword(endStep);
                }
                else title.AddKeyword(new CalDeactivated(step.Name));
            }
        }
        static private void AppendBoundaryCondition(FeModel model, BoundaryCondition boundaryCondition,
                                                    Dictionary<string, int[]> referencePointsNodeIds, CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                if (boundaryCondition is FixedBC fix)
                {
                    string nodeSetNameOfSurface = null;
                    if (fix.RegionType == RegionTypeEnum.SurfaceName)
                        nodeSetNameOfSurface = model.Mesh.Surfaces[fix.RegionName].NodeSetName;
                    CalFixedBC calFixedBC = new CalFixedBC(fix, referencePointsNodeIds, nodeSetNameOfSurface);
                    parent.AddKeyword(calFixedBC);
                }
                else if (boundaryCondition is DisplacementRotation dispRot)
                {
                    string nodeSetNameOfSurface = null;
                    if (dispRot.RegionType == RegionTypeEnum.SurfaceName)
                        nodeSetNameOfSurface = model.Mesh.Surfaces[dispRot.RegionName].NodeSetName;
                    CalDisplacementRotation calDisplacementRotation =
                        new CalDisplacementRotation(dispRot, referencePointsNodeIds, nodeSetNameOfSurface);
                    parent.AddKeyword(calDisplacementRotation);
                }
                else if (boundaryCondition is SubmodelBC sm)
                {
                    string surfaceNodeSetName = null;
                    if (sm.RegionType == RegionTypeEnum.SurfaceName)
                        surfaceNodeSetName = model.Mesh.Surfaces[sm.RegionName].NodeSetName;
                    CalSubmodelBC calSubmodelBC = new CalSubmodelBC(sm, surfaceNodeSetName);
                    parent.AddKeyword(calSubmodelBC);
                }
                else if (boundaryCondition is TemperatureBC tmp)
                {
                    string surfaceNodeSetName = null;
                    if (tmp.RegionType == RegionTypeEnum.SurfaceName)
                        surfaceNodeSetName = model.Mesh.Surfaces[tmp.RegionName].NodeSetName;
                    CalTemperatureBC calTemperatureBC = new CalTemperatureBC(tmp, surfaceNodeSetName);
                    parent.AddKeyword(calTemperatureBC);
                }
                else throw new NotImplementedException();
            }
        }
        static private void AppendLoad(FeModel model, Load load, Dictionary<string, int[]> referencePointsNodeIds,
                                       CalculixKeyword parent)
        {
            if (model.Mesh != null)
            {
                if (load is CLoad cl)
                {
                    CalCLoad cLoad = new CalCLoad(cl, referencePointsNodeIds);
                    parent.AddKeyword(cLoad);
                }
                else if (load is MomentLoad ml)
                {
                    CalMomentLoad mLoad = new CalMomentLoad(ml, referencePointsNodeIds);
                    parent.AddKeyword(mLoad);
                }
                else if (load is DLoad dl)
                {
                    CalDLoad dLoad = new CalDLoad(dl, model.Mesh.Surfaces[dl.SurfaceName]);
                    parent.AddKeyword(dLoad);
                }
                else if (load is HydrostaticPressure hpl)
                {
                    CalHydrostaticPressureLoad hpLoad = new CalHydrostaticPressureLoad(model, hpl);
                    parent.AddKeyword(hpLoad);
                }
                else if (load is ImportedPressure ipl)
                {
                    CalImportedPressureLoad ipLoad = new CalImportedPressureLoad(model, ipl);
                    parent.AddKeyword(ipLoad);
                }
                else if (load is STLoad stl)
                {
                    CalSTLoad stLoad = new CalSTLoad(model, stl);
                    parent.AddKeyword(stLoad);
                }
                else if (load is ShellEdgeLoad sel)
                {
                    CalShellEdgeLoad seLoad = new CalShellEdgeLoad(sel, model.Mesh.Surfaces[sel.SurfaceName]);
                    parent.AddKeyword(seLoad);
                }
                else if (load is GravityLoad gl)
                {
                    CalGravityLoad gLoad = new CalGravityLoad(gl);
                    parent.AddKeyword(gLoad);
                }
                else if (load is CentrifLoad cfl)
                {
                    CalCentrifLoad cLoad = new CalCentrifLoad(cfl);
                    parent.AddKeyword(cLoad);
                }
                else if (load is PreTensionLoad ptl)
                {
                    string name = ptl.SurfaceName;
                    if (!ptl.AutoComputeDirection) name += "@" + ptl.X.ToString() + ptl.Y.ToString() + ptl.Z.ToString();
                    //
                    CalculixKeyword calKey;
                    if (ptl.Type == PreTensionLoadType.Force)
                    {
                        int nodeId = referencePointsNodeIds[name][0];
                        CLoad cLoad = new CLoad(ptl.Name, nodeId, ptl.Magnitude, 0, 0, ptl.TwoD);
                        cLoad.AmplitudeName = ptl.AmplitudeName;
                        calKey = new CalCLoad(cLoad, referencePointsNodeIds);
                    }
                    else if (ptl.Type == PreTensionLoadType.Displacement)
                    {
                        DisplacementRotation dr = new DisplacementRotation(ptl.Name, name, RegionTypeEnum.ReferencePointName,
                                                                           ptl.TwoD);
                        dr.U1 = ptl.Magnitude;
                        dr.AmplitudeName = ptl.AmplitudeName;
                        calKey = new CalDisplacementRotation(dr, referencePointsNodeIds, null);
                    }
                    else throw new NotSupportedException();
                    //
                    parent.AddKeyword(calKey);
                }
                // Thermo
                else if (load is CFlux cf)
                {
                    CalCFlux cFlux = new CalCFlux(cf);
                    parent.AddKeyword(cFlux);
                }
                else if (load is DFlux df)
                {
                    CalDFlux dFlux = new CalDFlux(df);
                    parent.AddKeyword(dFlux);
                }
                else if (load is BodyFlux bf)
                {
                    CalBodyFlux bFlux = new CalBodyFlux(bf);
                    parent.AddKeyword(bFlux);
                }
                else if (load is FilmHeatTransfer fht)
                {
                    CalFilmHeatTransfer filmHeatTransfer = new CalFilmHeatTransfer(fht, model.Mesh.Surfaces[fht.SurfaceName]);
                    parent.AddKeyword(filmHeatTransfer);
                }
                else if (load is RadiationHeatTransfer rht)
                {
                    CalRadiationHeatTransfer radiationHeatTransfer =
                        new CalRadiationHeatTransfer(rht, model.Mesh.Surfaces[rht.SurfaceName]);
                    parent.AddKeyword(radiationHeatTransfer);
                }
                else throw new NotImplementedException();
            }
        }
        static private void AppendDefinedField(FeModel model, DefinedField definedField, CalculixKeyword parent)
        {
            if (definedField is DefinedTemperature dt)
            {
                CalDefinedTemperature definedTemperature = new CalDefinedTemperature(model, dt);
                parent.AddKeyword(definedTemperature);
            }
            else throw new NotImplementedException();
        }
        static private void AppendHistoryOutput(FeModel model, HistoryOutput historyOutput, CalculixKeyword parent)
        {
            if (historyOutput is NodalHistoryOutput nho)
            {
                CalNodePrint nodePrint;
                if (nho.RegionType == RegionTypeEnum.ReferencePointName)
                {
                    FeReferencePoint rp = model.Mesh.ReferencePoints[nho.RegionName];
                    nodePrint = new CalNodePrint(model, nho, rp.RefNodeSetName);
                    parent.AddKeyword(nodePrint);
                    nodePrint = new CalNodePrint(model, nho, rp.RotNodeSetName);
                    parent.AddKeyword(nodePrint);
                }
                else
                {
                    nodePrint = new CalNodePrint(model, nho);
                    parent.AddKeyword(nodePrint);
                }
            }
            else if (historyOutput is ElementHistoryOutput eho)
            {
                CalElPrint elPrint = new CalElPrint(eho);
                parent.AddKeyword(elPrint);
            }
            else if (historyOutput is ContactHistoryOutput cho)
            {
                ContactPair cp = model.ContactPairs[cho.RegionName];
                CalContactPrint contactPrint = new CalContactPrint(cho, cp.MasterRegionName, cp.SlaveRegionName);
                parent.AddKeyword(contactPrint);
            }
            else throw new NotImplementedException();
        }
        static private void AppendFieldOutput(FeModel model, FieldOutput fieldOutput, CalculixKeyword parent)
        {
            if (fieldOutput is NodalFieldOutput nfo)
            {
                CalNodeFile nodeFile = new CalNodeFile(nfo);
                parent.AddKeyword(nodeFile);
            }
            else if (fieldOutput is ElementFieldOutput efo)
            {
                CalElFile elFile = new CalElFile(efo);
                parent.AddKeyword(elFile);
            }
            else if (fieldOutput is ContactFieldOutput cfo)
            {
                CalContactFile conFile = new CalContactFile(cfo);
                parent.AddKeyword(conFile);
            }
            else throw new NotImplementedException();
        }
        //
        static void AppendTitle(StringBuilder sb, string title)
        {
            sb.AppendLine("************************************************************");
            sb.AppendLine("** " + title);
            sb.AppendLine("************************************************************");
        }
    }
}
