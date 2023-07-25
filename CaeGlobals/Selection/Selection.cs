using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CaeGlobals
{
    [Serializable]
    public class Selection : ISerializable
    {
        // Variables                                                                                                                
        private List<SelectionNode> _nodes;                 //ISerializable
        private vtkSelectItem _selectItem;                  //ISerializable
        private int _currentView;                           //ISerializable
        private int _maxNumberOfIds;                        //ISerializable
        private bool _limitSelectionToFirstPart;            //ISerializable
        private bool _limitSelectionToFirstGeometryType;    //ISerializable
        private bool _limitSelectionToFirstMesherType;      //ISerializable
        private bool _limitSelectionToShellEdges;           //ISerializable
        private bool _enableShellEdgeFaceSelection;         //ISerializable
        

        // Temporary storage for speed optimization: keep current ids; do not copy
        [NonSerialized] private Dictionary<double, int[]> _nodeIds;


        // Properties                                                                                                               
        public List<SelectionNode> Nodes { get { return _nodes; } set { _nodes = value; } }
        public vtkSelectItem SelectItem { get { return _selectItem; } set { _selectItem = value; } }
        public int CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; }
        }
        public int MaxNumberOfIds { get { return _maxNumberOfIds; } set { _maxNumberOfIds = value; } }
        public bool LimitSelectionToFirstPart 
        {
            get { return _limitSelectionToFirstPart; }
            set
            {
                _limitSelectionToFirstPart = value;
                if (_limitSelectionToFirstPart)
                {
                    _limitSelectionToFirstGeometryType = false;
                    _limitSelectionToFirstMesherType = false;
                }
            } 
        }
        public bool LimitSelectionToFirstGeometryType
        {
            get { return _limitSelectionToFirstGeometryType; }
            set
            {
                _limitSelectionToFirstGeometryType = value;
                if (_limitSelectionToFirstGeometryType)
                {
                    _limitSelectionToFirstPart = false;
                    _limitSelectionToFirstMesherType = false;
                }
            }
        }
        public bool LimitSelectionToFirstMesherType
        {
            get { return _limitSelectionToFirstMesherType; }
            set
            {
                _limitSelectionToFirstMesherType = value;
                if (_limitSelectionToFirstMesherType)
                {
                    _limitSelectionToFirstPart = false;
                    _limitSelectionToFirstGeometryType = false;
                }
            }
        }
        public bool LimitSelectionToShellEdges
        {
            get { return _limitSelectionToShellEdges; }
            set { _limitSelectionToShellEdges = value; }
        }
        public bool EnableShellEdgeFaceSelection
        {
            get { return _enableShellEdgeFaceSelection; }
            set { _enableShellEdgeFaceSelection = value; }
        }


        // Constructors                                                                                                             
        public Selection()
        {
            _nodes = new List<SelectionNode>();
            _selectItem = vtkSelectItem.None;
            _currentView = -1;
            _maxNumberOfIds = -1;
            _limitSelectionToFirstPart = false;
            _limitSelectionToFirstGeometryType = false;
            _limitSelectionToFirstMesherType = false;
            _limitSelectionToShellEdges = false;
            _enableShellEdgeFaceSelection = false;
            //
            _nodeIds = null;
        }
        public Selection(SerializationInfo info, StreamingContext context)
        {
            _currentView = -1;                          // Compatibility for version v0.5.2
            _maxNumberOfIds = -1;                       // Compatibility for version v0.8.0
            _limitSelectionToFirstPart = false;         // Compatibility for version v0.9.0
            _limitSelectionToFirstGeometryType = false; // Compatibility for version v0.9.0
            _enableShellEdgeFaceSelection = false;      // Compatibility for version v0.9.0
            //
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "_nodes":
                        _nodes = (List<SelectionNode>)entry.Value; 
                        break;
                    case "_selectItem":
                        _selectItem = (vtkSelectItem)entry.Value; break;
                    case "_currentView":
                        _currentView = (int)entry.Value; break;
                    case "_maxNumberOfIds":
                        _maxNumberOfIds = (int)entry.Value; break;
                    case "_limitSelectionToFirstPart":
                        _limitSelectionToFirstPart = (bool)entry.Value; break;
                    case "_limitSelectionToFirstGeometryType":
                        _limitSelectionToFirstGeometryType = (bool)entry.Value; break;
                    case "_limitSelectionToFirstMesherType":
                        _limitSelectionToFirstMesherType = (bool)entry.Value; break;
                    case "_limitSelectionToShellEdges":
                        _limitSelectionToShellEdges = (bool)entry.Value; break;
                    case "_enableShellEdgeFaceSelection":
                        _enableShellEdgeFaceSelection = (bool)entry.Value; break;
                    default:
                        break;
                }
            }
        }


        // Methods                                                                                                                  
        public void Add(SelectionNodeIds node)
        {
            Add(node, node.ItemIds);
        }
        public void Add(SelectionNode node, int[] ids)
        {
            _nodes.Add(node);
            if (_nodeIds == null) _nodeIds = new Dictionary<double, int[]>();
            //
            double hash;
            Random rnd = new Random((int)DateTime.Now.Ticks);
            do
            {
                hash = rnd.NextDouble();
            }
            while (_nodeIds.ContainsKey(hash));
            node.Hash = hash;
            _nodeIds.Add(hash, ids);
        }
        public bool TryGetNodeIds(SelectionNode node, out int[] ids)
        {
            ids = null;
            if (_nodeIds == null) return false;
            else if (_nodeIds.TryGetValue(node.Hash, out ids)) return true;
            else return false;
        }
        public void RemoveFirst()
        {
            if (_nodes.Count > 0)
            {
                SelectionNode node = _nodes.First();
                if (_nodeIds != null) _nodeIds.Remove(node.Hash);
                _nodes.Remove(node);
            }
        }
        public void RemoveLast()
        {
            if (_nodes.Count > 0)
            {
                SelectionNode node = _nodes.Last();
                if (_nodeIds != null) _nodeIds.Remove(node.Hash);
                _nodes.Remove(node);
            }
        }
        public void Clear()
        {
            _nodes.Clear();
            _nodeIds = null;
            //_selectItem = vtkSelectItem.None; - must not be used!!!
            //_currentView = -1;                - must not be used!!!
        }
        //
        public bool IsGeometryBased()
        {
            foreach (var node in _nodes)
            {
                if (!((node is SelectionNodeIds sni && sni.GeometryIds) || 
                      (node is SelectionNodeMouse snm && snm.IsGeometryBased))) return false;
            }
            return true;
        }
        // ISerialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Using typeof() works also for null fields
            info.AddValue("_nodes", _nodes, typeof(List<SelectionNode>));
            info.AddValue("_selectItem", _selectItem, typeof(vtkSelectItem));
            info.AddValue("_currentView", _currentView, typeof(int));
            info.AddValue("_maxNumberOfIds", _maxNumberOfIds, typeof(int));
            info.AddValue("_limitSelectionToFirstPart", _limitSelectionToFirstPart, typeof(bool));
            info.AddValue("_limitSelectionToFirstGeometryType", _limitSelectionToFirstGeometryType, typeof(bool));
            info.AddValue("_limitSelectionToFirstMesherType", _limitSelectionToFirstMesherType, typeof(bool));
            info.AddValue("_limitSelectionToShellEdges", _limitSelectionToShellEdges, typeof(bool));
            info.AddValue("_enableShellEdgeFaceSelection", _enableShellEdgeFaceSelection, typeof(bool));
        }
    }
}
