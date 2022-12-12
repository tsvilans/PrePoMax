using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{
    [Serializable]
    public class ElasticDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _youngsModulus;
        private double _poissonsRatio;


        // Properties                                                                                                               
        [DisplayName("Youngs modulus\n[?]")]
        [TypeConverter(typeof(StringPressureFromConverter))]
        public double YoungsModulus { get { return _youngsModulus; } set { _youngsModulus = value; } }
        //
        [DisplayName("Poissons ratio\n[?]")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double PoissonsRatio { get { return _poissonsRatio; } set { _poissonsRatio = value; } }


        // Constructors                                                                                                             
        public ElasticDataPoint()
            : base(0)
        {
            _youngsModulus = 0;
            _poissonsRatio = 0;
        }
        public ElasticDataPoint(double youngsModulus, double poissonsRatio, double temperature)
            :base(temperature)
        {
            _youngsModulus = youngsModulus;
            _poissonsRatio = poissonsRatio;
        }
    }
}
