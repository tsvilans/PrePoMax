using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing;

namespace PrePoMax.Settings
{
    [Serializable]
    public class ViewGraphicsSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private GraphicsSettings _graphicsSettings;
        private DynamicCustomTypeDescriptor _dctd = null;

        // Background                                                                               
        [CategoryAttribute("Background")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the background color type.")]
        public BackgroundType BackgroundType
        {
            get { return _graphicsSettings.BackgroundType; }
            set
            {
                _graphicsSettings.BackgroundType = value;
                //
                if (value == BackgroundType.White)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty(nameof(Color));
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty(nameof(TopColor));
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty(nameof(BottomColor));
                    cpd.SetIsBrowsable(false);
                }
                else if (value == BackgroundType.Solid)
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty(nameof(Color));
                    cpd.SetIsBrowsable(true);
                    cpd = _dctd.GetProperty(nameof(TopColor));
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty(nameof(BottomColor));
                    cpd.SetIsBrowsable(false);
                }
                else
                {
                    CustomPropertyDescriptor cpd;
                    cpd = _dctd.GetProperty(nameof(Color));
                    cpd.SetIsBrowsable(false);
                    cpd = _dctd.GetProperty(nameof(TopColor));
                    cpd.SetIsBrowsable(true);
                    cpd = _dctd.GetProperty(nameof(BottomColor));
                    cpd.SetIsBrowsable(true);
                }
            }
        }

        [CategoryAttribute("Background")]
        [OrderedDisplayName(1, 10, "Color")]
        [DescriptionAttribute("Select the background color.")]
        public Color Color
        {
            get { return _graphicsSettings.BottomColor; }
            set { _graphicsSettings.BottomColor = value; }
        }

        [CategoryAttribute("Background")]
        [OrderedDisplayName(2, 10, "Top color")]
        [DescriptionAttribute("Select the top background color.")]
        public Color TopColor
        {
            get { return _graphicsSettings.TopColor; }
            set { _graphicsSettings.TopColor = value; }
        }

        [CategoryAttribute("Background")]
        [OrderedDisplayName(3, 10, "Bottom color")]
        [DescriptionAttribute("Select the top bottom color.")]
        public Color BottomColor
        {
            get { return _graphicsSettings.BottomColor; }
            set { _graphicsSettings.BottomColor = value; }
        }

        [CategoryAttribute("Widgets")]
        [OrderedDisplayName(0, 10, "Coordinate system visibility")]
        [DescriptionAttribute("Turn coordinate system on or off.")]
        public bool CoorSysVisibility
        {
            get { return _graphicsSettings.CoorSysVisibility; }
            set { _graphicsSettings.CoorSysVisibility = value; }
        }
        [CategoryAttribute("Widgets")]
        [OrderedDisplayName(1, 10, "Scale widget visibility")]
        [DescriptionAttribute("Turn scale widget on or off.")]
        public bool ScaleWidgetVisibility
        {
            get { return _graphicsSettings.ScaleWidgetVisibility; }
            set { _graphicsSettings.ScaleWidgetVisibility = value; }
        }

        [CategoryAttribute("Lighting")]
        [OrderedDisplayName(0, 10, "Ambient component")]
        [DescriptionAttribute("Select the ambient light component (0 ... 1).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double AmbientComponent
        {
            get { return _graphicsSettings.AmbientComponent; }
            set { _graphicsSettings.AmbientComponent = value; }
        }

        [CategoryAttribute("Lighting")]
        [OrderedDisplayName(1, 10, "Diffuse component")]
        [DescriptionAttribute("Select the diffuse light component (0 ... 1).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double DiffuseComponent
        {
            get { return _graphicsSettings.DiffuseComponent; }
            set { _graphicsSettings.DiffuseComponent = value; }
        }

        [CategoryAttribute("Smoothing")]
        [OrderedDisplayName(0, 10, "Point smoothing")]
        [DescriptionAttribute("Enable/disable point smoothing.")]
        public bool PointSmoothing
        {
            get { return _graphicsSettings.PointSmoothing; }
            set { _graphicsSettings.PointSmoothing = value; }
        }

        [CategoryAttribute("Smoothing")]
        [OrderedDisplayName(1, 10, "Line smoothing")]
        [DescriptionAttribute("Enable/disable line smoothing.")]
        public bool LineSmoothing
        {
            get { return _graphicsSettings.LineSmoothing; }
            set { _graphicsSettings.LineSmoothing = value; }
        }

        [CategoryAttribute("Geometry")]
        [OrderedDisplayName(0, 10, "CAD deflection")]
        [DescriptionAttribute("This parameter controls the deflection of the geometry visualization, i.e. how accurate it is at approximating the actual curved surfaces")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double GeometryDeflection
        {
            get { return _graphicsSettings.GeometryDeflection; }
            set { _graphicsSettings.GeometryDeflection = value; }
        }


        // Constructors                                                                               
        public ViewGraphicsSettings(GraphicsSettings graphicsSettings)
        {
            _graphicsSettings = graphicsSettings;
            _dctd = ProviderInstaller.Install(this);

            BackgroundType = _graphicsSettings.BackgroundType;  // add this also to Reset()

            CustomPropertyDescriptor cpd;

            // now lets display On/Off instead of True/False
            _dctd.RenameBooleanPropertyToOnOff(nameof(CoorSysVisibility));
            _dctd.RenameBooleanPropertyToOnOff(nameof(ScaleWidgetVisibility));
            _dctd.RenameBooleanPropertyToOnOff(nameof(PointSmoothing));
            _dctd.RenameBooleanPropertyToOnOff(nameof(LineSmoothing));
        }


        // Methods                                                                               
        public ISettings GetBase()
        {
            return _graphicsSettings;
        }

        public void Reset()
        {
            _graphicsSettings.Reset();
            //
            BackgroundType = _graphicsSettings.BackgroundType;
        }
    }

}
