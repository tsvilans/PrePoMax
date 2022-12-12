using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CaeGlobals;
using DynamicTypeDescriptor;
using System.Drawing.Design;

namespace PrePoMax.Settings
{
    [Serializable]
    public enum ColorSpectrum
    {
        [StandardValue("CoolWarm", DisplayName = "Cool-warm", Description = "Cool-warm")]
        CoolWarm,
        //
        [StandardValue("Rainbow", DisplayName = "Rainbow", Description = "Rainbow")]
        Rainbow,
        //
        [StandardValue("Warm", DisplayName = "Warm", Description = "Warm")]
        Warm,
        //
        [StandardValue("Cool", DisplayName = "Cool", Description = "Cool")]
        Cool,
        //
        [StandardValue("Cividis", DisplayName = "Cividis", Description = "Cividis")]
        Cividis,
        //
        [StandardValue("Viridis", DisplayName = "Viridis", Description = "Viridis")]
        Viridis,
        //
        [StandardValue("Plasma", DisplayName = "Plasma", Description = "Plasma")]
        Plasma,
        //
        [StandardValue("BlackBody", DisplayName = "Black body", Description = "Black body")]
        BlackBody,
        //
        [StandardValue("Inferno", DisplayName = "Inferno", Description = "Inferno")]
        Inferno,
        //
        [StandardValue("Kindlmann", DisplayName = "Kindlmann", Description = "Kindlmann")]
        Kindlmann,
        //
        [StandardValue("Grayscale", DisplayName = "Grayscale", Description = "Grayscale")]
        Grayscale
    }

    [Serializable]
    public class ViewLegendSettings : IViewSettings, IReset
    {
        // Variables                                                                                                                
        private LegendSettings _legendSettings;
        private DynamicCustomTypeDescriptor _dctd = null;
       

