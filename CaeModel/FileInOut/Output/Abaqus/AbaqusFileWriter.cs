using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeModel;
using CaeMesh;
using CaeGlobals;

namespace FileInOut.Output
{
    [Serializable]
    public static class AbaqusFileWriter
    {
        // Methods                                                                                                                  
        static public void Write(string fileName, FeModel model)
        {
            StringBuilder sb = new StringBuilder();

            // heading
            AppendHeading(sb, model);

            sb.AppendLine("*Part, name=Part-1");
            sb.AppendLine("*End Part");
            sb.AppendLine("*Assembly, name=Assembly");
            sb.AppendLine("*Instance, name=Part-1-1, part=Part-1");
            sb.AppendLine("         0.0,          0.0,           0.");

            // mesh
            AppendNodes(sb, model);
            AppendElements(sb, model);

            AppendNodeSets(sb, model);
            AppendElementSets(sb, model);

            AppendSurfaces(sb, model);

            AppendSections(sb, model);

            sb.AppendLine("*End Instance");
            sb.AppendLine("*End Assembly");
            
            //
            AppendMaterials(sb, model);

            AppendSteps(sb, model);

            // file
            System.IO.File.WriteAllText(fileName, sb.ToString());
        }

        static private void AppendHeading(StringBuilder sb, FeModel model)
        {
            sb.AppendLine("*Heading");
            sb.AppendFormat("Model: {0},     Date: {1}", model.Name, DateTime.Now.ToShortDateString()).AppendLine();
        }

        static private void AppendNodes(StringBuilder sb, FeModel model)
        {
            FeNode node;
            Dictionary<int, FeNode> nodes = model.Mesh.Nodes;
            sb.AppendLine("*Node");
            foreach (var entry in nodes)
            {
                node = entry.Value;
                sb.AppendFormat("{0}, {1}, {2}, {3}", node.Id + 1, node.X, node.Y, node.Z).AppendLine();
            }
        }

