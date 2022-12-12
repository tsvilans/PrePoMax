using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;
using CaeGlobals;
using vtkControl;

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
    //                                                                      
    // https://doc.cfd.direct/openfoam/user-guide-v6/basic-file-format      
    //                                                                      
    public static class OpenFoamFileReader
    {
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
                    if (File.Exists(pointsFile) && File.Exists(boundaryFile) && File.Exists(facesFile)  && File.Exists(ownerFile))
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
                        int numOfAllNodes;
                        Dictionary<int, int> nodeIdsLookUp;
                        Dictionary<int, FeNode> nodes = GetNodes(pointsFile, allFaceNodeIds, out numOfAllNodes, out nodeIdsLookUp);
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
                                    GetField(resultFileName, sortedTime, globalIncrementId, stepId, numOfAllNodes,
                                             cellIdNodeIds, nodeIdsLookUp, nodeIdCellCount, out fieldData, out field);                                    
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
                        int numOfAllNodes;
                        Dictionary<int, int> nodeIdsLookUp;
                        Dictionary<int, FeNode> nodes = GetNodes(pointsFile, allFaceNodeIds, out numOfAllNodes, out nodeIdsLookUp);
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
                                    GetField(resultFileName, time, globalIncrementId, stepId, numOfAllNodes, cellIdNodeIds,
                                             nodeIdsLookUp, nodeIdCellCount, out fieldData, out field);
                                    //
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
        static public Dictionary<string, string[]> GetTimeResultScalarVariableNames(string fileName)
        {
            Dictionary<string, string[]> timeResultVariableNames = new Dictionary<string, string[]>();
            //
            if (fileName != null && File.Exists(fileName))
            {
                OpenFoamHeader header;
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
                        header = GetHeaderFromFile(resultFileName);
                        if (header.FieldType == OpenFoamFieldType.VolScalarField)
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
                                                        out int numOfAllNodes, out Dictionary<int, int> nodeIdsLookUp)
        {
            OpenFoamHeader header = GetHeaderFromFile(fileName);
            //
            if (header.FormatType == OpenFoamFormatType.Ascii)
                return GetNodesAscii(fileName, allFaceNodeIds, out numOfAllNodes, out nodeIdsLookUp);
            else if (header.FormatType == OpenFoamFormatType.Binary)
                return GetNodesBinary(fileName, allFaceNodeIds, out numOfAllNodes, out nodeIdsLookUp);
            else throw new NotSupportedException();
        }
        static private Dictionary<int, FeNode> GetNodesAscii(string fileName, HashSet<int> allFaceNodeIds,
                                                             out int numOfAllNodes, out Dictionary<int, int> nodeIdsLookUp)
        {
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")" };
            FeNode node;
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            nodeIdsLookUp = new Dictionary<int, int>();
            //
            numOfAllNodes = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            for (int i = 0; i < numOfAllNodes; i++)
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
        static private Dictionary<int, FeNode> GetNodesBinary(string fileName, HashSet<int> allFaceNodeIds,
                                                             out int numOfAllNodes, out Dictionary<int, int> nodeIdsLookUp)
        {
            numOfAllNodes = -1;
            double[] nodeData = null;
            Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
            nodeIdsLookUp = new Dictionary<int, int>();
            //
            if (!Tools.WaitForFileToUnlock(fileName, 5000)) return nodes;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                // Skip header
                if (SkipHeader(binaryReader))
                {
                    // Nodes
                    numOfAllNodes = GetNextIntFromLine(binaryReader);
                    nodeData = ReadDoubleArray(binaryReader, 3 * numOfAllNodes);
                }
                binaryReader.Close();
                fileStream.Close();
            }
            //
            if (nodeData != null)
            {
                FeNode node;
                for (int i = 0; i < numOfAllNodes; i++)
                {
                    if (allFaceNodeIds.Contains(i))
                    {
                        node = new FeNode(i, nodeData[3 * i], nodeData[3 * i + 1], nodeData[3 * i + 2]);
                        nodes.Add(i, node);
                        nodeIdsLookUp.Add(i, nodeIdsLookUp.Count);
                    }
                }
            }
            //
            return nodes;
        }
        static private HashSet<int> GetBoundaryFaceIds(string fileName)
        {
            // Ascii and binary boundary file is the same
            //OpenFoamHeader boundaryHeader = GetHeaderFromFile(fileName);
            //
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            GetFoamHeader(lines, ref lineId);
            //
            int numOfBoundaries;
            string line;
            // Read the number of boundaries
            numOfBoundaries = int.Parse(lines[lineId++].Split('(')[0]);
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
        //
        static private Dictionary<int, FeElement> GetBoundariesAsTriangles(string fileName, HashSet<int> boundaryFaceIds,
                                                                           out Dictionary<int, HashSet<int>> faceIdNodeIds,
                                                                           out HashSet<int> allFaceNodeIds)
        {
            OpenFoamHeader header = GetHeaderFromFile(fileName);
            //
            if (header.FormatType == OpenFoamFormatType.Ascii)
                return GetBoundariesAsTrianglesAscii(fileName, boundaryFaceIds, out faceIdNodeIds, out allFaceNodeIds);
            else if (header.FormatType == OpenFoamFormatType.Binary)
                return GetBoundariesAsTrianglesBinary(fileName, boundaryFaceIds, out faceIdNodeIds, out allFaceNodeIds);
            else throw new NotSupportedException();
        }
        static private Dictionary<int, FeElement> GetBoundariesAsTrianglesAscii(string fileName, HashSet<int> boundaryFaceIds,
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
        static private Dictionary<int, FeElement> GetBoundariesAsTrianglesBinary(string fileName, HashSet<int> boundaryFaceIds,
                                                                                 out Dictionary<int, HashSet<int>> faceIdNodeIds,
                                                                                 out HashSet<int> allFaceNodeIds)
        {
            int numOfFaces = -1;
            int[] indices = null;       // 3 faces      :   {0, 3, 6, 8} -> 0..3, 3..6, 6..8
            int[] allNodeIds = null;    // all node ids :   {0, 1, 2, 5, 6, 8, 12, 13, 14}
            faceIdNodeIds = new Dictionary<int, HashSet<int>>();
            allFaceNodeIds = new HashSet<int>();
            Dictionary<int, FeElement> triangles = new Dictionary<int, FeElement>();
            //
            if (!Tools.WaitForFileToUnlock(fileName, 5000)) return triangles;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                // Skip header
                if (SkipHeader(binaryReader))
                {
                    // Offsets
                    int numOfIndices = GetNextIntFromLine(binaryReader);
                    indices = ReadIntArray(binaryReader, numOfIndices);
                    numOfFaces = numOfIndices - 1;   // remove the last index which determines the end index of the last face
                    // Ids
                    int numOfNodeIds = GetNextIntFromLine(binaryReader);
                    allNodeIds = ReadIntArray(binaryReader, numOfNodeIds);
                }
                binaryReader.Close();
                fileStream.Close();
            }
            //
            if (indices != null && allNodeIds != null)
            {
                int[] nodeIds;
                int[] faceNodeIds;
                int triangleId = 0;
                HashSet<int> faceNodeIdsHash;
                LinearTriangleElement triangle;
                for (int i = 0; i < numOfFaces; i++)
                {
                    if (boundaryFaceIds.Contains(i))
                    {
                        faceNodeIds = new int[indices[i + 1] - indices[i]];
                        for (int j = 0; j < faceNodeIds.Length; j++) faceNodeIds[j] = allNodeIds[indices[i] + j];
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
                }
            }
            //
            return triangles;
        }
        //
        static private Dictionary<int, HashSet<int>> GetCellIdNodeIds(string fileName, HashSet<int> boundaryFaceIds,
                                                                      Dictionary<int, HashSet<int>> faceIdNodeIds)
        {
            OpenFoamHeader header = GetHeaderFromFile(fileName);
            //
            if (header.FormatType == OpenFoamFormatType.Ascii)
                return GetCellIdNodeIdsAscii(fileName, boundaryFaceIds, faceIdNodeIds);
            else if (header.FormatType == OpenFoamFormatType.Binary)
                return GetCellIdNodeIdsBinary(fileName, boundaryFaceIds, faceIdNodeIds);
            else throw new NotSupportedException();
        }
        static private Dictionary<int, HashSet<int>> GetCellIdNodeIdsAscii(string fileName, HashSet<int> boundaryFaceIds,
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
            int numOfFaces;
            HashSet<int> cellNodeIds;
            Dictionary<int, HashSet<int>> cellIdNodeIds = new Dictionary<int, HashSet<int>>();
            // Read number of faces
            numOfFaces = int.Parse(lines[lineId++]);
            // Skip bracket
            if (lines[lineId].StartsWith("(")) lineId++;
            //
            for (int i = 0; i < numOfFaces; i++)
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
        static private Dictionary<int, HashSet<int>> GetCellIdNodeIdsBinary(string fileName, HashSet<int> boundaryFaceIds,
                                                                           Dictionary<int, HashSet<int>> faceIdNodeIds)
        {
            int numOfFaces = -1;
            int[] faceIdCellIds = null;
            HashSet<int> cellNodeIds;
            Dictionary<int, HashSet<int>> cellIdNodeIds = new Dictionary<int, HashSet<int>>();
            //
            if (!Tools.WaitForFileToUnlock(fileName, 5000)) return cellIdNodeIds;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                // Skip header
                if (SkipHeader(binaryReader))
                {
                    // Face id - Cell id
                    numOfFaces = GetNextIntFromLine(binaryReader);
                    faceIdCellIds = ReadIntArray(binaryReader, numOfFaces);
                }
                binaryReader.Close();
                fileStream.Close();
            }
            //
            if (faceIdCellIds != null)
            {
                int cellId;
                for (int i = 0; i < numOfFaces; i++)
                {
                    if (boundaryFaceIds.Contains(i))
                    {
                        cellId = faceIdCellIds[i];
                        if (cellIdNodeIds.TryGetValue(cellId, out cellNodeIds)) cellNodeIds.UnionWith(faceIdNodeIds[i]);
                        else cellIdNodeIds.Add(cellId, new HashSet<int>(faceIdNodeIds[i]));
                    }
                }
            }
            //
            return cellIdNodeIds;
        }
        //
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
                                     int numOfAllNodes,
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
            // Header
            OpenFoamHeader header = GetHeaderFromFile(fileName);
            // This field types are not supported
            if (header.FieldType == OpenFoamFieldType.Unknown || header.FieldType == OpenFoamFieldType.SurfaceScalarField)
                return;
            // Read values
            float[][] cellValues;
            if (header.FormatType == OpenFoamFormatType.Ascii)
                cellValues = GetCellValuesAscii(fileName, header, numOfAllNodes);
            else if (header.FormatType == OpenFoamFormatType.Binary)
                cellValues = GetCellValuesBinary(fileName, header, numOfAllNodes);
            else throw new NotSupportedException();
            //
            if (cellValues != null)
            {
                int numOfComponents = cellValues.Length;
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
                // Add vector magnitude
                if (header.FieldType == OpenFoamFieldType.VolVectorField && nodeValues.Length == 3 && nodeValues[0] != null)
                {
                    componentName = "ALL";
                    float[] magnitude = new float[nodeValues[0].Length];
                    for (int i = 0; i < nodeValues[0].Length; i++)
                    {
                        magnitude[i] = (float)Math.Sqrt(nodeValues[0][i] * nodeValues[0][i] +
                                                        nodeValues[1][i] * nodeValues[1][i]+
                                                        nodeValues[2][i] * nodeValues[2][i]);
                    }
                    field.AddComponent(componentName, magnitude);
                }
                // Add components
                for (int i = 0; i < numOfComponents; i++)
                {
                    if (numOfComponents == 1) componentName = "VAL";
                    else componentName = "VAL" + (i + 1).ToString();
                    field.AddComponent(componentName, nodeValues[i]);
                }
            }
        }
        static private float[][] GetCellValuesAscii(string fileName, OpenFoamHeader header, int numOfAllNodes)
        {
            // Read file
            string[] lines = Tools.ReadAllLines(fileName);
            //
            int lineId = 0;
            lines = SkipCommentsAndEmptyLines(lines);
            //
            string[] tmp;
            string[] splitter = new string[] { "(", ",", " ", ")", ";" };
            int numOfCells;
            int numOfComponents = -1;
            float[][] cellValues = null;
            string line;
            // Skip keywords until: internalField   nonuniform List<scalar>
            while (!lines[lineId].ToUpper().StartsWith("INTERNALFIELD")) lineId++;
            //
            line = lines[lineId].ToUpper();
            if (line.Contains("NONUNIFORM"))
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
                // internalField   nonuniform List<scalar>
                if (header.FieldType == OpenFoamFieldType.VolScalarField)
                {
                    for (int i = 0; i < numOfCells; i++)
                    {
                        cellValues[0][i] = float.Parse(lines[lineId]);
                        lineId++;
                    }
                }
                // internalField   nonuniform List<vector> 
                else if (header.FieldType == OpenFoamFieldType.VolVectorField)
                {
                    for (int i = 0; i < numOfCells; i++)
                    {
                        tmp = lines[lineId].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < numOfComponents; j++) cellValues[j][i] = float.Parse(tmp[j]);
                        lineId++;
                    }
                }
            }
            // internalField   uniform 1
            else if (line.Contains("UNIFORM"))
            {
                cellValues = ReadUniformField(line, numOfAllNodes);
            }
            //
            return cellValues;
        }
        static private float[][] GetCellValuesBinary(string fileName, OpenFoamHeader header, int numOfAllNodes)
        {
            int numOfValues = -1;
            int numOfComponents = -1;
            double[] values = null;
            float[][] cellValues = null;
            //
            if (!Tools.WaitForFileToUnlock(fileName, 5000)) return cellValues;
            //
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                // Skip header
                if (SkipHeader(binaryReader))
                {
                    // Skip keywords until: internalField   nonuniform List<scalar>
                    string line;
                    do line = ReadLineFromBinaryReader(binaryReader).ToUpper();
                    while (!line.StartsWith("INTERNALFIELD"));
                    //
                    if (line.Contains("NONUNIFORM"))
                    {
                        numOfValues = GetNextIntFromLine(binaryReader);
                        // internalField   nonuniform List<scalar>
                        if (header.FieldType == OpenFoamFieldType.VolScalarField)
                        {
                            numOfComponents = 1;
                            values = ReadDoubleArray(binaryReader, numOfValues);
                        }
                        // internalField   nonuniform List<vector> 
                        else if (header.FieldType == OpenFoamFieldType.VolVectorField)
                        {
                            numOfComponents = 3;
                            values = ReadDoubleArray(binaryReader, numOfComponents * numOfValues);
                        }
                    }
                    // internalField   uniform 1
                    else if (line.Contains("UNIFORM"))
                    {
                        return ReadUniformField(line, numOfAllNodes);
                    }
                }
                binaryReader.Close();
                fileStream.Close();
                //
                if (values != null)
                {
                    cellValues = new float[numOfComponents][];
                    for (int i = 0; i < numOfComponents; i++)
                    {
                        cellValues[i] = new float[numOfValues];
                        for (int j = 0; j < cellValues[i].Length; j++)
                        {
                            cellValues[i][j] = (float)values[numOfComponents * j + i];
                        }
                    }
                }
            }
            //
            return cellValues;
        }
        static private float[][] ReadUniformField(string line, int numOfAllNodes)
        {
            float[][] cellValues = null;
            string[] splitter = new string[] { "(", ",", " ", ")", ";" };
            string[] tmp = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length >= 2)
            {
                float value = float.Parse(tmp[2]);
                int numOfComponents = 1;
                cellValues = new float[numOfComponents][];
                for (int i = 0; i < numOfComponents; i++)
                {
                    cellValues[i] = new float[numOfAllNodes];
                    for (int j = 0; j < cellValues[i].Length; j++) cellValues[i][j] = value;
                }
            }
            return cellValues;
        }
        // File
        static private string[] SkipCommentsAndEmptyLines(string[] lines)
        {
            bool containsCommentDelimiters;
            bool singleLineComment = false;
            bool multiLineComment = false;
            string line;
            StringBuilder sb = new StringBuilder();
            List<string> dataLines = new List<string>(lines.Length);
            //
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Trim();
                // Skip empty lines
                if (line.Length == 0) continue;
                // Comments
                singleLineComment = false;
                containsCommentDelimiters = false;
                //
                if (line.Length >= 2)
                {
                    for (int j = 0; j < line.Length - 1; j++)   // - 1 since the last char is inspected using j + 1
                    {
                        if (line[j] == '/')
                        {
                            if (line[j + 1] == '/')
                            {
                                containsCommentDelimiters = true;
                                break;
                            }
                            else if (line[j + 1] == '*')
                            {
                                containsCommentDelimiters = true;
                                break;
                            }
                        }
                        else if (line[j] == '*' && line[j + 1] == '/')
                        {
                            containsCommentDelimiters = true;
                            break;
                        }
                    }
                }
                // Add a line without comments
                if (!multiLineComment && !containsCommentDelimiters)
                {
                    dataLines.Add(line);
                }
                // Add a line with comments
                else
                {
                    sb.Clear();
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (line[j] == '/' && j + 1 < line.Length)
                        { 
                            if (line[j + 1] == '/')
                            {
                                if (!multiLineComment) singleLineComment = true;
                            }
                            else if (line[j + 1] == '*')
                            {
                                if (!singleLineComment) multiLineComment = true;
                            }
                        }
                        else if (line[j] == '*' && j + 1 < line.Length && line[j + 1] == '/')
                        {
                            if (!singleLineComment && multiLineComment)
                            {
                                multiLineComment = false;
                                j++;
                                continue; // skip end comment characters
                            }
                        }
                        // Add non commented character
                        if (!singleLineComment && !multiLineComment) sb.Append(line[j]);
                    }
                    // Add line
                    if (sb.Length > 0) dataLines.Add(sb.ToString().Trim());
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
        // Binary file
        static private bool SkipHeader(BinaryReader reader)
        {
            bool foamFile = false;
            bool endBracket = false;
            int lineCount = 0;
            string line;
            //
            while(reader.BaseStream.Position < reader.BaseStream.Length)
            {
                line = ReadLineFromBinaryReader(reader).ToUpper().TrimStart();
                if (line == null) return false;
                lineCount++;
                //
                if (line.StartsWith("FOAMFILE")) foamFile = true;
                if (line.StartsWith("}")) endBracket = true;
                //
                if (lineCount > 15 && !foamFile) break;
                if (endBracket)
                {
                    line = ReadLineFromBinaryReader(reader);
                    if (line == null) return false;
                    else return true;
                }
            }
            return false;
        }
        static public string ReadLineFromBinaryReader(BinaryReader reader)
        {
            if (reader.PeekChar() == -1) return null;
            //
            char c = ' ';
            StringBuilder sb = new StringBuilder();
            while (ReadChar(reader, ref c))
            {
                if (c == '\r' || c == '\n') return sb.ToString();
                else sb.Append(c);
            }
            //
            if (sb.Length > 0) return sb.ToString();
            else return null;
        }
        static private bool ReadChar(BinaryReader reader, ref char c)
        {
            if (reader.PeekChar() == -1) return false;
            c = reader.ReadChar();
            return true;
        }
        static private int GetNextIntFromLine(BinaryReader binaryReader)
        {
            int value = -1;
            string line;
            while (true)
            {
                line = ReadLineFromBinaryReader(binaryReader);
                if (line == null) return value; // end of stream
                if (int.TryParse(line, out value)) return value;
            }
        }
        static private int[] ReadIntArray(BinaryReader binaryReader, int numOfItems)
        {
            // Read parenthesis
            binaryReader.ReadChar();
            // Read data
            byte[] bytes = new byte[numOfItems * sizeof(int)];
            bytes = binaryReader.ReadBytes(bytes.Length);
            int[] items = new int[numOfItems];
            Buffer.BlockCopy(bytes, 0, items, 0, bytes.Length);
            // Read parenthesis
            binaryReader.ReadChar();
            //
            return items;
        }
        static private double[] ReadDoubleArray(BinaryReader binaryReader, int numOfItems)
        {
            // Read parenthesis
            binaryReader.ReadChar();
            // Read data
            byte[] bytes = new byte[numOfItems * sizeof(double)];
            bytes = binaryReader.ReadBytes(bytes.Length);
            double[] items = new double[numOfItems];
            Buffer.BlockCopy(bytes, 0, items, 0, bytes.Length);
            // Read parenthesis
            binaryReader.ReadChar();
            //
            return items;
        }
    }
}
