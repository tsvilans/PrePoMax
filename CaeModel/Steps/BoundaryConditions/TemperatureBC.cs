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
    public class TemperatureBC : BoundaryCondition
    {
        // Variables                                                                                                                
        private double _temperature;


        // Properties                                                                                                               
        public double Temperature { get { return _temperature; } set { _temperature = value; } }


        // Constructors                                                                                                             
        public TemperatureBC(string name, string regionName, RegionTypeEnum regionType, double temperature, bool twoD)
            : base(name, regionName, regionType, twoD, false, 0) 
        {
            _temperature = temperature;
        }


        // Methods                                                                                                                  
       
    }
}
