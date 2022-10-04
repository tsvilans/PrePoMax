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
    public abstract class VariablePressure : Load
    {
        // Variables                                                                                                                
        protected string _surfaceName;
        private RegionTypeEnum _regionType;


        // Properties                                                                                                               
        public string SurfaceName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override string RegionName { get { return _surfaceName; } set { _surfaceName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        

        // Constructors                                                                                                             
        public VariablePressure(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, twoD)
        {
            _surfaceName = regionName;
            _regionType = regionType;
        }


        // Methods                                                                                                                  
        public abstract double GetPressureForPoint(double[] point);
    }
}
