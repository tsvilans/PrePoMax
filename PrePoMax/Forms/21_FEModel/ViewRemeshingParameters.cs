using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewRemeshingParameters : ViewMultiRegion
    {
        // Variables                                                                                                                
        private string _selectionHidden;
        private RemeshingParameters _parameters;


        // Properties                                                                                                               
        [CategoryAttribute("Region")]
        [OrderedDisplayName(0, 10, "Region type")]
        [DescriptionAttribute("Select the region type for the creation of the remeshing.")]
        [Id(1, 1)]
        public override string RegionType { get { return base.RegionType; } set { base.RegionType = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(1, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 1)]
        public string SelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        //
        [CategoryAttribute("Region")]
        [OrderedDisplayName(2, 10, "Element set")]
        [DescriptionAttribute("Select the element set for the creation of the section.")]
        [Id(3, 1)]
        public string ElementSetName { get { return _parameters.RegionName; } set { _parameters.RegionName = value; } }
        //
        //
        [Category("Mesh size")]
        [OrderedDisplayName(0, 10, "Max element size")]
        [Description("The value for the maximum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(1, 2)]
        public double MaxH { get { return _parameters.MaxH; } set { _parameters.MaxH = value; } }
        //
        [Category("Mesh size")]
        [OrderedDisplayName(1, 10, "Min element size")]
        [Description("The value for the minimum element size.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 2)]
        public double MinH { get { return _parameters.MinH; } set { _parameters.MinH = value; } }
        // Maximal Hausdorff distance for the boundaries approximation.
        [Category("Mesh size")]
        [OrderedDisplayName(2, 10, "Hausdorff")]
        [Description("Maximal Hausdorff distance for the boundaries approximation. " +
                              "A value of 0.01 is a suitable value for an object of size 1 in each direction.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 2)]
        public double Hausdorff { get { return _parameters.Hausdorff; } set { _parameters.Hausdorff = value; } }
        //
        [Category("Mesh type")]
        [OrderedDisplayName(0, 10, "Second order")]
        [Description("Create second order elements.")]
        [Browsable(false)]
        [Id(1, 3)]
        public bool SecondOrder
        {
            get{return _parameters.SecondOrder;}
            set { _parameters.SecondOrder = value; }
        }
        //
        //
        [Category("Mesh operations")]
        [OrderedDisplayName(0, 10, "Keep model edges")]
        [Description("Select Yes to keep and No to ignore the model edges.")]
        [Browsable(false)]
        [Id(1, 4)]
        public bool KeepModelEdges { get { return _parameters.KeepModelEdges; } set { _parameters.KeepModelEdges = value; } }


        // Constructors                                                                                                             
        public ViewRemeshingParameters(RemeshingParameters parameters)
        {
            _parameters = parameters;
            //
            Dictionary<RegionTypeEnum, string> regionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            regionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SelectionHidden));
            regionTypePropertyNamePairs.Add(RegionTypeEnum.ElementSetName, nameof(ElementSetName));
            //
            SetBase(_parameters, regionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            // Category sorting
            DynamicCustomTypeDescriptor.CategorySortOrder = CustomSortOrder.AscendingById;
            DynamicCustomTypeDescriptor.PropertySortOrder = CustomSortOrder.AscendingById;    // seems not to work
            // Now lets display Yes/No instead of True/False
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(SecondOrder));
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(KeepModelEdges));
        }


        // Methods                                                                                                                  
        public RemeshingParameters GetBase()
        {
            return _parameters;
        }
        public void PopulateDropDownLists(string[] elementSetNames)
        {
            Dictionary<RegionTypeEnum, string[]> regionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            regionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            regionTypeListItemsPairs.Add(RegionTypeEnum.ElementSetName, elementSetNames);
            PopulateDropDownLists(regionTypeListItemsPairs);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            //
            CustomPropertyDescriptor cpd;
            if (base.RegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = DynamicCustomTypeDescriptor.GetProperty(nameof(SelectionHidden));
                cpd.SetIsBrowsable(false);
            }
        }
    }
}
