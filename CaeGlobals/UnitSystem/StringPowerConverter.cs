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
    public class StringPowerConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PowerUnit _powerUnit = PowerUnit.Watt;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _powerUnit = (PowerUnit)MyUnit.NoUnit;
                else if (value == MyUnit.PoundForceInchPerSecondAbbreviation) _powerUnit = MyUnit.PoundForceInchPerSecond;
                else { _powerUnit = Power.ParseUnit(value); }
            }
        }
        public static string GetUnitAbbreviation(PowerUnit powerUnit)
        {
            string unit;
            if ((int)powerUnit == MyUnit.NoUnit) unit = "";
            else if (powerUnit == MyUnit.PoundForceInchPerSecond) return MyUnit.PoundForceInchPerSecondAbbreviation;
            else unit = Power.GetAbbreviation(powerUnit);
            return unit;
        }


        // Constructors                                                                                                             
        public StringPowerConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
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
                        string unit = GetUnitAbbreviation(_powerUnit);
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
                // 1 inch = 0.0254 meters
                // 1 pound force = 4.44822162 newtons
                // 1 pound force inch = 0.112984829 joules
                // 1 pound force inch per second = 0.112984829 watts
                double conversion = 0.112984829;
                double scale = 1;
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                // Check if it is given in unsupported units
                if (valueWithUnitString.Contains(MyUnit.PoundForceInchPerSecondAbbreviation))
                {
                    valueWithUnitString = valueWithUnitString.Replace(MyUnit.PoundForceInchPerSecondAbbreviation, "W");
                    scale = conversion;
                }
                // Check if it must be converted to unsupported units
                double value;
                if ((int)_powerUnit == MyUnit.NoUnit)
                {
                    Power power = Power.Parse(valueWithUnitString);
                    value = (double)power.Value;
                }
                else if (_powerUnit == MyUnit.PoundForceInchPerSecond)
                {
                    Power power = Power.Parse(valueWithUnitString).ToUnit(PowerUnit.Watt);
                    if (scale == conversion) value = (double)power.Value;
                    else value = scale * (double)power.Value / conversion;
                }
                else
                {
                    Power power = Power.Parse(valueWithUnitString).ToUnit(_powerUnit);
                    value = scale * (double)power.Value;
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
            string supportedUnitAbbreviations = "Supported power abbreviations: ";
            var allUnits = Power.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Power.GetAbbreviation(allUnits[i]);
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