using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using CaeModel;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public abstract class ViewAmplitude
    {
        // Variables                                                                                                                
        private DynamicCustomTypeDescriptor _dctd;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the field output.")]
        public abstract string Name { get; set; }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Time span")]
        [DescriptionAttribute("Select if the amplitude is defined in global analysis time/frequency or in a local step time/frequency.")]
        public abstract AmplitudeTimeSpanEnum TimeSpan { get ; set; }
        //
        [CategoryAttribute("Modify")]
        [OrderedDisplayName(0, 10, "Shift time")]
        [DescriptionAttribute("Select the fixed amout used to shift the time/frequency values.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public abstract double ShiftX { get; set; }
        //
        [CategoryAttribute("Modify")]
        [OrderedDisplayName(1, 10, "Shift amplitude")]
        [DescriptionAttribute("Select the fixed amout used to shift the amplitude values.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public abstract double ShiftY { get; set; }
        //
        [Browsable(false)]
        public DynamicCustomTypeDescriptor DynamicCustomTypeDescriptor { get { return _dctd; } set { _dctd = value; } }
        //
        [Browsable(false)]
        public abstract Amplitude Base { get; }


        // Constructors                                                                                                             


        // Methods
       
    }
}
