using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class FixedBC : BoundaryCondition
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public FixedBC(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0)
        {
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            if (_twoD) return new int[] { 1, 2, 6 };
            else return new int[] { 1, 2, 3, 4, 5, 6 };
        }
    }
}
