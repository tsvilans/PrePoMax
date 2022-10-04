using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;

namespace CaeResults
{
    public enum OpenFoamFieldType
    {
        Unknown,
        SurfaceScalarField,
        VolScalarField,
        VolVectorField
    }
    public enum OpenFoamFormatType
    {
        Unknown,
        Ascii,
        Binary
    }
    public class OpenFoamHeader
    {
        public string Version;
        public OpenFoamFormatType FormatType;
        public OpenFoamFieldType FieldType;

        public OpenFoamHeader()
        {
            Version = null;
            FormatType = OpenFoamFormatType.Unknown;
            FieldType = OpenFoamFieldType.Unknown;
        }
    }

    public static class OpenFoamFileReader
    {
        // Variables                                                                                                                
        //static private HashSet<string> resultFileNames = new HashSet<string>() {
        //    "gammaInt", 
        //    "k",
        //    "nut",
        //    "omega",
        //    "p",
        //    "phi",
        //    "pMean",
        //    "ReThetat",
        //    "U",
        //    "yPlus",
        //};


        // Methods                                                                                                                  
        static public FeResults Read(string fileName)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string baseFolder = Path.GetDirectoryName(fileName);
                string meshFolder = Path.Combine(baseFolder, "constant", "polyMesh");
                if (Directory.Exists(meshFolder))
                {
                    string pointsFile = Path.Combine(meshFolder, "points");
                    string boundaryFile = Path.Combine(meshFolder, "boundary");
                    string facesFile = Path.Combine(meshFolder, "faces");
                    string ownerFile = Path.Combine(meshFolder, "owner");
                    //
                    OpenFoamHeader header;
                    string error = "Binary format of OpenFOAM data is not supported.";
                    if (File.Exists(pointsFile) && File.Exists(boundaryFile) && File.Exists(facesFile)  && File.Exists(ownerFile))
                    {
                        // Check for binary
                        header = GetHeaderFromFile(pointsFile);
                        if (header.FormatType == OpenFoamFormatType.Binary) throw new CaeException(error);
                        header = GetHeaderFromFile(boundaryFile);
                        if (header.FormatType == OpenFoamFormatType.Binary) throw new CaeException(error);
                        header = GetHeaderFromFile(facesFile);
                        if (header.FormatType == OpenFoamFormatType.Binary) throw new CaeException(error);
                        header = GetHeaderFromFile(ownerFile);
                        if (header.FormatType == OpenFoamFormatType.Binary) throw new CaeException(error);
                        // Read mesh
                        Dictionary<int, HashSet<int>> faceIdNodeIds;
                        HashSet<int> allFaceNodeIds;
                        HashSet<int> boundaryFaceIds = GetBoundaryFaceIds(boundaryFile);
                        Dictionary<int, FeElement> elements = GetBoundariesAsTriangles(facesFile, boundaryFaceIds,
                                                                                       out faceIdNodeIds, out allFaceNodeIds);
                        Dictionary<int, HashSet<int>> cellIdNodeIds = GetCellIdNodeIds(ownerFile, boundaryFaceIds, faceIdNodeIds);
                        Dictionary<int, int> nodeIdCellCount = GetNodeIdCellCount(cellIdNodeIds);
                        //
                        Dictionary<int, int> nodeIdsLookUp;
                        Dictionary<int, FeNode> nodes = GetNodes(pointsFile, allFaceNodeIds, out nodeIdsLookUp);
                        //
                        FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Results);
                        mesh.ResetPartsColor();
                        // Read results
                        FeResults result = new FeResults(fileName);
                        result.SetMesh(mesh, nodeIdsLookUp);
                        //
                        int globalIncrementId = 1;
                        int stepId = 1;
                        FieldData fieldData;
                        Field field;
                        string[] resultFileNames;
                        Dictionary<double, string> timeResultFolderNames = GerTimeResultFolderNames(baseFolder);
                        double[] sortedTimes = timeResultFolderNames.Keys.ToArray();
                        Array.Sort(sortedTimes);
                        //
                        foreach (double sortedTime in sortedTimes)
                        {
                            resultFileNames = Directory.GetFiles(timeResultFolderNames[sortedTime]);
                            foreach (var resultFileName in resultFileNames)
                            {
                                try
                                {
                                    GetField(resultFileName, sortedTime, globalIncrementId, stepId, cellIdNodeIds,
                                             nodeIdsLookUp, nodeIdCellCount, out fieldData, out field);                                    
                                    //
                                    if (field != null) result.AddFiled(fieldData, field);
                                }
                                catch
                                { }
                                //
                                globalIncrementId++;
                            }
                            //
                            stepId++;
                        }
                        //
                        return result;
                    }
                }
            }
            //
            return null;
        }
        static public FeResults Read(string fileName, double time, string variableName)
        {
            if (fileName != null && File.Exists(fileName))
            {
                string baseFolder = Path.GetDirectoryName(fileName);
                string meshFolder = Path.Combine(baseFolder, "constant", "polyMesh");
                if (Directory.Exists(meshFolder))
                {
                    string pointsFile = Path.Combine(meshFolder, "points");         // coordinates of the mehs nodes
                    string boundaryFile = Path.Combine(meshFolder, "boundary");     // names and face ids of boundary faces
                    string facesFile = Path.Combine(meshFolder, "faces");           // topology of the faces
                    string ownerFile = Path.Combine(meshFolder, "owner");           // face ids 
                    //
                    if (File.Exists(pointsFile) && File.Exists(boundaryFile) && File.Exists(facesFile) && File.Exists(ownerFile))
                    {
                        // Read mesh
                        Dictionary<int, HashSet<int>> faceIdNodeIds;
                        HashSet<int> allFaceNodeIds;
                        HashSet<int> boundaryFaceIds = GetBoundaryFaceIds(boundaryFile);
                        Dictionary<int, FeElement> elements = GetBoundariesAsTriangles(facesFile, boundaryFaceIds,
                                                                                       out faceIdNodeIds, out allFaceNodeIds);
                        Dictionary<int, HashSet<int>> cellIdNodeIds = GetCellIdNodeIds(ownerFile, boundaryFaceIds, faceIdNodeIds);
                        Dictionary<int, int> nodeIdCellCount = GetNodeIdCellCount(cellIdNodeIds);
                        //
                        Dictionary<int, int> nodeIdsLookUp;
                        Dictionary<int, FeNode> nodes = GetNodes(pointsFile, allFaceNodeIds, out nodeIdsLookUp);
                        //
                        FeMesh mesh = new FeMesh(nodes, elements, MeshRepresentation.Results);
                        mesh.ResetPartsColor();
                        // Read results
                        FeResults result = new FeResults(fileName);
                        result.SetMesh(mesh, nodeIdsLookUp);
                        //
                        int globalIncrementId = 1;
                        int stepId = 1;
                        FieldData fieldData;
                        Field field;
                        string[] resultFileNames;
                        Dictionary<double, string> timeResultFolderNames = GerTimeResultFolderNames(baseFolder);
                        if (!timeResultFolderNames.ContainsKey(time))
                            throw new CaeException("The selected OpenFOAM folder does not contain results for time: " + time + ".");
                        //
                        resultFileNames = Directory.GetFiles(timeResultFolderNames[time]);
                        foreach (var resultFileName in resultFileNames)
                        {
                            try
                            {
                                if (Path.GetFileNameWithoutExtension(resultFileName) == variableName)
                                {
                                    GetField(resultFileName, time, globalIncrementId, stepId, cellIdNodeIds,
                                             nodeIdsLookUp, nodeIdCellCount, out fieldData, out field);
                                    // Test
                                    //double[] coor;
                                    //float[] values = field.GetComponentValues("VAL");
                                    //foreach (var entry in mesh.Nodes)
                                    //{
                                    //    coor = entry.Value.Coor;
                                    //    values[nodeIdsLookUp[entry.Key]] = 2 - (float)coor[2];
                                    //}
                                    if (field != null) result.AddFiled(fieldData, field);
                                }
                            }
                            catch
                            { }
                            //
                            globalIncrementId++;
                        }
                        //
                        stepId++;
                        //
                        return result;
                    }
                }
            }
            //
            return null;
        }
        static public Dictionary<string, string[]> GetTimeResultVariableNames(string fileName)
        {
            Dictionary<string, string[]> timeResultVariableNames = new Dictionary<string, string[]>();
            //
            if (fileName != null && File.Exists(fileName))
            {
                string[] resultFileNames;
                HashSet<string> variableNamesHash = new HashSet<string>();
                //
                string baseFolder = Path.GetDirectoryName(fileName);
                Dictionary<double, string> timeResultFolderNames = GerTimeResultFolderNames(baseFolder);
                double[] sortedTimes = timeResultFolderNames.Keys.ToArray();
                Array.Sort(sortedTimes);
                //
                foreach (double sortedTime in sortedTimes)
                {
                    variableNamesHash.Clear();
                    resultFileNames = Directory.GetFiles(timeResultFolderNames[sortedTime]);
                    foreach (var resultFileName in resultFileNames)
                    {
                        variableNamesHash.Add(Path.GetFileNameWithoutExtension(resultFileName));
                    }
                    timeResultVariableNames.Add(sortedTime.ToString(), variableNamesHash.ToArray());
                }
            }
            //
            return timeResultVariableNames;
        }
        // Mesh
        static private OpenFoamHeader GetHeaderFromFile(string fileName)
        {
            bool foamFile = false;
            bool endBracket = false;
            string line;
            List<string> lines = new List<string>();
            OpenFoamHeader header = null;
            //
            if (!Tools.WaitForFileToUnlock(fileName, 5000)) return null;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 16 * 4096))
            {
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine().Trim().ToUpper();
                    lines.Add(line);
                    //
                    if (line.StartsWith("FOAMFILE")) foamFile = true;
                    if (line.StartsWith("}")) endBracket = true;
                    //
                    if (lines.Count > 15 && !foamFile) break;
                    if (endBracket)
                    {
                        int lineId = 0;
                        //
                        string[] linesArr = SkipCommentsAndEmptyLines(lines.ToArray());
                        header = GetFoamHeader(linesArr, ref lineId);
                        break;
                    }
                }
                //
                streamReader.Close();
                fileStream.Close();
            }
            //
            return header;
        }
        static private Dictionary<int, FeNode> GetNodes(string fileName, HashSet<int> allFaceNodeIds, 
                                                        out Dictionary<int, int> nodeIdsLookUp)
        {
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")" };
            int numOfNodes;
            FeNode node;
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            nodeIdsLookUp = new Dictionary<int, int>();
            //
            numOfNodes = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            for (int i = 0; i < numOfNodes; i++)
            {
                if (allFaceNodeIds.Contains(i))
                {
                    tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length == 3)
                    {
                        node = new FeNode(i, double.Parse(tmp[0]), double.Parse(tmp[1]), double.Parse(tmp[2]));
                        nodes.Add(i, node);
                        nodeIdsLookUp.Add(i, nodeIdsLookUp.Count);
                    }
                    else throw new NotSupportedException();
                }
                //
                lineId++;
            }
            //
            return nodes;
        }
        static private HashSet<int> GetBoundaryFaceIds(string fileName)
        {
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            int numOfBoundaries;
            string line;
            // Read the number of boundaries
            numOfBoundaries = int.Parse(lines[lineId++]);
            // Get boundaries
            List<string> boundary = new List<string>();
            List<List<string>> boundaries = new List<List<string>>();
            //
            for (int i = lineId; i < lines.Length; i++)
            {
                line = lines[i];
                //
                if (line[0] == '(' || line[0] == ')' || line[0] == '{' || line[0] == '}')
                {
                    if (boundary.Count > 0)
                    {
                        boundaries.Add(boundary);
                        boundary = new List<string>();
                    }
                }
                else
                {
                    boundary.Add(line);
                }
            }
            // Get ids
            string[] tmp;
            string[] splitter = new string[] { " ", ";" };
            int numberOfFaces;
            int startFaceId;
            HashSet<int> boundaryFaceIds = new HashSet<int>();
            foreach (var boundaryLines in boundaries)
            {
                numberOfFaces = -1;
                startFaceId = -1;
                // Boundary name
                if (boundaryLines.Count <= 1)
                { }
                else
                {
                    foreach (var boundaryLine in boundaryLines)
                    {
                        tmp = boundaryLine.ToUpper().Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp[0] == "NFACES") numberOfFaces = int.Parse(tmp[1]);
                        else if (tmp[0] == "STARTFACE") startFaceId = int.Parse(tmp[1]);
                    }
                }
                //
                if (numberOfFaces > 0 && startFaceId > 0)
                {
                    for (int i = startFaceId; i < startFaceId + numberOfFaces; i++) boundaryFaceIds.Add(i);
                }
            }
            //
            return boundaryFaceIds;
        }
        static private Dictionary<int, FeElement> GetBoundariesAsTriangles(string fileName, HashSet<int> boundaryFaceIds,
                                                                           out Dictionary<int, HashSet<int>> faceIdNodeIds,
                                                                           out HashSet<int> allFaceNodeIds)
        {
            // Read only faces on the boundary
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")" };
            int numOfNodes;
            int[] nodeIds;
            int[] faceNodeIds;
            int numOfFaces;
            int triangleId = 0;
            FeElement triangle;
            Dictionary<int, FeElement> triangles = new Dictionary<int, FeElement>();
            // Read number of faces
            numOfFaces = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            HashSet<int> faceNodeIdsHash;
            faceIdNodeIds = new Dictionary<int, HashSet<int>>();
            allFaceNodeIds = new HashSet<int>();
            //
            for (int i = 0; i < numOfFaces; i++)
            {
                if (boundaryFaceIds.Contains(i))
                {
                    tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    //
                    if (int.TryParse(tmp[0], out numOfNodes))
                    {
                        if (tmp.Length == numOfNodes + 1)
                        {
                            faceNodeIds = new int[tmp.Length - 1];
                            for (int j = 0; j < tmp.Length - 1; j++) faceNodeIds[j] = int.Parse(tmp[j + 1]);
                            // Read boudary as triangles
                            for (int j = 0; j < faceNodeIds.Length - 2; j++)
                            {
                                nodeIds = new int[] { faceNodeIds[0], faceNodeIds[j + 1], faceNodeIds[j + 2] };
                                triangle = new LinearTriangleElement(triangleId++, nodeIds);
                                triangles.Add(triangle.Id, triangle);
                            }
                            //
                            faceNodeIdsHash = new HashSet<int>(faceNodeIds);
                            faceIdNodeIds.Add(i, faceNodeIdsHash);
                            allFaceNodeIds.UnionWith(faceNodeIdsHash);
                        }
                        else throw new NotSupportedException();
                    }
                }
                //
                lineId++;
            }
            //
            return triangles;
        }
        static private Dictionary<int, HashSet<int>> GetCellIdNodeIds(string fileName, HashSet<int> boundaryFaceIds,
                                                                      Dictionary<int, HashSet<int>> faceIdNodeIds)
        {
            // Use only faces on the boundary
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            int cellId;
            int numOfCells;
            HashSet<int> cellNodeIds;
            Dictionary<int, HashSet<int>> cellIdNodeIds = new Dictionary<int, HashSet<int>>();
            // Read number of cells
            numOfCells = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            for (int i = 0; i < numOfCells; i++)
            {
                if (boundaryFaceIds.Contains(i))
                {
                    cellId = int.Parse(lines[lineId]);
                    if (cellIdNodeIds.TryGetValue(cellId, out cellNodeIds)) cellNodeIds.UnionWith(faceIdNodeIds[i]);
                    else cellIdNodeIds.Add(cellId, new HashSet<int>(faceIdNodeIds[i]));
                }
                //
                lineId++;
            }
            return cellIdNodeIds;
        }
        static private Dictionary<int, int> GetNodeIdCellCount(Dictionary<int, HashSet<int>> cellIdNodeIds)
        { 
            Dictionary<int, int> nodeIdCellCount = new Dictionary<int, int>();
            foreach (var entry in cellIdNodeIds)
            {
                foreach (var nodeId in entry.Value)
                {
                    if (nodeIdCellCount.ContainsKey(nodeId)) nodeIdCellCount[nodeId]++;
                    else nodeIdCellCount.Add(nodeId, 1);
                }
            }
            return nodeIdCellCount;
        }
        
        // Results
        static private Dictionary<double, string> GerTimeResultFolderNames(string baseFolder)
        {
            double time;
            string directoryName;
            Dictionary<double, string> timeResultFolderNames = new Dictionary<double, string>();
            //
            foreach (var directory in Directory.GetDirectories(baseFolder))
            {
                directoryName = Path.GetFileNameWithoutExtension(directory);
                //
                if (double.TryParse(directoryName, out time) && time > 0)
                {
                    timeResultFolderNames.Add(time, directory);
                }
            }
            return timeResultFolderNames;
        }
        static private void GetField(string fileName, double time, int globalIncrementId, int stepId,
                                     Dictionary<int, HashSet<int>> cellIdNodeIds,
                                     Dictionary<int, int> nodeIdsLookUp,
                                     Dictionary<int, int> nodeIdCellCount,
                                     out FieldData fieldData, out Field field)
        {
            fieldData = new FieldData(Path.GetFileNameWithoutExtension(fileName));
            fieldData.GlobalIncrementId = globalIncrementId;
            fieldData.Type = StepType.Static;
            fieldData.Time = (float)time;
            fieldData.MethodId = 1;
            fieldData.StepId = stepId;
            fieldData.StepIncrementId = 1;
            //
            field = null;
            // Read file
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            OpenFoamHeader header = GetFoamHeader(lines, ref lineId);
            // This field types are not supported
            if (header.FieldType == OpenFoamFieldType.Unknown || header.FieldType == OpenFoamFieldType.SurfaceScalarField) return;
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")", ";" };
            int numOfCells;
            int numOfComponents;
            float[][] cellValues;
            // Skip keywords until internalField
            while (!lines[lineId].ToUpper().StartsWith("INTERNALFIELD")) lineId++;
            // internalField   nonuniform List<scalar>
            if (lines[lineId].ToUpper().Contains("NONUNIFORM"))
            {
                if (lines[lineId].ToUpper().Contains("LIST<SCALAR>"))
                {
                    lineId++;
                    //
                    numOfCells = int.Parse(lines[lineId++]);
                    // Skip bracket
                    if (lines[lineId].StartsWith("(")) lineId++;
                    // Count the number of components
                    tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    numOfComponents = tmp.Length;
                    cellValues = new float[numOfComponents][];
                    for (int i = 0; i < numOfComponents; i++) cellValues[i] = new float[numOfCells];
                    //
                    string line;
                    if (header.FieldType == OpenFoamFieldType.VolScalarField)
                    {
                        for (int i = 0; i < numOfCells; i++)
                        {
                            line = lines[lineId];
                            cellValues[0][i] = float.Parse(line);
                            lineId++;
                        }
                    }
                    else if (header.FieldType == OpenFoamFieldType.VolVectorField)
                    {
                        for (int i = 0; i < numOfCells; i++)
                        {
                            tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < numOfComponents; j++) cellValues[j][i] = float.Parse(tmp[j]);
                            lineId++;
                        }
                    }
                    // Get sum of nodal values
                    float[][] nodeValues = new float[numOfComponents][];
                    for (int i = 0; i < numOfComponents; i++) nodeValues[i] = new float[nodeIdsLookUp.Count];
                    //
                    int localNodeId;
                    foreach (var entry in cellIdNodeIds)
                    {
                        foreach (var nodeId in entry.Value)
                        {
                            localNodeId = nodeIdsLookUp[nodeId];
                            for (int i = 0; i < numOfComponents; i++) nodeValues[i][localNodeId] += cellValues[i][entry.Key];
                        }
                    }
                    // Average nodal values
                    foreach (var entry in nodeIdCellCount)
                    {
                        localNodeId = nodeIdsLookUp[entry.Key];
                        for (int i = 0; i < numOfComponents; i++) nodeValues[i][localNodeId] /= entry.Value;
                    }
                    //
                    field = new Field(fieldData.Name);
                    string componentName;
                    for (int i = 0; i < numOfComponents; i++)
                    {
                        if (numOfComponents == 1) componentName = "VAL";
                        else componentName = "VAL" + (i + 1).ToString();
                        field.AddComponent(componentName, nodeValues[i]);
                    }
                }
            }
            // internalField   uniform 1
            else if (lines[lineId].ToUpper().Contains("UNIFORM"))
            {
                tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length >= 2)
                {
                    float value = float.Parse(tmp[2]);
                    float[] nodeValues = new float[nodeIdsLookUp.Count];
                    for (int i = 0; i < nodeValues.Length; i++) nodeValues[i] = value;
                    //
                    field = new Field(fieldData.Name);
                    string componentName = "VAL";
                    field.AddComponent(componentName, nodeValues);
                }
            }
        }
        // File
        static private string[] SkipCommentsAndEmptyLines(string[] lines)
        {
            string line;
            List<string> dataLines = new List<string>();
            //
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Trim();
                if (line.Length > 0 && line[0] != '/' && line[0] != '\\' && line[0] != '|')
                {
                    dataLines.Add(line);
                }
            }
            //
            return dataLines.ToArray();
        }
        static private OpenFoamHeader GetFoamHeader(string[] lines, ref int lineId)
        {
            //FoamFile
            //{
            //    version     2.0;
            //    format binary;
            //    arch        "LSB;label=32;scalar=64";
            //    class vectorField;
            //    location    "constant/polyMesh";
            //    object points;
            //}
            string line;
            string[] tmp;
            string[] splitter = new string[] { " ", ";" };
            OpenFoamHeader header = new OpenFoamHeader();
            //
            line = lines[lineId].ToUpper();
            //
            if (line.StartsWith("FOAMFILE"))
            {
                while (!line.StartsWith("}"))
                {
                    //All lines are trimmed: class volScalarField;
                    if (line.StartsWith("VERSION"))
                    {
                        tmp = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length == 2) header.Version = tmp[1];
                    }
                    else if (line.StartsWith("FORMAT"))
                    {
                        tmp = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length == 2)
                        {
                            switch (tmp[1])
                            {
                                case "ASCII":
                                    header.FormatType = OpenFoamFormatType.Ascii;
                                    break;
                                case "BINARY":
                                    header.FormatType = OpenFoamFormatType.Binary;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if (line.StartsWith("CLASS"))
                    {
                        tmp = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        if (tmp.Length == 2)
                        {
                            switch (tmp[1])
                            {
                                case "SURFACESCALARFIELD":
                                    header.FieldType = OpenFoamFieldType.SurfaceScalarField;
                                    break;
                                case "VOLSCALARFIELD":
                                    header.FieldType = OpenFoamFieldType.VolScalarField;
                                    break;
                                case "VOLVECTORFIELD":
                                    header.FieldType = OpenFoamFieldType.VolVectorField;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    lineId++;
                    line = lines[lineId].ToUpper();
                }
            }
            lineId++;
            //
            return header;
        }
    }
}
