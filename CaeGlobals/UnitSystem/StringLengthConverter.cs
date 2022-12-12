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
    public class StringLengthConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;


        // Properties                                                                                                               
        public static string SetUnit 
        {
            set
            {
                if (value == "") _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                else _lengthUnit = Length.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringLengthConverter()
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
                        if ((int)_lengthUnit != MyUnit.NoUnit) valueString += " " + Length.GetAbbreviation(_lengthUnit);
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
                Length length = Length.Parse(valueWithUnitString);
                if ((int)_lengthUnit != MyUnit.NoUnit) length = length.ToUnit(_lengthUnit);
                return length.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported length abbreviations: ";
            var allUnits = Length.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Length.GetAbbreviation(allUnits[i]);
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