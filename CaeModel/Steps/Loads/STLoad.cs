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
    public class STLoad : Load
    {
        // Variables                                                                                                                
        private string _surfaceName;
        private RegionTypeEnum _regionType;
        private double _f1;
        private double _f2;
        private double _f3;


        // Properties                                                                                                               
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public double F1 { get { return _f1; } set { _f1 = value; } }
        public double F2 { get { return _f2; } set { _f2 = value; } }
        public double F3 { get { return _f3; } set { _f3 = value; if (_twoD) _f3 = 0; } }


        // Constructors                                                                                                             
        public STLoad(string name, string surfaceName, RegionTypeEnum regionType, double f1, double f2, double f3, bool twoD,
                      bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _surfaceName = surfaceName;
            _regionType = regionType;
            //
            _f1 = f1;
            _f2 = f2;
            F3 = f3;    // account for 2D
        }


        // Methods                                                                                                                  
        
    }
}
