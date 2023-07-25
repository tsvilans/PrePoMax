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
    public class ViewSymmetry : ViewTransformation
    {
        // Variables                                                                                                                      
        private Symmetry _symmetry;


        // Properties                                                                                                               
        public override string Name { get { return _symmetry.Name; } set { _symmetry.Name = value; } }
        [Browsable(false)]
        [Category("Data")]
        [OrderedDisplayName(1, 10, "Symmetry plane")]
        [DescriptionAttribute("Select the symmetry plane.")]
        [Id(2, 1)]
        public SymmetryPlaneEnum SymmetryPlane { get { return _symmetry.SymmetryPlane; } set { _symmetry.SymmetryPlane = value; } }
        //
        [Category("Symmetry point coordinates")]
        [OrderedDisplayName(0, 10, "By selection")]
        [DescriptionAttribute("Use selection for the definition of the symmetry point.")]
        [EditorAttribute(typeof(SinglePointDataEditor), typeof(UITypeEditor))]
        [Id(1, 2)]
        public ItemSetData SymmetryPointItemSet
        {
            get { return _startPointItemSetData; }
            set { _startPointItemSetData = value; }
        }
        //
        [Category("Symmetry point coordinates")]
        [OrderedDisplayName(1, 10, "X")]
        [Description("X coordinate of the symmetry point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(2, 2)]
        public double SymmetryPointX { get { return _symmetry.PointCoor[0]; } set { _symmetry.PointCoor[0] = value; } }
        //
        [Category("Symmetry point coordinates")]
        [OrderedDisplayName(2, 10, "Y")]
        [Description("Y coordinate of the symmetry point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(3, 2)]
        public double SymmetryPointY { get { return _symmetry.PointCoor[1]; } set { _symmetry.PointCoor[1] = value; } }
        //
        [Category("Symmetry point coordinates")]
        [OrderedDisplayName(3, 10, "Z")]
        [Description("Z coordinate of the symmetry point.")]
        [TypeConverter(typeof(StringLengthConverter))]
        [Id(4, 2)]
        public double SymmetryPointZ { get { return _symmetry.PointCoor[2]; } set { _symmetry.PointCoor[2] = value; } }
        


        // Constructors                                                                                                             
        public ViewSymmetry(Symmetry symmetry)
        {
            _symmetry = symmetry;
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            _startPointItemSetData = new ItemSetData(); // needed to display ItemSetData.ToString()
            _startPointItemSetData.ToStringType = ItemSetDataToStringType.SelectSinglePoint;
        }


        // Methods                                                                                                                  
        public override Transformation Base { get { return _symmetry; } }
    }
}
