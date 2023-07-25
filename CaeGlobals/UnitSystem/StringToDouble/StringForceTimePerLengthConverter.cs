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
    public class StringForceTimePerLengthConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static ForceUnit _forceUnit = ForceUnit.Newton;
        protected static DurationUnit _durationUnit = DurationUnit.Second;
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetForceUnit 
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _durationUnit = (DurationUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                }
                else _forceUnit = Force.ParseUnit(value); 
            }
        }
        public static string SetTimeUnit
        {
            set
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _durationUnit = (DurationUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                }
                else _durationUnit = Duration.ParseUnit(value);
            }
        }
        public static string SetLengthUnit 
        { 
            set 
            {
                if (value == "")
                {
                    _forceUnit = (ForceUnit)MyUnit.NoUnit;
                    _durationUnit = (DurationUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                }
                else _lengthUnit = Length.ParseUnit(value);
            }
        }
        public static string GetUnitAbbreviation(ForceUnit forceUnit, DurationUnit durationUnit, LengthUnit lengthUnit)
        {
            if ((int)forceUnit == MyUnit.NoUnit || (int)durationUnit == MyUnit.NoUnit || (int)lengthUnit == MyUnit.NoUnit)
                return "";
            else
                return Force.GetAbbreviation(forceUnit) + "·" + Duration.GetAbbreviation(durationUnit) + "/" +
                       Length.GetAbbreviation(lengthUnit);
        }


        // Constructors                                                                                                             
        public StringForceTimePerLengthConverter()
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
                        string unit = GetUnitAbbreviation(_forceUnit, _durationUnit, _lengthUnit);
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
                string[] tmp = valueWithUnitString.Split(new string[] { "*", "·", "/" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != 3) throw new FormatException(error);
                Force force = Force.Parse(tmp[0]);
                // NoUnit
                if ((int)_forceUnit == MyUnit.NoUnit || (int)_durationUnit == MyUnit.NoUnit || (int)_lengthUnit == MyUnit.NoUnit)
                    return force.Value;
                else force = force.ToUnit(_forceUnit);
                //
                DurationUnit durationUnit = Duration.ParseUnit(tmp[1]);
                Duration duration = Duration.From(1, durationUnit).ToUnit(_durationUnit);

                LengthUnit lengthUnit = Length.ParseUnit(tmp[2]);
                Length length = Length.From(1, lengthUnit).ToUnit(_lengthUnit);
                double value = force.Value * duration.Value / length.Value;
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringForceConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations = StringTimeConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringLengthConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}