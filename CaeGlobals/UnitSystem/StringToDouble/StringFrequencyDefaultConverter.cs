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
    public class StringFrequencyDefaultConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static FrequencyUnit _frequencyUnit = FrequencyUnit.Hertz;
        //
        protected ArrayList values;
        protected string _default = "Default";
        protected static double _initialValue = 0;


        // Properties                                                                                                               
        public static string SetUnit 
        {
            set
            {
                if (value == "") _frequencyUnit = (FrequencyUnit)MyUnit.NoUnit;
                else _frequencyUnit = Frequency.ParseUnit(value);
            }
        }
        public static string SetInitialValue
        {
            set
            {
                if (_frequencyUnit == (FrequencyUnit)MyUnit.NoUnit) _initialValue = Length.Parse(value).Value;
                else _initialValue = Frequency.Parse(value).ToUnit(_frequencyUnit).Value;
                _initialValue = Tools.RoundToSignificantDigits(_initialValue, 3);
            }
        }


        // Constructors                                                                                                             
        public StringFrequencyDefaultConverter()
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new double[] { double.NaN, _initialValue });
        }


        // Methods                                                                                                                  
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
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                if (String.Equals(value, _default)) return double.NaN;
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
                        if (double.IsNaN(valueDouble)) return _default;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            if ((int)_frequencyUnit != MyUnit.NoUnit) valueString += " " + Frequency.GetAbbreviation(_frequencyUnit);
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
                Frequency frequency = Frequency.Parse(valueWithUnitString);
                if ((int)_frequencyUnit != MyUnit.NoUnit) frequency = frequency.ToUnit(_frequencyUnit);
                return frequency.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            string abb;
            string supportedUnitAbbreviations = "Supported frequency abbreviations: ";
            var allUnits = Frequency.Units;
            for (int i = 0; i < allUnits.Length; i++)
            {
                abb = Frequency.GetAbbreviation(allUnits[i]);
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