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
    public class StringTemperatureConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static TemperatureUnit _temperatureUnit = TemperatureUnit.DegreeCelsius;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _temperatureUnit = (TemperatureUnit)MyUnit.NoUnit;
                else _temperatureUnit = Temperature.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringTemperatureConverter()
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
                        if ((int)_temperatureUnit != MyUnit.NoUnit)
                            valueString += " " + Temperature.GetAbbreviation(_temperatureUnit);
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
                Temperature temperature = Temperature.Parse(valueWithUnitString);
                if ((int)_temperatureUnit != MyUnit.NoUnit) temperature = temperature.ToUnit(_temperatureUnit);
                return temperature.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported temperature abbreviations: ";
            var allUnits = Temperature.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Temperature.GetAbbreviation(allUnits[i]);
                if (abb != null) abb.Trim();
                if (abb.Length > 0) supportedUnitAbbreviations += abb;
                if (i != allUnits.Length - 1) supportedUnitAbbreviations += ", ";
            }
            supportedUnitAbbreviations += ".";
            //
            return supportedUnitAbbreviations;
        }
        public static string SupportedDeltaUnitAbbreviations()
        {
            string abb;
            // Must be temperature and not temperature delta since ∆ is only internal
            string supportedUnitAbbreviations = "Supported temperature abbreviations: ";
            var allUnits = TemperatureDelta.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = TemperatureDelta.GetAbbreviation(allUnits[i]).Replace("∆", "");
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