        // Color spectrum settings                                                                   
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(0, 10, "Color spectrum type")]
        [DescriptionAttribute("Select the color spectrum type for the visualization of the results.")]
        public ColorSpectrum ColorSpectrum 
        { 
            get
            {
                if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.CoolWarm) return ColorSpectrum.CoolWarm;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Rainbow) return ColorSpectrum.Rainbow;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Warm) return ColorSpectrum.Warm;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Cool) return ColorSpectrum.Cool;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Cividis) return ColorSpectrum.Cividis;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Viridis) return ColorSpectrum.Viridis;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Plasma) return ColorSpectrum.Plasma;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.BlackBody) return ColorSpectrum.BlackBody;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Inferno) return ColorSpectrum.Inferno;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Kindlmann) return ColorSpectrum.Kindlmann;
                else if (_legendSettings.ColorSpectrum.Type == vtkControl.vtkColorSpectrumType.Grayscale) return ColorSpectrum.Grayscale;
                else throw new NotSupportedException();
            }
            set
            {
                if (value == ColorSpectrum.CoolWarm) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.CoolWarm;
                else if (value == ColorSpectrum.Rainbow) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Rainbow;
                else if (value == ColorSpectrum.Warm) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Warm;
                else if (value == ColorSpectrum.Cool) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Cool;
                else if (value == ColorSpectrum.Cividis) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Cividis;
                else if (value == ColorSpectrum.Viridis) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Viridis;
                else if (value == ColorSpectrum.Plasma) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Plasma;
                else if (value == ColorSpectrum.BlackBody) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.BlackBody;
                else if (value == ColorSpectrum.Inferno) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Inferno;
                else if (value == ColorSpectrum.Kindlmann) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Kindlmann;
                else if (value == ColorSpectrum.Grayscale) _legendSettings.ColorSpectrum.Type = vtkControl.vtkColorSpectrumType.Grayscale;
                else throw new NotSupportedException();
            } 
        }
        //
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(1, 10, "Brightness")]
        [DescriptionAttribute("Set the brightness of the color legend (0 ... 1).")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double ColorBrightness
        {
            get { return _legendSettings.ColorSpectrum.ColorBrightness; }
            set { _legendSettings.ColorSpectrum.ColorBrightness = value; }
        }
        //
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(2, 10, "Reverse colors")]
        [DescriptionAttribute("Reverse colors.")]
        public bool ReverseColors
        {
            get { return _legendSettings.ColorSpectrum.ReverseColors; }
            set { _legendSettings.ColorSpectrum.ReverseColors = value; }
        }
        //
        [CategoryAttribute("Color spectrum settings")]
        [OrderedDisplayName(3, 10, "Number of discrete colors")]
        [DescriptionAttribute("Set the number of discrete colors (2 ... 24).")]
        public int NumberOfDiscreteColors
        {
            get { return _legendSettings.ColorSpectrum.NumberOfColors; }
            set { _legendSettings.ColorSpectrum.NumberOfColors = value; }
        }


        // Color spectrum values                                                                     
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(0, 10, "Number format")]
        [DescriptionAttribute("Select the number format.")]
        public AnnotationNumberFormat LegendNumberFormat { get { return _legendSettings.NumberFormat; } set { _legendSettings.NumberFormat = value; } }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(1, 10, "Number of significant digits")]
        [DescriptionAttribute("Set the number of significant digits (2 ... 8).")]
        public int NumberOfSignificantDigits { get { return _legendSettings.NumberOfSignificantDigits; } set { _legendSettings.NumberOfSignificantDigits = value; } }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(2, 10, "Min/max limit type")]
        [DescriptionAttribute("Select the min/max limit type.")]
        public vtkControl.vtkColorSpectrumMinMaxType ColorSpectrumMinMaxType
        {
            get { return _legendSettings.ColorSpectrum.MinMaxType; }
            set 
            {
                _legendSettings.ColorSpectrum.MinMaxType = value;
                if (value == vtkControl.vtkColorSpectrumMinMaxType.Automatic)
                {
                    _dctd.GetProperty(nameof(ColorSpectrumMin)).SetIsBrowsable(false);
                    _dctd.GetProperty(nameof(ColorSpectrumMax)).SetIsBrowsable(false);
                    _dctd.GetProperty(nameof(ColorSpectrumMinColor)).SetIsBrowsable(false);
                    _dctd.GetProperty(nameof(ColorSpectrumMaxColor)).SetIsBrowsable(false);
                }
                else 
                {
                    _dctd.GetProperty(nameof(ColorSpectrumMin)).SetIsBrowsable(true);
                    _dctd.GetProperty(nameof(ColorSpectrumMax)).SetIsBrowsable(true);
                    _dctd.GetProperty(nameof(ColorSpectrumMinColor)).SetIsBrowsable(true);
                    _dctd.GetProperty(nameof(ColorSpectrumMaxColor)).SetIsBrowsable(true);
                }
            }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(4, 10, "Max value")]
        [DescriptionAttribute("Set the max limit value.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double ColorSpectrumMax
        {
            get { return _legendSettings.ColorSpectrum.MaxUserValue; }
            set { _legendSettings.ColorSpectrum.MaxUserValue = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(3, 10, "Min value")]
        [DescriptionAttribute("Set the min limit value.")]
        [TypeConverter(typeof(StringDoubleConverter))]
        public double ColorSpectrumMin
        {
            get { return _legendSettings.ColorSpectrum.MinUserValue; }
            set { _legendSettings.ColorSpectrum.MinUserValue = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(6, 10, "Max color")]
        [DescriptionAttribute("Select the color for the values above the max value.")]
        public System.Drawing.Color ColorSpectrumMaxColor
        {
            get { return _legendSettings.ColorSpectrum.MaxColor; }
            set { _legendSettings.ColorSpectrum.MaxColor = value; }
        }
        //
        [CategoryAttribute("Color spectrum values")]
        [OrderedDisplayName(5, 10, "Min color")]
        [DescriptionAttribute("Select the color for the values below the min value.")]
        public System.Drawing.Color ColorSpectrumMinColor
        {
            get { return _legendSettings.ColorSpectrum.MinColor; }
            set { _legendSettings.ColorSpectrum.MinColor = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(0, 10, "Background type")]
        [DescriptionAttribute("Select the background type.")]
        public AnnotationBackgroundType BackgroundType
        {
            get { return _legendSettings.BackgroundType; }
            set { _legendSettings.BackgroundType = value; }
        }
        //
        [CategoryAttribute("Design")]
        [OrderedDisplayName(1, 10, "Draw a border rectangle")]
        [DescriptionAttribute("Draw a border rectangle around the legend.")]
        public bool DrawBorder
        {
            get { return _legendSettings.DrawBorder; }
            set { _legendSettings.DrawBorder = value; }
        }


        // Constructors                                                                               
        public ViewLegendSettings(LegendSettings legendSettings)
        {
            _legendSettings = legendSettings;
            _dctd = ProviderInstaller.Install(this);
            //
            ColorSpectrumMinMaxType = _legendSettings.ColorSpectrum.MinMaxType; // add this also to Reset()
            // Now lets display Yes/No instead of True/False
            _dctd.RenameBooleanPropertyToYesNo(nameof(ReverseColors));
            _dctd.RenameBooleanPropertyToYesNo(nameof(DrawBorder));
        }


        // Methods                                                                               
        public ISettings GetBase()
        {
            return _legendSettings;
        }
        public void Reset()
        {
            _legendSettings.Reset();
            //
            ColorSpectrumMinMaxType = _legendSettings.ColorSpectrum.MinMaxType;
        }
    }

}
