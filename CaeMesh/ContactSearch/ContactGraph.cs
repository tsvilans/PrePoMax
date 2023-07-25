using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class ContactGraph
    {
        // Variables                                                                                                                
        private Graph<NodeData> _graph;


        // Constructors                                                                                                             
        public ContactGraph()
        {
            _graph = new Graph<NodeData>();
        }


        // Methods                                                                                                                  
        public void AddMasterSlaveItems(IEnumerable<MasterSlaveItem> masterSlaveItems, FeMesh mesh)
        {
            HashSet<int> mergedItemIds;
            List<HashSet<int>> itemIdsList = new List<HashSet<int>>();
            List<HashSet<int>> itemIdsListToRemove = new List<HashSet<int>>();
            // Collect item ids
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                // Master side                                                          
                mergedItemIds = new HashSet<int>(masterSlaveItem.MasterGeometryIds);
                // Find intersecting
                foreach (var node in itemIdsList)
                {
                    if (node.Intersect(mergedItemIds).Count() > 0)
                    {
                        mergedItemIds.UnionWith(node);
                        itemIdsListToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in itemIdsListToRemove) itemIdsList.Remove(node);
                // Add new/merged item
                itemIdsList.Add(mergedItemIds);
                // Slave side                                                           
                itemIdsListToRemove.Clear();
                mergedItemIds = new HashSet<int>(masterSlaveItem.SlaveGeometryIds);
                // Find intersecting
                foreach (var node in itemIdsList)
                {
                    if (node.Intersect(mergedItemIds).Count() > 0)
                    {
                        mergedItemIds.UnionWith(node);
                        itemIdsListToRemove.Add(node);
                    }
                }
                // Remove merged
                foreach (var node in itemIdsListToRemove) itemIdsList.Remove(node);
                // Add new/merged item
                itemIdsList.Add(mergedItemIds);
            }
            // Add items to graph
            int id;
            string name;
            List<string> allNames = new List<string>();
            double size;
            NodeData nodeData;
            foreach (var itemIds in itemIdsList)
            {
                id = _graph.Nodes.Count() + 1;
                name = GetNameFromItemIds(itemIds, allNames, mesh);
                size = GetSizeFromItemIds(itemIds, mesh);
                nodeData = new NodeData(id, name, itemIds, size);
                //
                _graph.AddNode(new Node<NodeData>(nodeData));
                allNames.Add(name);
            }
            // Add edges to graph
            Node<NodeData> masterNode;
            Node<NodeData> slaveNode;
            foreach (var masterSlaveItem in masterSlaveItems)
            {
                masterNode = null;
                slaveNode = null;
                foreach (Node<NodeData> node in _graph.Nodes)
                {
                    // Find the nodes
                    if (masterNode == null && node.Value.ItemIds.Intersect(masterSlaveItem.MasterGeometryIds).Count() > 0)
                        masterNode = node;
                    else if (slaveNode == null && node.Value.ItemIds.Intersect(masterSlaveItem.SlaveGeometryIds).Count() > 0)
                        slaveNode = node;
                    if (masterNode != null && slaveNode != null) break;
                }
                //
                _graph.AddUndirectedEdge(masterNode, slaveNode);
            }
        }
        //
        public List<MasterSlaveItem> GetMasterSlaveItems(bool checkUnresolved)
        {
            List<Graph<NodeData>> connectedSubgraphList = _graph.GetConnectedSubgraphs();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            //
            foreach (var connectedGraph in connectedSubgraphList)
            {
                if (connectedGraph.IsGraphWithoutCycles())
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithoutCycles(connectedGraph));
                }
                else if (connectedGraph.IsGraphWithOneCycle())
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithOneCycle(connectedGraph));
                }
                else
                {
                    masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithMultipleCycles(connectedGraph, checkUnresolved));
                }
            }
            return masterSlaveItems;
        }
        //
        public static string GetNameFromItemIds(HashSet<int> itemIds, List<string> allNames, FeMesh mesh)
        {
            string name;
            HashSet<int> partIds = new HashSet<int>();
            foreach (var itemId in itemIds)
            {
                partIds.Add(FeMesh.GetPartIdFromGeometryId(itemId));
            }
            if (partIds.Count == 1) name = mesh.GetPartNamesByIds(partIds.ToArray())[0];
            else name = allNames.GetNextNumberedKey("Merged");
            //
            return name;
        }
        public static double GetSizeFromItemIds(HashSet<int> itemIds, FeMesh mesh)
        {
            double size = 0;
            foreach (var itemId in itemIds) size += GetSize(itemId, mesh);
            return size;
        }
        private static double GetSize(int geometryId, FeMesh mesh)
        {
            int[] itemTypePartIds = FeMesh.GetItemTypePartIdsFromGeometryId(geometryId);
            GeometryType geomType = (GeometryType)itemTypePartIds[1];
            VisualizationData vis = mesh.GetPartById(itemTypePartIds[2]).Visualization;
            // Face
            if (geomType == GeometryType.SolidSurface ||
                geomType == GeometryType.ShellFrontSurface ||
                geomType == GeometryType.ShellBackSurface)
            {
                int faceId = itemTypePartIds[0];
                return vis.FaceAreas[faceId] * 1E6;
            }
            // Edge
            else if (geomType == GeometryType.Edge ||
                     geomType == GeometryType.ShellEdgeSurface)
            {
                int edgeId = itemTypePartIds[0];
                return vis.EdgeLengths[edgeId];
            }
            // Vertex
            else return 0;
        }
        //
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithoutCycles(Graph<NodeData> graph)
        {
            Graph<NodeData> reducedGraph;
            return GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph);
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithoutCycles(Graph<NodeData> graph,
                                                                                       out Graph<NodeData> reducedGraph)
        {
            Node<NodeData> neighbour;
            List<Node<NodeData>> singleConnectedNodes = new List<Node<NodeData>>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            reducedGraph = new Graph<NodeData>(graph);
            //
            do
            {
                singleConnectedNodes.Clear();
                //
                foreach (Node<NodeData> node in reducedGraph.Nodes)
                {
                    if (node.Neighbors.Count() == 1)
                    {
                        neighbour = node.Neighbors[0];
                        masterSlaveItems.Add(new MasterSlaveItem(node.Value.Name, neighbour.Value.Name,
                                                                 node.Value.ItemIds, neighbour.Value.ItemIds));
                        //
                        singleConnectedNodes.Add(node);
                        //
                        break;  // this makes master slave parts more organized for the user
                    }
                }
                //
                foreach (var node in singleConnectedNodes)
                {
                    reducedGraph.Remove(node);
                }
            }
            while (singleConnectedNodes.Count > 0);
            //
            return masterSlaveItems;
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithOneCycle(Graph<NodeData> graph)
        {
            Graph<NodeData> reducedGraph;
            List<MasterSlaveItem> masterSlaveItems = GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph);
            //
            Node<NodeData> currentNode;
            Node<NodeData> parentNode;
            Queue<Node<NodeData>> queue = new Queue<Node<NodeData>>();
            Queue<Node<NodeData>> parentQueue = new Queue<Node<NodeData>>();
            HashSet<Node<NodeData>> visitedNodes = new HashSet<Node<NodeData>>();
            //
            queue.Enqueue(reducedGraph.Nodes.First());
            parentQueue.Enqueue(reducedGraph.Nodes.First());
            //
            while (queue.Count() > 0)
            {
                currentNode = queue.Dequeue();
                parentNode = parentQueue.Dequeue();
                // Check for cycles
                if (visitedNodes.Add(currentNode))
                {
                    // Add all neighbours to the queue
                    foreach (var neighbour in currentNode.Neighbors)
                    {
                        if (neighbour != parentNode)
                        {
                            masterSlaveItems.Add(new MasterSlaveItem(currentNode.Value.Name, neighbour.Value.Name,
                                                                     currentNode.Value.ItemIds, neighbour.Value.ItemIds));
                            //
                            queue.Enqueue(neighbour);
                            parentQueue.Enqueue(currentNode);
                            break;  // add only the first neighbor and continue
                        }
                    }
                }
            }
            //
            return masterSlaveItems;
        }
        private static List<MasterSlaveItem> GetMasterSlaveItemsFromGraphWithMultipleCycles(Graph<NodeData> graph,
                                                                                            bool checkUnresolved)
        {
            string prefix = "";
            HashSet<int> ids = new HashSet<int>();
            List<MasterSlaveItem> masterSlaveItems = new List<MasterSlaveItem>();
            // Create an unresolved master slave item that is shown to the user as a single surface
            if (checkUnresolved)
            {
                MasterSlaveItem masterSlaveItem;
                foreach (var node in graph.Nodes) ids.UnionWith(node.Value.ItemIds);
                masterSlaveItem = new MasterSlaveItem("Unresolved", "", ids, null);
                masterSlaveItem.Unresolved = true;
                masterSlaveItems.Add(masterSlaveItem);
                //
                prefix = "Unresolved_";
            }
            // Create master slave items
            Graph<NodeData> reducedGraph;
            masterSlaveItems.AddRange(GetMasterSlaveItemsFromGraphWithoutCycles(graph, out reducedGraph));
            //
            foreach (var node in reducedGraph.Nodes)
            {
                foreach (var neighbor in node.Neighbors)
                {
                    masterSlaveItems.Add(new MasterSlaveItem(prefix + node.Value.Name, neighbor.Value.Name,
                                                             node.Value.ItemIds, neighbor.Value.ItemIds));
                    reducedGraph.RemoveDirectedEdge(neighbor, node);
                }
                node.Neighbors.Clear();
            }
            //
            return masterSlaveItems;
        }
    }
}
