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
    public class ViewGravityLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.GravityLoad _gLoad;


        // Properties                                                                                                               
        public override string Name { get { return _gLoad.Name; } set { _gLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Part")]
        [DescriptionAttribute("Select the part for the creation of the load.")]
        [Id(3, 2)]
        public string PartName { get { return _gLoad.RegionName; } set { _gLoad.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the load.")]
        [Id(4, 2)]
        public string ElementSetName { get { return _gLoad.RegionName; } set { _gLoad.RegionName = value; } }
        //
        [CategoryAttribute("Gravity components")]
        [OrderedDisplayName(0, 10, "F1")]
        [DescriptionAttribute("Value of the gravity component in the direction of the first axis.")]
        [TypeConverter(typeof(StringAccelerationConverter))]
        [Id(1, 3)]
        public double F1 { get { return _gLoad.F1; } set { _gLoad.F1 = value; } }
        //
        [CategoryAttribute("Gravity components")]
        [OrderedDisplayName(1, 10, "F2")]
        [DescriptionAttribute("Value of the gravity component in the direction of the second axis.")]
        [TypeConverter(typeof(StringAccelerationConverter))]
        [Id(2, 3)]
        public double F2 { get { return _gLoad.F2; } set { _gLoad.F2 = value; } }
        //
        [CategoryAttribute("Gravity components")]
        [OrderedDisplayName(2, 10, "F3")]
        [DescriptionAttribute("Value of the gravity component in the direction of the third axis.")]
        [TypeConverter(typeof(StringAccelerationConverter))]
        [Id(3, 3)]
        public double F3 { get { return _gLoad.F3; } set { _gLoad.F3 = value; } }
        //
        [CategoryAttribute("Gravity magnitude")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Value of the gravity load magnitude.")]
        [TypeConverter(typeof(StringAccelerationConverter))]
        [Id(1, 4)]
        public double Flength
        {
            get { return Math.Sqrt(_gLoad.F1 * _gLoad.F1 + _gLoad.F2 * _gLoad.F2 + _gLoad.F3 * _gLoad.F3); }
            set
            {
                if (value <= 0)
                    throw new Exception("Value of the gravity load magnitude must be greater than 0.");
                //
                double len = Math.Sqrt(_gLoad.F1 * _gLoad.F1 + _gLoad.F2 * _gLoad.F2 + _gLoad.F3 * _gLoad.F3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _gLoad.F1 *= r;
                _gLoad.F2 *= r;
                _gLoad.F3 *= r;
            }
        }
        //
        [CategoryAttribute("Gravity phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the gravity load phase.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        [Id(1, 5)]
        public double Phase { get { return _gLoad.PhaseDeg; } set { _gLoad.PhaseDeg = value; } }
        ////
        public override string AmplitudeName { get { return _gLoad.AmplitudeName; } set { _gLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _gLoad.Color; } set { _gLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewGravityLoad(CaeModel.GravityLoad gLoad)
        {
            // The order is important
            _gLoad = gLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, nameof(PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            SetBase(_gLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(F3)).SetIsBrowsable(!gLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(gLoad.Complex);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _gLoad;
        }
        public void PopulateDropDownLists(string[] partNames, string[] elementSetNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
    }

}
