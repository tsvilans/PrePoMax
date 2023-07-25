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
    public enum PowerPerVolumeUnit { }
    public class StringPowerPerVolumeConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PowerUnit _powerUnit = PowerUnit.Watt;
        protected static VolumeUnit _volumeUnit = VolumeUnit.CubicMeter;
        protected static PowerPerVolumeUnit _powerPerVolumeUnit = MyUnit.PoundForcePerSquareInchSecond;
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
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                }
                else
                {
                    if (value == MyUnit.PoundForceInchPerSecondAbbreviation) _powerUnit = MyUnit.PoundForceInchPerSecond;
                    else _powerUnit = Power.ParseUnit(value);
                }
                //
                _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
            }
        }
        public static string SetVolumeUnit
        { 
            set 
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _volumeUnit = (VolumeUnit)MyUnit.NoUnit;
                }
                else _volumeUnit = Volume.ParseUnit(value);
                //
                _powerPerVolumeUnit = (PowerPerVolumeUnit)MyUnit.NoUnit;
            }
        }
        public static string SetUnit
        {
            set
            {
                if (value == MyUnit.PoundForcePerSquareInchSecondAbbreviation)
                    _powerPerVolumeUnit = MyUnit.PoundForcePerSquareInchSecond;
                else throw new NotSupportedException();
            }
        }
        public static string GetUnitAbbreviation(PowerUnit powerUnit, VolumeUnit volumeUnit,
                                                 PowerPerVolumeUnit powerPerVolumeUnit)
        {
            string unit;
            if (powerPerVolumeUnit == MyUnit.PoundForcePerSquareInchSecond)
                unit = MyUnit.PoundForcePerSquareInchSecondAbbreviation;
            else if ((int)powerUnit == MyUnit.NoUnit || (int)volumeUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Power.GetAbbreviation(powerUnit) + "/" + Volume.GetAbbreviation(volumeUnit);
            return unit;
        }
        
        
        // Constructors                                                                                                             
        public StringPowerPerVolumeConverter()
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
                        string unit = GetUnitAbbreviation(_powerUnit, _volumeUnit, _powerPerVolumeUnit);
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
            if (valueWithUnitString.Contains(MyUnit.PoundForcePerSquareInchSecondAbbreviation))
            {
                valueWithUnitString = valueWithUnitString.Replace(MyUnit.PoundForcePerSquareInchSecondAbbreviation, "");
                if (double.TryParse(valueWithUnitString, out value))
                {
                    // 1 pound force = 4.44822162 newtons
                    // 1 inch = 0.0254 meters
                    // 1 s = 1 s
                    conversionToSI = 6894.7573;
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
                VolumeUnit volumeUnit = Volume.ParseUnit(tmp[1]);
                Volume volume = Volume.From(1, volumeUnit).ToUnit(VolumeUnit.CubicMeter);
                //
                conversionToSI = (double)power.Value / volume.Value;
            }
        }
        private static void GetConversionFromSI(out double conversionFromSI)
        {
            // To my unit
            if (_powerPerVolumeUnit == MyUnit.PoundForcePerSquareInchSecond)
            {
                // 1 pound force = 4.44822162 newtons
                // 1 inch = 0.0254 meters
                // 1 s = 1 s
                conversionFromSI = 1 / 6894.7573;
            }
            // To no unit
            else if ((int)_powerUnit == MyUnit.NoUnit || (int)_volumeUnit == MyUnit.NoUnit)
            {
                conversionFromSI = 1;
            }
            // To supported unit
            else
            {
                Power power = Power.From(1, PowerUnit.Watt).ToUnit(_powerUnit);
                Volume volume = Volume.From(1, VolumeUnit.CubicMeter).ToUnit(_volumeUnit);
                //
                conversionFromSI = (double)power.Value / volume.Value;
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
            supportedUnitAbbreviations += StringVolumeConverter.SupportedUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}