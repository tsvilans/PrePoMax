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
    public enum DOFType
    {
        Free,
        Zero,
        Fixed,
        Prescribed
    }

    [Serializable]
    public class DisplacementRotation : BoundaryCondition
    {
        // Variables                                                                                                                
        private double _u1;
        private double _u2;
        private double _u3;
        private double _ur1;
        private double _ur2;
        private double _ur3;


        // Properties                                                                                                               
        public double U1 { get { return _u1; } set { _u1 = value; } }
        public double U2 { get { return _u2; } set { _u2 = value; } }
        public double U3 { get { return _u3; } set { _u3 = value; if (_twoD) _u3 = double.NaN; } }
        public double UR1 { get { return _ur1; } set { _ur1 = value; if (_twoD) _ur1 = double.NaN; } }
        public double UR2 { get { return _ur2; } set { _ur2 = value; if (_twoD) _ur2 = double.NaN; } }
        public double UR3 { get { return _ur3; } set { _ur3 = value; } }


        // Constructors                                                                                                             
        public DisplacementRotation(string name, string regionName, RegionTypeEnum regionType, bool twoD,
                                    bool complex, double phaseDeg)
            : base(name, regionName, regionType, twoD, complex, phaseDeg)
        {
            U1 = double.NaN;
            U2 = double.NaN;
            U3 = double.NaN;
            UR1 = double.NaN;
            UR2 = double.NaN;
            UR3 = double.NaN;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (!double.IsNaN(_u1)) directions.Add(1);
            if (!double.IsNaN(_u2)) directions.Add(2);
            if (!double.IsNaN(_u3)) directions.Add(3);
            if (!double.IsNaN(_ur1)) directions.Add(4);
            if (!double.IsNaN(_ur2)) directions.Add(5);
            if (!double.IsNaN(_ur3)) directions.Add(6);
            return directions.ToArray();
        }
        public int GetConstraintHash()
        {
            int[] dir = GetConstrainedDirections();
            int hash = 0;
            for (int i = 0; i < dir.Length; i++) hash += (int)Math.Pow(2, dir[i]);
            return hash;
        }
        public double[] GetConstrainValues()
        {
            List<double> values = new List<double>();
            if (!double.IsNaN(_u1)) values.Add(_u1);
            if (!double.IsNaN(_u2)) values.Add(_u2);
            if (!double.IsNaN(_u3)) values.Add(_u3);
            if (!double.IsNaN(_ur1)) values.Add(_ur1);
            if (!double.IsNaN(_ur2)) values.Add(_ur2);
            if (!double.IsNaN(_ur3)) values.Add(_ur3);
            return values.ToArray();
        }
        public bool IsFreeFixedOrZero()
        {
            for (int i = 1; i <= 6; i++)
            {
                if (GetDofType(i) == DOFType.Prescribed) return false;
            }
            //
            return true;
        }
        public int[] GetFixedDirections()
        {
            List<int> directions = new List<int>();
            if (double.IsPositiveInfinity(_u1)) directions.Add(1);
            if (double.IsPositiveInfinity(_u2)) directions.Add(2);
            if (double.IsPositiveInfinity(_u3)) directions.Add(3);
            if (double.IsPositiveInfinity(_ur1)) directions.Add(4);
            if (double.IsPositiveInfinity(_ur2)) directions.Add(5);
            if (double.IsPositiveInfinity(_ur3)) directions.Add(6);
            return directions.ToArray();
        }
        public bool IsProperlyDefined(out string error)
        {
            error = "";
            if (GetConstrainedDirections().Length + GetFixedDirections().Length == 0)
            {
                error = "At least one degree of freedom must be defined for the boundary condition.";
                return false;
            }
            if (GetFixedDirections().Length != 0 && GetConstrainedDirections().Length != GetFixedDirections().Length)
            {
                error = "Only fixed and unconstrained degrees of freedom can be used at the same time.";
                return false;
            }
            return true;
        }
        public DOFType GetDofType(int dof)
        {
            double value;
            switch (dof)
            {
                case 1: value = _u1; break;
                case 2: value = _u2; break;
                case 3: value = _u3; break;
                case 4: value = _ur1; break;
                case 5: value = _ur2; break;
                case 6: value = _ur3; break;
                default: throw new NotSupportedException();
            }
            if (double.IsNaN(value)) return DOFType.Free;
            else if (double.IsPositiveInfinity(value)) return DOFType.Fixed;
            else if (value == 0) return DOFType.Zero;
            else return DOFType.Prescribed;
        }
    }
}
