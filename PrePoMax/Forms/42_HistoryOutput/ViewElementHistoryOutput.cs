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
    [EnumResource("PrePoMax.Properties.Resources")]
    [Editor(typeof(StandardValueEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Flags]
    public enum ViewElementHistoryVariable
    {
        // must start at 1 for the UI to work
        [StandardValue("S", Description = "True (Cauchy) stresses.")]
        S = 1,
        [StandardValue("E", Description = "Total Lagrangian strain or total Eulerian strain.")]
        E = 2,
        [StandardValue("ME", Description = "Mechanical Lagrangian strain or mechanical Eulerian strain.")]
        ME = 4,
        [StandardValue("PEEQ", Description = "Equivalent plastic strain.")]
        PEEQ = 8,
        // Thermal
        [StandardValue("HFL", Description = "Heat flux.")]
        HFL = 16,
        // Whole element
        [StandardValue("ENER", Description = "Energy density.")]
        ENER = 32,
        [StandardValue("ELSE", Description = "Internal energy.")]
        ELSE = 64,
        [StandardValue("ELKE", Description = "Kinetic energy.")]
        ELKE = 128,
        [StandardValue("EVOL", Description = "Volume.")]
        EVOL = 256,
        [StandardValue("EBHE", Description = "Heating power.")]
        EBHE = 512
    }

    [Serializable]
    public class ViewElementHistoryOutput : ViewHistoryOutput
    {
        // Variables                                                                                                                
        private CaeModel.ElementHistoryOutput _historyOutput;


        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        public override int Frequency { get { return _historyOutput.Frequency; } set { _historyOutput.Frequency = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Variables to output")]
        [DescriptionAttribute("Element history variables")]
        public ViewElementHistoryVariable Variables
        {
            get
            {
                return (ViewElementHistoryVariable)_historyOutput.Variables;
            }
            set
            {
                _historyOutput.Variables = (CaeModel.ElementHistoryVariable)value;
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Totals")]
        [DescriptionAttribute("The parameter totals only applies to whole element variables (ELSE, EVOL).")]
        public CaeModel.TotalsTypeEnum TotalsType { get { return _historyOutput.TotalsType; } set { _historyOutput.TotalsType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the history output.")]
        public string ElementSetName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }
        

        // Constructors                                                                                                             
        public ViewElementHistoryOutput(CaeModel.ElementHistoryOutput historyOutput)
        {
            // The order is important
            _historyOutput = historyOutput;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            base.SetBase(_historyOutput, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  
        public override CaeModel.HistoryOutput GetBase()
        {
            return _historyOutput;
        }
        public void PopulateDropDownLists(string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
        }
    }



   
}
