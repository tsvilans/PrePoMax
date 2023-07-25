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
    public class StringStringLengthDefaultConverter : StringLengthDefaultConverter
    {
        // Variables                                                                                                                


        // Properties                                                                                                               


        // Constructors                                                                                                             
        public StringStringLengthDefaultConverter()
        {
        }


        // Methods                                                                                                                  
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Initializes the standard values list with defaults.
            values = new ArrayList(new string[] { _default, _initialValue.ToString() });
            // Passes the local integer array.
            StandardValuesCollection svc = new StandardValuesCollection(values);
            return svc;
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from string
            if (value is string valueString)
            {
                valueString = valueString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                double result = (double)base.ConvertFrom(context, culture, valueString);
                string resultStr = (string)ConvertTo(context, culture, result, typeof(string));
                if (valueString.Contains("=")) return valueString.Trim() + "; (" + resultStr + ")";
                else return resultStr;
            }
            else return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            System.Diagnostics.Debug.WriteLine("StringString-ConvertTo");

            return base.ConvertTo(context, culture, value, destinationType);
        }

        //

    }
    

}