using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;

namespace PrePoMax
{
    [Serializable]
    public class ViewMembraneSection : ViewSection
    {
        // Variables                                                                                                                
        protected CaeModel.MembraneSection _membraneSection;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Thickness")]
        [DescriptionAttribute("Set the membrane thickness.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Thickness { get { return _membraneSection.Thickness; } set { _membraneSection.Thickness = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Offset")]
        [DescriptionAttribute("Set the offset of the membrane mid-surface in regard to the selected geometry. "
                              + "The unit is the shell thickness.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Offset { get { return _membraneSection.Offset; } set { _membraneSection.Offset = value; } }


        // Constructors                                                                                                             
        public ViewMembraneSection(CaeModel.MembraneSection membraneSection)
        {
            _membraneSection = membraneSection;
            SetBase(_membraneSection);
        }


        // Methods                                                                                                                  


    }

}
