using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewSlipWear : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private SlipWear _slipWear;

        // Properties                                                                                                               
        public override string Name
        {
            get { return "Slip Wear"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base { get { return _slipWear; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Hardness")]
        [DescriptionAttribute("The value of the surface hardness.")]
        [TypeConverter(typeof(StringPressureConverter))]
        public double Hardness { get { return _slipWear.Hardness; } set { _slipWear.Hardness = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Wear coefficient")]
        [DescriptionAttribute("The value of the wear coefficient.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double WearCoefficient { get { return _slipWear.WearCoefficient; } set { _slipWear.WearCoefficient = value; } }


        // Constructors                                                                                                             
        public ViewSlipWear(SlipWear slipWear)
        {
            _slipWear = slipWear;
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
    }
}
