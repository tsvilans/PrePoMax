using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;
using System.Drawing.Design;

namespace PrePoMax
{
    [Serializable]
    public class ViewContactPair : ViewMasterSlaveMultiRegion
    {
        // Variables                                                                                                                
        private string _selectionHidden;
        private CaeModel.ContactPair _contactPair;


        // Properties                                                                                                               
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Name")]
        [DescriptionAttribute("Name of the contact pair.")]
        [Id(1, 1)]
        public string Name { get { return _contactPair.Name; } set { _contactPair.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Surface interaction")]
        [DescriptionAttribute("Select the surface interaction defining the properties of the contact pair.")]
        [Id(2, 1)]
        public string SurfaceInteractionName
        {
            get { return _contactPair.SurfaceInteractionName; }
            set { _contactPair.SurfaceInteractionName = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Method")]
        [DescriptionAttribute("Select the contact method (all contact pairs in the model must use the same method).")]
        [Id(3, 1)]
        public CaeModel.ContactPairMethod Method
        {
            get { return _contactPair.Method; }
            set
            {
                _contactPair.Method = value;
                CustomPropertyDescriptor cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(SmallSliding));
                cpd.SetIsBrowsable(_contactPair.Method == CaeModel.ContactPairMethod.NodeToSurface);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "Small sliding")]
        [DescriptionAttribute("Use small sliding (pairing is done only at the start of the increment).")]
        [Id(4, 1)]
        public bool SmallSliding { get { return _contactPair.SmallSliding; } set { _contactPair.SmallSliding = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "Adjust")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        [Id(5, 1)]
        public bool Adjust
        {
            get
            { return _contactPair.Adjust; }
            set
            {
                _contactPair.Adjust = value;
                CustomPropertyDescriptor cpd = base.DynamicCustomTypeDescriptor.GetProperty(nameof(AdjustmentSize));
                cpd.SetIsBrowsable(_contactPair.Adjust);
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "Adjustment size")]
        [DescriptionAttribute("Set a distance inside which the slave nodes are projected to the master surface.")]
        [TypeConverter(typeof(StringLengthDefaultConverter))]
        [Id(6, 1)]
        public double AdjustmentSize { get { return _contactPair.AdjustmentSize; } set { _contactPair.AdjustmentSize = value; } }
        // MASTER ------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(0, 10, "Master region type")]
        [DescriptionAttribute("Select the master region type for the creation of the contact pair definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(1, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 2)]
        //
        public string MasterSelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(2, 10, "Master surface")]
        [DescriptionAttribute("Select the master surface for the creation of the contact pair definition.")]
        [Id(3, 2)]
        public string MasterSurfaceName
        {
            get { return _contactPair.MasterRegionName; }
            set { _contactPair.MasterRegionName = value; }
        }
        // SLAVE -------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(3, 10, "Slave region type")]
        [DescriptionAttribute("Select the slave region type for the creation of the contact pair definition.")]
        [Id(1, 3)]
        public override string SlaveRegionType { get { return base.SlaveRegionType; } set { base.SlaveRegionType = value; } }
        //
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(4, 10, "Hidden")]
        [DescriptionAttribute("Hidden.")]
        [Id(2, 3)]
        public string SlaveSelectionHidden { get { return _selectionHidden; } set { _selectionHidden = value; } }
        //
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(5, 10, "Slave surface")]
        [DescriptionAttribute("Select the slave surface for the creation of the contact pair definition.")]
        [Id(3, 3)]
        public string SlaveSurfaceName
        {
            get { return _contactPair.SlaveRegionName; }
            set { _contactPair.SlaveRegionName = value; }
        }
        //
        [Category("Appearance")]
        [DisplayName("Master surface color")]
        [Description("Select the master surface color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color MasterColor { get { return _contactPair.MasterColor; } set { _contactPair.MasterColor = value; } }
        //
        [Category("Appearance")]
        [DisplayName("Slave surface color")]
        [Description("Select the slave surface color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(2, 10)]
        public Color SlaveColor { get { return _contactPair.SlaveColor; } set { _contactPair.SlaveColor = value; } }


        // Constructors                                                                                                             
        public ViewContactPair(CaeModel.ContactPair contactPair)
        {
            _contactPair = contactPair;           
            // Master
            Dictionary<RegionTypeEnum, string> masterRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(MasterSurfaceName));
            // Slave
            Dictionary<RegionTypeEnum, string> slaveRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SlaveSelectionHidden));
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SlaveSurfaceName));
            //
            SetBase(contactPair, masterRegionTypePropertyNamePairs, slaveRegionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            Method = _contactPair.Method;   // update visibility
            Adjust = _contactPair.Adjust;   // update visibility
            ApplyYesNo();
            //
            StringLengthDefaultConverter.SetInitialValue = "0.01 mm";
        }


        // Methods                                                                                                                  
        public CaeModel.ContactPair GetBase()
        {
            return _contactPair;
        }
        protected void ApplyYesNo()
        {
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(SmallSliding));
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(Adjust));
        }
        public void PopulateDropDownLists(string[] surfaceInteracionNames, string[] surfaceNames)
        {
            DynamicCustomTypeDescriptor.PopulateProperty(nameof(SurfaceInteractionName), surfaceInteracionNames);
            //
            Dictionary<RegionTypeEnum, string[]> masterRegionTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            masterRegionTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            masterRegionTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            Dictionary<RegionTypeEnum, string[]> slaveTypeListItemsPairs = new Dictionary<RegionTypeEnum, string[]>();
            slaveTypeListItemsPairs.Add(RegionTypeEnum.Selection, new string[] { "Hidden" });
            slaveTypeListItemsPairs.Add(RegionTypeEnum.SurfaceName, surfaceNames);
            //
            PopulateDropDownLists(masterRegionTypeListItemsPairs, slaveTypeListItemsPairs);
        }
        public override void UpdateRegionVisibility()
        {
            base.UpdateRegionVisibility();
            // Hide SelectionHidden
            CustomPropertyDescriptor cpd;
            // Master
            if (base.MasterRegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = DynamicCustomTypeDescriptor.GetProperty(nameof(MasterSelectionHidden));
                cpd.SetIsBrowsable(false);
            }
            // Slave
            if (base.SlaveRegionType == RegionTypeEnum.Selection.ToFriendlyString())
            {
                cpd = DynamicCustomTypeDescriptor.GetProperty(nameof(SlaveSelectionHidden));
                cpd.SetIsBrowsable(false);
            }
        }
    }

}
