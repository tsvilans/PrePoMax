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
    public class GravityLoad : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double F1 { get; set; }
        public double F2 { get; set; }
        public double F3 { get; set; }


        // Constructors                                                                                                             
        public GravityLoad(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool complex, double phaseDeg)
            : this(name, regionName, regionType, 0, 0, 0, twoD, complex, phaseDeg)
        {
        }
        public GravityLoad(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3, bool twoD,
                           bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg) 
        {
            _regionName = regionName;
            RegionType = regionType;
            //
            F1 = f1;
            F2 = f2;
            F3 = f3;
        }
        
        
        // Methods                                                                                                                  
    }
}
