using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CaeMesh;

namespace FileInOut.Input
{
    static public class MmgFileReader
    {
        public static FeMesh Read(string fileName, MeshRepresentation meshRepresentation,
                                  bool convertToSecondOrder = false,
                                  int firstNodeId = 1,
                                  int firstElementId = 1,
                                  Dictionary<int, FeNode> existingNodes = null,
                                  Dictionary<int[], FeNode> existingMidNodes = null,
                                  double epsilon = 1E-6,
                                  Dictionary<string, Dictionary<int, int>> partIdNewSurfIdOldSurfId = null,
                                  Dictionary<string, Dictionary<int, int>> partIdNewEdgeIdOldEdgeId = null)
        {
            if (File.Exists(fileName))
            {
                FeNode node;
                LinearBeamElement beam;
                LinearTriangleElement triangle;
                Dictionary<int, FeNode> nodes = new Dictionary<int, FeNode>();
                Dictionary<int, FeElement> elements = new Dictionary<int, FeElement>();
                HashSet<int> surfaceNodeIds;
                HashSet<int> edgeNodeIds;
                Dictionary<int, HashSet<int>> surfaceIdNodeIds = new Dictionary<int, HashSet<int>>();
                Dictionary<int, HashSet<int>> edgeIdNodeIds = new Dictionary<int, HashSet<int>>();
                Dictionary<int, Dictionary<int, bool>> edgeIdNodeIdCount = new Dictionary<int, Dictionary<int, bool>>();
                Dictionary<int, bool> nodeIdCount;
                Dictionary<int, int> oldNodeIdNewNodeId = new Dictionary<int, int>();
                //
                int numOfNodes;
                int numOfElements;
                int numOfEdges;
                int elementId = firstElementId;
                int possibleNodeId;
                string[] splitter = new string[] { " " };
                string[] tmp;
                string[] lines = CaeGlobals.Tools.ReadAllLines(fileName);
                int surfaceId;
                int edgeId;
                //
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].ToUpper().StartsWith("VERTICES"))
                    {
                        i++;
                        if (i < lines.Length)
                        {
                            numOfNodes = int.Parse(lines[i]);
                            i++;
                            for (int j = 0; j < numOfNodes && i + j < lines.Length; j++)
                            {
                                tmp = lines[i + j].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                if (tmp.Length >= 3)
                                {
                                    node = new FeNode();
                                    node.Id = j + firstNodeId;
                                    node.X = double.Parse(tmp[0]);
                                    node.Y = double.Parse(tmp[1]);
                                    node.Z = double.Parse(tmp[2]);
                                    possibleNodeId = int.Parse(tmp[3]);
                                    // If the node id is not equal to 0 check if the node exists by coordinates
                                    if (existingNodes != null && possibleNodeId != 0)
                                        node.Id = GetExistingNodeId(node, possibleNodeId, existingNodes, epsilon);
                                    nodes.Add(node.Id, node);
                                    //
                                    oldNodeIdNewNodeId.Add(j + 1, node.Id);
                                }
                            }
                        }
                    }
                    else if (lines[i].ToUpper().StartsWith("TRIANGLES"))
                    {
                        i++;
                        if (i < lines.Length)
                        {
                            numOfElements = int.Parse(lines[i]);
                            i++;
                            for (int j = 0; j < numOfElements && i + j < lines.Length; j++)
                            {
                                tmp = lines[i + j].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                if (tmp.Length >= 4)
                                {
                                    triangle = new LinearTriangleElement(elementId++, new int[3]);
                                    triangle.NodeIds[0] = oldNodeIdNewNodeId[int.Parse(tmp[0])];
                                    triangle.NodeIds[1] = oldNodeIdNewNodeId[int.Parse(tmp[1])];
                                    triangle.NodeIds[2] = oldNodeIdNewNodeId[int.Parse(tmp[2])];
                                    surfaceId = int.Parse(tmp[3]) - 1;
                                    //
                                    elements.Add(triangle.Id, triangle);
                                    if (surfaceIdNodeIds.TryGetValue(surfaceId, out surfaceNodeIds))
                                        surfaceNodeIds.UnionWith(triangle.NodeIds);
                                    else
                                        surfaceIdNodeIds.Add(surfaceId, new HashSet<int>(triangle.NodeIds));
                                }
                            }
                        }
                    }
                    else if (lines[i].ToUpper().StartsWith("EDGES"))
                    {
                        i++;
                        if (i < lines.Length)
                        {
                            numOfEdges = int.Parse(lines[i]);
                            i++;
                            for (int j = 0; j < numOfEdges && i + j < lines.Length; j++)
                            {
                                tmp = lines[i + j].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                if (tmp.Length >= 3)
                                {
                                    edgeId = int.Parse(tmp[2]);
                                    if (edgeId > 0)
                                    {
                                        edgeId--;
                                        //
                                        if (!edgeIdNodeIdCount.TryGetValue(edgeId, out nodeIdCount))
                                        {
                                            nodeIdCount = new Dictionary<int, bool>();
                                            edgeIdNodeIdCount.Add(edgeId, nodeIdCount);
                                        }
                                        //
                                        beam = new LinearBeamElement(elementId++, new int[2]);
                                        beam.NodeIds[0] = oldNodeIdNewNodeId[int.Parse(tmp[0])];
                                        beam.NodeIds[1] = oldNodeIdNewNodeId[int.Parse(tmp[1])];
                                        // If node already on the edge remove it
                                        if (!nodeIdCount.Remove(beam.NodeIds[0])) nodeIdCount.Add(beam.NodeIds[0], true);
                                        if (!nodeIdCount.Remove(beam.NodeIds[1])) nodeIdCount.Add(beam.NodeIds[1], true);
                                        //
                                        elements.Add(beam.Id, beam);
                                        if (edgeIdNodeIds.TryGetValue(edgeId, out edgeNodeIds))
                                            edgeNodeIds.UnionWith(beam.NodeIds);
                                        else
                                            edgeIdNodeIds.Add(edgeId, new HashSet<int>(beam.NodeIds));
                                    }
                                }
                            }
                        }
                    }
                }
                // Count number of edges in an edge node
                Dictionary<int, int> nodeIdEdgeCount = new Dictionary<int, int>();
                foreach (var edgeEntry in edgeIdNodeIdCount)
                {
                    foreach (var nodeEntry in edgeEntry.Value)
                    {
                        if (nodeIdEdgeCount.ContainsKey(nodeEntry.Key)) nodeIdEdgeCount[nodeEntry.Key]++;
                        else nodeIdEdgeCount.Add(nodeEntry.Key, 1);
                    }
                }
                // Get vertices
                HashSet<int> vertexNodeIds = new HashSet<int>();
                foreach (var entry in nodeIdEdgeCount)
                {
                    if (entry.Value > 1) vertexNodeIds.Add(entry.Key);
                }
                //
                if (convertToSecondOrder) FeMesh.LinearToParabolic(ref nodes, ref elements, existingMidNodes);
                //
                FeMesh mesh = new FeMesh(nodes, elements, meshRepresentation, null, null, false,
                                         ImportOptions.DetectEdges);
                //
                mesh.ConvertLineFeElementsToEdges(vertexNodeIds);
                // Collect surfaceIdNodeIds for each part
                var allPartsSurfaceIdNodeIds = new Dictionary<string, Dictionary<int, HashSet<int>>>();
                Dictionary<int, HashSet<int>> partSurfaceIdNodeIds;
                HashSet<int> intersect;
                foreach (var partEntry in mesh.Parts)
                {
                    partSurfaceIdNodeIds = new Dictionary<int, HashSet<int>>();
                    if (partEntry.Value.PartType != PartType.Wire)
                    {
                        foreach (var surfaceEntry in surfaceIdNodeIds)
                        {
                            intersect = new HashSet<int>(surfaceEntry.Value.Intersect(partEntry.Value.NodeLabels));
                            partSurfaceIdNodeIds.Add(surfaceEntry.Key, intersect);
                        }
                    }
                    allPartsSurfaceIdNodeIds.Add(partEntry.Key, partSurfaceIdNodeIds);
                }
                // Renumber surfaces                                                                        
                mesh.RenumberVisualizationSurfaces(allPartsSurfaceIdNodeIds, null, partIdNewSurfIdOldSurfId);
                // Collect edgeIdNodeIds for each part
                var allPartsEdgeIdNodeIds = new Dictionary<string, Dictionary<int, HashSet<int>>>();
                Dictionary<int, HashSet<int>> partEdgeIdNodeIds;
                foreach (var partEntry in mesh.Parts)
                {
                    partEdgeIdNodeIds = new Dictionary<int, HashSet<int>>();
                    if (partEntry.Value.PartType != PartType.Wire)
                    {
                        foreach (var edgeEntry in edgeIdNodeIds)
                        {
                            intersect = new HashSet<int>(edgeEntry.Value.Intersect(partEntry.Value.NodeLabels));
                            partEdgeIdNodeIds.Add(edgeEntry.Key, intersect);
                        }
                    }
                    allPartsEdgeIdNodeIds.Add(partEntry.Key, partEdgeIdNodeIds);
                }
                // Renumber edges                                                                        
                mesh.RenumberVisualizationEdges(allPartsEdgeIdNodeIds, partIdNewEdgeIdOldEdgeId);
                //
                mesh.RemoveElementsByType<FeElement1D>();
                //
                return mesh;
            }
            //
            return null;
        }
        private static int GetExistingNodeId(FeNode node, int possibleId, Dictionary<int, FeNode> existingNodes, double epsilon)
        {
            FeNode node2;
            // Check if the node ids represent the same coordinates
            if (existingNodes.TryGetValue(possibleId, out node2))
            {
                if (Math.Abs(node.X - node2.X) < epsilon)
                {
                    if (Math.Abs(node.Y - node2.Y) < epsilon)
                    {
                        if (Math.Abs(node.Z - node2.Z) < epsilon)
                        {
                            return node2.Id;
                        }
                    }
                }
            }
            // Search for the same coordinates
            foreach (var entry in existingNodes)
            {
                if (Math.Abs(node.X - entry.Value.X) < epsilon)
                {
                    if (Math.Abs(node.Y - entry.Value.Y) < epsilon)
                    {
                        if (Math.Abs(node.Z - entry.Value.Z) < epsilon)
                        {
                            return entry.Key;
                        }
                    }
                }
            }
            //
            return node.Id;
        }
    }
}
