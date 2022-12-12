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
    public class StringStefanBoltzmannUndefinedConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static MassUnit _massUnit = MassUnit.Kilogram;
        protected static DurationUnit _timeUnit = DurationUnit.Second;
        protected static TemperatureDeltaUnit _temperatureDeltaUnit = TemperatureDeltaUnit.DegreeCelsius;
        protected static string error = "Unable to parse quantity. Expected the form \"{value} {unit abbreviation}" +
                                        "\", such as \"5.5 m\". The spacing is optional.";
        //
        protected static ArrayList values;
        protected static double _initialValue = 0;      // use initial value for the constructor to work
        protected static string _undefined = "Undefined";


        // Properties                                                                                                               
        public static string SetMassUnit
        { 
            set 
            {
                if (value == "")
                {
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _timeUnit = (DurationUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _massUnit = Mass.ParseUnit(value);
            }
        }
        public static string SetTimeUnit
        {
            set
            {
                if (value == "")
                {
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _timeUnit = (DurationUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else
                {
                    _timeUnit = Duration.ParseUnit(value);
                }
            }
        }
        public static string SetTemperatureDeltaUnit
        {
            set
            {
                if (value == "")
                {
                    _massUnit = (MassUnit)MyUnit.NoUnit;
                    _timeUnit = (DurationUnit)MyUnit.NoUnit;
                    _temperatureDeltaUnit = (TemperatureDeltaUnit)MyUnit.NoUnit;
                }
                else _temperatureDeltaUnit = TemperatureDelta.ParseUnit(value);
            }
        }
        public static double SetInitialValue
        {
            set
            {
                _initialValue = value;
                CreateListOfStandardValues();
            }
        }
        //
        public static string GetUnitAbbreviation(MassUnit massUnit, DurationUnit timeUnit, 
                                                 TemperatureDeltaUnit temperatureDeltaUnit)
        {
            string unit;
            if ((int)massUnit == MyUnit.NoUnit || (int)timeUnit == MyUnit.NoUnit || (int)temperatureDeltaUnit == MyUnit.NoUnit)
                unit = "";
            else unit = Mass.GetAbbreviation(massUnit) + "/(" + Duration.GetAbbreviation(timeUnit) + "³·" +
                        TemperatureDelta.GetAbbreviation(temperatureDeltaUnit) + "⁴)";
            return unit.Replace("∆", "");
        }
        

        // Constructors                                                                                                             
        public StringStefanBoltzmannUndefinedConverter()
        {
            CreateListOfStandardValues();
        }


        // Methods                                                                                                                  
        private static void CreateListOfStandardValues()
        {
            values = new ArrayList(new double[] { double.PositiveInfinity, _initialValue });
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Passes the local integer array.
            StandardValuesCollection svc = new StandardValuesCollection(values);
            return svc;
        }
        //
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                if (string.Equals(valueString, _undefined)) return double.PositiveInfinity;
                else return MyNCalc.ConvertFromString(valueString, ConvertToCurrentUnits);
            }
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
                        if (double.IsPositiveInfinity(valueDouble)) return _undefined;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            string unit = GetUnitAbbreviation(_massUnit, _timeUnit, _temperatureDeltaUnit);
                            if (unit.Length > 0) valueString += " " + unit;
                            return valueString;
                        }
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
                Mass mass = Mass.Parse(tmp[0]);
                // NoUnit
                if ((int)_massUnit == MyUnit.NoUnit || (int)_timeUnit == MyUnit.NoUnit || (int)_temperatureDeltaUnit == MyUnit.NoUnit)
                    return mass.Value;
                else mass = mass.ToUnit(_massUnit);
                //
                if (tmp[1].StartsWith("(") && tmp[1].EndsWith(")"))
                {
                    tmp[1] = tmp[1].Replace("(", "").Replace(")", "");
                    tmp = tmp[1].Split(new string[] { "*", "·" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else throw new FormatException(error);
                if (tmp.Length != 2) throw new FormatException(error);
                //
                if (tmp[0].EndsWith("³") || tmp[0].EndsWith("^3")) tmp[0] = tmp[0].Replace("³", "").Replace("^3", "");
                else throw new FormatException(error);
                DurationUnit timeUnit = Duration.ParseUnit(tmp[0]);
                Duration time = Duration.From(1, timeUnit).ToUnit(_timeUnit);
                //
                if (tmp[1].EndsWith("⁴") || tmp[1].EndsWith("^4")) tmp[1] = tmp[1].Replace("⁴", "").Replace("^4", "");
                else throw new FormatException(error);
                if (!tmp[1].Contains("∆")) tmp[1] = "∆" + tmp[1];
                TemperatureDeltaUnit temperatureDeltaUnit = TemperatureDelta.ParseUnit(tmp[1]);
                TemperatureDelta temperatureDelta = TemperatureDelta.From(1, temperatureDeltaUnit).ToUnit(_temperatureDeltaUnit);
                //
                double value = (double)mass.Value / (Math.Pow(time.Value, 3) * Math.Pow(temperatureDelta.Value, 4));
                return value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.Replace("∆", "") + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string supportedUnitAbbreviations = StringMassConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringTimeConverter.SupportedUnitAbbreviations();
            supportedUnitAbbreviations += Environment.NewLine + Environment.NewLine;
            supportedUnitAbbreviations += StringTemperatureConverter.SupportedDeltaUnitAbbreviations();
            return supportedUnitAbbreviations;
        }
    }


}