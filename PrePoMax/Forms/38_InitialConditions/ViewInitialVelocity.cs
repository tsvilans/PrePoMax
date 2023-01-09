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
    public class ViewInitialVelocity : ViewInitialCondition
    {
        // Variables                                                                                                                
        private CaeModel.InitialVelocity _initialVelocity;


        // Properties                                                                                                               
        public override string Name { get { return _initialVelocity.Name; } set { _initialVelocity.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Part")]
        [DescriptionAttribute("Select the part for the assignment of the initial velocity.")]
        [Id(3, 2)]
        public string PartName { get { return _initialVelocity.RegionName; } set { _initialVelocity.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the assignment of the initial velocity.")]
        [Id(4, 2)]
        public string ElementSetName { get { return _initialVelocity.RegionName; } set { _initialVelocity.RegionName = value; } }
        //
        [CategoryAttribute("Velocity components")]
        [OrderedDisplayName(0, 10, "V1")]
        [DescriptionAttribute("Value of the velocity component in the direction of the first axis.")]
        [TypeConverter(typeof(StringVelocityConverter))]
        [Id(1, 3)]
        public double V1 { get { return _initialVelocity.V1; } set { _initialVelocity.V1 = value; } }
        //
        [CategoryAttribute("Velocity components")]
        [OrderedDisplayName(1, 10, "V2")]
        [DescriptionAttribute("Value of the velocity component in the direction of the second axis.")]
        [TypeConverter(typeof(StringVelocityConverter))]
        [Id(2, 3)]
        public double V2 { get { return _initialVelocity.V2; } set { _initialVelocity.V2 = value; } }
        //
        [CategoryAttribute("Velocity components")]
        [OrderedDisplayName(2, 10, "V3")]
        [DescriptionAttribute("Value of the velocity component in the direction of the third axis.")]
        [TypeConverter(typeof(StringVelocityConverter))]
        [Id(3, 3)]
        public double V3 { get { return _initialVelocity.V3; } set { _initialVelocity.V3 = value; } }
        //
        [CategoryAttribute("Velocity magnitude")]
        [OrderedDisplayName(3, 10, "Magnitude")]
        [DescriptionAttribute("Value of the velocity magnitude.")]
        [TypeConverter(typeof(StringVelocityConverter))]
        [Id(1, 4)]
        public double Vlength
        {
            get { return Math.Sqrt(_initialVelocity.V1 * _initialVelocity.V1 +
                                   _initialVelocity.V2 * _initialVelocity.V2 +
                                   _initialVelocity.V3 * _initialVelocity.V3); }
            set
            {
                if (value <= 0)
                    throw new Exception("Value of the velocity magnitude must be greater than 0.");
                //
                double len = Math.Sqrt(_initialVelocity.V1 * _initialVelocity.V1 +
                                       _initialVelocity.V2 * _initialVelocity.V2 +
                                       _initialVelocity.V3 * _initialVelocity.V3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _initialVelocity.V1 *= r;
                _initialVelocity.V2 *= r;
                _initialVelocity.V3 *= r;
            }
        }
        //

        // Constructors                                                                                                             
        public ViewInitialVelocity(CaeModel.InitialVelocity initialVelocity)
        {
            // The order is important
            _initialVelocity = initialVelocity;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.PartName, nameof(PartName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            SetBase(_initialVelocity, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(V3)).SetIsBrowsable(!initialVelocity.TwoD);
        }


        // Methods                                                                                                                  
        public override CaeModel.InitialCondition GetBase()
        {
            return _initialVelocity;
        }
        public void PopulateDropDownLists(string[] partNames, string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.PartName, partNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
        }
    }



   
}
