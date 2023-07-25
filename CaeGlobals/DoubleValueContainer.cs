using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace CaeGlobals
{
    public class DoubleValueContainer
    {
        // Variables                                                                                                                
        private Type _stringDoubleConverterType;
        private string _equation;
        public double _value;


        // Properties                                                                                                               
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _equation = null;
            }
        }
        public string Equation
        {
            get
            {
                TypeConverter _stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
                //
                if (_equation == null) _equation = (string)_stringDoubleConverter.ConvertTo(_value, typeof(string));
                return _equation;
            }
            set
            {
                TypeConverter _stringDoubleConverter = (TypeConverter)Activator.CreateInstance(_stringDoubleConverterType);
                //
                _value = (double)_stringDoubleConverter.ConvertFrom(value); // get the equation result
                _equation = value;                                          // save the equation
            }
        }


        // Constructors                                                                                                             
        public DoubleValueContainer(Type stringDoubleConverterType)
        {
            _stringDoubleConverterType = stringDoubleConverterType;
        }
    }
}
