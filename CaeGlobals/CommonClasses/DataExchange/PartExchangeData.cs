using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CaeGlobals
{
    [Serializable]
    public class PartExchangeData
    {
        // Variables                                                                                                                
        public NodesExchangeData Nodes;
        public CellsExchangeData Cells;
        public NodesExchangeData ExtremeNodes;              // [0 - min, 1 - max]
        public NodesExchangeData[] NodesAnimation;          // [frame]
        public NodesExchangeData[] ExtremeNodesAnimation;   // [frame][0 - min, 1 - max]
        

        // Constructors                                                                                                             
        public PartExchangeData()
        {
            Nodes = new NodesExchangeData();
            Cells = new CellsExchangeData();
            ExtremeNodes = new NodesExchangeData();
            NodesAnimation = null;
            ExtremeNodesAnimation = null;
        }

        // Methods                                                                                                                  
       
    }
}
