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
    public class CLoad : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        private int _nodeId;
        private double _f1;
        private double _f2;
        private double _f3;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public int NodeId { get { return _nodeId; } set { _nodeId = value; } }
        public double F1 { get { return _f1; } set { _f1 = value; } }
        public double F2 { get { return _f2; } set { _f2 = value; } }
        public double F3 { get { return _f3; } set { _f3 = value; if (_twoD) _f3 = 0; } }
        public double GetComponent(int direction)
        {
            if (direction == 0) return F1;
            else if (direction == 1) return F2;
            else return F3;
        }


        // Constructors                                                                                                             
        public CLoad(string name, string regionName, RegionTypeEnum regionType, double f1, double f2, double f3,
                     bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _regionName = regionName;
            RegionType = regionType;
            _nodeId = -1;
            //
            _f1 = f1;
            _f2 = f2;
            F3 = f3;    // account for 2D
        }
        public CLoad(string name, int nodeId, double f1, double f2, double f3, bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _regionName = null;
            RegionType = RegionTypeEnum.NodeId;
            _nodeId = nodeId;
            //
            _f1 = f1;
            _f2 = f2;
            F3 = f3;    // account for 2D
        }


        // Methods                                                                                                                  
        
    }
}
