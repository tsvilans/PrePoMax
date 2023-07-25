using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;
using System.Drawing;
using CaeModel;

namespace PrePoMax
{
    [Serializable]
    public class ViewTie : ViewConstraint
    {
        // Variables                                                                                                                        
        private Tie _tie;


        // Properties                                                                                                                      
        public override string Name { get { return _tie.Name; } set { _tie.Name = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "Position tolerance")]
        [DescriptionAttribute("Enter the value of largest distance from the master surface for which the slave nodes will be " +
                              "included in the tie constraint. Default value equals 2.5 % of the typical element size.")]
        [TypeConverter(typeof(StringLengthDefaultConverter))]
        [Id(2, 1)]
        public double PositionTolerance { get { return _tie.PositionTolerance; } set { _tie.PositionTolerance = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "Adjust")]
        [DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        [Id(3, 1)]
        public bool Adjust { get { return _tie.Adjust; } set { _tie.Adjust = value; } }

        //[CategoryAttribute("Data")]
        //[OrderedDisplayName(3, 10, "Test")]
        //[DescriptionAttribute("Set adjust to No to prevent the projection of the slave nodes on the master surface.")]
        //[TypeConverter(typeof(StringStringLengthDefaultConverter))]
        //[Id(4, 1)]
        //public string Equation { get { return _container.Equation; } set { _container.Equation = value; } }

        // MASTER ------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(0, 10, "Master region type")]
        [DescriptionAttribute("Select the master region type for the creation of the tie definition.")]
        [Id(1, 2)]
        public override string MasterRegionType { get { return base.MasterRegionType; } set { base.MasterRegionType = value; } }
        //
        [CategoryAttribute("Master Region")]
        [OrderedDisplayName(1, 10, "Master surface")]
        [DescriptionAttribute("Select the master surface for the creation of the tie definition.")]
        [Id(2, 2)]
        public string MasterSurfaceName { get { return _tie.MasterRegionName; } set { _tie.MasterRegionName = value; } }
        // SLAVE -------------------------------------------------------------------------------------------------------------------
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(0, 10, "Slave region type")]
        [DescriptionAttribute("Select the slave region type for the creation of the tie definition.")]
        [Id(1, 3)]
        public override string SlaveRegionType { get { return base.SlaveRegionType; } set { base.SlaveRegionType = value; } }
        //
        [CategoryAttribute("Slave Region")]
        [OrderedDisplayName(1, 10, "Slave surface")]
        [DescriptionAttribute("Select the slave surface for the creation of the tie definition.")]
        [Id(2, 3)]
        public string SlaveSurfaceName { get { return _tie.SlaveRegionName; } set { _tie.SlaveRegionName = value; } }
        // -------------------------------------------------------------------------------------------------------------------------
        [Category("Appearance")]
        [DisplayName("Master surface color")]
        [Description("Select the master surface color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(1, 10)]
        public Color MasterColor { get { return _tie.MasterColor; } set { _tie.MasterColor = value; } }
        //
        [Category("Appearance")]
        [DisplayName("Slave surface color")]
        [Description("Select the slave surface color.")]
        [Editor(typeof(UserControls.ColorEditorEx), typeof(UITypeEditor))]
        [Id(2, 10)]
        public Color SlaveColor { get { return _tie.SlaveColor; } set { _tie.SlaveColor = value; } }


        // Constructors                                                                                                             
        public ViewTie(Tie tie)
        {
            _tie = tie;
            // Master
            Dictionary<RegionTypeEnum, string> masterRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(MasterSelectionHidden));
            masterRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(MasterSurfaceName));
            // Slave
            Dictionary<RegionTypeEnum, string> slaveRegionTypePropertyNamePairs = new Dictionary<RegionTypeEnum, string>();
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.Selection, nameof(SlaveSelectionHidden));
            slaveRegionTypePropertyNamePairs.Add(RegionTypeEnum.SurfaceName, nameof(SlaveSurfaceName));
            //
            SetBase(tie, masterRegionTypePropertyNamePairs, slaveRegionTypePropertyNamePairs);
            DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            DynamicCustomTypeDescriptor.RenameBooleanPropertyToYesNo(nameof(Adjust));
            //
            StringLengthDefaultConverter.SetInitialValue = "0.05 mm";
        }


        // Methods                                                                                                                  
        public override Constraint GetBase()
        {
            return _tie;
        }
        public void PopulateDropDownLists(string[] surfaceNames)
        {
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
        
    }

}
