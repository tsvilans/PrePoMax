using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeGlobals;

namespace PrePoMax
{   
    [Serializable]
    public class ViewSurfaceBehavior : ViewSurfaceInteractionProperty
    {
        // Variables                                                                                                                
        private CaeModel.SurfaceBehavior _surfaceBehavior;
        private List<PressureOverclosureDataPoint> _points;


        // Properties                                                                                                               
        [Browsable(false)]
        public override string Name
        {
            get { return "Surface behavior"; }
        }
        //
        [Browsable(false)]
        public List<PressureOverclosureDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [Browsable(false)]
        public override CaeModel.SurfaceInteractionProperty Base
        {
            get
            {
                int i = 0;
                double[][] pressureOverclosure = new double[_points.Count][];
                foreach (PressureOverclosureDataPoint point in _points)
                {
                    pressureOverclosure[i] = new double[2];
                    pressureOverclosure[i][0] = point.Pressure;
                    pressureOverclosure[i][1] = point.Overclosure;
                    i++;
                }
                _surfaceBehavior.PressureOverclosure = pressureOverclosure;
                //
                return _surfaceBehavior;
            }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(0, 10, "Type")]
        [DescriptionAttribute("Select the pressure-overclosure type for this surface bahavior.")]
        public CaeModel.PressureOverclosureEnum PressureOverclosureType
        {
            get { return _surfaceBehavior.PressureOverclosureType; } 
            set { _surfaceBehavior.PressureOverclosureType = value; UpdateVisibility(); } 
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(1, 10, "K")]
        [Description("Slope of the pressure-overclosure curve (usually 5 to 50 times the E modulus of the adjacent materials).")]
        [TypeConverter(typeof(StringForcePerVolumeConverter))]
        public double K { get { return _surfaceBehavior.K; } set { _surfaceBehavior.K = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(2, 10, "σ infinity")]
        [Description("Tension value for large clearances (about 0.25 % of the maximum expected von Mises stress).")]
        [TypeConverter(typeof(StringPressureConverter))]
        public double Sinf { get { return _surfaceBehavior.Sinf; } set { _surfaceBehavior.Sinf = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(3, 10, "c₀")]
        [DescriptionAttribute("The value from which the maximum clearance is calculated for which a spring contact " +
                              "element is generated. The default for c₀ is 0.001 (dimensionless). " + 
                              "Not needed for surface-to-surface contact.")]
        [TypeConverter(typeof(StringDoubleDefaultConverter))]
        public double C0_lin
        {
            get
            {
                CaeModel.SurfaceBehavior surfaceBehavior = new CaeModel.SurfaceBehavior();
                StringDoubleDefaultConverter.InitialValue = surfaceBehavior.C0;
                //
                return _surfaceBehavior.C0;
            }
            set { _surfaceBehavior.C0 = value; }
        }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(4, 10, "p₀")]
        [DescriptionAttribute("The contact pressure at zero clerance (p₀ > 0).")]
        [TypeConverter(typeof(StringPressureConverter))]
        public double P0 { get { return _surfaceBehavior.P0; } set { _surfaceBehavior.P0 = value; } }
        //
        [CategoryAttribute("Data")]
        [OrderedDisplayName(5, 10, "c₀")]
        [DescriptionAttribute("The clerance at which the contact pressure is decreased to 1 % of p₀ (c₀ > 0).")]
        [TypeConverter(typeof(StringLengthConverter))]
        public double C0_exp { get { return _surfaceBehavior.C0; } set { _surfaceBehavior.C0 = value; } }
       


        // Constructors                                                                                                             
        public ViewSurfaceBehavior(CaeModel.SurfaceBehavior surfaceBehavior)
        {
            _surfaceBehavior = surfaceBehavior;
            //
            _points = new List<PressureOverclosureDataPoint>();
            for (int i = 0; i < _surfaceBehavior.PressureOverclosure.Length; i++)
            {
                _points.Add(new PressureOverclosureDataPoint(_surfaceBehavior.PressureOverclosure[i][0],
                                                             _surfaceBehavior.PressureOverclosure[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
            //
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Hard)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(K)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(P0)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp)).SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Linear)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(K)).SetIsBrowsable(true);
                DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf)).SetIsBrowsable(true);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin)).SetIsBrowsable(true);
                DynamicCustomTypeDescriptor.GetProperty(nameof(P0)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp)).SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Exponential)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(K)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(P0)).SetIsBrowsable(true);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp)).SetIsBrowsable(true);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Tabular)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(K)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(P0)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp)).SetIsBrowsable(false);
            }
            else if (_surfaceBehavior.PressureOverclosureType == CaeModel.PressureOverclosureEnum.Tied)
            {
                DynamicCustomTypeDescriptor.GetProperty(nameof(K)).SetIsBrowsable(true);
                DynamicCustomTypeDescriptor.GetProperty(nameof(Sinf)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_lin)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(P0)).SetIsBrowsable(false);
                DynamicCustomTypeDescriptor.GetProperty(nameof(C0_exp)).SetIsBrowsable(false);
            }
            else throw new NotSupportedException();
        }

    }
}
