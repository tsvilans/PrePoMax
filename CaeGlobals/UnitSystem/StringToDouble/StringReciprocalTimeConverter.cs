using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using UnitsNet.Units;
using UnitsNet;

namespace CaeGlobals
{
    public class StringReciprocalTimeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static DurationUnit _durationUnit = DurationUnit.Second;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "")
                {
                    _durationUnit = (DurationUnit)MyUnit.NoUnit;
                }
                else
                {
                    _durationUnit = Duration.ParseUnit(value);
                }
            }
        }
        public static string GetUnitAbbreviation(DurationUnit durationUnit)
        {
            if ((int)durationUnit == MyUnit.NoUnit) return "";
            else return "/" + Duration.GetAbbreviation(durationUnit);
        }
        
        
        // Constructors                                                                                                             
        public StringReciprocalTimeConverter()
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
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble)
                    {
                        string valueString = valueDouble.ToString();
                        string unit = GetUnitAbbreviation(_durationUnit);
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
                valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
                //
                string[] tmp = valueWithUnitString.Split('/');
                if (tmp.Length != 2) throw new FormatException(error);
                //
                double baseValue;
                if (!double.TryParse(tmp[0], out baseValue)) throw new FormatException(error);
                // NoUnit
                if ((int)_durationUnit == MyUnit.NoUnit) return baseValue;
                //
                DurationUnit durationUnit = Duration.ParseUnit(tmp[1]);
                Duration duration = Duration.From(1, durationUnit).ToUnit(_durationUnit);
                //
                double value = baseValue / duration.Value;
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            return StringTimeConverter.SupportedUnitAbbreviations();
        }
    }


}