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
    public class ViewMomentLoad : ViewLoad
    {
        // Variables                                                                                                                
        private CaeModel.MomentLoad _momentLoad;


        // Properties                                                                                                               
        public override string Name { get { return _momentLoad.Name; } set { _momentLoad.Name = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the load.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(3, 10, "Reference point")]
        [DescriptionAttribute("Select the reference point for the creation of the load.")]
        [Id(4, 2)]
        public string ReferencePointName { get { return _momentLoad.RegionName; } set { _momentLoad.RegionName = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(0, 10, "M1")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the first axis.")]
        [TypeConverter(typeof(StringMomentConverter))]
        [Id(1, 3)]
        public double M1 { get { return _momentLoad.M1; } set { _momentLoad.M1 = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(1, 10, "M2")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the second axis.")]
        [TypeConverter(typeof(StringMomentConverter))]
        [Id(2, 3)]
        public double M2 { get { return _momentLoad.M2; } set { _momentLoad.M2 = value; } }
        //
        [CategoryAttribute("Moment components")]
        [OrderedDisplayName(2, 10, "M3")]
        [DescriptionAttribute("Value of the moment component per node in the direction of the third axis.")]
        [TypeConverter(typeof(StringMomentConverter))]
        [Id(3, 3)]
        public double M3 { get { return _momentLoad.M3; } set { _momentLoad.M3 = value; } }
        //
        [CategoryAttribute("Moment magnitude")]
        [OrderedDisplayName(3, 10, "Magnitude")]
        [DescriptionAttribute("Value of the moment load magnitude.")]
        [TypeConverter(typeof(StringMomentConverter))]
        [Id(1, 4)]
        public double Mlength
        {
            get { return Math.Sqrt(_momentLoad.M1 * _momentLoad.M1 + 
                                   _momentLoad.M2 * _momentLoad.M2 + 
                                   _momentLoad.M3 * _momentLoad.M3); }
            set
            {
                if (value <= 0)
                    throw new Exception("Value of the moment load magnitude must be greater than 0.");
                //
                double len = Math.Sqrt(_momentLoad.M1 * _momentLoad.M1 +
                                       _momentLoad.M2 * _momentLoad.M2 +
                                       _momentLoad.M3 * _momentLoad.M3);
                double r;
                if (len == 0) r = 0;
                else r = value / len;
                _momentLoad.M1 *= r;
                _momentLoad.M2 *= r;
                _momentLoad.M3 *= r;
            }
        }
        //
        [CategoryAttribute("Moment phase")]
        [OrderedDisplayName(0, 10, "Phase")]
        [DescriptionAttribute("Value of the moment phase.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        [Id(1, 5)]
        public double Phase { get { return _momentLoad.PhaseDeg; } set { _momentLoad.PhaseDeg = value; } }
        //
        public override string AmplitudeName
        {
            get { return _momentLoad.AmplitudeName; }
            set { _momentLoad.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color { get { return _momentLoad.Color; } set { _momentLoad.Color = value; } }


        // Constructors                                                                                                             
        public ViewMomentLoad(CaeModel.MomentLoad momentLoad)
        {
            // The order is important
            _momentLoad = momentLoad;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ReferencePointName, nameof(ReferencePointName));
            //
            SetBase(_momentLoad, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // 2D
            DynamicCustomTypeDescriptor.GetProperty(nameof(M1)).SetIsBrowsable(!momentLoad.TwoD);
            DynamicCustomTypeDescriptor.GetProperty(nameof(M2)).SetIsBrowsable(!momentLoad.TwoD);
            // Phase
            DynamicCustomTypeDescriptor.GetProperty(nameof(Phase)).SetIsBrowsable(momentLoad.Complex);
        }


        // Methods                                                                                                                  
        public override CaeModel.Load GetBase()
        {
            return _momentLoad;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] referencePointNames, string[] amplitudeNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.ReferencePointName, referencePointNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            PopulateAmplitudeNames(amplitudeNames);
        }
      
    }

}
