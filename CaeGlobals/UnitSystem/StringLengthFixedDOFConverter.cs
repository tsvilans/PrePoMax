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
    public class StringLengthFixedDOFConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static LengthUnit _lengthUnit = LengthUnit.Meter;
        //
        protected ArrayList values;
        protected string _fixed = "Fixed";


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
        public StringLengthFixedDOFConverter()
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new double[] { double.PositiveInfinity, 0});
        }


        // Methods                                                                                                                  

        // Indicates this converter provides a list of standard values.
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
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            else return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                if (string.Equals(valueString, _fixed)) return double.PositiveInfinity;
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
                        if (double.IsPositiveInfinity(valueDouble)) return _fixed;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            if ((int)_lengthUnit != MyUnit.NoUnit) valueString += " " + Length.GetAbbreviation(_lengthUnit);
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
            return StringLengthConverter.SupportedUnitAbbreviations();
        }
    }

}