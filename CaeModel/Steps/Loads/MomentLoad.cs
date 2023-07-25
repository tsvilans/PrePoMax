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
    public class MomentLoad : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;
        private double _m1;
        private double _m2;
        private double _m3;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double M1 { get { return _m1; } set { _m1 = value; if (_twoD) _m1 = 0; } }
        public double M2 { get { return _m2; } set { _m2 = value; if (_twoD) _m2 = 0; } }
        public double M3 { get { return _m3; } set { _m3 = value; } }
        public double GetDirection(int direction)
        {
            if (direction == 0) return M1;
            else if (direction == 1) return M2;
            else if (direction == 2) return M3;
            else throw new NotSupportedException();
        }

        // Constructors                                                                                                             
        public MomentLoad(string name, string regionName, RegionTypeEnum regionType, double m1, double m2, double m3,
                          bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg) 
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;
            //
            M1 = m1;    // account for 2D
            M2 = m2;    // account for 2D
            _m3 = m3;
        }

        public MomentLoad(string name, int nodeId, double m1, double m2, double m3, bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _regionName = null;
            RegionType = RegionTypeEnum.NodeId;
            _nodeId = nodeId;
            //
            M1 = m1;    // account for 2D
            M2 = m2;    // account for 2D
            _m3 = m3;
        }

        // Methods                                                                                                                  
        
    }
}
