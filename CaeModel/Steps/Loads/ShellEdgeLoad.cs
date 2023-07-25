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
    public class ShellEdgeLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private double _magnitude;


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double Magnitude { get { return _magnitude; } set { _magnitude = value; } }


        // Constructors                                                                                                             
        public ShellEdgeLoad(string name, string surfaceName, RegionTypeEnum regionType, double magnitude, bool twoD,
                             bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg) 
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            _magnitude = magnitude;
        }

        // Methods                                                                                                                  
    }
}
