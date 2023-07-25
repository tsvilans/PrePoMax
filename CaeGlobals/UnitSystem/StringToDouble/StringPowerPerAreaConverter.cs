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
    public enum PowerPerAreaUnit { }
    public class StringPowerPerAreaConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PowerUnit _powerUnit = PowerUnit.Watt;
        protected static AreaUnit _areaUnit = AreaUnit.SquareMeter;
        protected static PowerPerAreaUnit _powerPerAreaUnit = MyUnit.PoundForcePerInchSecond;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";


        // Properties                                                                                                               
        public static string SetPowerUnit
        {
            set
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _areaUnit = (AreaUnit)MyUnit.NoUnit;
                }
                else
                {
                    if (value == MyUnit.PoundForceInchPerSecondAbbreviation) _powerUnit = MyUnit.PoundForceInchPerSecond;
                    else _powerUnit = Power.ParseUnit(value);
                }
                //
                _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
            }
        }
        public static string SetAreaUnit
        { 
            set 
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _areaUnit = (AreaUnit)MyUnit.NoUnit;
                }
                else _areaUnit = Area.ParseUnit(value);
                //
                _powerPerAreaUnit = (PowerPerAreaUnit)MyUnit.NoUnit;
            }
        }
        public static string SetUnit
        {
            set
            {
                if (value == MyUnit.PoundForcePerInchSecondAbbreviation)
                    _powerPerAreaUnit = MyUnit.PoundForcePerInchSecond;
                else throw new NotSupportedException();
            }
        }
        public static string GetUnitAbbreviation(PowerUnit powerUnit, AreaUnit areaUnit,
                                                 PowerPerAreaUnit powerPerAreaUnit)
        {
            string unit;
            if (powerPerAreaUnit == MyUnit.PoundForcePerInchSecond)
                unit = MyUnit.PoundForcePerInchSecondAbbreviation;
            else if ((int)powerUnit == MyUnit.NoUnit || (int)areaUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Power.GetAbbreviation(powerUnit) + "/" + Area.GetAbbreviation(areaUnit);
            return unit;
        }
        
        
        // Constructors                                                                                                             
        public StringPowerPerAreaConverter()
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
                        string unit = GetUnitAbbreviation(_powerUnit, _areaUnit, _powerPerAreaUnit);
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
        private static void GetConversionToSI(string valueWithUnitString, out double value, out double conversionToSI)
        {
            valueWithUnitString = valueWithUnitString.Trim().Replace(" ", "");
            // From my unit
            if (valueWithUnitString.Contains(MyUnit.PoundForcePerInchSecondAbbreviation))
            {
                valueWithUnitString = valueWithUnitString.Replace(MyUnit.PoundForcePerInchSecondAbbreviation, "");
                if (double.TryParse(valueWithUnitString, out value))
                {
                    // 1 pound force = 4.44822162 newtons
                    // 1 inch = 0.0254 meters
                    // 1 s = 1 s
                    conversionToSI = 175.12684;
                }
                else throw new ArgumentException(error);
            }
            // From no unit
            else if (double.TryParse(valueWithUnitString, out value))
            {
                conversionToSI = 1;
            }
            // From supported unit
            else
            {
                string[] tmp = valueWithUnitString.Split('/');
                if (tmp.Length != 2) throw new FormatException(error);
                //
                Power power = Power.Parse(tmp[0]);
                value = (double)power.Value;
                PowerUnit powerUnit = power.Unit;
                power = Power.From(1, powerUnit).ToUnit(PowerUnit.Watt);
                //
                AreaUnit areaUnit = Area.ParseUnit(tmp[1]);
                Area area = Area.From(1, areaUnit).ToUnit(AreaUnit.SquareMeter);
                //
                conversionToSI = (double)power.Value / area.Value;
            }
        }
        private static void GetConversionFromSI(out double conversionFromSI)
        {
            // To my unit
            if (_powerPerAreaUnit == MyUnit.PoundForcePerInchSecond)
            {
                // 1 pound force = 4.44822162 newtons
                // 1 inch = 0.0254 meters
                // 1 s = 1 s
                conversionFromSI = 1 / 175.12684;
            }
            // To no unit
            else if ((int)_powerUnit == MyUnit.NoUnit || (int)_areaUnit == MyUnit.NoUnit)
            {
                conversionFromSI = 1;
            }
            // To supported unit
            else
            {
                Power power = Power.From(1, PowerUnit.Watt).ToUnit(_powerUnit);
                Area area = Area.From(1, AreaUnit.SquareMeter).ToUnit(_areaUnit);
                //
                conversionFromSI = (double)power.Value / area.Value;
            }
        }
        //
        public static double ConvertToCurrentUnits(string valueWithUnitString)
        {
            try
            {
                double valueDouble;
                double conversionToSI;
                double conversionFromSI;
                GetConversionToSI(valueWithUnitString, out valueDouble, out conversionToSI);
                GetConversionFromSI(out conversionFromSI);
                //
                if (Math.Abs(conversionToSI - 1 / conversionFromSI) > 1E-6)
                    valueDouble *= conversionToSI * conversionFromSI;
                //
                return valueDouble;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringPowerConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringAreaConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}