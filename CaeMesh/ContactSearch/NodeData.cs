using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class NodeData : IComparable<NodeData>
    {
        // Variables                                                                                                                
        private int _id;
        private string _name;
        private HashSet<int> _itemIds;
        private double _value;


        // Properties                                                                                                               
        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public HashSet<int> ItemIds { get { return _itemIds; } set { _itemIds = value; } }
        public double Value { get { return _value; } }


        // Constructors                                                                                                             
        public NodeData(int id, string name, HashSet<int> data, double value)
        {
            _id = id;
            _name = name;
            _itemIds = data;
            _value = value;
        }
        public int CompareTo(NodeData other)
        {
            // Reverse value for sorting
            if (_value < other._value) return 1;
            else if (_value > other._value) return -1;
            else return 0;
        }
    }

    
}
