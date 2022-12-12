using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet;
using UnitsNet.Units;

namespace CaeGlobals
{
    public class StringEnergyConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static EnergyUnit _energyUnit = EnergyUnit.Joule;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _energyUnit = (EnergyUnit)MyUnit.NoUnit;
                else if (value == MyUnit.PoundForceInchAbbreviation) _energyUnit = MyUnit.PoundForceInch;
                else { _energyUnit = Energy.ParseUnit(value); }
            }
        }
        public static string GetUnitAbbreviation(EnergyUnit energyUnit)
        {
            string unit;
            if ((int)energyUnit == MyUnit.NoUnit) unit = "";
            else if (energyUnit == MyUnit.PoundForceInch) return MyUnit.PoundForceInchAbbreviation;
            else unit = Energy.GetAbbreviation(energyUnit);
            return unit;
        }


        // Constructors                                                                                                             
        public StringEnergyConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString) return MyNCalc.ConvertFromString(valueString, ConvertToCurrentUnits);
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Convert to string
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        string valueString = valueDouble.ToString();
                        string unit = GetUnitAbbreviation(_energyUnit);
                        if (unit.Length > 0) valueString += " " + unit;
                        return valueString;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
        //
        public static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                // 1 inch = 1/12 foot
                double conversion = 1.0 / 12.0;
                double scale = 1;
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                // Check if it is given in unsupported units
                if (valueWithUnitString.Contains(MyUnit.PoundForceInchAbbreviation))
                {
                    valueWithUnitString = valueWithUnitString.Replace(MyUnit.PoundForceInchAbbreviation, "ft·lb");
                    scale = conversion;
                }
                // Check if it must be converted to unsupported units
                double value;
                if ((int)_energyUnit == MyUnit.NoUnit)
                {
                    Energy energy = Energy.Parse(valueWithUnitString);
                    value = energy.Value;
                }
                else if (_energyUnit == MyUnit.PoundForceInch)
                {
                    Energy energy = Energy.Parse(valueWithUnitString).ToUnit(EnergyUnit.FootPound);
                    if (scale == conversion) value = energy.Value;
                    else value = scale * energy.Value / conversion;
                }
                else
                {
                    Energy energy = Energy.Parse(valueWithUnitString).ToUnit(_energyUnit);
                    value = scale * energy.Value;
                }
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported energy abbreviations: ";
            var allUnits = Energy.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Energy.GetAbbreviation(allUnits[i]);
                if (abb != null) abb.Trim();
                if (abb.Length > 0) supportedUnitAbbreviations += abb;
                if (i != allUnits.Length - 1) supportedUnitAbbreviations += ", ";
            }
            supportedUnitAbbreviations += ".";
            //
            return supportedUnitAbbreviations;
        }
    }

}