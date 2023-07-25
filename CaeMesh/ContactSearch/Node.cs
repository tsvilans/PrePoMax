using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class Node<T> : IComparable<Node<T>> where T : IComparable<T>
    {
        // Variables                                                                                                                
        private T _data;
        private NodeList<T> _neighbors = null;


        // Propeties                                                                                                                  
        public T Value { get { return _data; } set { _data = value; } }
        public NodeList<T> Neighbors
        {
            get
            {
                if (_neighbors == null) _neighbors = new NodeList<T>();
                return _neighbors;
            }
            set { _neighbors = value; }
        }


        // Constructors                                                                                                             
        public Node()
        { }
        public Node(T data)
            : this(data, null)
        { }
        public Node(T data, NodeList<T> neighbors)
        {
            _data = data;
            _neighbors = neighbors;
        }
        public int CompareTo(Node<T> other)
        {
            int n1 = _neighbors == null ? 0 : _neighbors.Count;
            int n2 = other.Neighbors == null ? 0 : other.Neighbors.Count;
            //
            if (n1 < n2)
                return -1;
            else if (n1 > n2)
                return 1;
            else
                return _data.CompareTo(other._data);
        }
    }
}
