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
    public class StringTemperatureUndefinedConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static TemperatureUnit _temperatureUnit = TemperatureUnit.DegreeCelsius;
        //
        protected static ArrayList values;
        protected static double _initialValue = 0;      // use initial value for the constructor to work
        protected static string _undefined = "Undefined";


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _temperatureUnit = (TemperatureUnit)MyUnit.NoUnit;
                else _temperatureUnit = Temperature.ParseUnit(value);
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


        // Constructors                                                                                                             
        public StringTemperatureUndefinedConverter()
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
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Passes the local integer array.
            StandardValuesCollection svc = new StandardValuesCollection(values);
            return svc;
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        //
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
            // Convert to string
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
                            if ((int)_temperatureUnit != MyUnit.NoUnit)
                                valueString += " " + Temperature.GetAbbreviation(_temperatureUnit);
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
            return StringTemperatureConverter.SupportedUnitAbbreviations();
        }
    }

}