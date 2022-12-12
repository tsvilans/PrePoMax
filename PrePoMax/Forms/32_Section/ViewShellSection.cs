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
    public class ViewShellSection : ViewSection
    {
        // Variables                                                                                                                
        protected CaeModel.ShellSection _shellSection;
        

        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Thickness")]
        [DescriptionAttribute("Set the shell thickness.")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double Thickness { get { return _shellSection.Thickness; } set { _shellSection.Thickness = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Offset")]
        [DescriptionAttribute("Set the offset of the shell mid-surface in regard to the selected geometry. " +
                              "The unit is the shell thickness.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double Offset { get { return _shellSection.Offset; } set { _shellSection.Offset = value; } }


        // Constructors                                                                                                             
        public ViewShellSection(CaeModel.ShellSection shellSection)
        {
            _shellSection = shellSection;
            SetBase(_shellSection);
        }


        // Methods                                                                                                                  


    }

}
