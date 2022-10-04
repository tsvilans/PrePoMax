using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using CaeGlobals;
using System.ComponentModel;
using DynamicTypeDescriptor;
using System.Drawing.Design;
using CaeResults;

namespace PrePoMax.Forms
{
    [Serializable]
    public class ViewCircularPattern : ViewTransformation
    {
        // Variables                                                                                                                      
        private CircularPattern _circularPattern;


        // Properties                                                                                                               
        public override string Name { get { return _circularPattern.Name; } set { _circularPattern.Name = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Number of items")]
        [DescriptionAttribute("Enter the number of all items.")]
        [Id(2, 1)]
        public int NumberOfItems { get { return _circularPattern.NumberOfItems; } set { _circularPattern.NumberOfItems = value; } }
        //
        [Category("Data")]
        [OrderedDisplayName(2, 10, "Angle")]
        [DescriptionAttribute("Enter the angle between two items.")]
        [TypeConverter(typeof(StringAngleDegConverter))]
        [Id(3, 1)]
        public double Angle { get { return _circularPattern.Angle; } set { _circularPattern.Angle = value; } }
        //
        [Category("Start axis point coordinates")]
        [OrderedDisplayName(0, 10, "By selection")]
        [DescriptionAttribute("Use selection for the definition of the start point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData FirstPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("Start axis point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the first axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 2)]
        public double FirstPointX
        {
            get { return _circularPattern.AxisFirstPoint[0]; }
            set { _circularPattern.AxisFirstPoint[0] = value; }
        }
        //
        [Category("Start axis point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the first axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 2)]
        public double FirstPointY
        {
            get { return _circularPattern.AxisFirstPoint[1]; }
            set { _circularPattern.AxisFirstPoint[1] = value; }
        }
        //
        [Category("Start axis point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the first axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 2)]
        public double FirstPointZ
        {
            get { return _circularPattern.AxisFirstPoint[2]; }
            set { _circularPattern.AxisFirstPoint[2] = value; }
        }
        //
        [Category("End axis point coordinates")]
        [OrderedDisplayName(0, 10, "By selection ")]    // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the end point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 3)]
        public ItemSetData SecondPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("End axis point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the second axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 3)]
        public double SecondPointX
        {
            get { return _circularPattern.AxisSecondPoint[0]; }
            set { _circularPattern.AxisSecondPoint[0] = value; }
        }
        //
        [Category("End axis point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the second axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 3)]
        public double SecondPointY
        {
            get { return _circularPattern.AxisSecondPoint[1]; }
            set { _circularPattern.AxisSecondPoint[1] = value; }
        }
        //
        [Category("End axis point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the second axis point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 3)]
        public double SecondPointZ
        {
            get { return _circularPattern.AxisSecondPoint[2]; }
            set { _circularPattern.AxisSecondPoint[2] = value; }
        }


        // Constructors                                                                                                             
        public ViewCircularPattern(CircularPattern circularPattern)
        {
            _circularPattern = circularPattern;
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            _startPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _startPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
            //
            _endPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _endPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
        }


        // Methods                                                                                                                  
        public override Transformation Base { get { return _circularPattern; } }
    }
}
