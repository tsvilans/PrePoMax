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
    public class StringDoubleConverter : TypeConverter
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public StringDoubleConverter()
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
            if (value is string valueString)
            {
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
                    else throw new Exception(valueString + " is not a valid value for Double.");
                }
                //
                return valueDouble;
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
                    return value.ToString();
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