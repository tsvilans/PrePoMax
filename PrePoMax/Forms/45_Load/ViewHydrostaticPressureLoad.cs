using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax
{
    [Serializable]
    public class ViewHydrostaticPressureLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.HydrostaticPressure _hpLoad;
        private ItemSetData _singlePointItemSetData;
        private ItemSetData _twoPointPointItemSetData;


        // Properties                                                                                                               
        public override string Name { get { return _hpLoad.Name; } set { _hpLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName { get { return _hpLoad.RegionName; } set { _hpLoad.RegionName = value; } }
        //                                                                                                                          
        [Category("First point coordinates")]
        [OrderedDisplayName(0, 10, "By selection")]
        [DescriptionAttribute("Use selection for the definition of the first point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 3)]
        public ItemSetData FirstPointItemSet
        {
            get { return _singlePointItemSetData; }
            set
            {
                if (value != _singlePointItemSetData)
                    _singlePointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [DescriptionAttribute("X coordinate of the first point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 3)]
        public double X1 { get { return _hpLoad.FirstPointCoor[0]; } set { _hpLoad.FirstPointCoor[0] = value; } }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [DescriptionAttribute("Y coordinate of the first point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 3)]
        public double Y1 { get { return _hpLoad.FirstPointCoor[1]; } set { _hpLoad.FirstPointCoor[1] = value; } }
        //
        [CategoryAttribute("First point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [DescriptionAttribute("Z coordinate of the first point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 3)]
        public double Z1 { get { return _hpLoad.FirstPointCoor[2]; } set { _hpLoad.FirstPointCoor[2] = value; } }
        //                                                                                                              
        [CategoryAttribute("First point pressure")]
        [OrderedDisplayName(0, 10, "Magnitude")]
        [DescriptionAttribute("Pressure magnitude at the first point.")]
        [TypeConverter(typeof(StringPressureConverter))]
        [Id(1, 4)]
        public double FirstPointPressure
        {
            get { return _hpLoad.FirstPointPressure; }
            set { _hpLoad.FirstPointPressure = value; }
        }
        //                                                                                                              
        [Category("Second point coordinates")]
        [OrderedDisplayName(0, 10, "By selection ")]        // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the second point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 5)]
        public ItemSetData SecondPointItemSet
        {
            get { return _singlePointItemSetData; }
            set
            {
                if (value != _singlePointItemSetData)
                    _singlePointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(1, 10, "X ")]
        [DescriptionAttribute("X coordinate of the second point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 5)]
        public double X2 { get { return _hpLoad.SecondPointCoor[0]; } set { _hpLoad.SecondPointCoor[0] = value; } }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(2, 10, "Y ")]
        [DescriptionAttribute("Y coordinate of the second point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 5)]
        public double Y2 { get { return _hpLoad.SecondPointCoor[1]; } set { _hpLoad.SecondPointCoor[1] = value; } }
        //
        [CategoryAttribute("Second point coordinates")]    // must be a different name than for the first point !!!
        [OrderedDisplayName(3, 10, "Z ")]
        [DescriptionAttribute("Z coordinate of the second point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 5)]
        public double Z2 { get { return _hpLoad.SecondPointCoor[2]; } set { _hpLoad.SecondPointCoor[2] = value; } }
        //                                                                                                                          
        [CategoryAttribute("Second point pressure")]        // must be a different name than for the first point !!!
        [OrderedDisplayName(0, 10, "Magnitude ")]
        [DescriptionAttribute("Pressure magnitude at the second point.")]
        [TypeConverter(typeof(StringPressureConverter))]
        [Id(1, 6)]
        public double SecondPointPressure
        {
            get { return _hpLoad.SecondPointPressure; }
            set { _hpLoad.SecondPointPressure = value; }
        }
        //                                                                                                                          
        [Category("Pressure direction")]
        [OrderedDisplayName(0, 10, "By selection  ")]    // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the pressure direction.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 7)]
        public ItemSetData PressureDirectionItemSet
        {
            get { return _twoPointPointItemSetData; }
            set
            {
                if (value != _twoPointPointItemSetData)
                    _twoPointPointItemSetData = value;
            }
        }
        //
        [CategoryAttribute("Pressure direction")]
        [OrderedDisplayName(0, 10, "N1")]
        [DescriptionAttribute("Direction component in the direction of the first axis.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 7)]
        public double N1 { get { return _hpLoad.N1; } set { _hpLoad.N1 = value; } }
        //
        [CategoryAttribute("Pressure direction")]
        [OrderedDisplayName(1, 10, "N2")]
        [DescriptionAttribute("Direction component in the direction of the second axis.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 7)]
        public double N2 { get { return _hpLoad.N2; } set { _hpLoad.N2 = value; } }
        //
        [CategoryAttribute("Pressure direction")]
        [OrderedDisplayName(2, 10, "N3")]
        [DescriptionAttribute("Direction component in the direction of the third axis.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 7)]
        public double N3 { get { return _hpLoad.N3; } set { _hpLoad.N3 = value; } }
        //
        public override string AmplitudeName { get { return _hpLoad.AmplitudeName; } set { _hpLoad.AmplitudeName = value; } }
        public override System.Drawing.Color Color { get { return _hpLoad.Color; } set { _hpLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewHydrostaticPressureLoad(CaeModel.HydrostaticPressure hpLoad)
        {
            // The order is important
            _hpLoad = hpLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_hpLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            _singlePointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _singlePointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            _twoPointPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _twoPointPointItemSetData.ToStringType = ItemSetDataToStringType.SelectTwoPoints;
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(Z1)).SetIsBrowsable(!hpLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(Z2)).SetIsBrowsable(!hpLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(N3)).SetIsBrowsable(!hpLoad.TwoD);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _hpLoad;
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
