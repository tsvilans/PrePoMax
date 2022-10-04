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
    public class ViewDefinedTemperature : ViewDefinedField
    {
        // Variables                                                                                                                
        private CaeModel.DefinedTemperature _definedTemperature;


        // Properties                                                                                                               
        public override string Name { get { return _definedTemperature.Name; } set { _definedTemperature.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Define temperature")]
        [DescriptionAttribute("Define the temperature by a constant value or read the temperature from a file.")]
        [Id(2, 1)]
        public CaeModel.DefinedTemperatureTypeEnum DefinedTemperatureType
        {
            get { return _definedTemperature.Type; }
            set { _definedTemperature.Type = value; UpdateFieldView(); }
        }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Node set")]
        [DescriptionAttribute("Select the node set for the creation of the defined temperature.")]
        [Id(3, 2)]
        public string NodeSetName { get { return _definedTemperature.RegionName; } set { _definedTemperature.RegionName = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(4, 10, "Surface")]
        [DescriptionAttribute("Select the surface for the creation of the defined temperature.")]
        [Id(4, 2)]
        public string SurfaceName { get { return _definedTemperature.RegionName; } set { _definedTemperature.RegionName = value; } }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(0, 10, "Temperature")]
        [DescriptionAttribute("Value of the defined temperature.")]
        [TypeConverter(typeof(StringTemperatureConverter))]
        [Id(1, 3)]
        public double Temperature
        {
            get { return _definedTemperature.Temperature; }
            set { _definedTemperature.Temperature = value; }
        }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(1, 10, "Results file .frd")]
        [DescriptionAttribute("Results file name (.frd) without path.")]
        [EditorAttribute(typeof(FilteredFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Id(2, 3)]
        public string FileName
        {
            get { return _definedTemperature.FileName; }
            set
            {
                value = System.IO.Path.GetFileName(value);
                //
                if (value.Contains(" ") || value.Contains(":") || value.Contains("\\") || value.Contains("/")
                    || value.ToUTF8() != value)
                    throw new Exception("Enter the results file name (.frd) without path. " +
                                        "The results file name must not contain any special characters.");
                _definedTemperature.FileName = value;
            }
        }
        //
        [CategoryAttribute("Magnitude")]
        [OrderedDisplayName(2, 10, "Step number")]
        [DescriptionAttribute("Enter the results step number from which to read the temperatures.")]
        [Id(3, 3)]
        public int StepNumber
        {
            get { return _definedTemperature.StepNumber; }
            set { _definedTemperature.StepNumber = value; }
        }


        // Constructors                                                                                                             
        public ViewDefinedTemperature(CaeModel.DefinedTemperature definedTemperature)
        {
            // The order is important
            _definedTemperature = definedTemperature;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.NodeSetName, nameof(NodeSetName));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SurfaceName));
            //
            SetBase(_definedTemperature, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            FilteredFileNameEditor.Filter = "Calculix result files|*.frd";
        }


        // Methods                                                                                                                  
        public override CaeModel.DefinedField GetBase()
        {
            return _definedTemperature;
        }
        public void PopulateDropDownLists(string[] nodeSetNames, string[] surfaceNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.NodeSetName, nodeSetNames);
            regionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
            //
            UpdateFieldView();
        }
        private void UpdateFieldView()
        {
            bool byValue = _definedTemperature.Type == CaeModel.DefinedTemperatureTypeEnum.ByValue;
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(Temperature)).SetIsBrowsable(byValue);
            DynamicCustomTypeDescriptor.GetProperty(nameof(FileName)).SetIsBrowsable(!byValue);
            DynamicCustomTypeDescriptor.GetProperty(nameof(StepNumber)).SetIsBrowsable(!byValue);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(RegionType)).SetIsBrowsable(byValue);
            if (byValue)
            {
                RegionType = RegionType;
            }
            else
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(NodeSetName)).SetIsBrowsable(byValue);
                DynamicCustomTypeDescriptor.GetProperty(nameof(SurfaceName)).SetIsBrowsable(byValue);
            }
        }
    }



   
}
