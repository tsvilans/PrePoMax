using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using DynamicTypeDescriptor;
using System.ComponentModel;
using CaeGlobals;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewRadiationHeatTransfer : ViewLoad
    {
        // Variables                                                                                                                
        private RadiationHeatTransfer _radiationHeatTransfer;


        // Properties                                                                                                               
        public override string Name { get { return _radiationHeatTransfer.Name; } set { _radiationHeatTransfer.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Cavity radiation")]
        [DescriptionAttribute("Selected faces belong to a radiation in a cavity.")]
        [Id(2, 1)]
        public bool CavityRadiation
        {
            get { return _radiationHeatTransfer.CavityRadiation; }
            set
            {
                _radiationHeatTransfer.CavityRadiation = value;
                DynamicCustomTypeDescriptor.GetProperty(nameof(CavityName)).SetIsBrowsable(_radiationHeatTransfer.CavityRadiation);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Cavity name")]
        [DescriptionAttribute("To separate cavities enter the cavity name (at most 3 characters).")]
        [Id(3, 1)]
        public string CavityName
        {
            get { return _radiationHeatTransfer.CavityName; }
            set { _radiationHeatTransfer.CavityName = value; }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the load.")]
        [Id(3, 2)]
        public string SurfaceName
        {
            get { return _radiationHeatTransfer.SurfaceName; }
            set {_radiationHeatTransfer.SurfaceName = value;}
        }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(0, 10, "Sink temperature")]
        [DescriptionAttribute("Value of the sink temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(1, 3)]
        public double SinkTemperature
        {
            get { return _radiationHeatTransfer.SinkTemperature; }
            set { _radiationHeatTransfer.SinkTemperature = value; }
        }
        //
        [CategoryAttribute("Parameters")]
        [OrderedDisplayName(1, 10, "Emissivity")]
        [DescriptionAttribute("Value of the surface emissivity (blackbody radiation is characterized by 1).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(2, 3)]
        public double Emissivity
        {
            get { return _radiationHeatTransfer.Emissivity; }
            set { _radiationHeatTransfer.Emissivity = value; }
        }
        //
        public override string AmplitudeName
        {
            get { return _radiationHeatTransfer.AmplitudeName; }
            set { _radiationHeatTransfer.AmplitudeName = value; }
        }
        public override System.Drawing.Color Color
        {
            get { return _radiationHeatTransfer.Color; }
            set { _radiationHeatTransfer.Color = value; }
        }


        // Constructors                                                                                                             
        public ViewRadiationHeatTransfer(RadiationHeatTransfer radiationHeatTransfer)
        {
            _radiationHeatTransfer = radiationHeatTransfer;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_radiationHeatTransfer, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Now lets display Yes/No instead of True/False
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(CavityRadiation));
            //
            CavityRadiation = _radiationHeatTransfer.CavityRadiation; // update CavityName visibility
        }


        // Methods                                                                                                                  
        public override Load GetBase()
        {
            return _radiationHeatTransfer;
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
