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
    public enum ViewContactHistoryVariable
    {
        // Must start at 1 for the UI to work
        [StandardValue("CDIS", Description = "Relative contact displacements.")]
        CDIS = 1,
        //
        [StandardValue("CSTR", Description = "Contact stresses.")]
        CSTR = 2,
        //
        [StandardValue("CELS", Description = "Contact energy.")]
        CELS = 4,
        //
        [StandardValue("CNUM", Description = "Total number of contact elements.")]
        CNUM = 8,
        //
        [StandardValue("CF", Description = "Total contact forces on slave surface.")]
        CF = 16,
        //CFN = 32,
        //CFS = 64
    }

    [Serializable]
    public class ViewContactHistoryOutput : ViewHistoryOutput
    {
        // Variables                                                                                                                
        private CaeModel.ContactHistoryOutput _historyOutput;


        // Properties                                                                                                               
        public override string Name { get { return _historyOutput.Name; } set { _historyOutput.Name = value; } }
        //
        [OrderedDisplayName(2, 10, "Variables to output")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("Contact history variables")]
        public ViewContactHistoryVariable Variables
        {
            get
            {
                return (ViewContactHistoryVariable)_historyOutput.Variables;
            }
            set
            {
                _historyOutput.Variables = (CaeModel.ContactHistoryVariable)value;
                UpdateVisibility();
            }
        }
        //
        [OrderedDisplayName(3, 10, "Totals")]
        [CategoryAttribute("Data")]
        [DescriptionAttribute("The parameter totals only applies to the energy (CELS).")]
        public CaeModel.TotalsTypeEnum TotalsType { get { return _historyOutput.TotalsType; } set { _historyOutput.TotalsType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Contact pair")]
        [DescriptionAttribute("Select the contact pair for the creation of the history output.")]
        public string ContactPairName { get { return _historyOutput.RegionName; } set { _historyOutput.RegionName = value; } }

       
        // Constructors                                                                                                             
        public ViewContactHistoryOutput(CaeModel.ContactHistoryOutput historyOutput)
        {
            // The order is important
            _historyOutput = historyOutput;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ContactPair, nameof(ContactPairName));
            //
            base.SetBase(_historyOutput, regionTypePropertyNamePairs);
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
            //
            StringIntegerDefaultConverter.SetInitialValue = 1;
        }


        // Methods                                                                                                                  
        public override CaeModel.HistoryOutput GetBase()
        {
            return _historyOutput;
        }
        public void PopulateDropDownLists(string[] contactPairNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.ContactPair, contactPairNames);
            base.PopulateDropDownLists(regionTypeListItemsPairs);
            //
            DynamicCustomTypeDescriptor.GetProperty(nameof(SelectionHidden)).SetIsBrowsable(false);
            //
            UpdateVisibility(); //  must be here
        }
        public void UpdateVisibility()
        {
            bool cf = Variables.HasFlag(ViewContactHistoryVariable.CF);
            DynamicCustomTypeDescriptor.GetProperty(nameof(ContactPairName)).SetIsBrowsable(cf);
        }
    }



   
}
