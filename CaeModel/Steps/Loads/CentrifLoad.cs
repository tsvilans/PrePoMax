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
    public class CentrifLoad : Load
    {
        // Variables                                                                                                                
        private string _regionName;
        private RegionTypeEnum _regionType;
        public double _x;
        public double _y;
        public double _z;
        public double _n1;
        public double _n2;
        public double _n3;


        // Properties                                                                                                               
        public override string RegionName { get { return _regionName; } set { _regionName = value; } }
        public override RegionTypeEnum RegionType { get { return _regionType; } set { _regionType = value; } }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; if (_twoD) _z = 0; } }
        public double N1 { get { return _n1; } set { _n1 = value; if (_twoD) _n1 = 0; } }
        public double N2 { get { return _n2; } set { _n2 = value; if (_twoD) _n2 = 0; } }
        public double N3 { get { return _n3; } set { _n3 = value; if (_twoD) _n3 = 1; } }
        public double RotationalSpeed2 { get; set; }
        public double RotationalSpeed
        {
            get
            {
                if (RotationalSpeed2 >= 0) return Math.Sqrt(RotationalSpeed2);
                else throw new NotSupportedException();
            } 
            set
            {
                if (value < 0) throw new CaeException("The value of the rotationl speed must be non-negative.");
                else RotationalSpeed2 = Math.Pow(value, 2);
            }
        }


        // Constructors                                                                                                             
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType, bool twoD, bool complex, double phaseDeg)
            : this(name, regionName, regionType, new double[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0, twoD, complex, phaseDeg)
        {
        }
        public CentrifLoad(string name, string regionName, RegionTypeEnum regionType, double[] point, double[] normal,
                           double rotationalSpeed2, bool twoD, bool complex, double phaseDeg)
            : base(name, twoD, complex, phaseDeg)
        {
            _regionName = regionName;
            RegionType = regionType;
            //
            _x = point[0];
            _y = point[1];
            Z = point[2];       // account for 2D
            //
            N1 = normal[0];     // account for 2D
            N2 = normal[1];     // account for 2D
            N3 = normal[2];     // account for 2D
            //
            RotationalSpeed2 = rotationalSpeed2;
        }


        // Methods                                                                                                                  
    }
}
