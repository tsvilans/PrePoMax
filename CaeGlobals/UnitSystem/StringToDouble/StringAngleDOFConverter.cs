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
    public class StringAngleDOFConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected static AngleUnit _angleUnit = AngleUnit.Radian;
        //
        protected ArrayList values;
        protected string _free = "Unconstrained";
        //protected string _fixed = "Fixed";


        // Properties                                                                                                               
        public static string SetUnit
        {
            set
            {
                if (value == "") _angleUnit = (AngleUnit)MyUnit.NoUnit;
                else _angleUnit = Angle.ParseUnit(value);
            }
        }


        // Constructors                                                                                                             
        public StringAngleDOFConverter()
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new double[] { double.NaN, /*double.PositiveInfinity,*/ 0 });
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
                if (String.Equals(value, _free)) return double.NaN;
                //else if (String.Equals(value, _fixed)) return double.PositiveInfinity;
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
                        if (double.IsNaN(valueDouble)) return _free;
                        //else if (double.IsPositiveInfinity(valueDouble)) return _fixed;
                        else
                        {
                            string valueString = valueDouble.ToString();
                            if ((int)_angleUnit != MyUnit.NoUnit) valueString += " " + Angle.GetAbbreviation(_angleUnit);
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
                Angle angle = Angle.Parse(valueWithUnitString);
                if ((int)_angleUnit != MyUnit.NoUnit) angle = angle.ToUnit(_angleUnit);
                return angle.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + Environment.NewLine + Environment.NewLine + SupportedUnitAbbreviations());
            }
        }
        public static string SupportedUnitAbbreviations()
        {
            return StringAngleConverter.SupportedUnitAbbreviations();
        }
    }
}