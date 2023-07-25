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
    public class ViewSTLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.STLoad _stLoad;


        // Properties                                                                                                               
        public override string Name { get { return _stLoad.Name; } set { _stLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _stLoad.SurfaceName; } set { _stLoad.SurfaceName = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(0, 10, "F1")]
        [DescriptionAttribute("Value of the force component in the direction of the first axis.")]
        [TypeConverter(typeof(StringForceConverter))]
        [Id(1, 3)]
        public double F1 { get { return _stLoad.F1; } set { _stLoad.F1 = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(1, 10, "F2")]
        [DescriptionAttribute("Value of the force component in the direction of the second axis.")]
        [TypeConverter(typeof(StringForceConverter))]
        [Id(2, 3)]
        public double F2 { get { return _stLoad.F2; } set { _stLoad.F2 = value; } }
        //
        [CategoryAttribute("Force components")]
        [OrderedDisplayName(2, 10, "F3")]
        [DescriptionAttribute("Value of the force component in the direction of the third axis.")]
        [TypeConverter(typeof(StringForceConverter))]
        [Id(3, 3)]
        public double F3 { get { return _stLoad.F3; } set { _stLoad.F3 = value; } }
        //
        [CategoryAttribute("Force magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Value of the surface traction load magnitude.")]
        [TypeConverter(typeof(StringForceConverter))]
        [Id(1, 4)]
        public double Flength
        {
            get { return Math.Sqrt(_stLoad.F1 * _stLoad.F1 + _stLoad.F2 * _stLoad.F2 + _stLoad.F3 * _stLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("Value of the surface traction load magnitude must be greater than 0.");

                double len = Math.Sqrt(_stLoad.F1 * _stLoad.F1 + _stLoad.F2 * _stLoad.F2 + _stLoad.F3 * _stLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _stLoad.F1 *= r;
                _stLoad.F2 *= r;
                _stLoad.F3 *= r;
            }
        }
        //
        [CategoryAttribute("Force phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the surface traction phase.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        [Id(1, 5)]
        public double Phase { get { return _stLoad.PhaseDeg; } set { _stLoad.PhaseDeg = value; } }
        //
        public override string AmplitudeName { get { return _stLoad.AmplitudeName; } set { _stLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _stLoad.Color; } set { _stLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewSTLoad(CaeModel.STLoad stLoad)
        {
            // The order is important
            _stLoad = stLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_stLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(F3)).SetIsBrowsable(!stLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(stLoad.Complex);
        }



        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _stLoad;
        }

        public void PopulateDropDownLists(string[] surfaceNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
    }

}
