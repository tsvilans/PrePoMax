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
    public class StringHeatTransferCoefficientFromConverter : StringHeatTransferCoefficientConverter
    {
        // Variables                                                                                                                
       

        // Properties                                                                                                               
        
        
        // Constructors                                                                                                             
        public StringHeatTransferCoefficientFromConverter()
        {
        }


        // Methods                                                                                                                  
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Convert to string
            try
            {
                if (destinationType == typeof(string))
                {
                    if (value is double valueDouble) return value.ToString();
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