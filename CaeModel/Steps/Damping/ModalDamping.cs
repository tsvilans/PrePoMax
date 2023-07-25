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
    public enum ModalDampingTypeEnum
    {
        Off,
        Constant,
        Direct,
        Rayleigh
    }

    [Serializable]
    public class ModalDamping : NamedClass
    {
        // Variables                                                                                                                
        private ModalDampingTypeEnum _dampingType;
        private double _alpha;
        private double _beta;
        private double _viscousDampingRatio;
        private List<DampingRatioAndRange> _dampingRatiosAndRanges;


        // Properties                                                                                                               
        public ModalDampingTypeEnum DampingType { get { return _dampingType; } set { _dampingType = value; } }
        public double Alpha { get { return _alpha; } set { _alpha = value; } }
        public double Beta { get { return _beta; } set { _beta = value; } }
        public double ViscousDampingRatio { get { return _viscousDampingRatio; } set { _viscousDampingRatio = value; } }
        public List<DampingRatioAndRange> DampingRatiosAndRanges
        {
            get { return _dampingRatiosAndRanges; }
            set { _dampingRatiosAndRanges = value; }
        }


        // Constructors                                                                                                             
        public ModalDamping()
            : base("ModalDamping")
        {
            _dampingType = ModalDampingTypeEnum.Off;
            _alpha = 0.0;
            _beta = 0.0;
            _viscousDampingRatio = 0.0;
        }


        // Methods                                                                                                                  
    }
}
