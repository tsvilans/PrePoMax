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
    public class StringForcePerLenghtConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static ForcePerLengthUnit _forcePerLengthUnit = ForcePerLengthUnit.NewtonPerMeter;


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _forcePerLengthUnit = (ForcePerLengthUnit)MyUnit.NoUnit;
                else _forcePerLengthUnit = ForcePerLength.ParseUnit(value);
            }
        }

        // Constructors                                                                                                             
        public StringForcePerLenghtConverter()
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
                        if ((int)_forcePerLengthUnit != MyUnit.NoUnit)
                            valueString += " " + ForcePerLength.GetAbbreviation(_forcePerLengthUnit);
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
        private static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                ForcePerLength forcePerLength = ForcePerLength.Parse(valueWithUnitString);
                if ((int)_forcePerLengthUnit != MyUnit.NoUnit) forcePerLength = forcePerLength.ToUnit(_forcePerLengthUnit);
                return forcePerLength.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported force per length abbreviations: ";
            var allUnits = ForcePerLength.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = ForcePerLength.GetAbbreviation(allUnits[i]);
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