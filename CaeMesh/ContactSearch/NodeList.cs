using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeGlobals;

namespace CaeMesh
{
    public class NodeList<T> : List<Node<T>> where T : IComparable<T>
    {
        // Constructors                                                                                                             
        public NodeList()
            : base()
        { }


        // Methods                                                                                                                  
        public NodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++) Add(default(Node<T>));
        }
        public Node<T> FindByValue(T value)
        {
            // Search the list for the value
            foreach (Node<T> node in this)
            {
                if (node.Value.Equals(value)) return node;
            }
            // If we reached here, we didn't find a matching node
            return null;
        }
    }
}
