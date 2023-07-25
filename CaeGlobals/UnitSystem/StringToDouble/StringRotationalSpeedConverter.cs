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
    public class StringRotationalSpeedConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static RotationalSpeedUnit _rotationalSpeedUnit = RotationalSpeedUnit.RadianPerSecond;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _rotationalSpeedUnit = (RotationalSpeedUnit)MyUnit.NoUnit;
                else _rotationalSpeedUnit = RotationalSpeed.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringRotationalSpeedConverter()
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
                        if ((int)_rotationalSpeedUnit != MyUnit.NoUnit)
                            valueString += " " + RotationalSpeed.GetAbbreviation(_rotationalSpeedUnit);
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
                RotationalSpeed rotationalSpeed = RotationalSpeed.Parse(valueWithUnitString);
                if ((int)_rotationalSpeedUnit != MyUnit.NoUnit) rotationalSpeed = rotationalSpeed.ToUnit(_rotationalSpeedUnit);
                return rotationalSpeed.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported rotational speed abbreviations: ";
            var allUnits = RotationalSpeed.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = RotationalSpeed.GetAbbreviation(allUnits[i]);
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