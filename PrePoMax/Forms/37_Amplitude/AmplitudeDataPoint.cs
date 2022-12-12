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
    public class AmplitudeDataPoint
    {
        // Variables                                                                                                                
        private double _time;
        private double _amplitude;


        // Properties                                                                                                               
        [DisplayName("Time [?]\nFrequency [?]")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Time { get { return _time; } set { _time = value; } }
        //
        [DisplayName("Amplitude\n[?]")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Amplitude { get { return _amplitude; } set { _amplitude = value; } }


        // Constructors                                                                                                             
        public AmplitudeDataPoint()
        {
            _time = 0;
            _amplitude = 0;
        }
        public AmplitudeDataPoint(double time, double amplitude)
        {
            _time = time;
            _amplitude = amplitude;
        }
    }
}
