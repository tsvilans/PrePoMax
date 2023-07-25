using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DynamicTypeDescriptor;
using CaeModel;

namespace PrePoMax
{
   [Serializable]
    public class ViewThermalConductivity : ViewMaterialProperty
    {
        // Variables                                                                                                                
        private List<ThermalConductivityDataPoint> _points;


        // Properties                                                                                                               
        public override string Name
        {
            get { return "Thermal Conductivity"; }
        }
        //
        [Browsable(false)]
        public override MaterialProperty Base
        {
            get
            {
                int i = 0;
                double[][] thermalConductivityTemp = new double[_points.Count][];
                //
                foreach (ThermalConductivityDataPoint point in _points)
                {
                    thermalConductivityTemp[i] = new double[2];
                    thermalConductivityTemp[i][0] = point.ThermalConductivity;
                    thermalConductivityTemp[i][1] = point.Temperature;
                    i++;
                }
                ThermalConductivity thermalConductivity = new ThermalConductivity(thermalConductivityTemp);
                //
                return thermalConductivity;
            }
        }
        //
        [Browsable(false)]
        public List<ThermalConductivityDataPoint> DataPoints { get { return _points; } set { _points = value; } }
        //
        [CategoryAttribute("Data"),
        DisplayName("Thermal conductivity"),
        DescriptionAttribute("The value of the thermal conductivity coefficient.")]
        [TypeConverter(typeof(CaeGlobals.StringThermalConductivityConverter))]
        public double ThermalConductivity
        {
            get
            {
                if (_points != null && _points.Count > 0) return _points[0].ThermalConductivity;
                else return 0;
            }
            set
            {
                if (_points != null && _points.Count > 0) _points[0].ThermalConductivity = value;
            }
        }


        // Constructors                                                                                                             
        public ViewThermalConductivity(ThermalConductivity thermalConductivity)
        {
            _points = new List<ThermalConductivityDataPoint>();
            for (int i = 0; i < thermalConductivity.ThermalConductivityTemp.Length; i++)
            {
                _points.Add(new ThermalConductivityDataPoint(thermalConductivity.ThermalConductivityTemp[i][0],
                                                             thermalConductivity.ThermalConductivityTemp[i][1]));
            }
            //
            base.DynamicCustomTypeDescriptor = ProviderInstaller.Install(this);
        }


        // Methods                                                                                                                  

    }
}
