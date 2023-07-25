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

namespace PrePoMax.Forms
{    
    [Serializable]
    public class ViewExplodedViewParameters
    {
        // Variables                                                                                                                
        private DynamicCustomTypeDescriptor _dctd = null;
        private ExplodedViewParameters _explodedViewParameters;


        // Properties                                                                                                               
        [Browsable(false)]
        public ExplodedViewParameters Parameters
        {
            get { return _explodedViewParameters; }
            set { _explodedViewParameters = value; }
        }
        //
        [Category("Method")]
        [OrderedDisplayName(0, 10, "Exploded view method")]
        [DescriptionAttribute("Select the exploded view method.")]
        [Id(1, 1)]
        public ExplodedViewMethodEnum Method
        {
            get { return _explodedViewParameters.Method; }
            set
            {
                _explodedViewParameters.Method = value;
                UpdateVisibility();
            }
        }
        //
        [Category("Method")]
        [OrderedDisplayName(1, 10, "X coordinate")]
        [DescriptionAttribute("Enter the center point X coordinate.")]
        [Id(2, 1)]
        public double CenterX
        {
            get { return _explodedViewParameters.CenterX; }
            set { _explodedViewParameters.CenterX = value; }
        }
        //
        [Category("Method")]
        [OrderedDisplayName(2, 10, "Y coordinate")]
        [DescriptionAttribute("Enter the center point Y coordinate.")]
        [Id(3, 1)]
        public double CenterY
        {
            get { return _explodedViewParameters.CenterY; }
            set { _explodedViewParameters.CenterY = value; }
        }
        //
        [Category("Method")]
        [OrderedDisplayName(3, 10, "Z coordinate")]
        [DescriptionAttribute("Enter the center point Z coordinate.")]
        [Id(4, 1)]
        public double CenterZ
        {
            get { return _explodedViewParameters.CenterZ; }
            set { _explodedViewParameters.CenterZ = value; }
        }
        //
        [Category("Direction and scaling")]
        [OrderedDisplayName(0, 10, "Exploded view direction")]
        [DescriptionAttribute("Select the exploded view direction.")]
        [Id(1, 2)]
        public ExplodedViewDirectionEnum Direction
        {
            get { return _explodedViewParameters.Direction; }
            set { _explodedViewParameters.Direction = value; }
        }
        //
        [Category("Direction and scaling")]
        [OrderedDisplayName(1, 10, "Magnification")]
        [DescriptionAttribute("Select the exploded view magnification (larger than 1).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(2, 2)]
        public double Magnification
        {
            get { return _explodedViewParameters.Magnification; }
            set
            {
                _explodedViewParameters.Magnification = value;
                if (_explodedViewParameters.Magnification < 1) _explodedViewParameters.Magnification = 1;
                else if (_explodedViewParameters.Magnification > 25) _explodedViewParameters.Magnification = 25;
            }
        }
        //
        [Category("Direction and scaling")]
        [OrderedDisplayName(2, 10, "Scale factor")]
        [DescriptionAttribute("Select the exploded view scale factor [0, 1].")]
        [TypeConverter(typeof(StringDoubleConverter))]
        [Id(3, 2)]
        public double ScaleFactor
        {
            get { return _explodedViewParameters.ScaleFactor; }
            set
            {
                _explodedViewParameters.ScaleFactor = value;
                if (_explodedViewParameters.ScaleFactor < 0) _explodedViewParameters.ScaleFactor = 0;
                else if (_explodedViewParameters.ScaleFactor > 1) _explodedViewParameters.ScaleFactor = 1;
            }
        }


        // Constructors                                                                                                             
        public ViewExplodedViewParameters()
        {
            _explodedViewParameters = new ExplodedViewParameters();
            //
            _dctd = ProviderInstaller.Install(this);
            _dctd.CategorySortOrder = CustomSortOrder.AscendingById;
            _dctd.PropertySortOrder = CustomSortOrder.AscendingById;
            //
            UpdateVisibility();
        }


        // Methods                                                                                                                  
        public void UpdateVisibility()
        {
            bool visible = _explodedViewParameters.Method == ExplodedViewMethodEnum.CenterPoint;
            _dctd.GetProperty(nameof(CenterX)).SetIsBrowsable(visible);
            _dctd.GetProperty(nameof(CenterY)).SetIsBrowsable(visible);
            _dctd.GetProperty(nameof(CenterZ)).SetIsBrowsable(visible);
        }

       

    }
}
