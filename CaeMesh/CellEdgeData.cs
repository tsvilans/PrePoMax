using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaeMesh
{
    [Serializable]
    public class CellEdgeData
    {
        public int[] Key;
        public int[] NodeIds;
        public List<int> CellIds;
    }
}