        static private void AppendElements(StringBuilder sb, FeModel model)
        {
            Dictionary<int, FeElement> elements = model.Mesh.Elements;
            Dictionary<string, List<FeElement>> elementTypes = new Dictionary<string, List<FeElement>>();

            // get separate lists of element types
            foreach (var entry in elements)
            {
                // Shell
                if (entry.Value is LinearTriangleElement)
                {
                    if (elementTypes.ContainsKey("S3")) elementTypes["S3"].Add(entry.Value);
                    else elementTypes.Add("S3", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is LinearQuadrilateralElement)
                {
                    if (elementTypes.ContainsKey("S4")) elementTypes["S4"].Add(entry.Value);
                    else elementTypes.Add("S4", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is ParabolicTriangleElement)
                {
                    if (elementTypes.ContainsKey("STRI65")) elementTypes["STRI65"].Add(entry.Value);
                    else elementTypes.Add("STRI65", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is ParabolicQuadrilateralElement)
                {
                    if (elementTypes.ContainsKey("S8R")) elementTypes["S8R"].Add(entry.Value);
                    else elementTypes.Add("S8R", new List<FeElement>() { entry.Value });
                }
                // Solid
                else if (entry.Value is LinearTetraElement)
                {
                    if (elementTypes.ContainsKey("C3D4")) elementTypes["C3D4"].Add(entry.Value);
                    else elementTypes.Add("C3D4", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is LinearWedgeElement)
                {
                    if (elementTypes.ContainsKey("C3D6")) elementTypes["C3D6"].Add(entry.Value);
                    else elementTypes.Add("C3D6", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is LinearHexaElement)
                {
                    if (elementTypes.ContainsKey("C3D8")) elementTypes["C3D8"].Add(entry.Value);
                    else elementTypes.Add("C3D8", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is ParabolicTetraElement)
                {
                    if (elementTypes.ContainsKey("C3D10")) elementTypes["C3D10"].Add(entry.Value);
                    else elementTypes.Add("C3D10", new List<FeElement>() { entry.Value });
                }
                else if (entry.Value is ParabolicWedgeElement)
                {
                    if (elementTypes.ContainsKey("C3D15")) elementTypes["C3D15"].Add(entry.Value);
                    else elementTypes.Add("C3D15", new List<FeElement>() { entry.Value });
                }
                else
                    throw new NotImplementedException();
            }

            // write elements by type
            int count;
            foreach (var entry in elementTypes)
            {
                sb.Append("*ELEMENT, TYPE=").AppendLine(entry.Key);

                foreach (FeElement element in entry.Value)
                {
                    sb.AppendFormat("{0}", element.Id + 1);
                    count = 1;
                    foreach (int nodeId in element.NodeIds)
                    {
                        count++;
                        if (count == 17)        // 16 entries per line; 17th entry goes in new line
                        {
                            sb.AppendLine();
                            sb.Append(nodeId + 1);
                        }
                        else
                            sb.AppendFormat(", {0}", nodeId + 1);

                    }
                    sb.AppendLine();
                }
            }
        }

        static private void AppendNodeSets(StringBuilder sb, FeModel model)
        {
            IDictionary<string, FeNodeSet> nodeSets = model.Mesh.NodeSets;

            int count;
            foreach (var entry in nodeSets)
            {
                if (entry.Value.Active)
                {
                    count = 0;
                    sb.AppendFormat("*Nset, nset={0}", entry.Value.Name).AppendLine();

                    foreach (var nodeId in entry.Value.Labels)
                    {
                        sb.Append(nodeId + 1);
                        if (count < entry.Value.Labels.Length - 1)
                            sb.Append(", ");
                        if (++count % 16 == 0) sb.AppendLine();
                    }
                    sb.AppendLine();
                }
            }
        }

        static private void AppendElementSets(StringBuilder sb, FeModel model)
        {
            IDictionary<string, FeElementSet> elementSets = model.Mesh.ElementSets;

            int count;
            foreach (var entry in elementSets)
            {
                if (entry.Value.Active)
                {
                    count = 0;
                    sb.AppendFormat("*Elset, elset={0}", entry.Value.Name).AppendLine();

                    foreach (var elementId in entry.Value.Labels)
                    {
                        sb.Append(elementId + 1);
                        if (count < entry.Value.Labels.Length - 1)
                            sb.Append(", ");
                        if (++count % 16 == 0) sb.AppendLine();
                    }
                    sb.AppendLine();
                }
            }
        }

        static private void AppendSurfaces(StringBuilder sb, FeModel model)
        {
            FeSurfaceType type;
            IDictionary<string, FeSurface> surfaces = model.Mesh.Surfaces;
            foreach (var entry in surfaces)
            {
                if (entry.Value.Active)
                {
                    type = entry.Value.Type;
                    sb.AppendFormat("*Surface, name={0}, type={1}", entry.Value.Name, type).AppendLine();
                    if (type == FeSurfaceType.Element)
                    {
                        foreach (var elementSetEntry in entry.Value.ElementFaces)
                        {
                            sb.AppendFormat("{0}, {1}", elementSetEntry.Value, elementSetEntry.Key).AppendLine();
                        }
                    }
                    else throw new NotImplementedException();
                }
            }
        }

        static private void AppendMaterials(StringBuilder sb, FeModel model)
        {
            IDictionary<string, Material> materials = model.Materials;
            HashSet<string> unsupported = new HashSet<string>();
            //
            foreach (var entry in materials)
            {
                if (entry.Value.Active)
                {
                    sb.AppendFormat("*Material, name={0}", entry.Value.Name).AppendLine();
                    foreach (var property in entry.Value.Properties)
                    {
                        if (property is Density)
                        {
                            Density density = (Density)property;
                            sb.AppendLine("*Density");
                            sb.AppendFormat("{0}", density.DensityTemp[0][0]).AppendLine();
                        }
                        else if (property is Elastic)
                        {
                            Elastic elastic = (Elastic)property;
                            sb.AppendLine("*Elastic");
                            sb.AppendFormat("{0}, {1}", elastic.YoungsPoissonsTemp[0][0],
                                            elastic.YoungsPoissonsTemp[0][1]).AppendLine();
                        }
                        else
                        {
                            unsupported.Add(property.GetType().Name);
                        }
                    }
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following material properties are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }

        static private void AppendSections(StringBuilder sb, FeModel model)
        {
            IDictionary<string, Section> sections = model.Sections;
            HashSet<string> unsupported = new HashSet<string>();
            //
            foreach (var entry in sections)
            {
                if (entry.Value.Active)
                {
                    if (entry.Value is SolidSection solid)
                    {
                        sb.AppendFormat("*Solid section, elset={0}, material={1}", solid.RegionName, solid.MaterialName).AppendLine();
                        if (solid.TwoD) sb.AppendLine(solid.Thickness.ToString());
                    }
                    else if (entry.Value is ShellSection shell)
                    {
                        sb.AppendFormat("*Shell section, elset={0}, material={1}", shell.RegionName, shell.MaterialName).AppendLine();
                         sb.AppendLine(shell.Thickness.ToString());
                    }
                    else
                    {
                        unsupported.Add(entry.Value.GetType().Name);
                    }
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following sections are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }

        static private void AppendSteps(StringBuilder sb, FeModel model)
        {
            HashSet<string> unsupported = new HashSet<string>();
            //
            foreach (var step in model.StepCollection.StepsList)
            {
                if (step is InitialStep) continue;
                //
                if (step.Active)
                {
                    sb.Append("*Step");
                    if (step.Nlgeom) sb.Append(", Nlgeom");
                    sb.AppendLine();
                    //
                    if (step.GetType() == typeof(StaticStep))
                    {
                        StaticStep staticStep = (StaticStep)step;
                        sb.AppendLine("*Static");
                        sb.AppendFormat("{0}, {1}", staticStep.InitialTimeIncrement, staticStep.TimePeriod);
                        sb.AppendLine();
                        // Boundary conditions
                        foreach (var bcEntry in staticStep.BoundaryConditions)
                        {
                            AppendBoundaryCondition(sb, bcEntry.Value);
                        }
                        // Loads
                        foreach (var loadEntry in staticStep.Loads)
                        {
                            AppendLoad(sb, loadEntry.Value, model);
                        }
                        // Field outputs
                        foreach (var fieldOutputEntry in staticStep.FieldOutputs)
                        {
                            AppendFieldOutput(sb, fieldOutputEntry.Value);
                        }

                    }
                    else
                    {
                        unsupported.Add(step.GetType().Name);
                    }
                    //
                    sb.AppendLine("*End step");
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following steps are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }

        static private void AppendBoundaryCondition(StringBuilder sb, BoundaryCondition boundaryCondition)
        {
            HashSet<string> unsupported = new HashSet<string>();
            //
            if (boundaryCondition.Active)
            {
                if (boundaryCondition is DisplacementRotation dispRot)
                {
                    int[] directions = dispRot.GetConstrainedDirections();
                    double[] values = dispRot.GetConstrainValues();
                    //
                    sb.AppendLine("*Boundary");
                    for (int i = 0; i < directions.Length; i++)
                    {
                        sb.AppendFormat("PART-1-1.{0}, {1}, {2}, {3}", dispRot.RegionName, directions[i], directions[i], values[i].ToString());
                        sb.AppendLine();
                    }
                }
                else if (boundaryCondition is FixedBC fix)
                {
                    int[] directions = new int[] { 1, 2, 3, 4, 5, 6};
                    double[] values = new double[] { 0, 0, 0, 0, 0, 0 };
                    //
                    sb.AppendLine("*Boundary");
                    for (int i = 0; i < directions.Length; i++)
                    {
                        sb.AppendFormat("PART-1-1.{0}, {1}, {2}, {3}", fix.RegionName, directions[i], directions[i], values[i].ToString());
                        sb.AppendLine();
                    }
                }
                else
                {
                    unsupported.Add(boundaryCondition.GetType().Name);
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following boundary conditions are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }

        static private void AppendLoad(StringBuilder sb, Load load, FeModel model)
        {
            HashSet<string> unsupported = new HashSet<string>();
            //
            if (load.Active)
            {
                if (load is DLoad)
                {
                    //*Dload
                    //_obremenitev_el_surf_S3, P3, 1
                    //_obremenitev_el_surf_S4, P4, 1
                    //_obremenitev_el_surf_S1, P1, 1
                    //_obremenitev_el_surf_S2, P2, 1
                    DLoad dload = (DLoad)load;
                    FeSurface surface = model.Mesh.Surfaces[dload.SurfaceName];
                    sb.AppendLine("*Dload");
                    foreach (var entry in surface.ElementFaces)
                    {
                        sb.AppendFormat("PART-1-1.{0}, P{1}, {2}", entry.Value, entry.Key.ToString()[1], dload.Magnitude);
                        sb.AppendLine();
                    }

                }
                else if (load is CLoad)
                {
                    CLoad cLoad = (CLoad)load;
                    sb.AppendLine("*Cload");
                    if (cLoad.F1 != 0)
                    {
                        sb.AppendFormat("PART-1-1.{0}, {1}, {2}", cLoad.RegionName, 0, cLoad.F1.ToString());
                        sb.AppendLine();
                    }
                    if (cLoad.F2 != 0)
                    {
                        sb.AppendFormat("PART-1-1.{0}, {1}, {2}", cLoad.RegionName, 1, cLoad.F2.ToString());
                        sb.AppendLine();
                    }
                    if (cLoad.F3 != 0)
                    {
                        sb.AppendFormat("PART-1-1.{0}, {1}, {2}", cLoad.RegionName, 2, cLoad.F3.ToString());
                        sb.AppendLine();
                    }
                }
                else
                {
                    unsupported.Add(load.GetType().Name);
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following loads are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }

        static private void AppendFieldOutput(StringBuilder sb, FieldOutput fieldOutput)
        {
            HashSet<string> unsupported = new HashSet<string>();
            //
            if (fieldOutput.Active)
            {
                if (fieldOutput is NodalFieldOutput)
                {
                    NodalFieldOutput nodalFieldOutput = (NodalFieldOutput)fieldOutput;
                    sb.AppendLine("*Node file");
                    sb.AppendLine(nodalFieldOutput.Variables.ToString());
                }
                else if (fieldOutput is ElementFieldOutput)
                {
                    ElementFieldOutput elementFieldOutput = (ElementFieldOutput)fieldOutput;
                    sb.AppendLine("*El file");
                    sb.AppendLine(elementFieldOutput.Variables.ToString());
                }
                else
                {
                    unsupported.Add(fieldOutput.GetType().Name);
                }
            }
            //
            if (unsupported.Count > 0)
            {
                string msg = "The following field outputs are not supported and will be skipped:" + Environment.NewLine;
                foreach (var propertyName in unsupported) msg += Environment.NewLine + propertyName;
                MessageBoxes.ShowWarning(msg);
            }
        }


    }
}
