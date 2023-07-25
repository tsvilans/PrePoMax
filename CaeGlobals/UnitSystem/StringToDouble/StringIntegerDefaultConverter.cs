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
    public class StringIntegerDefaultConverter : TypeConverter
    {
        // Variables                                                                                                                
        protected ArrayList values;
        protected string _default = "Default";
        protected static int _initialValue = 0;


        // Properties                                                                                                               
        public static int SetInitialValue
        {
            set
            {
                _initialValue = value;
            }
        }


        // Constructors                                                                                                             
        public StringIntegerDefaultConverter()
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new int[] { int.MinValue, _initialValue });
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
                if (Equals(valueString, _default)) return int.MinValue;
                else
                {
                    // From StringIntegerConverter
                    double valueDouble;
                    valueString = valueString.Trim();
                    //
                    if (valueString.Length == 0 || valueString == "=") return 0;   // empty string -> 0
                    if (!double.TryParse(valueString, out valueDouble))
                    {
                        if (valueString.StartsWith("="))
                        {
                            valueString = valueString.Substring(1, valueString.Length - 1);
                            NCalc.Expression e = MyNCalc.GetExpression(valueString);
                            if (!e.HasErrors())
                            {
                                object result = e.Evaluate();
                                if (result is int) valueDouble = (int)result;
                                else if (result is double) valueDouble = (double)result;
                            }
                            else
                            {
                                throw new CaeException("Equation error:" + Environment.NewLine + e.Error);
                            }
                        }
                        else throw new Exception(valueString + " is not a valid value for Int.");
                    }
                    //
                    return (int)Math.Round(valueDouble);
                }
            }
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (destinationType == typeof(string))
                {
                    if ((int)value == int.MinValue) return _default;
                    else return value.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            catch
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
    

}