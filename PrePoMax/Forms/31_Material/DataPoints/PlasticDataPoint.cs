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
    public class PlasticDataPoint : TempDataPoint
    {
        // Variables                                                                                                                
        private double _strain;
        private double _stress;


        // Properties                                                                                                               
        [DisplayName("Yield stress\n[?]")]
        [TypeConverter(typeof(StringPressureFromConverter))]
        public double Stress { get { return _stress; } set { _stress = value; } }
        //
        [DisplayName("Plastic strain\n[?]")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Strain { get { return _strain; } set { _strain = value; } }


        // Constructors                                                                                                             
        public PlasticDataPoint()
            : base(0)
        {
            _stress = 0;
            _strain = 0;
        }
        public PlasticDataPoint(double stress, double strain, double temperature)
            :base(temperature)
        {
            _stress = stress;
            _strain = strain;
        }
    }
}
