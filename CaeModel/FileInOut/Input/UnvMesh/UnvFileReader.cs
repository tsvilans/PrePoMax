using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;

namespace FileInOut.Input
{
    [Serializable]
    public static class UnvFileReader
    {
        //
        // http://www.sdrl.uc.edu/sdrl/referenceinfo/universalfileformats/file-format-storehouse/universal-dataset-number-2467
        // 

        static string[] splitter = new string[] { " " };

        // Methods                                                                                                                  
        static public FeMesh Read(string fileName, ElementsToImport elementsToImport)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string[] lines = CaeGlobals.Tools.ReadAllLines(fileName);
                //
                List<List<string>> dataSets = GetDataSets(lines);
                //
                int setID;
                Dictionary<int, FeNode> nodes = null;
                Dictionary<int, FeElement> elements = null;
                Dictionary<int, FeNodeSet> nodeGroups = new Dictionary<int, FeNodeSet>();
                Dictionary<int, FeElementSet> elementGroups = new Dictionary<int, FeElementSet>();
                //
                string line;
                foreach (List<string> dataSet in dataSets)
                {
                    line = dataSet[0];
                    setID = int.Parse(line.Split(splitter, StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (setID == 164 || setID == 2420) // Data
                    { }
                    else if (setID == 2411) // Nodes
                    {
                        nodes = GetNodes(dataSet.ToArray());
                    }
                    else if (setID == 2412) // Elements
                    {
                        elements = GetElements(dataSet.ToArray());
                    }
                    else if (setID == 2467 || setID == 2477) // Groups
                    {
                        GetGroups(dataSet.ToArray(), ref nodeGroups, ref elementGroups);
                    }
                }
                FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Mesh);
                //
                foreach (var entry in nodeGroups) mesh.AddNodeSet(entry.Value);
                //
                foreach (var entry in elementGroups)
                {
                    mesh.AddElementSet(entry.Value);
                    mesh.AddNodeSetFromElementSet(entry.Value.Name);
                }
                //
                if (elementsToImport != ElementsToImport.All)
                {
                    if (!elementsToImport.HasFlag(ElementsToImport.Beam)) mesh.RemoveElementsByType<FeElement1D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Shell)) mesh.RemoveElementsByType<FeElement2D>();
                    if (!elementsToImport.HasFlag(ElementsToImport.Solid)) mesh.RemoveElementsByType<FeElement3D>();
                }
                //
                return mesh;
            }
            //
            return null;
        }
        static private List<List<string>> GetDataSets(string[] lines)
        {
            int count = 0;
            List<string> dataSet = new List<string>();
            List<List<string>> dataSets = new List<List<string>>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "    -1")
                {
                    count++;
                    if (count == 2)
                    {
                        dataSets.Add(dataSet);
                        dataSet = new List<string>();
                        count = 0;
                    }
                }
                else
                    dataSet.Add(lines[i]);
            }

            return dataSets;
        }
        static private Dictionary<int, FeNode> GetNodes(string[] lines)
        {
            // Gmsh output
            // 5.9503897502616780D+000 - 6.7545275265579088D+000 - 1.2674495655059456D+001

            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            int id;
            FeNode node;
            string[] record1;
            string[] record2;
            bool replaceDwithE = false;
            // line 0 is the line with the SetID

            if (lines.Length > 2) replaceDwithE = lines[2].Contains("D");

            for (int i = 1; i < lines.Length; i += 2)
            {
                if (replaceDwithE) lines[i + 1] = lines[i + 1].Replace('D', 'E');

                record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                record2 = lines[i + 1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                id = int.Parse(record1[0]);
                node = new FeNode();
                node.Id = id;
                node.X = double.Parse(record2[0]);
                node.Y = double.Parse(record2[1]);
                node.Z = double.Parse(record2[2]);

                nodes.Add(id, node);
            }

            return nodes;
        }
        static private Dictionary<int, FeElement> GetElements(string[] lines)
        {
            Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
            int id;
            UnvFeDescriptorId feDescriptorId;
            FeElement element;
            string[] record1;
            string[] record2;
            string[] record3 = null;
            string[] record4 = null;

            // line 0 is the line with the SetID
            for (int i = 1; i < lines.Length; i += 2)
            {
                record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                record2 = lines[i + 1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                id = int.Parse(record1[0]);
                feDescriptorId = (UnvFeDescriptorId)int.Parse(record1[1]);

                if (HasThreeRecords(feDescriptorId)) // beam, parabolic tetrahedron, parabolic wedge and parabolic hexahedron have 3 records
                {
                    record3 = lines[i + 2].Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    if (HasFourRecords(feDescriptorId)) // parabolic hexahedron
                    {
                        record4 = lines[i + 3].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        i++;
                    }

                    i++;
                }

                // Salome fix
                if (feDescriptorId == UnvFeDescriptorId.TaperedBeam && record1[5] == "3") feDescriptorId = UnvFeDescriptorId.ParabolicBeam;

                switch (feDescriptorId)
                {
                    // LINEAR ELEMENTS                                                                                          

                    case UnvFeDescriptorId.Rod:
                    case UnvFeDescriptorId.LinearBeam:
                    case UnvFeDescriptorId.TaperedBeam:
                    case UnvFeDescriptorId.StraightPipe:
                        // linear beam element
                        element = GetLinearBeamElement(id, record1, record2, record3);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.PlaneStressLinearTriangle:
                    case UnvFeDescriptorId.PlaneStrainLinearTriangle:
                    case UnvFeDescriptorId.PlateLinearTriangle:
                    case UnvFeDescriptorId.MembraneLinearTriangle:
                    case UnvFeDescriptorId.AxisymetricSolidLinearTriangle:
                    case UnvFeDescriptorId.ThinShellLinearTriangle:
                        // linear triangle element
                        element = GetLinearTriangleElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.PlaneStressLinearQuadrilateral:
                    case UnvFeDescriptorId.PlaneStrainLinearQuadrilateral:
                    case UnvFeDescriptorId.PlateLinearQuadrilateral:
                    case UnvFeDescriptorId.MembraneLinearQuadrilateral:
                    case UnvFeDescriptorId.AxisymetricSolidLinearQuadrilateral:
                    case UnvFeDescriptorId.ThinShellLinearQuadrilateral:
                        // linear quadrilateral element
                        element = GetLinearQuadrilateralElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidLinearTetrahedron:
                        // linear tetrahedron element
                        element = GetLinearTetraElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidLinearWedge:
                        // linear wedge element
                        element = GetLinearWedgeElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidLinearBrick:
                        // linear hexahedron element
                        element = GetLinearHexaElement(id, record1, record2);
                        elements.Add(id, element);
                        break;

                    // PARABOLIC ELEMENTS                                                                                       

                    case UnvFeDescriptorId.CurvedBeam:
                    case UnvFeDescriptorId.ParabolicBeam:
                    case UnvFeDescriptorId.CurvedPipe:
                        // parabolic beam element
                        element = GetParabolicBeamElement(id, record1, record2, record3);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.PlaneStressParabolicTriangle:
                    case UnvFeDescriptorId.PlaneStrainParabolicTriangle:
                    case UnvFeDescriptorId.PlateParabolicTriangle:
                    case UnvFeDescriptorId.MembraneParabolicTriangle:
                    case UnvFeDescriptorId.AxisymetricSolidParabolicTriangle:
                    case UnvFeDescriptorId.ThinShellParabolicTriangle:
                        // parabolic triangle element
                        element = GetParabolicTriangleElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.PlaneStressParabolicQuadrilateral:
                    case UnvFeDescriptorId.PlaneStrainParabolicQuadrilateral:
                    case UnvFeDescriptorId.PlateParabolicQuadrilateral:
                    case UnvFeDescriptorId.MembraneParabolicQuadrilateral:
                    case UnvFeDescriptorId.AxisymetricSolidParabolicQuadrilateral:
                    case UnvFeDescriptorId.ThinShellParabolicQuadrilateral:
                        // parabolic quadrilateral element
                        element = GetParabolicQuadrilateralElement(id, record1, record2);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidParabolicTetrahedron:
                        // parabolic tetrahedron element
                        element = GetParabolicTetraElement(id, record1, record2, record3);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidParabolicWedge:
                        // parabolic wedge element
                        element = GetParabolicWedgeElement(id, record1, record2, record3);
                        elements.Add(id, element);
                        break;
                    case UnvFeDescriptorId.SolidParabolicBrick:
                        // parabolic hexahedron element
                        element = GetParabolicHexaElement(id, record1, record2, record3, record4);
                        elements.Add(id, element);
                        break;
                    default:
                        throw new Exception("The element type " + feDescriptorId.ToString() + " is not supported.");
                }
            }

            return elements;
        }
        static private void GetGroups(string[] lines, ref Dictionary<int, FeNodeSet> nodeGroups,
                                      ref Dictionary<int, FeElementSet> elementGroups)
        {
            //
            // http://www2.me.rochester.edu/courses/ME204/nx_help/index.html#uid:id973882
            //
            int id;
            int n;
            string name;
            string[] record1;
            string[] record2;
            string[] recordN;
            bool isNodeGroup = true;
            List<int> labels;
            int row;
            // Line 0 is the line with the SetID
            for (int i = 1; i < lines.Length; i++)
            {
                record1 = lines[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                //record2 = lines[i + 1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                record2 = new string[] { lines[i + 1].Replace(' ', '_') };  // group name fix
                //
                id = int.Parse(record1[0]);
                n = int.Parse(record1[7]);  // number of elements in record
                name = record2[0];
                //
                labels = new List<int>();
                i += 2;
                row = 0;
                while (labels.Count < n)
                {
                    recordN = lines[i + row].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (row == 0)
                    {
                        if (int.Parse(recordN[0]) == 7) isNodeGroup = true;
                        else isNodeGroup = false;
                    }
                    //
                    labels.Add(int.Parse(recordN[1]));
                    if (recordN.Length > 4) labels.Add(int.Parse(recordN[5]));
                    //
                    row++;
                }
                //
                if (isNodeGroup) nodeGroups.Add(id, new FeNodeSet(name, labels.ToArray(), null));
                else elementGroups.Add(id, new FeElementSet(name, labels.ToArray()));
                //
                i += row - 1;
            }
        }
        private static bool HasThreeRecords(UnvFeDescriptorId feDescriptorId)
        {
            switch (feDescriptorId)
            {
                case UnvFeDescriptorId.Rod:         // edge with 2 nodes
                case UnvFeDescriptorId.LinearBeam:
                case UnvFeDescriptorId.TaperedBeam: // edge with 3 nodes
                case UnvFeDescriptorId.CurvedBeam:  // curved beam
                case UnvFeDescriptorId.ParabolicBeam:
                case UnvFeDescriptorId.StraightPipe:
                case UnvFeDescriptorId.CurvedPipe:

                case UnvFeDescriptorId.SolidParabolicTetrahedron:
                case UnvFeDescriptorId.SolidParabolicWedge:
                case UnvFeDescriptorId.SolidParabolicBrick:
                    return true;
            }
            return false;
        }
        private static bool HasFourRecords(UnvFeDescriptorId feDescriptorId)
        {
            if (feDescriptorId == UnvFeDescriptorId.SolidParabolicBrick) return true;
            else return false;
        }


        //  LINEAR ELEMENTS                                                                                        
        static private LinearBeamElement GetLinearBeamElement(int id, string[] record1, string[] record2, string[] record3)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record3[i]);
            }
            return new LinearBeamElement(id, nodes);
        }
        static private LinearTriangleElement GetLinearTriangleElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record2[i]);
            }
            return new LinearTriangleElement(id, nodes);
        }
        static private LinearQuadrilateralElement GetLinearQuadrilateralElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record2[i]);
            }
            return new LinearQuadrilateralElement(id, nodes);
        }
        static private LinearTetraElement GetLinearTetraElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record2[i]);
            }
            return new LinearTetraElement(id, nodes);
        }
        static private LinearWedgeElement GetLinearWedgeElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record2[i]);
            }
            return new LinearWedgeElement(id, nodes);
        }
        static private LinearHexaElement GetLinearHexaElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            int[] nodes = new int[n];
            for (int i = 0; i < n; i++)
            {
                nodes[i] = int.Parse(record2[i]);
            }
            return new LinearHexaElement(id, nodes);
        }


        //  PARABOLIC ELEMENTS                                                                                    
        static private ParabolicBeamElement GetParabolicBeamElement(int id, string[] record1, string[] record2, string[] record3)
        {
            int n = int.Parse(record1[5]);
            if (n != 3) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record3[0]);
            nodes[1] = int.Parse(record3[2]);
            nodes[2] = int.Parse(record3[1]);

            return new ParabolicBeamElement(id, nodes);
        }
        static private ParabolicTriangleElement GetParabolicTriangleElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            if (n != 6) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record2[0]);
            nodes[1] = int.Parse(record2[2]);
            nodes[2] = int.Parse(record2[4]);
            nodes[3] = int.Parse(record2[1]);
            nodes[4] = int.Parse(record2[3]);
            nodes[5] = int.Parse(record2[5]);

            return new ParabolicTriangleElement(id, nodes);
        }
        static private ParabolicQuadrilateralElement GetParabolicQuadrilateralElement(int id, string[] record1, string[] record2)
        {
            int n = int.Parse(record1[5]);
            if (n != 8) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record2[0]);
            nodes[1] = int.Parse(record2[2]);
            nodes[2] = int.Parse(record2[4]);
            nodes[3] = int.Parse(record2[6]);
            nodes[4] = int.Parse(record2[1]);
            nodes[5] = int.Parse(record2[3]);
            nodes[6] = int.Parse(record2[5]);
            nodes[7] = int.Parse(record2[7]);

            return new ParabolicQuadrilateralElement(id, nodes);
        }
        static private ParabolicTetraElement GetParabolicTetraElement(int id, string[] record1, string[] record2, string[] record3)
        {
            int n = int.Parse(record1[5]);
            if (n != 10) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record2[0]);
            nodes[1] = int.Parse(record2[2]);
            nodes[2] = int.Parse(record2[4]);
            nodes[3] = int.Parse(record3[1]); // 9
            nodes[4] = int.Parse(record2[1]);
            nodes[5] = int.Parse(record2[3]);
            nodes[6] = int.Parse(record2[5]);
            nodes[7] = int.Parse(record2[6]);
            nodes[8] = int.Parse(record2[7]);
            nodes[9] = int.Parse(record3[0]); // 8

            return new ParabolicTetraElement(id, nodes);
        }
        static private ParabolicWedgeElement GetParabolicWedgeElement(int id, string[] record1, string[] record2, string[] record3)
        {
            int n = int.Parse(record1[5]);
            if (n != 15) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record2[0]);
            nodes[1] = int.Parse(record2[2]);
            nodes[2] = int.Parse(record2[4]);
            nodes[3] = int.Parse(record3[1]); // 9
            nodes[4] = int.Parse(record3[3]); // 11
            nodes[5] = int.Parse(record3[5]); // 13
            nodes[6] = int.Parse(record2[1]);
            nodes[7] = int.Parse(record2[3]);
            nodes[8] = int.Parse(record2[5]);
            nodes[9] = int.Parse(record3[2]); // 10
            nodes[10] = int.Parse(record3[4]); // 12
            nodes[11] = int.Parse(record3[6]); // 14
            nodes[12] = int.Parse(record2[6]);
            nodes[13] = int.Parse(record2[7]); 
            nodes[14] = int.Parse(record3[0]); // 8

            return new ParabolicWedgeElement(id, nodes);
        }
        static private ParabolicHexaElement GetParabolicHexaElement(int id, string[] record1, string[] record2, string[] record3, string[] record4)
        {
            int n = int.Parse(record1[5]);
            if (n != 20) throw new ArgumentOutOfRangeException();

            int[] nodes = new int[n];
            nodes[0] = int.Parse(record2[0]);
            nodes[1] = int.Parse(record2[2]);
            nodes[2] = int.Parse(record2[4]);
            nodes[3] = int.Parse(record2[6]);
            nodes[4] = int.Parse(record3[4]); // 12
            nodes[5] = int.Parse(record3[6]); // 14
            nodes[6] = int.Parse(record4[0]); // 16 
            nodes[7] = int.Parse(record4[2]); // 18
            nodes[8] = int.Parse(record2[1]);
            nodes[9] = int.Parse(record2[3]);
            nodes[10] = int.Parse(record2[5]);
            nodes[11] = int.Parse(record2[7]);
            nodes[12] = int.Parse(record3[5]); // 13
            nodes[13] = int.Parse(record3[7]); // 15
            nodes[14] = int.Parse(record4[1]); // 17
            nodes[15] = int.Parse(record4[3]); // 19
            nodes[16] = int.Parse(record3[0]); // 8
            nodes[17] = int.Parse(record3[1]); // 9
            nodes[18] = int.Parse(record3[2]); // 10
            nodes[19] = int.Parse(record3[3]); // 11

            return new ParabolicHexaElement(id, nodes);
        }
    }
}
