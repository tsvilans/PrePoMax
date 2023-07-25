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
    public class SubmodelBC : BoundaryCondition
    {
        // Variables                                                                                                                
        private int _stepNumber;
        private bool _u1;
        private bool _u2;
        private bool _u3;
        private bool _ur1;
        private bool _ur2;
        private bool _ur3;


        // Properties                                                                                                               
        public int StepNumber 
        { 
            get { return _stepNumber; } 
            set 
            {
                _stepNumber = value;
                if (_stepNumber < 1) _stepNumber = 1;
            }
        }
        public bool U1 { get { return _u1; } set { _u1 = value; } }
        public bool U2 { get { return _u2; } set { _u2 = value; } }
        public bool U3 { get { return _u3; } set { _u3 = value; if (_twoD) _u3 = false; } }
        public bool UR1 { get { return _ur1; } set { _ur1 = value; if (_twoD) _ur1 = false; } }
        public bool UR2 { get { return _ur2; } set { _ur2 = value; if (_twoD) _ur2 = false; } }
        public bool UR3 { get { return _ur3; } set { _ur3 = value; } }


        // Constructors                                                                                                             
        public SubmodelBC(string name, string regionName, RegionTypeEnum regionType, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0) 
        {
            _stepNumber = 1;
            U1 = false;
            U2 = false;
            U3 = false;
            UR1 = false;
            UR2 = false;
            UR3 = false;
        }


        // Methods                                                                                                                  
        public int[] GetConstrainedDirections()
        {
            List<int> directions = new List<int>();
            if (_u1) directions.Add(1);
            if (_u2) directions.Add(2);
            if (_u3) directions.Add(3);
            if (_ur1) directions.Add(4);
            if (_ur2) directions.Add(5);
            if (_ur3) directions.Add(6);
            return directions.ToArray();
        }
    }
}
