using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaeMesh;
using System.ComponentModel;
using CaeGlobals;

namespace CaeModel
{
    [Serializable]
    public class Material : NamedClass
    {
        // Variables                                                                                                                
        private string _description;
        private bool _temperatureDependent;
        private List<MaterialProperty> _properties;


        // Properties                                                                                                               
        public string Description { get { return _description; } set { _description = value; } }
        public bool TemperatureDependent { get { return _temperatureDependent; } set { _temperatureDependent = value; } }
        public List<MaterialProperty> Properties { get { return _properties; } }


        // Constructors                                                                                                             
        public Material(string name)
            : base(name)
        {
            _properties = new List<MaterialProperty>();
        }


        // Methods                                                                                                                  
        public void AddProperty(MaterialProperty property)
        {
            _properties.Add(property);
        }
        public MaterialProperty GetProperty<T>()
        {
            foreach (MaterialProperty property in _properties)
            {
                if (property is T) return property;
            }
            return null;
        }
        public void ConvertUnits(UnitSystem currentSystem, UnitSystem fromSystem, UnitSystem toSystem)
        {
            try
            {
                StringDensityConverter sdc = new StringDensityConverter();
                StringPressureConverter spc = new StringPressureConverter();
                StringTemperatureConverter stc = new StringTemperatureConverter();
                //
                StringThermalExpansionConverter stec = new StringThermalExpansionConverter();
                StringThermalConductivityConverter stcc = new StringThermalConductivityConverter();
                StringSpecificHeatConverter sshc = new StringSpecificHeatConverter();
                //
                foreach (var property in _properties)
                {
                    if (property is Density den)
                    {
                        for (int i = 0; i < den.DensityTemp.Length; i++)
                        {
                            // Desity
                            den.DensityTemp[i][0] = fromSystem.Convert(den.DensityTemp[i][0], sdc, toSystem);
                            // Temp
                            den.DensityTemp[i][1] = fromSystem.Convert(den.DensityTemp[i][1], stc, toSystem);
                        }
                    }
                    else if (property is SlipWear sw)
                    {
                        // Hardness
                        sw.Hardness = fromSystem.Convert(sw.Hardness, spc, toSystem);
                        // Wear coefficient
                        sw.WearCoefficient = sw.WearCoefficient;
                    }
                    else if (property is Elastic el)
                    {
                        for (int i = 0; i < el.YoungsPoissonsTemp.Length; i++)
                        {
                            // Youngs modulus
                            el.YoungsPoissonsTemp[i][0] = fromSystem.Convert(el.YoungsPoissonsTemp[i][0], spc, toSystem);
                            // Temp
                            el.YoungsPoissonsTemp[i][2] = fromSystem.Convert(el.YoungsPoissonsTemp[i][2], stc, toSystem);
                        }
                    }
                    else if (property is ElasticWithDensity ewd)
                    {
                        ewd.YoungsModulus = fromSystem.Convert(ewd.YoungsModulus, spc, toSystem);
                        ewd.Density = fromSystem.Convert(ewd.Density, sdc, toSystem);
                    }
                    else if (property is Plastic pl)
                    {
                        for (int i = 0; i < pl.StressStrainTemp.Length; i++)
                        {
                            // Stress
                            pl.StressStrainTemp[i][0] = fromSystem.Convert(pl.StressStrainTemp[i][0], spc, toSystem);
                            // Temp
                            pl.StressStrainTemp[i][2] = fromSystem.Convert(pl.StressStrainTemp[i][2], stc, toSystem);
                        }
                    }
                    else if (property is ThermalExpansion te)
                    {
                        for (int i = 0; i < te.ThermalExpansionTemp.Length; i++)
                        {
                            // Expansion
                            te.ThermalExpansionTemp[i][0] = fromSystem.Convert(te.ThermalExpansionTemp[i][0], stec, toSystem);
                            // Temp
                            te.ThermalExpansionTemp[i][1] = fromSystem.Convert(te.ThermalExpansionTemp[i][1], stc, toSystem);
                        }
                    }
                    else if (property is ThermalConductivity tc)
                    { 
                        for (int i = 0; i < tc.ThermalConductivityTemp.Length; i++)
                        {
                            // Conductivity
                            tc.ThermalConductivityTemp[i][0] = fromSystem.Convert(tc.ThermalConductivityTemp[i][0], stcc, toSystem);
                            // Temp
                            tc.ThermalConductivityTemp[i][1] = fromSystem.Convert(tc.ThermalConductivityTemp[i][1], stc, toSystem);
                        }
                    }
                    else if (property is SpecificHeat sh)
                    {
                        for (int i = 0; i < sh.SpecificHeatTemp.Length; i++)
                        {
                            // Conductivity
                            sh.SpecificHeatTemp[i][0] = fromSystem.Convert(sh.SpecificHeatTemp[i][0], sshc, toSystem);
                            // Temp
                            sh.SpecificHeatTemp[i][1] = fromSystem.Convert(sh.SpecificHeatTemp[i][1], stc, toSystem);
                        }
                    }
                    else throw new NotSupportedException();
                }
            }
            catch (Exception ex)
            {
                ExceptionTools.Show(this, ex); 
            }
            finally
            {
                // fromSystem.Convert calls changes converter units - reset it here
                currentSystem.SetConverterUnits();
            }
        }
    }
}
