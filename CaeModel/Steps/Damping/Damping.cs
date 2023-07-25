using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;
using CaeJob;

namespace CaeModel
{
    [Serializable]
    public enum DampingTypeEnum
    {
        Off,
        Rayleigh
    }

    [Serializable]
    public class Damping : NamedClass
    {
        // Variables                                                                                                                
        private DampingTypeEnum _dampingType;
        private double _alpha;
        private double _beta;


        // Properties                                                                                                               
        public DampingTypeEnum DampingType { get { return _dampingType; } set { _dampingType = value; } }
        public double Alpha { get { return _alpha; } set { _alpha = value; } }
        public double Beta { get { return _beta; } set { _beta = value; } }


        // Constructors                                                                                                             
        public Damping()
            : base("Damping")
        {
            _dampingType = DampingTypeEnum.Off;
            _alpha = 0.0;
            _beta = 0.0;
        }


        // Methods                                                                                                                  
    }
}
