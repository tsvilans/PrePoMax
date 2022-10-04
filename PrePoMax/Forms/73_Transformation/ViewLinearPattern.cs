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
    public class ViewLinearPattern : ViewTransformation
    {
        // Variables                                                                                                                      
        private LinearPattern _linearPattern;


        // Properties                                                                                                               
        public override string Name { get { return _linearPattern.Name; } set { _linearPattern.Name = value; } }

        [Category("Data")]
        [OrderedDisplayName(1, 10, "Number of items")]
        [DescriptionAttribute("Enter the number of all items.")]
        [Id(2, 1)]
        public int NumberOfItems { get { return _linearPattern.NumberOfItems; } set { _linearPattern.NumberOfItems = value; } }
        //
        [Category("Start point coordinates")]
        [OrderedDisplayName(0, 10, "By selection")]
        [DescriptionAttribute("Use selection for the definition of the start point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData StartPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("Start point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the start point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 2)]
        public double StartPointX { get { return _linearPattern.StartPoint[0]; } set { _linearPattern.StartPoint[0] = value; } }
        //
        [Category("Start point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the start point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 2)]
        public double StartPointY { get { return _linearPattern.StartPoint[1]; } set { _linearPattern.StartPoint[1] = value; } }
        //
        [Category("Start point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the start point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 2)]
        public double StartPointZ { get { return _linearPattern.StartPoint[2]; } set { _linearPattern.StartPoint[2] = value; } }

        //
        [Category("End point coordinates")]
        [OrderedDisplayName(0, 10, "By selection ")]    // must be a different name than for the first point !!!
        [DescriptionAttribute("Use selection for the definition of the end point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 3)]
        public ItemSetData EndPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("End point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the end point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 3)]
        public double EndPointX { get { return _linearPattern.EndPoint[0]; } set { _linearPattern.EndPoint[0] = value; } }
        //
        [Category("End point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the end point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 3)]
        public double EndPointY { get { return _linearPattern.EndPoint[1]; } set { _linearPattern.EndPoint[1] = value; } }
        //
        [Category("End point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the end point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 3)]
        public double EndPointZ { get { return _linearPattern.EndPoint[2]; } set { _linearPattern.EndPoint[2] = value; } }



        // Constructors                                                                                                             
        public ViewLinearPattern(LinearPattern linearPattern)
        {
            _linearPattern = linearPattern;
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
        public override Transformation Base { get { return _linearPattern; } }
    }
}
