using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax.PropertyViews
{
    [Serializable]
    public class ViewElasticWithDensity : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private CaeModel.ElasticWithDensity _elasticWithDensity;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "ElasticWithDensity"; }
        }
        //
        [Browsable(false)]
        public override CaeModel.MaterialProperty Base
        {
            get { return _elasticWithDensity; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 5, "Young's modulus")]
        [DescriptionAttribute("The value of the Young's modulus.")]
        [TypeConverter(typeof(StringPressureConverter))]
        public double YoungsModulus
        {
            get { return _elasticWithDensity.YoungsModulus; }
            set { _elasticWithDensity.YoungsModulus = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 5, "Poisson's ratio")]
        [DescriptionAttribute("The value of the Poisson's ratio.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double PoissonsRatio
        {
            get { return _elasticWithDensity.PoissonsRatio; }
            set { _elasticWithDensity.PoissonsRatio = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 5, "Density")]
        [DescriptionAttribute("The value of the density.")]
        [TypeConverter(typeof(StringDensityConverter))]
        public double Density { get { return _elasticWithDensity.Density; } set { _elasticWithDensity.Density = value; } }


        // Constructors                                                                                                             
        public ViewElasticWithDensity(CaeModel.ElasticWithDensity elasticWithDensity)
        {
            _elasticWithDensity = elasticWithDensity;
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
    }
}
