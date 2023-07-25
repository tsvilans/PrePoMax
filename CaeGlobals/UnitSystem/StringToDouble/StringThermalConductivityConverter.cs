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
    public class StringThermalConductivityConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static PowerUnit _powerUnit = PowerUnit.Watt;
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;
        protected static TemperatureDeltaUnit _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
        protected static ThermalConductivityUnit _thermalConductivityUnit = MyUnit.PoundForcePerSecondFahrenheit;
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
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else
                {
                    if (value == MyUnit.PoundForceInchPerSecondAbbreviation) _powerUnit = MyUnit.PoundForceInchPerSecond;
                    else _powerUnit = Power.ParseUnit(value);
                }
                //
                _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
            }
        }
        public static string SetLengthUnit
        { 
            set 
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _lengthUnit = Length.ParseUnit(value);
                //
                _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
            }
        }
        public static string SetTemperatureDeltaUnit
        {
            set
            {
                if (value == "")
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _temperatureDeltaUnit = TemperatureDelta.ParseUnit(value);
                //
                _thermalConductivityUnit = (ThermalConductivityUnit)MyUnit.NoUnit;
            }
        }
        public static string SetUnit
        {
            set
            {
                if (value == MyUnit.PoundForcePerSecondFahrenheitAbbreviation)
                {
                    _powerUnit = (PowerUnit)MyUnit.NoUnit;
                    _lengthUnit = (LengthUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                    //
                    _thermalConductivityUnit = MyUnit.PoundForcePerSecondFahrenheit;
                }
                else throw new NotSupportedException();
            }
        }
        public static string GetUnitAbbreviation(PowerUnit powerUnit, LengthUnit lengthUnit,
                                                 TemperatureDeltaUnit temperatureDeltaUnit,
                                                 ThermalConductivityUnit thermalConductivityUnit)
        {
            string unit;
            if (thermalConductivityUnit == MyUnit.PoundForcePerSecondFahrenheit)
                unit = MyUnit.PoundForcePerSecondFahrenheitAbbreviation;
            else if ((int)powerUnit == MyUnit.NoUnit || (int)lengthUnit == MyUnit.NoUnit ||
                     (int)temperatureDeltaUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Power.GetAbbreviation(powerUnit) + "/(" + Length.GetAbbreviation(lengthUnit) + "·" +
                        TemperatureDelta.GetAbbreviation(temperatureDeltaUnit) + ")";
            return unit.Replace("∆", "");
        }
        
        
        // Constructors                                                                                                             
        public StringThermalConductivityConverter()
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
                        string unit = GetUnitAbbreviation(_powerUnit, _lengthUnit, _temperatureDeltaUnit, _thermalConductivityUnit);
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
            if (valueWithUnitString.Contains(MyUnit.PoundForcePerSecondFahrenheitAbbreviation))
            {
                valueWithUnitString = valueWithUnitString.Replace(MyUnit.PoundForcePerSecondFahrenheitAbbreviation, "");
                if (double.TryParse(valueWithUnitString, out value))
                {
                    // 1 pound force = 4.44822162 newtons
                    // 1 s = 1 s
                    // 1 °F = 0.555555556 °C
                    conversionToSI = 8.006798852;
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
                tmp = tmp[1].Replace("(", "").Replace(")", "").Split(new string[] { "*", "·" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length != 2) throw new FormatException(error);
                //
                LengthUnit lengthUnit = Length.ParseUnit(tmp[0]);
                Length length = Length.From(1, lengthUnit).ToUnit(LengthUnit.Meter);
                //
                if (!tmp[1].Contains("∆")) tmp[1] = "∆" + tmp[1];
                TemperatureDeltaUnit temperatureDeltaUnit = TemperatureDelta.ParseUnit(tmp[1]);
                TemperatureDelta temperatureDelta =
                    TemperatureDelta.From(1, temperatureDeltaUnit).ToUnit(TemperatureDeltaUnit.DegreeCelsius);
                //
                conversionToSI = (double)power.Value / (length.Value * temperatureDelta.Value);
            }
        }
        private static void GetConversionFromSI(out double conversionFromSI)
        {
            // To my unit
            if (_thermalConductivityUnit == MyUnit.PoundForcePerSecondFahrenheit)
            {
                // 1 pound force = 4.44822162 newtons
                // 1 s = 1 s
                // 1 °F = 0.555555556 °C
                conversionFromSI = 1 / 8.006798852;
            }
            // To no unit
            else if ((int)_powerUnit == MyUnit.NoUnit || (int)_lengthUnit == MyUnit.NoUnit ||
                     (int)_temperatureDeltaUnit == MyUnit.NoUnit)
            {
                conversionFromSI = 1;
            }
            // To supported unit
            else
            {
                Power power = Power.From(1, PowerUnit.Watt).ToUnit(_powerUnit);
                Length length = Length.From(1, LengthUnit.Meter).ToUnit(_lengthUnit);
                TemperatureDelta temperatureDelta =
                    TemperatureDelta.From(1, TemperatureDeltaUnit.DegreeCelsius).ToUnit(_temperatureDeltaUnit);
                //
                conversionFromSI = (double)power.Value / (length.Value * temperatureDelta.Value);
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
                //
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
                throw new Exception(ex.Message.Replace("∆", "") + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringPowerConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringLengthConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringTemperatureConverter.SupportedDeltaUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}